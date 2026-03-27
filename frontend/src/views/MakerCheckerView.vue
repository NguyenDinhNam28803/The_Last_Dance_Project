<template>
  <div class="main-content flex-column">
    <Toolbar 
      title="Phê Duyệt (Maker - Checker)" 
      :features="toolbarFeatures" 
      @action="handleAction"
    />
    
    <div class="filter-bar p-3 bg-white border-bottom">
      <div class="row align-items-end">
        <div class="col col-3">
          <ValidationInput label="Mã giao dịch (Trans ID)" v-model="filters.transId" />
        </div>
        <div class="col col-3">
          <div class="form-group">
            <label class="form-label">Loại nghiệp vụ</label>
            <select class="form-control" v-model="filters.type">
              <option value="">Tất cả</option>
              <option value="CustomerContact">Liên hệ khách hàng</option>
              <option value="Customer">Khách hàng</option>
            </select>
          </div>
        </div>
        <div class="col col-3">
          <button class="btn btn-primary" @click="fetchRequests"><i class="fas fa-search"></i> Lọc dữ liệu</button>
        </div>
      </div>
    </div>
    
    <div class="grid-content bg-white flex-1 p-3" style="overflow-y:auto">
      <div v-if="loading" class="text-center py-5">
        <div class="spinner-border text-primary"></div>
        <p class="mt-2">Đang tải danh sách chờ duyệt...</p>
      </div>
      <table v-else class="grid-table">
        <thead>
          <tr>
            <th width="40"><input type="checkbox" @change="toggleAll" :checked="isAllSelected"/></th>
            <th>Mã GD</th>
            <th>Loại nghiệp vụ</th>
            <th>Hành động</th>
            <th>Dữ liệu chi tiết</th>
            <th>Người tạo</th>
            <th>Thời gian tạo</th>
            <th>Trạng thái</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="filteredRequests.length === 0">
            <td colspan="8" class="text-center text-muted py-4">Không có dữ liệu chờ duyệt</td>
          </tr>
          <tr 
            v-for="req in filteredRequests" 
            :key="req.mtTranId"
            :class="{ active: selectedIds.includes(req.mtTranId) }"
            @click="toggleSelection(req.mtTranId)"
          >
            <td @click.stop><input type="checkbox" :checked="selectedIds.includes(req.mtTranId)" @change="toggleSelection(req.mtTranId)"/></td>
            <td class="font-weight-bold text-primary">{{ req.mtTranId }}</td>
            <td>{{ req.objChange }}</td>
            <td>
              <span class="badge" :class="getActionBadge(req.mtlType)">{{ req.mtlType }}</span>
            </td>
            <td class="text-truncate" style="max-width: 300px" :title="req.description">
              {{ req.description }}
            </td>
            <td>{{ req.maker }}</td>
            <td>{{ formatDate(req.actionDate) }}</td>
            <td>
              <span class="badge" :class="getStatusBadge(req.mtlStatus)">{{ getStatusLabel(req.mtlStatus) }}</span>
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
          <div class="form-group">
            <label class="form-label">Lý do từ chối <span class="required">*</span></label>
            <textarea class="form-control" v-model="rejectReason" rows="3" placeholder="Nhập lý do từ chối..."></textarea>
            <div v-if="rejectError" class="text-danger small mt-1">{{ rejectError }}</div>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-outline" @click="showRejectModal = false">Hủy</button>
          <button class="btn btn-danger" @click="confirmReject">Xác nhận Từ chối</button>
        </div>
      </div>
    </div>

    <div v-if="toast" class="toast" :class="'toast-' + toast.type">{{ toast.message }}</div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import Toolbar from '@/components/common/Toolbar.vue'
import ValidationInput from '@/components/common/ValidationInput.vue'
import { MakerCheckerService } from '@/services/api'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const requests = ref([])
const loading = ref(false)
const filters = ref({ transId: '', type: '' })
const selectedIds = ref([])
const toast = ref(null)

const showRejectModal = ref(false)
const rejectReason = ref('')
const rejectError = ref('')

const toolbarFeatures = computed(() => {
  const features = ['Refresh']
  if (authStore.isMaker || authStore.isAdmin) {
    features.push('Cancel')
  }
  if (authStore.isChecker || authStore.isAdmin) {
    features.push('Approve', 'Reject')
  }
  return features
})

const fetchRequests = async () => {
  loading.value = true
  try {
    const response = await MakerCheckerService.getPending()
    requests.value = response.data
    selectedIds.value = []
  } catch (err) {
    showToast('Không thể tải danh sách chờ duyệt', 'error')
  } finally {
    loading.value = false
  }
}

onMounted(fetchRequests)

const filteredRequests = computed(() => {
  return requests.value.filter(r => {
    const matchId = !filters.value.transId || r.mtTranId.toString().includes(filters.value.transId)
    const matchType = !filters.value.type || r.objChange === filters.value.type
    return matchId && matchType
  })
})

const isAllSelected = computed(() => {
  return filteredRequests.value.length > 0 && selectedIds.value.length === filteredRequests.value.length
})

const toggleAll = (e) => {
  if (e.target.checked) selectedIds.value = filteredRequests.value.map(r => r.mtTranId)
  else selectedIds.value = []
}

const toggleSelection = (id) => {
  const idx = selectedIds.value.indexOf(id)
  if (idx > -1) selectedIds.value.splice(idx, 1)
  else selectedIds.value.push(id)
}

const handleAction = async (action) => {
  if (action === 'refresh') {
    await fetchRequests()
  } else if (action === 'cancel') {
    if (selectedIds.value.length === 0) return alert('Vui lòng chọn ít nhất 1 bản ghi để Hủy!')
    
    // Validate: Chỉ có thể hủy những bản ghi do mình tạo ra (hoặc Admin có thể hủy tất cả)
    const myRequests = requests.value.filter(r => selectedIds.value.includes(r.mtTranId) && r.maker === authStore.user?.id)
    if (myRequests.length !== selectedIds.value.length && !authStore.isAdmin) {
      return alert('Bạn chỉ có thể hủy các yêu cầu do chính bạn tạo ra!')
    }

    if (confirm(`Bạn chắc chắn muốn HỦY ${selectedIds.value.length} bản ghi?`)) {
      let successCount = 0
      for (const id of selectedIds.value) {
        try {
          await MakerCheckerService.cancel(id)
          successCount++
        } catch (err) {
          console.error(`Lỗi khi hủy ID ${id}:`, err)
        }
      }
      showToast(`Đã hủy thành công ${successCount} yêu cầu.`, 'success')
      await fetchRequests()
    }
  } else if (action === 'approve') {
    if (selectedIds.value.length === 0) return alert('Vui lòng chọn ít nhất 1 bản ghi để Duyệt!')
    
    // Validate: Không được duyệt giao dịch của chính mình (Maker != Checker)
    const ownRequests = requests.value.filter(r => selectedIds.value.includes(r.mtTranId) && r.maker === authStore.user?.id)
    if (ownRequests.length > 0 && !authStore.isAdmin) {
      return alert('Bạn không thể DUYỆT yêu cầu do chính mình tạo ra!')
    }

    if (confirm(`Bạn chắc chắn muốn DUYỆT ${selectedIds.value.length} bản ghi?`)) {
      let successCount = 0
      for (const id of selectedIds.value) {
        try {
          await MakerCheckerService.approve(id)
          successCount++
        } catch (err) {
          console.error(`Lỗi khi duyệt ID ${id}:`, err)
        }
      }
      showToast(`Đã duyệt thành công ${successCount} yêu cầu.`, 'success')
      await fetchRequests()
    }
  } else if (action === 'reject') {
    if (selectedIds.value.length === 0) return alert('Vui lòng chọn ít nhất 1 bản ghi để Từ chối!')
    
    // Validate: Không được từ chối giao dịch của chính mình
    const ownRequests = requests.value.filter(r => selectedIds.value.includes(r.mtTranId) && r.maker === authStore.user?.id)
    if (ownRequests.length > 0 && !authStore.isAdmin) {
      return alert('Bạn không thể TỪ CHỐI yêu cầu do chính mình tạo ra! Vui lòng sử dụng tính năng Hủy.')
    }

    rejectReason.value = ''
    rejectError.value = ''
    showRejectModal.value = true
  }
}

const confirmReject = async () => {
  if (!rejectReason.value) {
    rejectError.value = 'Bạn phải nhập lý do từ chối!'
    return
  }
  
  let successCount = 0
  for (const id of selectedIds.value) {
    try {
      await MakerCheckerService.reject(id, rejectReason.value)
      successCount++
    } catch (err) {
      console.error(`Lỗi khi từ chối ID ${id}:`, err)
    }
  }
  
  showRejectModal.value = false
  showToast(`Đã từ chối ${successCount} yêu cầu.`, 'success')
  await fetchRequests()
}

const getActionBadge = (type) => {
  if (type === 'CREATE') return 'bg-success text-white'
  if (type === 'UPDATE') return 'bg-warning text-dark'
  if (type === 'DELETE') return 'bg-danger text-white'
  return 'bg-secondary text-white'
}

const getStatusBadge = (status) => {
  if (status === 'N') return 'bg-warning text-dark'
  if (status === 'A') return 'bg-success text-white'
  if (status === 'R') return 'bg-danger text-white'
  return 'bg-secondary text-white'
}

const getStatusLabel = (status) => {
  if (status === 'N') return 'Chờ duyệt'
  if (status === 'A') return 'Đã duyệt'
  if (status === 'R') return 'Từ chối'
  return status
}

const formatDate = (dateStr) => {
  if (!dateStr) return '—'
  const date = new Date(dateStr)
  return date.toLocaleString('vi-VN')
}

function showToast(message, type = 'success') {
  toast.value = { message, type }
  setTimeout(() => { toast.value = null }, 3000)
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
.modal-content { background: white; padding: 20px; border-radius: 8px; width: 100%; }
.text-truncate { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }

.toast {
  position: fixed; bottom: 20px; right: 20px; padding: 12px 24px;
  border-radius: 8px; color: white; z-index: 2000; box-shadow: 0 4px 12px rgba(0,0,0,0.15);
}
.toast-success { background: #10b981; }
.toast-error { background: #ef4444; }
</style>
