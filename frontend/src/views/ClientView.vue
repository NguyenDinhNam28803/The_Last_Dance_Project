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
              <ValidationInput label="Mã khách hàng" required v-model="formData.clientId" :disabled="mode !== 'add'" :error="errors.clientId" />
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
        <span>K.Quả ({{ clientStore.clients.length }})</span>
        <button class="btn btn-outline" @click="isGridCollapsed = !isGridCollapsed">Thu gọn</button>
      </div>
      
      <div class="grid-content">
        <table class="grid-table">
          <thead>
            <tr><th>Mã KH</th><th>Tên khách hàng</th></tr>
          </thead>
          <tbody>
            <tr v-for="cli in clientStore.clients" :key="cli.clientId" @click="selectClient(cli)">
              <td class="font-weight-bold">{{ cli.clientId }}</td>
              <td>{{ cli.name }}</td>
            </tr>
          </tbody>
        </table>
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
import { ImportExportService } from '@/services/api'

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

const toolbarFeatures = computed(() => {
  if (mode.value === 'add' || mode.value === 'edit') return ['Save', 'Cancel']
  
  const base = ['Search', 'Template', 'Export']
  if (authStore.isMaker || authStore.isAdmin) {
    base.push('Add', 'Edit', 'Delete', 'Import')
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