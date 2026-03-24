import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useAuthStore = defineStore('auth', () => {
  const user = ref(JSON.parse(localStorage.getItem('user') || 'null'))
  const token = ref(localStorage.getItem('token') || null)

  const isAuthenticated = computed(() => !!user.value)
  const isAdmin = computed(() => user.value?.role === 'ADMIN')
  const isManager = computed(() => user.value?.role === 'MANAGER')

  function login(username, password, rememberMe) {
    // Mock login — will be replaced with real API call
    const mockAccounts = {
      admin: { userId: 'CUST001', userName: 'admin', name: 'Nguyễn Văn Admin', email: 'admin@company.vn', phoneNumber: '0901234567', role: 'ADMIN' },
      manager01: { userId: 'CUST002', userName: 'manager01', name: 'Trần Thị Manager', email: 'manager@company.vn', phoneNumber: '0912345678', role: 'MANAGER' },
      user01: { userId: 'CUST003', userName: 'user01', name: 'Lê Minh User', email: 'user01@company.vn', phoneNumber: '0923456789', role: 'USER' }
    }

    const account = mockAccounts[username]
    if (!account) throw new Error('Tài khoản không tồn tại')
    if (password !== 'Navi@2024') throw new Error('Mật khẩu không đúng')

    const mockToken = 'mock-jwt-token-' + Date.now()

    user.value = account
    token.value = mockToken

    if (rememberMe) {
      localStorage.setItem('user', JSON.stringify(account))
      localStorage.setItem('token', mockToken)
    } else {
      sessionStorage.setItem('user', JSON.stringify(account))
      sessionStorage.setItem('token', mockToken)
    }

    return { user: account, token: mockToken }
  }

  function register(username, password, email, phoneNumber) {
    // Mock register
    if (username.length < 3) throw new Error('Tên đăng nhập phải có ít nhất 3 ký tự')
    if (password.length < 6) throw new Error('Mật khẩu phải có ít nhất 6 ký tự')
    return { message: 'Đăng ký thành công!' }
  }

  function logout() {
    user.value = null
    token.value = null
    localStorage.removeItem('user')
    localStorage.removeItem('token')
    sessionStorage.removeItem('user')
    sessionStorage.removeItem('token')
  }

  function loadSession() {
    const stored = localStorage.getItem('user') || sessionStorage.getItem('user')
    if (stored) {
      user.value = JSON.parse(stored)
      token.value = localStorage.getItem('token') || sessionStorage.getItem('token')
    }
  }

  return { user, token, isAuthenticated, isAdmin, isManager, login, register, logout, loadSession }
})
