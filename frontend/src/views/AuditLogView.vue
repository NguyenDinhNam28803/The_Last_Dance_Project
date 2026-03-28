<template>
  <div class="main-content flex-column">
    <Toolbar 
      title="Nhật ký hệ thống (Audit Trail)" 
      :features="['Search', 'Refresh']" 
      @action="handleAction"
    />
    
    <div class="filter-bar p-3 bg-white border-bottom">
      <div class="row align-items-end">
        <div class="col col-3">
          <ValidationInput type="date" label="Từ ngày" v-model="filters.fromDate" />
        </div>
        <div class="col col-3">
          <ValidationInput type="date" label="Đến ngày" v-model="filters.toDate" />
        </div>
        <div class="col col-3">
          <ValidationInput label="Người thực hiện (Maker)" v-model="filters.maker" />
        </div>
        <div class="col col-3">
          <button class="btn btn-primary" @click="handleAction('search')"><i class="fas fa-search"></i> Tìm kiếm</button>
        </div>
      </div>
    </div>
    
    <div v-if="auditStore.loading" class="text-center p-5">Đang tải nhật ký...</div>
    <div v-else class="grid-content bg-white flex-1 p-3" style="overflow-y:auto">
      <table class="grid-table">
        <thead>
          <tr>
            <th width="40"></th>
            <th>Thời gian</th>
            <th>Đối tượng</th>
            <th>Loại tác động</th>
            <th>Maker</th>
            <th>Checker</th>
            <th>Trạng thái</th>
          </tr>
        </thead>
        <tbody>
          <template v-for="log in auditLogs" :key="log.id">
            <tr @click="log.expanded = !log.expanded" style="cursor:pointer">
              <td class="text-center">
                <i class="fas" :class="log.expanded ? 'fa-minus-square text-primary' : 'fa-plus-square text-muted'"></i>
              </td>
              <td>{{ log.busDate }}</td>
              <td class="font-weight-bold">{{ log.objChange }}</td>
              <td>
                <span class="badge" :class="getActionBadge(log.mtlType)">{{ log.mtlType }}</span>
              </td>
              <td>{{ log.maker }}</td>
              <td>{{ log.checker || '-' }}</td>
              <td>{{ log.mtlStatus }}</td>
            </tr>
            <tr v-if="log.expanded" class="expanded-row bg-light">
              <td colspan="7" class="p-3">
                <h6 class="text-primary font-weight-bold mb-2">Chi tiết thay đổi (History change)</h6>
                <table class="data-table">
                  <thead>
                    <tr>
                      <th>Trường thông tin</th>
                      <th>Giá trị cũ (Old Value)</th>
                      <th>Giá trị mới (New Value)</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(val, key) in parseJson(log.newVal)" :key="key">
                      <td class="font-weight-bold">{{ key }}</td>
                      <td class="text-danger text-decoration-line-through">{{ parseJson(log.oldVal)[key] || '-' }}</td>
                      <td class="text-success">{{ val }}</td>
                    </tr>
                  </tbody>
                </table>
              </td>
            </tr>
          </template>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useNotify } from '@/composables/useNotify'
import Toolbar from '@/components/common/Toolbar.vue'
import ValidationInput from '@/components/common/ValidationInput.vue'
import { useAuditStore } from '@/stores/system'

const auditStore = useAuditStore()
const filters = ref({ fromDate: '', toDate: '', maker: '' })

// Add local state for expanded rows to original data
const auditLogs = computed(() => auditStore.logs.map(l => ({ ...l, expanded: false })))

onMounted(() => auditStore.fetchLogs())

const parseJson = (str) => {
  try { return JSON.parse(str || '{}') } catch (e) { return {} }
}

const notify = useNotify()
const handleAction = async (action) => {
  if (action === 'refresh') {
    await auditStore.fetchLogs()
  } else if (action === 'search') {
    notify.info('Tìm kiếm theo filter chưa được triển khai phía backend')
  }
}

const getActionBadge = (type) => {
  if (type === 'ADD') return 'bg-success text-white'
  if (type === 'EDIT') return 'bg-warning text-dark'
  if (type === 'DELETE') return 'bg-danger text-white'
  return 'bg-secondary text-white'
}
</script>

<style scoped>
.flex-column { display: flex; flex-direction: column; height: 100vh; }
.flex-1 { flex: 1; }
.bg-light { background-color: #f8fafc; }
.text-decoration-line-through { text-decoration: line-through; }
</style>
