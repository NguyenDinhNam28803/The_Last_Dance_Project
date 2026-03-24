<template>
  <div class="auth-page">
    <div class="auth-bg">
      <div class="auth-shape shape-1"></div>
      <div class="auth-shape shape-2"></div>
    </div>
    <div class="auth-container">
      <div class="auth-card">
        <div class="auth-header">
          <div class="auth-logo">💃</div>
          <h1>Đăng ký</h1>
          <p>Tạo tài khoản mới trên hệ thống</p>
        </div>

        <div v-if="error" class="auth-error">⚠️ {{ error }}</div>
        <div v-if="success" class="auth-success">✅ {{ success }}</div>

        <form @submit.prevent="handleRegister" class="auth-form">
          <div class="form-group">
            <label class="form-label">Tên đăng nhập <span class="required">*</span></label>
            <input v-model="form.userName" type="text" class="form-control" placeholder="Ít nhất 3 ký tự" required />
          </div>
          <div class="form-group">
            <label class="form-label">Mật khẩu <span class="required">*</span></label>
            <input v-model="form.password" type="password" class="form-control" placeholder="Ít nhất 6 ký tự" required />
          </div>
          <div class="form-group">
            <label class="form-label">Email <span class="required">*</span></label>
            <input v-model="form.email" type="email" class="form-control" placeholder="example@email.com" required />
          </div>
          <div class="form-group">
            <label class="form-label">Số điện thoại <span class="required">*</span></label>
            <input v-model="form.phoneNumber" type="tel" class="form-control" placeholder="09xxxxxxxx" required />
          </div>

          <button type="submit" class="btn btn-primary btn-full" :disabled="loading">
            {{ loading ? '⏳ Đang xử lý...' : '📝 Đăng ký' }}
          </button>
        </form>

        <div class="auth-footer">
          <p>Đã có tài khoản? <router-link to="/login">Đăng nhập</router-link></p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const form = reactive({ userName: '', password: '', email: '', phoneNumber: '' })
const loading = ref(false)
const error = ref('')
const success = ref('')

async function handleRegister() {
  error.value = ''
  success.value = ''
  loading.value = true
  try {
    const result = auth.register(form.userName, form.password, form.email, form.phoneNumber)
    success.value = result.message
    form.userName = ''
    form.password = ''
    form.email = ''
    form.phoneNumber = ''
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #0f0f1a;
  position: relative;
  overflow: hidden;
}

.auth-bg { position: absolute; inset: 0; pointer-events: none; }
.auth-shape { position: absolute; border-radius: 50%; filter: blur(80px); opacity: 0.3; }
.shape-1 { width: 350px; height: 350px; background: #7c3aed; top: -80px; left: -80px; animation: float 12s ease-in-out infinite; }
.shape-2 { width: 300px; height: 300px; background: #4f46e5; bottom: -60px; right: -60px; animation: float 14s ease-in-out infinite reverse; }

@keyframes float { 0%, 100% { transform: translate(0, 0); } 50% { transform: translate(20px, -20px); } }

.auth-container { position: relative; z-index: 2; width: 100%; max-width: 440px; padding: var(--spacing-lg); }

.auth-card {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: var(--border-radius-lg);
  padding: var(--spacing-xl);
  box-shadow: 0 30px 60px rgba(0, 0, 0, 0.5);
}

.auth-header { text-align: center; margin-bottom: var(--spacing-xl); }
.auth-logo { font-size: 3rem; margin-bottom: var(--spacing-md); }
.auth-header h1 { font-size: 1.8rem; font-weight: 800; color: white; margin-bottom: 8px; }
.auth-header p { color: rgba(255, 255, 255, 0.5); font-size: 0.9rem; }

.auth-error { background: rgba(239, 68, 68, 0.15); border: 1px solid rgba(239, 68, 68, 0.3); color: #fca5a5; padding: 12px 16px; border-radius: var(--border-radius-sm); margin-bottom: var(--spacing-md); font-size: 0.85rem; }
.auth-success { background: rgba(16, 185, 129, 0.15); border: 1px solid rgba(16, 185, 129, 0.3); color: #6ee7b7; padding: 12px 16px; border-radius: var(--border-radius-sm); margin-bottom: var(--spacing-md); font-size: 0.85rem; }

.auth-form .form-label { color: rgba(255, 255, 255, 0.8); }
.auth-form .form-control { background: rgba(255, 255, 255, 0.06); border-color: rgba(255, 255, 255, 0.12); color: white; }
.auth-form .form-control:focus { border-color: var(--color-primary); background: rgba(255, 255, 255, 0.08); }
.auth-form .form-control::placeholder { color: rgba(255, 255, 255, 0.3); }

.btn-full { width: 100%; padding: 14px; font-size: 1rem; margin-top: var(--spacing-sm); }

.auth-footer { text-align: center; margin-top: var(--spacing-lg); color: rgba(255, 255, 255, 0.5); font-size: 0.85rem; }
.auth-footer a { color: #a78bfa; font-weight: 600; }
.auth-footer a:hover { color: #c4b5fd; }
</style>
