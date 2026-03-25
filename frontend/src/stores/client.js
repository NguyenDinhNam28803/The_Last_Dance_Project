import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { CustomerContactService } from '@/services/api' // Client often shares logic with customer contacts
import { useApi } from '@/composables/useApi'
import axios from 'axios'

// Assuming similar CRUD for Client based on system patterns
export const useClientStore = defineStore('client', () => {
  const clients = ref([])
  const clientApi = axios.create({ baseURL: 'https://localhost:7280/api/Client' })
  
  const getAllApi = useApi(() => clientApi.get('/'))
  const createApi = useApi((dto) => clientApi.post('/', dto))
  const updateApi = useApi((id, dto) => clientApi.put(`/${id}`, dto))
  const deleteApi = useApi((id) => clientApi.delete(`/${id}`))

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
