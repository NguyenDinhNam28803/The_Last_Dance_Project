<template>
  <div class="main-content">
    
    <!-- LEFT PANEL: THÔNG TIN CHI TIẾT (FORM) -->
    <div class="form-panel">
      <Toolbar 
        title="Tài khoản" 
        :features="toolbarFeatures" 
        @action="handleToolbarAction" 
      />
      <input type="file" ref="fileInput" hidden accept=".xlsx, .xls" @change="handleFileUpload" />
      
      <!-- Loading State -->
      <div v-if="clientStore.loading" class="text-center p-4">Đang tải...</div>

      <div v-else :style="selectedIds.length > 1 ? 'opacity: 0.4; pointer-events: none; filter: grayscale(100%);' : ''">
      <!-- Tabs -->
      <div class="tabs-header mt-4 mb-4">
        <button class="tab-btn" :class="{ active: currentTab === 'general' }" @click="currentTab = 'general'">Thông tin chung</button>
        <button class="tab-btn" :class="{ active: currentTab === 'contact' }" @click="currentTab = 'contact'">Liên hệ</button>
      </div>
      
      <!-- TAB 1: THÔNG TIN CHUNG -->
      <div v-show="currentTab === 'general'" class="tab-content">
        <fieldset class="form-section">
          <legend class="form-section-title">Thông tin khách hàng</legend>
          <div class="row">
            <div class="col col-3">
              <div style="display:flex; align-items:flex-end; gap:6px;">
                <ValidationInput label="Mã khách hàng" required v-model="formData.clientId" :disabled="mode !== 'add'" :error="errors.clientId" style="flex:1;" />
                <button v-if="mode === 'add'" type="button" class="btn btn-outline" title="Tự sinh mã" @click="generateClientId">#</button>
              </div>
            </div>
            <div class="col col-4">
              <ValidationInput label="Tên khách hàng" required v-model="formData.name" :disabled="mode === 'view'" :error="errors.name" />
            </div>
          </div>
          <!-- (rest of the fields simplified for brevity, assume similar pattern) -->
        </fieldset>
      </div>
      
      <!-- TAB 2: LIÊN HỆ -->
      <div v-show="currentTab === 'contact'" class="tab-content">
        <table class="data-table">
          <thead>
            <tr><th>Loại LH</th><th>Giá trị</th></tr>
          </thead>
          <tbody>
            <tr v-for="(c, i) in contacts" :key="i">
              <td>{{ c.contactType }}</td>
              <td>{{ c.detailInfo }}</td>
            </tr>
          </tbody>
        </table>
      </div>
      
      </div>
    </div>
    
    <!-- RIGHT PANEL: GRID -->
    <div class="grid-panel" :class="{ collapsed: isGridCollapsed }">
      <div class="grid-panel-header">
        <span>K.Quả ({{ grid.totalItems.value }})</span>
        <button class="btn btn-outline" @click="isGridCollapsed = !isGridCollapsed">Thu gọn</button>
      </div>

      <!-- Thanh tìm kiếm + công cụ lưới -->
      <div class="grid-toolbar">
        <input
          v-model="grid.searchTerm.value"
          class="grid-search"
          placeholder="Tìm kiếm (MIN*, *MIN, *MIN*, chính xác)"
        />
        <button class="btn btn-outline" title="Xóa tìm kiếm" @click="grid.clearSearch()">Clear</button>
        <select v-model.number="grid.pageSize.value" class="grid-pagesize">
          <option v-for="s in grid.PAGE_SIZES" :key="s" :value="s">{{ s }}/trang</option>
        </select>
        <button class="btn btn-outline" title="Cấu hình cột" @click="showColumnConfig = !showColumnConfig">⚙ Cột</button>
      </div>

      <!-- Popup cấu hình cột -->
      <div v-if="showColumnConfig" class="column-config">
        <div v-for="col in grid.columns.value" :key="col.key" class="column-config-row">
          <label>
            <input type="checkbox" :checked="col.visible" @change="grid.toggleColumn(col.key)" />
            {{ col.label }}
          </label>
          <span class="column-config-actions">
            <button class="btn btn-ghost btn-sm" @click="grid.moveColumn(col.key, 'up')">↑</button>
            <button class="btn btn-ghost btn-sm" @click="grid.moveColumn(col.key, 'down')">↓</button>
          </span>
        </div>
        <div class="column-config-footer">
          <button class="btn btn-outline" @click="grid.resetColumns()">Mặc định</button>
          <button class="btn btn-primary" @click="showColumnConfig = false">Đóng</button>
        </div>
      </div>

      <div class="grid-content">
        <table class="grid-table">
          <thead>
            <tr>
              <th v-for="col in grid.visibleColumns.value" :key="col.key">{{ col.label }}</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="cli in grid.paged.value" :key="cli.custId || cli.clientId" @click="selectClient(cli)">
              <td v-for="col in grid.visibleColumns.value" :key="col.key" :class="{ 'font-weight-bold': col.key === 'custId' }">
                <span v-if="col.key === 'recordStatus'" class="badge" :class="recordStatusBadge(cli.recordStatus)">
                  {{ recordStatusLabel(cli.recordStatus) }}
                </span>
                <span v-else>{{ cellValue(cli, col.key) }}</span>
              </td>
            </tr>
            <tr v-if="grid.paged.value.length === 0">
              <td :colspan="grid.visibleColumns.value.length" class="empty-state">Không tìm thấy bản ghi nào</td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Phân trang -->
      <div class="grid-pagination">
        <button class="btn btn-outline btn-sm" :disabled="grid.currentPage.value <= 1" @click="grid.goToPage(grid.currentPage.value - 1)">‹</button>
        <span>Trang {{ grid.currentPage.value }} / {{ grid.totalPages.value }}</span>
        <button class="btn btn-outline btn-sm" :disabled="grid.currentPage.value >= grid.totalPages.value" @click="grid.goToPage(grid.currentPage.value + 1)">›</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import Toolbar from '@/components/common/Toolbar.vue'
import ValidationInput from '@/components/common/ValidationInput.vue'
import { useClientStore } from '@/stores/client'
import { useAuthStore } from '@/stores/auth'
import { useCustomerContactStore } from '@/stores/customerContact'
import { useNotify } from '@/composables/useNotify'
import { ImportExportService, ClientService } from '@/services/api'
import { recordStatusLabel, recordStatusBadge } from '@/constants/recordStatus'
import { useDataGrid } from '@/composables/useDataGrid'

const clientStore = useClientStore()
const authStore = useAuthStore()
const contactStore = useCustomerContactStore()
const notify = useNotify()

const contacts = ref([])
const contactLoading = ref(false)

const isGridCollapsed = ref(false)
const currentTab = ref('general')
const mode = ref('view')
const selectedIds = ref([])
const formData = ref({})
const errors = ref({})
const fileInput = ref(null)
const showColumnConfig = ref(false)

// Lưới Kết quả tìm kiếm: wildcard + phân trang + cấu hình cột (lưu theo user)
const clientColumns = [
  { key: 'custId', label: 'Mã KH' },
  { key: 'name', label: 'Tên khách hàng' },
  { key: 'registrationType', label: 'Loại hình' },
  { key: 'nationality', label: 'Quốc tịch' },
  { key: 'recordStatus', label: 'Trạng thái' }
]
const grid = useDataGrid({
  source: computed(() => clientStore.clients),
  columns: clientColumns,
  storageKey: 'grid.client',
  userId: authStore.user?.id || 'anon'
})

// Lấy giá trị ô hiển thị (xử lý riêng Mã KH dùng custId hoặc clientId)
const cellValue = (cli, key) => {
  if (key === 'custId') return cli.custId || cli.clientId || ''
  const v = cli[key]
  return (v === null || v === undefined || v === '') ? '—' : v
}

onMounted(() => clientStore.fetchAll())

// Format ISO date → yyyy-MM-dd cho input type=date, và hiển thị
const toDateInput = (iso) => iso ? iso.split('T')[0] : ''
const toDisplayDate = (iso) => iso
  ? new Date(iso).toLocaleString('vi-VN')
  : ''

const selectClient = (cli) => {
  formData.value = {
    ...cli,
    dateOfBirth: toDateInput(cli.dateOfBirth),
    createdDateDisplay: toDisplayDate(cli.createdDate),
  }
  selectedIds.value = [cli.custId]
  mode.value = 'view'
  // load contacts for this customer
  loadContactsFor(cli.custId)
}

async function loadContactsFor(custId) {
  contactLoading.value = true
  try {
    // contactStore.getByCustomer will call API appropriate for role
    contacts.value = await contactStore.getByCustomer(custId)
  } catch (e) {
    console.error('Failed to load contacts', e)
    notify.error('Không tải được thông tin liên hệ cho khách hàng')
    contacts.value = []
  } finally {
    contactLoading.value = false
  }
}

// Tự sinh ClientID qua API (icon #)
const generateClientId = async () => {
  try {
    const res = await ClientService.getNextId()
    formData.value.clientId = res.data.clientId
    if (errors.value.clientId) errors.value = { ...errors.value, clientId: undefined }
  } catch (e) {
    notify.error('Không sinh được mã khách hàng tự động')
  }
}

const validate = () => {
  const e = {}
  if (!formData.value.userName?.trim()) e.userName = 'Bắt buộc nhập'
  if (!formData.value.name?.trim()) e.name = 'Bắt buộc nhập'
  if (!formData.value.email?.trim()) e.email = 'Bắt buộc nhập'
  else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.value.email))
    e.email = 'Email không hợp lệ'
  errors.value = e
  return Object.keys(e).length === 0
}

// Toolbar động theo vai trò (URD: Maker vs Checker)
const toolbarFeatures = computed(() => {
  if (mode.value === 'add' || mode.value === 'edit') return ['Save', 'Cancel']

  const base = ['Search', 'Refresh', 'Audit', 'Template', 'Export']
  if (authStore.isMaker || authStore.isAdmin) {
    base.push('Add', 'Edit', 'Copy', 'Delete', 'Import')
  }
  if (authStore.isChecker || authStore.isAdmin) {
    base.push('Approve', 'Reject')
  }
  return base
})

const handleToolbarAction = async (action) => {
  if (action === 'add') {
    mode.value = 'add'
    formData.value = {}
  } else if (action === 'save') {
    if (!validate()) return
    try {
        if (mode.value === 'add') await clientStore.create(formData.value)
        else await clientStore.update(formData.value.custId, formData.value)
        alert('Lưu thành công!')
        mode.value = 'view'
        await clientStore.fetchAll()
    } catch (e) { alert('Lỗi') }
  } else if (action === 'export') {
    await exportExcel()
  } else if (action === 'import') {
    fileInput.value.click()
  } else if (action === 'template') {
    await downloadTemplate()
  } else if (action === 'refresh') {
    await clientStore.fetchAll()
    notify.info('Đã làm mới danh sách')
  } else if (action === 'cancel') {
    mode.value = 'view'
    errors.value = {}
  } else if (action === 'edit') {
    if (!selectedIds.value.length) { notify.warn('Vui lòng chọn một bản ghi'); return }
    mode.value = 'edit'
  } else if (action === 'search') {
    notify.info('Nhập tiêu chí vào ô tìm kiếm phía trên lưới (hỗ trợ *)')
  } else {
    // Approve/Reject/Delete/Copy/Audit: sẽ hoàn thiện ở giai đoạn tiếp theo
    notify.info(`Chức năng "${action}" đang được phát triển`)
  }
}

const downloadTemplate = async () => {
  try {
    const response = await ImportExportService.getTemplate()
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', 'Template_Import_Client.xlsx')
    document.body.appendChild(link)
    link.click()
    link.remove()
  } catch (error) {
    console.error("Lỗi tải template", error)
    alert('Lỗi khi tải template!')
  }
}

const exportExcel = async () => {
  try {
    const response = await ImportExportService.exportCustomers()
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    const dateStr = new Date().toISOString().replace(/[:.]/g, '')
    link.setAttribute('download', `Customers_Export_${dateStr}.xlsx`)
    document.body.appendChild(link)
    link.click()
    link.remove()
  } catch (error) {
    console.error("Lỗi tải file excel", error)
    alert('Lỗi khi xuất file Excel!')
  }
}

const handleFileUpload = async (event) => {
  const file = event.target.files[0]
  if (!file) return

  const formData = new FormData()
  formData.append('file', file)

  try {
    const res = await ImportExportService.importCustomers(formData)
    alert(`Import thành công: ${res.data.successCount} dòng.\nThất bại: ${res.data.failCount} dòng.`)
    await clientStore.fetchAll() // Reload lại danh sách
  } catch (error) {
    console.error("Lỗi import file", error)
    // Hiển thị chi tiết lỗi nếu có từ server
    if (error.response?.data?.errors?.length > 0) {
      const errMsgs = error.response.data.errors.map(e => `Dòng ${e.rowNumber}: ${e.reason}`).join('\n')
      alert(`Import gặp lôi:\n${errMsgs}`)
    } else {
      alert(error.response?.data || 'Lỗi khi import file Excel!')
    }
  } finally {
    event.target.value = null // reset input để cho phép chọn lại cùng 1 file
  }
}
</script>

<style scoped>
.grid-toolbar {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  border-bottom: 1px solid var(--color-border, #e5e7eb);
}
.grid-search {
  flex: 1;
  padding: 6px 10px;
  border: 1px solid var(--color-border, #d1d5db);
  border-radius: 4px;
  font-size: 13px;
}
.grid-pagesize {
  padding: 6px 8px;
  border: 1px solid var(--color-border, #d1d5db);
  border-radius: 4px;
  font-size: 13px;
}
.column-config {
  position: absolute;
  right: 12px;
  z-index: 20;
  background: #fff;
  border: 1px solid var(--color-border, #d1d5db);
  border-radius: 6px;
  box-shadow: 0 6px 18px rgba(0, 0, 0, 0.12);
  padding: 10px;
  min-width: 220px;
}
.column-config-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 4px 2px;
  font-size: 13px;
}
.column-config-actions {
  display: flex;
  gap: 2px;
}
.column-config-footer {
  display: flex;
  justify-content: flex-end;
  gap: 6px;
  margin-top: 8px;
  border-top: 1px solid var(--color-border, #eee);
  padding-top: 8px;
}
.grid-pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  padding: 8px;
  font-size: 13px;
  border-top: 1px solid var(--color-border, #e5e7eb);
}
.empty-state {
  text-align: center;
  color: #9ca3af;
  padding: 16px;
}
.badge {
  display: inline-block;
  padding: 2px 8px;
  border-radius: 10px;
  font-size: 12px;
  background: #e5e7eb;
  color: #374151;
}
.badge-success { background: #d1fae5; color: #065f46; }
.badge-warning { background: #fef3c7; color: #92400e; }
.badge-danger { background: #fee2e2; color: #991b1b; }
.badge-default { background: #e5e7eb; color: #374151; }
</style>