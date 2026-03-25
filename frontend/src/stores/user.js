import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { UserService } from '@/services/api'
import { useApi } from '@/composables/useApi'

export const useUserStore = defineStore('user', () => {
  const users = ref([])
  
  const getAllApi = useApi(UserService.getAll)
  const getByIdApi = useApi(UserService.getById)
  const createApi = useApi(UserService.create)
  const updateApi = useApi(UserService.update)
  const updateRoleApi = useApi(UserService.updateRole)
  const toggleStatusApi = useApi(UserService.toggleStatus)

  async function fetchAll() {
    const data = await getAllApi.execute()
    users.value = data
    return data
  }

  return { 
    users, 
    fetchAll, 
    create: createApi.execute, 
    update: updateApi.execute, 
    updateRole: updateRoleApi.execute, 
    toggleStatus: toggleStatusApi.execute,
    loading: computed(() => getAllApi.loading.value || createApi.loading.value || updateApi.loading.value),
    error: computed(() => getAllApi.error.value || createApi.error.value || updateApi.error.value)
  }
})
