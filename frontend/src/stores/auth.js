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

  async function login(credentials) {
    const data = await loginApi.execute(credentials)
    
    user.value = data.user
    token.value = data.token

    const storage = credentials.rememberMe ? localStorage : sessionStorage
    storage.setItem('user', JSON.stringify(data.user))
    storage.setItem('token', data.token)
    
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

  return { user, token, isAuthenticated, login, register, logout, 
           loading: computed(() => loginApi.loading.value || registerApi.loading.value),
           error: computed(() => loginApi.error.value || registerApi.error.value) }
})
