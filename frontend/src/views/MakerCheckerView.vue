<template>
  <div class="main-content flex-column">
    <Toolbar 
      title="Phê Duyệt (Maker - Checker)" 
      :features="['Approve', 'Reject', 'Refresh']" 
      @action="handleAction"
    />
    
    <div class="filter-bar p-3 bg-white border-bottom">
      <div class="row align-items-end">
        <div class="col col-3">
          <ValidationInput label="Mã giao dịch (Trans ID)" v-model="filters.transId" />
        </div>
        <div class="col col-3">
          <div class="form-group">
            <label class="form-label">Loại thay đổi</label>
            <select class="form-control" v-model="filters.type">
              <option value="">Tất cả</option>
              <option value="Khách hàng">Khách hàng</option>
              <option value="Liên hệ">Liên hệ</option>
            </select>
          </div>
        </div>
        <div class="col col-3">
          <button class="btn btn-primary" @click="handleAction('refresh')"><i class="fas fa-search"></i> Lọc dữ liệu</button>
        </div>
      </div>
    </div>
    
    <div class="grid-content bg-white flex-1 p-3" style="overflow-y:auto">
      <table class="grid-table">
        <thead>
          <tr>
            <th width="40"><input type="checkbox" @change="toggleAll" :checked="isAllSelected"/></th>
            <th>Mã GD</th>
            <th>Loại nghiệp vụ</th>
            <th>Hành động (Action)</th>
            <th>Nội dung</th>
            <th>Người tạo (Maker)</th>
            <th>Thời gian tạo</th>
            <th>Trạng thái</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="requests.length === 0">
            <td colspan="8" class="text-center text-muted py-4">Không có dữ liệu chờ duyệt</td>
          </tr>
          <tr 
            v-for="req in requests" 
            :key="req.mkckId"
            :class="{ active: selectedIds.includes(req.mkckId) }"
            @click="toggleSelection(req.mkckId)"
          >
            <td @click.stop><input type="checkbox" :checked="selectedIds.includes(req.mkckId)" @change="toggleSelection(req.mkckId)"/></td>
            <td class="font-weight-bold text-primary">{{ req.mkckId }}</td>
            <td>{{ req.tableName }}</td>
            <td>
              <span class="badge" :class="getActionBadge(req.actionType)">{{ req.actionType }}</span>
            </td>
            <td>{{ req.recordName }}</td>
            <td>{{ req.makerId }}</td>
            <td>{{ req.createdAt }}</td>
            <td>
              <span class="badge" :class="getStatusBadge(req.status)">{{ req.status }}</span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Reject Reason Modal -->
    <div v-if="showRejectModal" class="modal-overlay">
      <div class="modal-content" style="max-width: 500px">
        <div class="modal-header">
          <div class="modal-title text-danger"><i class="fas fa-times-circle"></i> Từ chối yêu cầu</div>
        </div>
        <div class="modal-body">
          <ValidationInput label="Lý do từ chối" required v-model="rejectReason" :error="rejectError" />
        </div>
        <div class="modal-footer">
          <button class="btn btn-outline" @click="showRejectModal = false">Hủy</button>
          <button class="btn btn-danger" @click="confirmReject">Xác nhận Từ chối</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import Toolbar from '@/components/common/Toolbar.vue'
import ValidationInput from '@/components/common/ValidationInput.vue'
import { mockMakerCheckerRequests } from '@/data/mockData'

const requests = ref(mockMakerCheckerRequests.filter(r => r.status === 'Pending'))
const filters = ref({ transId: '', type: '' })
const selectedIds = ref([])

const showRejectModal = ref(false)
const rejectReason = ref('')
const rejectError = ref('')

const isAllSelected = computed(() => {
  return requests.value.length > 0 && selectedIds.value.length === requests.value.length
})

const toggleAll = (e) => {
  if (e.target.checked) selectedIds.value = requests.value.map(r => r.mkckId)
  else selectedIds.value = []
}

const toggleSelection = (id) => {
  const idx = selectedIds.value.indexOf(id)
  if (idx > -1) selectedIds.value.splice(idx, 1)
  else selectedIds.value.push(id)
}

const handleAction = (action) => {
  if (action === 'refresh') {
    // refresh logic
  } else if (action === 'approve') {
    if (selectedIds.value.length === 0) return alert('Vui lòng chọn ít nhất 1 bản ghi để Duyệt!')
    if (confirm(`Bạn chắc chắn muốn DUYỆT ${selectedIds.value.length} bản ghi?`)) {
      requests.value = requests.value.filter(r => !selectedIds.value.includes(r.mkckId))
      selectedIds.value = []
      alert('Đã duyệt thành công!')
    }
  } else if (action === 'reject') {
    if (selectedIds.value.length === 0) return alert('Vui lòng chọn ít nhất 1 bản ghi để Từ chối!')
    rejectReason.value = ''
    rejectError.value = ''
    showRejectModal.value = true
  }
}

const confirmReject = () => {
  if (!rejectReason.value) {
    rejectError.value = 'Bạn phải nhập lý do từ chối!'
    return
  }
  requests.value = requests.value.filter(r => !selectedIds.value.includes(r.mkckId))
  selectedIds.value = []
  showRejectModal.value = false
  alert('Đã từ chối các bản ghi được chọn!')
}

const getActionBadge = (type) => {
  if (type === 'INSERT') return 'bg-success text-white'
  if (type === 'UPDATE') return 'bg-warning text-dark'
  if (type === 'DELETE') return 'bg-danger text-white'
  return 'bg-secondary text-white'
}

const getStatusBadge = (status) => {
  if (status === 'Pending') return 'bg-warning text-dark'
  if (status === 'Approved') return 'bg-success text-white'
  if (status === 'Rejected') return 'bg-danger text-white'
  return 'bg-secondary text-white'
}
</script>

<style scoped>
.flex-column { display: flex; flex-direction: column; height: 100vh; }
.flex-1 { flex: 1; }
.border-bottom { border-bottom: 1px solid var(--color-border); }
.py-4 { padding-top: 1.5rem; padding-bottom: 1.5rem; }
.bg-success { background: #10b981; }
.bg-danger { background: #ef4444; }
.bg-warning { background: #f59e0b; }
.modal-overlay {
  position: fixed; top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 1000;
}
</style>
