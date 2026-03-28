import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { CustomerContactService } from '@/services/api'
import { useApi } from '@/composables/useApi'
import { useAuthStore } from '@/stores/auth'

export const useCustomerContactStore = defineStore('customerContact', () => {
  const contacts = ref([])
  const authStore = useAuthStore()
  
  const getAllApi = useApi(CustomerContactService.getAll)
  const getByIdApi = useApi(CustomerContactService.getById)
  const getByCustomerApi = useApi(CustomerContactService.getByCustomer)
  const createApi = useApi(CustomerContactService.create)
  const updateApi = useApi(CustomerContactService.update)
  const deleteApi = useApi(CustomerContactService.delete)
  const setDefaultApi = useApi(CustomerContactService.setDefault)

  async function fetchAll() {
    // If the current user is admin, fetch all; otherwise fetch contacts for current user
    try {
      if (authStore.isAdmin) {
        contacts.value = await getAllApi.execute()
      } else {
        const custId = authStore.currentUser?.id
        if (!custId) {
          contacts.value = []
        } else {
          contacts.value = await getByCustomerApi.execute(custId)
        }
      }
      return contacts.value
    } catch (e) {
      // bubble up error via individual api error states
      throw e
    }
  }

  return {
    contacts,
    fetchAll,
    getById: getByIdApi.execute,
    getByCustomer: getByCustomerApi.execute,
    create: createApi.execute,
    update: updateApi.execute,
    delete: deleteApi.execute,
    setDefault: setDefaultApi.execute,
    loading: computed(() => getAllApi.loading.value || getByCustomerApi.loading.value || createApi.loading.value || updateApi.loading.value),
    error: computed(() => getAllApi.error.value || getByCustomerApi.error.value || createApi.error.value || updateApi.error.value)
  }
})
