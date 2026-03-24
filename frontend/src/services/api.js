import axios from 'axios'

const api = axios.create({
  baseURL: 'https://localhost:7001/api', // Thay đổi khi có backend
  timeout: 10000,
  headers: { 'Content-Type': 'application/json' }
})

// Request interceptor - Tự động thêm token
api.interceptors.request.use(config => {
  const token = localStorage.getItem('token') || sessionStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Response interceptor - Xử lý lỗi chung
api.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
      localStorage.removeItem('user')
      localStorage.removeItem('token')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default api

// ===== API Endpoints (sẵn sàng khi có backend) =====
// Auth
// api.post('/Auth/login', { userName, password, rememberMe })
// api.post('/Auth/register', { userName, password, email, phoneNumber })
// api.post('/Auth/logout')
// api.post('/Auth/refresh-token', { accessToken, refreshToken })
// api.get('/Auth/me')

// User Management
// api.get('/User')
// api.get('/User/{id}')
// api.post('/User', dto)
// api.put('/User/{id}', dto)
// api.patch('/User/{id}/role', { roleId })
// api.patch('/User/{id}/toggle-status')

// Customer Contact
// api.get('/CustomerContact')
// api.get('/CustomerContact/{id}')
// api.get('/CustomerContact/customer/{custId}')
// api.post('/CustomerContact', dto)
// api.put('/CustomerContact/{id}', dto)
// api.delete('/CustomerContact/{id}')
// api.patch('/CustomerContact/{id}/set-default')

// Audit
// api.get('/Audit')

// Maker-Checker
// api.post('/MakerChecker/submit', { entityName, entityId, action, details })
// api.post('/MakerChecker/{id}/approve')
// api.post('/MakerChecker/{id}/reject', reason)

// System Code
// api.get('/SystemCode')
// api.post('/SystemCode', systemCode)
