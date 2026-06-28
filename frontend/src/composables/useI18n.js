import { storeToRefs } from 'pinia'
import { useLocaleStore } from '@/stores/locale'

// Composable tiện dụng: { t, tc, locale, setLocale }
export function useI18n() {
  const store = useLocaleStore()
  const { locale } = storeToRefs(store)
  return {
    locale,
    setLocale: store.setLocale,
    t: store.t,
    tc: store.tc
  }
}
