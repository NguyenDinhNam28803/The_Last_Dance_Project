<template>
  <div class="login-container">
    <div class="login-header">
      <div class="logo">
        <span class="logo-text">NAVI <span class="fw-light">SOFTWARE</span></span>
        <span class="company-name">Công ty Cổ phần Giải pháp và Dịch vụ phần mềm Nam Việt</span>
      </div>
      <div class="partners">
        <img src="https://fpt.vn/storage/upload/images/logo/fpt_logo.png" alt="FPT" height="40" v-if="false" /> <!-- Mock partner logo -->
        <span class="partner-text">FPT</span>
        <span class="logo-text ms-3">NAVI <span class="fw-light">SOFTWARE</span></span>
      </div>
    </div>

    <div class="login-wrapper">
      <div class="login-box">
        <h2 class="sys-title">Hệ thống BO</h2>
        
        <form @submit.prevent="handleLogin" class="login-form">
          <ValidationInput
            v-model="username"
            placeholder="Tài khoản"
            :error="errors.username"
            @blur="validateUsername"
          >
            <template #icon>
              <i class="fas fa-user text-muted"></i>
            </template>
          </ValidationInput>

          <ValidationInput
            v-model="password"
            :type="showPassword ? 'text' : 'password'"
            placeholder="Mật khẩu"
            :error="errors.password"
            @blur="validatePassword"
          >
            <template #icon>
              <i class="fas" :class="showPassword ? 'fa-eye-slash' : 'fa-eye'" @click="showPassword = !showPassword"></i>
              <i class="fas fa-lock text-muted ms-2" style="margin-left:8px"></i>
            </template>
          </ValidationInput>

          <div class="remember-me mt-2 mb-4">
            <label class="custom-checkbox">
              <input type="checkbox" v-model="rememberMe" />
              <span class="checkmark"></span>
              Ghi nhớ mật khẩu
            </label>
          </div>

          <button type="submit" class="btn btn-login" :disabled="authStore.loading || !isValid">
            {{ authStore.loading ? 'Đang xử lý...' : 'Đăng nhập' }}
          </button>
        </form>
        
        <div v-if="authStore.error" class="alert-error mt-3">
          {{ authStore.error }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import ValidationInput from '@/components/common/ValidationInput.vue'

const router = useRouter()
const authStore = useAuthStore()

const username = ref('')
const password = ref('')
const rememberMe = ref(false)
const showPassword = ref(false)

const errors = reactive({
  username: '',
  password: ''
})

// Auto-fill if remembered
onMounted(() => {
  const savedUser = localStorage.getItem('nav_remember_user')
  if (savedUser) {
    username.value = savedUser
    rememberMe.value = true
  }
})

const validateUsername = () => {
  if (!username.value) {
    errors.username = 'Tài khoản không được để trống'
    return false
  }
  errors.username = ''
  return true
}

const validatePassword = () => {
  if (!password.value) {
    errors.password = 'Mật khẩu không được để trống'
    return false
  }
  errors.password = ''
  return true
}

const isValid = computed(() => {
  return username.value && password.value && !errors.username && !errors.password
})

const handleLogin = async () => {
  if (!validateUsername() || !validatePassword()) return
  
  try {
    await authStore.login({ 
      userName: username.value, 
      password: password.value, 
      rememberMe: rememberMe.value 
    })
    
    if (rememberMe.value) {
      localStorage.setItem('nav_remember_user', username.value)
    } else {
      localStorage.removeItem('nav_remember_user')
    }
    router.push('/dashboard')
  } catch (err) {
    // Error is handled by authStore
  }
}
</script>

<style scoped>
.login-container {
  height: 100vh;
  display: flex;
  flex-direction: column;
  background: linear-gradient(135deg, #e0e7ff 0%, #3b82f6 100%); /* gradient backgound similiar to mockup */
}

.login-header {
  padding: 20px 40px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: transparent;
}

.logo-text {
  color: #f59e0b;
  font-weight: bold;
  font-size: 24px;
}

.company-name {
  color: #1e3a8a;
  font-weight: 600;
  margin-left: 15px;
  font-size: 14px;
}

.partners {
  display: flex;
  align-items: center;
  gap: 15px;
}

.partner-text {
  font-weight: 800;
  font-size: 24px;
  color: #ea580c;
  font-style: italic;
}

.login-wrapper {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
}

.login-box {
  width: 100%;
  max-width: 400px;
  text-align: center;
}

.sys-title {
  color: #1e3a8a;
  font-size: 28px;
  font-weight: 700;
  margin-bottom: 30px;
}

.login-form :deep(.form-control) {
  height: 40px;
  border-radius: 8px;
  padding-left: 36px; /* Space for icon */
}

.login-form :deep(.input-icon) {
  left: 10px;
  right: auto;
}

/* Reverse icon position for password show/hide */
.login-form :deep(.input-icon) {
  display: flex;
  align-items: center;
}

.remember-me {
  text-align: left;
  font-size: 13px;
  color: #4b5563;
}

.btn-login {
  width: 100%;
  height: 40px;
  background: #3b82f6;
  color: white;
  font-weight: 600;
  border-radius: 8px;
  margin-top: 10px;
}

.btn-login:disabled {
  background: #9ca3af;
}

.alert-error {
  color: #ef4444;
  font-size: 13px;
  background: #fef2f2;
  padding: 8px;
  border-radius: 4px;
}

.ms-2 { margin-left: 8px; }
.ms-3 { margin-left: 16px; }
</style>
