import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { AuthService } from '@/services/api'
import { useApi } from '@/composables/useApi'

export const useAuthStore = defineStore('auth', () => {
  const user = ref(JSON.parse(localStorage.getItem('user') || sessionStorage.getItem('user') || 'null'))
  const token = ref(localStorage.getItem('token') || sessionStorage.getItem('token') || null)
  
  const loginApi = useApi(AuthService.login)
  const registerApi = useApi(AuthService.register)

  const isAuthenticated = computed(() => !!user.value)
  const currentUser = computed(() => user.value)
  const isAdmin = computed(() => user.value?.role?.toUpperCase() === 'ADMINISTRATOR' || user.value?.role?.toUpperCase() === 'ADMIN')
  const isMaker = computed(() => user.value?.role?.toUpperCase() === 'MAKER')
  const isChecker = computed(() => user.value?.role?.toUpperCase() === 'CHECKER')
  const isGuest = computed(() => user.value?.role?.toUpperCase() === 'USER')

  async function login(credentials) {
    const data = await loginApi.execute(credentials)
    
    // Transform backend response to frontend user object
    user.value = {
      id: data.userId,
      username: data.userName,
      email: data.email,
      role: data.role
    }
    token.value = data.authorization?.accessToken

    const storage = credentials.rememberMe ? localStorage : sessionStorage
    storage.setItem('user', JSON.stringify(user.value))
    storage.setItem('token', token.value)
    
    return data
  }

  async function register(userData) {
    return await registerApi.execute(userData)
  }

  function logout() {
    user.value = null
    token.value = null
    localStorage.clear()
    sessionStorage.clear()
    AuthService.logout()
  }

  return { user, token, isAuthenticated, currentUser, isAdmin, isMaker, isChecker, login, register, logout, 
           loading: computed(() => loginApi.loading.value || registerApi.loading.value),
           error: computed(() => loginApi.error.value || registerApi.error.value) }
})
