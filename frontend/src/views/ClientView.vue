<template>
  <div class="main-content">
    
    <!-- LEFT PANEL: THÔNG TIN CHI TIẾT (FORM) -->
    <div class="form-panel">
      <Toolbar 
        title="Tài khoản" 
        :features="toolbarFeatures" 
        @action="handleToolbarAction" 
      />
      
      <!-- MULTIPLE SELECT WARNING -->
      <div v-if="selectedIds.length > 1" class="alert-info mt-3 p-2 text-center" style="font-size: 12.5px; background: #eef2ff; color: #4338ca; border-radius: 4px; border: 1px dashed #c7d2fe;">
        <i class="fas fa-info-circle"></i> Đang chọn <b>{{ selectedIds.length }}</b> khách hàng. Tính năng Sửa bị khóa để thao tác hàng loạt.
      </div>

      <div :style="selectedIds.length > 1 ? 'opacity: 0.4; pointer-events: none; filter: grayscale(100%);' : ''">
      <!-- Tabs -->
      <div class="tabs-header" :class="selectedIds.length > 1 ? 'mt-3 mb-4' : 'mt-4 mb-4'">
        <button 
          class="tab-btn" 
          :class="{ active: currentTab === 'general' }" 
          @click="currentTab = 'general'"
        >
          Thông tin chung
        </button>
        <button 
          class="tab-btn" 
          :class="{ active: currentTab === 'contact' }" 
          @click="currentTab = 'contact'"
        >
          Liên hệ
        </button>
      </div>
      
      <!-- TAB 1: THÔNG TIN CHUNG -->
      <div v-show="currentTab === 'general'" class="tab-content">
        <!-- Section: Thông tin khách hàng -->
        <fieldset class="form-section">
          <legend class="form-section-title">Thông tin khách hàng</legend>
          <div class="row">
            <div class="col col-3">
              <ValidationInput 
                label="Mã khách hàng" 
                required 
                v-model="formData.clientId" 
                :disabled="mode !== 'add'" 
                :error="errors.clientId" 
                placeholder="Nhập mã 6 số"
              />
            </div>
            <div class="col col-4">
              <ValidationInput 
                label="Tên khách hàng" 
                required 
                v-model="formData.name" 
                :disabled="mode === 'view'" 
                :error="errors.name" 
              />
            </div>
            <div class="col col-5">
              <ValidationInput 
                label="Tên KH (Ngôn ngữ khác)" 
                v-model="formData.nameOther" 
                :disabled="mode === 'view'" 
              />
            </div>
          </div>
          
          <div class="row">
            <div class="col col-3">
              <div class="form-group">
                <label class="form-label required">Loại hình khách hàng</label>
                <select class="form-control" v-model="formData.registrationType" :disabled="mode === 'view'" @change="handleRegTypeChange">
                  <option value="">-- Lựa chọn --</option>
                  <option value="LOCAL_RETAIL">Cá nhân trong nước</option>
                  <option value="FOREIGN_RETAIL">Cá nhân nước ngoài</option>
                  <option value="LOCAL_INSTITUTION">Tổ chức trong nước</option>
                  <option value="FOREIGN_INSTITUTION">Tổ chức nước ngoài</option>
                </select>
              </div>
            </div>
            <div class="col col-3">
              <div class="form-group">
                <label class="form-label required">Loại ĐKSH</label>
                <select class="form-control" v-model="formData.idType" :disabled="mode === 'view'">
                  <option value="">-- Lựa chọn --</option>
                  <option v-for="opt in allowedIdTypes" :key="opt.value" :value="opt.value">{{ opt.label }}</option>
                </select>
              </div>
            </div>
            <div class="col col-3">
              <ValidationInput 
                label="Số ĐKSH" 
                required 
                v-model="formData.idNumber" 
                :disabled="mode === 'view'" 
                :error="errors.idNumber" 
              />
            </div>
            <div class="col col-3">
              <ValidationInput 
                type="date" 
                label="Ngày cấp" 
                required 
                v-model="formData.idIssueDate" 
                :disabled="mode === 'view'" 
                :error="errors.idIssueDate" 
                @blur="validateDates" 
              />
            </div>
          </div>
          
          <div class="row">
            <div class="col col-3">
              <ValidationInput 
                type="date" 
                label="Ngày hết hạn" 
                :required="isExpiryRequired" 
                v-model="formData.idExpiryDate" 
                :disabled="mode === 'view'" 
                :error="errors.idExpiryDate" 
                @blur="validateDates" 
              />
            </div>
            <div class="col col-3">
              <ValidationInput 
                label="Nơi cấp" 
                required 
                v-model="formData.idIssuePlace" 
                :disabled="mode === 'view'" 
                :error="errors.idIssuePlace" 
              />
            </div>
            <div class="col col-3">
              <div class="form-group">
                <label class="form-label required">Quốc tịch</label>
                <select class="form-control" v-model="formData.nationality" :disabled="mode === 'view' || isLocalClient" @change="handleNationalityChange">
                  <option value="">-- Lựa chọn --</option>
                  <option value="VN">Việt Nam</option>
                  <option value="US">Hoa Kỳ (UNITED STATES)</option>
                  <option value="JP">Nhật Bản</option>
                  <option value="KR">Hàn Quốc</option>
                </select>
              </div>
            </div>
            <div class="col col-3">
              <div class="form-group">
                <label class="form-label" :class="{ required: isInstitutionTypeRequired }">Loại tổ chức</label>
                <select class="form-control" v-model="formData.institutionType" :disabled="mode === 'view' || !isInstitution">
                  <option value="">-- Lựa chọn --</option>
                  <option value="BANK">Ngân hàng</option>
                  <option value="FUND">Quỹ đầu tư</option>
                  <option value="OTHER">Khác</option>
                </select>
              </div>
            </div>
          </div>
        </fieldset>
        
        <!-- Section: Thông tin khác -->
        <fieldset class="form-section">
          <legend class="form-section-title">Thông tin khác</legend>
          <div class="row">
            <div class="col col-3">
              <div class="form-group">
                <label class="form-label">Giới tính</label>
                <select class="form-control" v-model="formData.gender" :disabled="mode === 'view' || isInstitution">
                  <option value="">-- Lựa chọn --</option>
                  <option value="M">Nam</option>
                  <option value="F">Nữ</option>
                  <option value="O">Khác</option>
                </select>
              </div>
            </div>
            <div class="col col-3">
              <ValidationInput 
                type="date" 
                label="Ngày sinh" 
                :required="!isInstitution" 
                v-model="formData.dob" 
                :disabled="mode === 'view' || isInstitution" 
                :error="errors.dob" 
                @blur="validateDob" 
              />
            </div>
            <div class="col col-3">
              <div class="form-group">
                <label class="form-label">
                  <input type="checkbox" v-model="formData.fatca" :disabled="mode === 'view'"> FATCA (NĐT có dấu hiệu Hoa Kỳ)
                </label>
              </div>
            </div>
            <div class="col col-3">
              <ValidationInput 
                label="Số TK lưu ký" 
                required 
                v-model="formData.custodyId" 
                :disabled="mode === 'view'" 
                :error="errors.custodyId" 
              />
            </div>
          </div>
        </fieldset>
      </div>
      
      <!-- TAB 2: LIÊN HỆ -->
      <div v-show="currentTab === 'contact'" class="tab-content">
        <div class="alert-warning p-3 mb-4" v-if="!hasRequiredContacts">
          Bắt buộc nhập ít nhất 1 Địa chỉ và 1 Số điện thoại!
        </div>
        
        <div class="contact-form-area p-3 mb-4" v-if="mode !== 'view'">
          <h6 class="mb-3 font-weight-bold">Thêm / Sửa Liên Hệ</h6>
          <div class="row">
            <div class="col col-3">
              <div class="form-group">
                <label class="form-label required">Loại liên hệ</label>
                <select class="form-control" v-model="contactForm.contactType">
                  <option value="ADDRESS">Địa chỉ</option>
                  <option value="PHONE">Số điện thoại</option>
                  <option value="EMAIL">Email</option>
                  <option value="FAX">Fax</option>
                </select>
              </div>
            </div>
            <div class="col col-3">
              <div class="form-group">
                <label class="form-label required">Loại thông tin</label>
                <select class="form-control" v-model="contactForm.infoType">
                  <template v-if="contactForm.contactType === 'ADDRESS'">
                    <option value="PERMANENT">Địa chỉ thường trú</option>
                    <option value="CONTACT">Địa chỉ liên lạc</option>
                    <option value="BILLING">Địa chỉ hóa đơn</option>
                  </template>
                  <template v-else-if="contactForm.contactType === 'PHONE'">
                    <option value="HOME">Tel nhà</option>
                    <option value="OFFICE">Tel công ty</option>
                    <option value="MOBILE">Di động</option>
                  </template>
                  <template v-else>
                    <option value="DEFAULT">Mặc định</option>
                  </template>
                </select>
              </div>
            </div>
            <div class="col col-4">
              <ValidationInput label="Giá trị" required v-model="contactForm.detailInfo" />
            </div>
            <div class="col col-2 d-flex align-items-end">
              <button class="btn btn-primary w-100" @click="saveContact">Lưu LH</button>
            </div>
          </div>
        </div>
        
        <table class="data-table">
          <thead>
            <tr>
              <th>Loại LH</th>
              <th>Loại TT</th>
              <th>Giá trị</th>
              <th>Mặc định</th>
              <th width="100" v-if="mode !== 'view'">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="contacts.length === 0">
              <td colspan="5" class="text-center text-muted">Chưa có thông tin liên hệ</td>
            </tr>
            <tr v-for="(c, i) in contacts" :key="i">
              <td>{{ c.contactType }}</td>
              <td>{{ c.infoType }}</td>
              <td>{{ c.detailInfo }}</td>
              <td><input type="checkbox" :checked="c.isDefault" readonly disabled/></td>
              <td v-if="mode !== 'view'">
                <button class="btn btn-outline btn-sm" @click="removeContact(i)">Xóa</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      
      </div><!-- End Wrapper Form Or Warning -->
    </div>
    
    <!-- RIGHT PANEL: GRID TÌM KIẾM -->
    <div class="grid-panel" :class="{ collapsed: isGridCollapsed }">
      <div class="grid-panel-header">
        <span>K.Quả ({{ clients.length }})</span>
        <button class="btn btn-outline" style="padding: 0 10px;" @click="isGridCollapsed = !isGridCollapsed" title="Đóng/Mở lưới">
          <i class="fas" :class="isGridCollapsed ? 'fa-chevron-left' : 'fa-chevron-right'"></i>
          <span v-if="!isGridCollapsed" style="margin-left:5px">Thu gọn</span>
        </button>
      </div>
      
      <div class="grid-content">
        <table class="grid-table">
          <thead>
            <tr>
              <th width="40">
                <input type="checkbox" 
                  :checked="clients.length > 0 && selectedIds.length === clients.length" 
                  @change="e => selectedIds = e.target.checked ? clients.map(c => c.clientId) : []" 
                />
              </th>
              <th>Mã KH</th>
              <th>Tên khách hàng</th>
              <th>Trạng thái</th>
            </tr>
          </thead>
          <tbody>
            <tr 
              v-for="cli in clients" 
              :key="cli.clientId"
              :class="{ active: selectedClient?.clientId === cli.clientId }"
              @click="selectClient(cli)"
            >
              <td @click.stop><input type="checkbox" :value="cli.clientId" v-model="selectedIds" /></td>
              <td class="font-weight-bold">{{ cli.clientId }}</td>
              <td>{{ cli.name }}</td>
              <td>
                <span class="status-badge" :class="getStatusBadgeClass(cli.status)">
                  {{ cli.status }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import Toolbar from '@/components/common/Toolbar.vue'
import ValidationInput from '@/components/common/ValidationInput.vue'
import { mockClients } from '@/data/mockData'

const isGridCollapsed = ref(false)
const currentTab = ref('general')
const mode = ref('view')

const clients = ref(mockClients)
const selectedClient = ref(null)
const selectedIds = ref([])
const formData = ref({})
const errors = ref({})
const contacts = ref([])
const contactForm = ref({ contactType: 'ADDRESS', infoType: 'PERMANENT', detailInfo: '' })

const toolbarFeatures = computed(() => {
  if (mode.value === 'add' || mode.value === 'edit') return ['Save', 'Cancel']
  
  const features = ['Search', 'Add']
  // Chỉ khả dụng Sửa/Copy nếu đang chọn <= 1 bản ghi
  if (selectedIds.value.length <= 1) {
    features.push('Edit', 'Copy')
  }
  
  features.push('Delete', 'Approve', 'Reject', 'Audit')
  return features
})

const handleToolbarAction = (action) => {
  if (action === 'add') {
    mode.value = 'add'
    resetForm()
  } else if (action === 'cancel') {
    mode.value = 'view'
    if (selectedClient.value) selectClient(selectedClient.value)
    else resetForm()
  } else if (action === 'save') {
    if (validateForm()) {
      alert('Lưu thành công! Trạng thái bản ghi chuyển sang Pending Insert/Update')
      mode.value = 'view'
    }
  }
}

// Computed Properties
const isLocalClient = computed(() => ['LOCAL_RETAIL', 'LOCAL_INSTITUTION'].includes(formData.value.registrationType))
const isInstitution = computed(() => ['LOCAL_INSTITUTION', 'FOREIGN_INSTITUTION'].includes(formData.value.registrationType))
const isExpiryRequired = computed(() => {
  return formData.value.idType === 'PASSPORT' && !isLocalClient.value ||
         ['CMND', 'CCCD'].includes(formData.value.idType) && isLocalClient.value
})
const isInstitutionTypeRequired = computed(() => isInstitution.value)
const hasRequiredContacts = computed(() => {
  const hasAddr = contacts.value.some(c => c.contactType === 'ADDRESS')
  const hasPhone = contacts.value.some(c => c.contactType === 'PHONE')
  return hasAddr && hasPhone
})

const allowedIdTypes = computed(() => {
  const t = formData.value.registrationType
  if (!t) return []
  if (t === 'LOCAL_RETAIL') return [
    {value: 'CMND', label: 'Chứng minh thư'},
    {value: 'CCCD', label: 'Căn cước công dân'},
    {value: 'PASSPORT', label: 'Hộ chiếu'}
  ]
  if (t === 'FOREIGN_RETAIL') return [
    {value: 'PASSPORT', label: 'Hộ chiếu'},
    {value: 'OTHER', label: 'Giấy tờ khác'}
  ]
  if (t === 'LOCAL_INSTITUTION' || t === 'FOREIGN_INSTITUTION') return [
    {value: 'BUSINESS_REG', label: 'Giấy phép ĐKKD'},
    {value: 'GOV_ID', label: 'Cơ quan chính phủ'}
  ]
  return []
})

// Handlers
const handleRegTypeChange = () => {
  formData.value.idType = ''
  if (isLocalClient.value) {
    formData.value.nationality = 'VN'
    formData.value.fatca = false
  }
}

const handleNationalityChange = () => {
  if (formData.value.nationality === 'US') {
    formData.value.fatca = true
  }
}

const saveContact = () => {
  if (!contactForm.value.detailInfo) return
  contacts.value.push({ ...contactForm.value, isDefault: false })
  contactForm.value.detailInfo = ''
}

const removeContact = (idx) => { 
  contacts.value.splice(idx, 1) 
}

const resetForm = () => {
  selectedClient.value = null
  formData.value = { nationality: 'VN', registrationType: 'LOCAL_RETAIL', gender: 'M' }
  contacts.value = []
  errors.value = {}
}

const selectClient = (cli) => {
  if (mode.value !== 'view') {
    if (!confirm('Bạn có thay đổi chưa lưu, muốn chuyển Data?')) return
  }
  selectedClient.value = cli
  mode.value = 'view'
  formData.value = { ...cli }
  contacts.value = cli.contacts || []
}

// Validation
const validateDates = () => {
  errors.value.idIssueDate = ''
  errors.value.idExpiryDate = ''
  const today = new Date()
  const issue = new Date(formData.value.idIssueDate)
  if (issue > today) errors.value.idIssueDate = 'Ngày cấp không được lớn hơn ngày hiện tại'
  
  if (formData.value.idExpiryDate) {
    const expr = new Date(formData.value.idExpiryDate)
    if (expr <= issue) errors.value.idExpiryDate = 'Ngày hết hạn phải lớn hơn Ngày cấp'
  }
}

const validateDob = () => {
  errors.value.dob = ''
  if (!isInstitution.value && formData.value.dob) {
    const ageDiffMs = Date.now() - new Date(formData.value.dob).getTime()
    const ageDate = new Date(ageDiffMs)
    const age = Math.abs(ageDate.getUTCFullYear() - 1970)
    if (age < 18) errors.value.dob = 'Khách hàng phải từ 18 tuổi trở lên'
  }
}

const validateForm = () => {
  errors.value = {}
  let isValid = true
  
  if (!formData.value.clientId || !/^\d{6}$/.test(formData.value.clientId)) {
    errors.value.clientId = 'Mã KH phải là 6 chữ số'
    isValid = false
  }
  if (!formData.value.name) { 
    errors.value.name = 'Không được để trống'; 
    isValid = false 
  }
  if (!formData.value.registrationType) { 
    errors.value.registrationType = 'Không được để trống'; 
    isValid = false 
  }
  if (!formData.value.idType) { 
    errors.value.idType = 'Không được để trống'; 
    isValid = false 
  }
  
  validateDates()
  validateDob()
  if (errors.value.idIssueDate || errors.value.idExpiryDate || errors.value.dob) isValid = false
  
  if (!hasRequiredContacts.value) {
    currentTab.value = 'contact'
    isValid = false
  }
  
  return isValid
}

const getStatusBadgeClass = (status) => {
  if (status === 'Pending Insert') return 'warning'
  if (status === 'Active') return 'success'
  if (status === 'Closed') return 'danger'
  return 'secondary'
}
</script>

<style scoped>
/* Tabs */
.tabs-header {
  border-bottom: 2px solid var(--color-border);
  display: flex;
  gap: 20px;
}
.tab-btn {
  background: none;
  border: none;
  padding: 8px 16px;
  font-size: 14px;
  font-weight: 600;
  color: var(--color-text-secondary);
  cursor: pointer;
  border-bottom: 2px solid transparent;
}
.tab-btn.active {
  color: var(--color-primary);
  border-bottom-color: var(--color-primary);
}

/* Alert */
.alert-warning {
  background: #fef3c7;
  color: #92400e;
  border: 1px solid #fcd34d;
  border-radius: 4px;
  font-size: 13px;
}

/* Contact form area */
.contact-form-area {
  border: 1px dashed #d1d5db;
  border-radius: 6px;
  background: #f9fafb;
}

/* ==================== STATUS BADGE - GIỮ KÍCH THƯỚC CŨ, KHÔNG XUỐNG DÒNG ==================== */
.status-badge {
  display: inline-block;
  padding: 3px 9px;
  border-radius: 9999px;
  font-size: 11.5px;
  font-weight: 500;
  white-space: nowrap;        /* Không cho chữ xuống dòng */
  line-height: 1.3;
  min-width: 78px;
  text-align: center;
}

/* Màu badge */
.status-badge.success {
  background-color: #10b981;
  color: white;
}
.status-badge.warning {
  background-color: #f59e0b;
  color: white;
}
.status-badge.danger {
  background-color: #ef4444;
  color: white;
}
.status-badge.secondary {
  background-color: #6b7280;
  color: white;
}

/* Table */
.grid-content {
  overflow-y: auto;
  flex: 1;
  padding: 0 8px;
}

.grid-table th,
.grid-table td {
  padding: 8px 10px;
  vertical-align: middle;
}
</style>