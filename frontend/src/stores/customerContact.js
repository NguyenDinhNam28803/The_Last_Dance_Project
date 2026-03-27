import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { CustomerContactService } from '@/services/api'
import { useApi } from '@/composables/useApi'

export const useCustomerContactStore = defineStore('customerContact', () => {
  const contacts = ref([])
  
  const getAllApi = useApi(CustomerContactService.getAll)
  const getByIdApi = useApi(CustomerContactService.getById)
  const getByCustomerApi = useApi(CustomerContactService.getByCustomer)
  const createApi = useApi(CustomerContactService.create)
  const updateApi = useApi(CustomerContactService.update)
  const deleteApi = useApi(CustomerContactService.delete)
  const setDefaultApi = useApi(CustomerContactService.setDefault)

  async function fetchAll() {
    contacts.value = await getAllApi.execute()
    return contacts.value
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
    loading: computed(() => getAllApi.loading.value || createApi.loading.value),
    error: computed(() => getAllApi.error.value || createApi.error.value)
  }
})
