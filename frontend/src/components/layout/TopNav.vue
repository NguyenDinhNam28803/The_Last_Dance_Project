<template>
  <div class="topnav">
    <div class="topnav-menu">
      <!-- Logo: Chỉ là text, click được, không tô màu, không hiệu ứng nút -->
      <router-link to="/dashboard" class="logo-wrapper">
        <span class="logo-text">NAVI</span>
        <span class="logo-sub">SOFTWARE</span>
      </router-link>
     
      <router-link v-if="authStore.isAdmin" to="/system-code" class="topnav-link" active-class="active">{{ t('nav.systemParams') }}</router-link>
      <router-link v-if="authStore.isAdmin" to="/users" class="topnav-link" active-class="active">{{ t('nav.users') }}</router-link>
      <router-link to="/clients" class="topnav-link" active-class="active">{{ t('nav.clients') }}</router-link>
      <router-link to="/contacts" class="topnav-link" active-class="active">{{ t('nav.contacts') }}</router-link>
      <router-link v-if="authStore.isMaker || authStore.isChecker" to="/maker-checker" class="topnav-link" active-class="active">{{ t('nav.approval') }}</router-link>
      <router-link v-if="authStore.isAdmin" to="/audit" class="topnav-link" active-class="active">{{ t('nav.auditLog') }}</router-link>
    </div>
   
    <div class="user-info">
      <div class="top-search">
        <input type="text" :placeholder="t('nav.quickSearch')" />
      </div>

      <div class="lang-switch">
        <button :class="{ active: locale === 'vi' }" @click="setLocale('vi')">VN</button>
        <span class="lang-sep">|</span>
        <button :class="{ active: locale === 'en' }" @click="setLocale('en')">EN</button>
      </div>

      <div class="user-profile">
        <div class="user-details">
          <span class="username">{{ authStore.currentUser?.username }}</span>
          <span class="role">{{ authStore.currentUser?.role }}</span>
        </div>
        <button @click="handleLogout" class="logout-btn">{{ t('nav.logout') }}</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'
import { useI18n } from '@/composables/useI18n'

const authStore = useAuthStore()
const router = useRouter()
const { t, locale, setLocale } = useI18n()

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}
</script>

<style scoped>
.topnav {
  height: var(--topbar-height);
  background: #1e40af;
  color: white;
  display: flex;
  align-items: center;
  padding: 0;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.topnav-menu {
  display: flex;
  height: 100%;
  align-items: center;
}

/* ==================== LOGO - KHÔNG TÔ MÀU, KHÔNG HIỆU ỨNG NÚT ==================== */
.logo-wrapper {
  display: flex;
  align-items: center;
  padding: 0 28px;
  height: 100%;
  color: white;
  font-weight: 700;
  font-size: 18px;
  text-decoration: none;
  transition: color 0.2s;
}

.logo-wrapper:hover {
  color: #e0f2fe;           /* chỉ đổi màu chữ nhẹ khi hover */
}

.logo-text {
  font-weight: 700;
}

.logo-sub {
  font-weight: 300;
  font-size: 13.5px;
  margin-left: 6px;
  opacity: 0.9;
  letter-spacing: 1.5px;
}

/* Menu bình thường */
.topnav-link {
  padding: 0 20px;
  height: 100%;
  display: flex;
  align-items: center;
  font-size: 12.8px;
  font-weight: 500;
  color: white;
  text-transform: uppercase;
  text-decoration: none;
  border-right: 1px solid rgba(255,255,255,0.12);
  transition: all 0.2s;
}

.topnav-link:hover {
  background: rgba(255,255,255,0.12);
}

.topnav-link.active {
  background: white;
  color: #1e40af;
  font-weight: 600;
}

/* User section */
.user-info {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-left: auto;
  padding-right: 20px;
}

.top-search {
  background: white;
  border-radius: 4px;
  padding: 4px 12px;
  width: 220px;
}

.top-search input {
  border: none;
  outline: none;
  width: 100%;
  font-size: 13px;
}

.user-profile {
  display: flex;
  align-items: center;
  gap: 12px;
}

.username { font-weight: 600; font-size: 13.5px; }
.role { font-size: 11px; color: #cbd5e1; }

/* Bộ chuyển ngôn ngữ */
.lang-switch { display: flex; align-items: center; gap: 4px; }
.lang-switch button {
  background: none;
  border: none;
  color: #cbd5e1;
  cursor: pointer;
  font-size: 12.5px;
  font-weight: 600;
  padding: 2px 4px;
}
.lang-switch button.active { color: white; text-decoration: underline; }
.lang-sep { color: rgba(255,255,255,0.4); }

.logout-btn {
  background: none;
  border: none;
  color: white;
  cursor: pointer;
  padding: 6px 12px;
  border-radius: 4px;
}

.logout-btn:hover {
  background: rgba(255,255,255,0.15);
}
</style>