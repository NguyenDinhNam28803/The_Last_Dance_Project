import { defineStore } from 'pinia'
import { ref } from 'vue'
import { messages } from '@/i18n/messages'

// Quản lý ngôn ngữ hệ thống (VN/EN), lưu localStorage, đổi tức thì không reload.
export const useLocaleStore = defineStore('locale', () => {
  const locale = ref(localStorage.getItem('locale') || 'vi')

  function setLocale(l) {
    if (l !== 'vi' && l !== 'en') return
    locale.value = l
    localStorage.setItem('locale', l)
  }

  // Dịch theo khóa
  function t(key) {
    const dict = messages[locale.value] || messages.vi
    return dict[key] ?? messages.vi[key] ?? key
  }

  // Dịch object hằng số dạng { vi, en } theo ngôn ngữ hiện tại
  function tc(obj) {
    if (!obj) return ''
    return obj[locale.value] ?? obj.vi ?? obj.en ?? ''
  }

  return { locale, setLocale, t, tc }
})
