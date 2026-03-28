import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { useApi } from '@/composables/useApi'
import { ClientService } from '@/services/api'

// Assuming similar CRUD for Client based on system patterns
export const useClientStore = defineStore('client', () => {
  const clients = ref([])
  // Use centralized ClientService defined in services/api.js
  const getAllApi = useApi(ClientService.getAll)
  const createApi = useApi(ClientService.create)
  const updateApi = useApi(ClientService.update)
  const deleteApi = useApi(ClientService.delete)

  async function fetchAll() {
    clients.value = await getAllApi.execute()
    return clients.value
  }

  return {
    clients,
    fetchAll,
    create: createApi.execute,
    update: updateApi.execute,
    delete: deleteApi.execute,
    loading: computed(() => getAllApi.loading.value || createApi.loading.value),
    error: computed(() => getAllApi.error.value || createApi.error.value)
  }
})
