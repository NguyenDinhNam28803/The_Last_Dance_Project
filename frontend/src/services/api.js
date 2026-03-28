import axios from 'axios'

const api = axios.create({
  baseURL: 'https://localhost:7280/api',
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
      // console.error("Lỗi không đăng nhập được")
    }
    return Promise.reject(error)
  }
)

export default api

// ===== API Services =====
export const AuthService = {
  login: (credentials) => api.post('/Auth/login', credentials),
  register: (userData) => api.post('/Auth/register', userData),
  logout: () => api.post('/Auth/logout'),
  refreshToken: (tokenData) => api.post('/Auth/refresh-token', tokenData),
  me: () => api.get('/Auth/me')
}

export const UserService = {
  getAll: () => api.get('/User'),
  getById: (id) => api.get(`/User/${id}`),
  create: (dto) => api.post('/User', dto),
  update: (id, dto) => api.put(`/User/${id}`, dto),
  updateRole: (id, roleData) => api.patch(`/User/${id}/role`, roleData),
  toggleStatus: (id) => api.patch(`/User/${id}/toggle-status`)
}

export const CustomerContactService = {
  getAll: () => api.get('/CustomerContact'),
  getById: (id) => api.get(`/CustomerContact/${id}`),
  getByCustomer: (custId) => api.get(`/CustomerContact/customer/${custId}`),
  create: (dto) => api.post('/CustomerContact', dto),
  update: (id, dto) => api.put(`/CustomerContact/${id}`, dto),
  delete: (id) => api.delete(`/CustomerContact/${id}`),
  setDefault: (id) => api.patch(`/CustomerContact/${id}/set-default`)
}

export const AuditService = {
  getAll: () => api.get('/Audit')
}

export const MakerCheckerService = {
  getPending: () => api.get('/MakerChecker/pending'),
  submit: (data) => api.post('/MakerChecker/submit', data),
  approve: (id) => api.post(`/MakerChecker/${id}/approve`),
  reject: (id, reason) => api.post(`/MakerChecker/${id}/reject`, { reason }),
  cancel: (id) => api.post(`/MakerChecker/${id}/cancel`)
}

export const SystemCodeService = {
  getAll: () => api.get('/SystemCode'),
  create: (systemCode) => api.post('/SystemCode', systemCode)
}

export const ImportExportService = {
  getTemplate: () => api.get('/ImportExport/template', { responseType: 'blob' }),
  exportCustomers: () => api.get('/ImportExport/export-customers', { responseType: 'blob' }),
  importCustomers: (formData) => api.post('/ImportExport/import-customers', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
}
