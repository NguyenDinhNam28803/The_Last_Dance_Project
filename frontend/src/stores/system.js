import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { AuditService, MakerCheckerService, SystemCodeService } from '@/services/api'
import { useApi } from '@/composables/useApi'

export const useAuditStore = defineStore('audit', () => {
  const logs = ref([])
  const getLogsApi = useApi(AuditService.getAll)

  async function fetchLogs() {
    logs.value = await getLogsApi.execute()
    return logs.value
  }

  return { logs, fetchLogs, loading: getLogsApi.loading, error: getLogsApi.error }
})

export const useMakerCheckerStore = defineStore('makerChecker', () => {
  const submitApi = useApi(MakerCheckerService.submit)
  const approveApi = useApi(MakerCheckerService.approve)
  const rejectApi = useApi(MakerCheckerService.reject)

  return { 
    submit: submitApi.execute, 
    approve: approveApi.execute, 
    reject: rejectApi.execute,
    loading: computed(() => submitApi.loading.value || approveApi.loading.value),
    error: computed(() => submitApi.error.value || approveApi.error.value)
  }
})

export const useSystemCodeStore = defineStore('systemCode', () => {
  const codes = ref([])
  const getAllApi = useApi(SystemCodeService.getAll)
  const createApi = useApi(SystemCodeService.create)

  async function fetchAll() {
    codes.value = await getAllApi.execute()
    return codes.value
  }

  return { 
    codes, 
    fetchAll, 
    create: createApi.execute,
    loading: computed(() => getAllApi.loading.value || createApi.loading.value),
    error: computed(() => getAllApi.error.value || createApi.error.value)
  }
})
