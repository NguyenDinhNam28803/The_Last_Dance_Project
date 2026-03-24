<template>
  <div class="app-container">
    <TopNav />
   
    <router-view v-slot="{ Component }">
      <transition name="fade" mode="out-in">
        <component :is="Component" />
      </transition>
    </router-view>
   
    <div class="bottom-bar">
      <div>Kết nối thành công</div>
      <div class="d-flex align-items-center gap-3">
        <span>{{ authStore.currentUser?.username }}</span>
        <span>1.0.1103</span>
        <span>{{ currentDate }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import TopNav from './TopNav.vue'
import { useAuthStore } from '@/stores/auth'
import { ref, onMounted } from 'vue'

const authStore = useAuthStore()
const currentDate = ref('')

onMounted(() => {
  const now = new Date()
  currentDate.value = now.toLocaleDateString('vi-VN', { 
    day: '2-digit', month: '2-digit', year: 'numeric' 
  })
})
</script>

<style scoped>
.fade-enter-active, .fade-leave-active { transition: opacity 0.2s ease; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
</style>