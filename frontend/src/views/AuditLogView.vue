<template>
  <div class="main-content flex-column">
    <Toolbar 
      title="Nhật ký hệ thống (Audit Trail)" 
      :features="['Search', 'Refresh', 'Clear']" 
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
    
    <div class="grid-content bg-white flex-1 p-3" style="overflow-y:auto">
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
          <template v-for="log in logs" :key="log.id">
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
                    <tr v-for="(val, key) in JSON.parse(log.newVal || '{}')" :key="key">
                      <td class="font-weight-bold">{{ key }}</td>
                      <td class="text-danger text-decoration-line-through">{{ JSON.parse(log.oldVal || '{}')[key] || '-' }}</td>
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
import { ref } from 'vue'
import Toolbar from '@/components/common/Toolbar.vue'
import ValidationInput from '@/components/common/ValidationInput.vue'
import { mockAuditLogs } from '@/data/mockData'

const filters = ref({ fromDate: '', toDate: '', maker: '' })

// Thêm prop `expanded` vào logs
const logs = ref(mockAuditLogs.map(l => ({ ...l, expanded: false })))

const handleAction = (action) => {
  if (action === 'search') {
    alert('Tìm kiếm audit logs...')
  } else if (action === 'clear') {
    filters.value = { fromDate: '', toDate: '', maker: '' }
  } else if (action === 'refresh') {
    // refresh logic
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
