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
        <fieldset class="form-section" :disabled="isReadonly">
          <legend class="form-section-title">Thông tin khách hàng</legend>
          <div class="form-grid">
            <div class="fld">
              <div style="display:flex; align-items:flex-end; gap:6px;">
                <ValidationInput label="Mã khách hàng" required v-model="formData.clientId" :disabled="mode !== 'add'" :error="errors.clientId" style="flex:1;" />
                <button v-if="mode === 'add'" type="button" class="btn btn-outline" title="Tự sinh mã" @click="generateClientId">#</button>
              </div>
            </div>
            <ValidationInput class="fld" label="Tên khách hàng" required v-model="formData.name" :disabled="isReadonly" :error="errors.name" />
            <ValidationInput class="fld" label="Tên (ngôn ngữ khác)" v-model="formData.nameOther" :disabled="isReadonly" />
            <ValidationInput class="fld" label="Tên viết tắt" v-model="formData.shortName" :disabled="isReadonly" />

            <ValidationInput class="fld" label="Loại hình khách hàng" required :error="errors.registrationType">
              <select v-model="formData.registrationType" class="form-control" :disabled="isReadonly">
                <option value="">-- Chọn --</option>
                <option v-for="o in registrationTypes" :key="o.id" :value="o.id">{{ o.vi }}</option>
              </select>
            </ValidationInput>
            <ValidationInput class="fld" label="Quốc tịch" v-model="formData.nationality" :disabled="isReadonly" />

            <ValidationInput v-if="isInstitution" class="fld" label="Loại tổ chức" required :error="errors.institutionType">
              <select v-model="formData.institutionType" class="form-control" :disabled="isReadonly">
                <option value="">-- Chọn --</option>
                <option v-for="o in institutionOptions" :key="o.id" :value="o.id">{{ o.vi }}</option>
              </select>
            </ValidationInput>
            <ValidationInput v-if="isForeign" class="fld" label="Investor code" required v-model="formData.investorCode" :disabled="isReadonly" :error="errors.investorCode" />

            <ValidationInput v-if="isIndividual" class="fld" label="Giới tính" required :error="errors.gender">
              <select v-model="formData.gender" class="form-control" :disabled="isReadonly">
                <option value="">-- Chọn --</option>
                <option value="M">Nam</option>
                <option value="F">Nữ</option>
                <option value="O">Khác</option>
              </select>
            </ValidationInput>
            <ValidationInput v-if="isIndividual" class="fld" type="date" label="Ngày sinh" required v-model="formData.dateOfBirth" :disabled="isReadonly" :error="errors.dateOfBirth" />

            <ValidationInput class="fld" label="Nơi sinh" v-model="formData.placeOfBirth" :disabled="isReadonly" />
            <ValidationInput class="fld" label="Quốc gia cư trú" v-model="formData.residentCountryId" :disabled="isReadonly" />
            <ValidationInput class="fld" label="Email" v-model="formData.email" :disabled="isReadonly" :error="errors.email" />
            <ValidationInput class="fld" label="Số điện thoại" v-model="formData.phoneNumber" :disabled="isReadonly" />

            <ValidationInput class="fld" label="Kênh mở TK">
              <select v-model="formData.creationMethod" class="form-control" :disabled="isReadonly">
                <option value="">-- Chọn --</option>
                <option value="COUNTER">Tại quầy</option>
                <option value="EKYC">EKYC</option>
                <option value="BROKER">Qua môi giới</option>
              </select>
            </ValidationInput>
            <div class="fld checkbox-fld">
              <label><input type="checkbox" :checked="formData.isStaff === 'Y'" :disabled="isReadonly" @change="formData.isStaff = $event.target.checked ? 'Y' : 'N'" /> Nhân viên công ty</label>
            </div>
          </div>
        </fieldset>

        <!-- Trạng thái / thông tin sinh tự động (chỉ đọc) -->
        <fieldset class="form-section">
          <legend class="form-section-title">Trạng thái</legend>
          <div class="form-grid">
            <div class="fld">
              <label class="form-label">Trạng thái bản ghi</label>
              <span class="badge" :class="recordStatusBadge(formData.recordStatus)">{{ recordStatusLabel(formData.recordStatus) }}</span>
            </div>
            <div class="fld"><label class="form-label">Số TK lưu ký</label><div>{{ formData.custodyCd || '—' }}</div></div>
            <div class="fld"><label class="form-label">FATCA</label><div>{{ formData.fatca === 'Y' ? 'Có' : 'Không' }}</div></div>
          </div>
        </fieldset>
      </div>
      
      <!-- TAB 2: LIÊN HỆ -->
      <div v-show="currentTab === 'contact'" class="tab-content">
        <div v-if="!formData.custId" class="empty-state">
          Chọn (hoặc lưu) một khách hàng để quản lý thông tin liên hệ.
        </div>
        <template v-else>
          <div class="contact-header">
            <span>Danh sách liên hệ ({{ contacts.length }})</span>
            <button v-if="canManageContacts" class="btn btn-primary btn-sm" @click="openAddContact">+ Thêm liên hệ</button>
          </div>
          <p class="contact-note">Lưu ý: thêm/sửa/xóa liên hệ sẽ gửi yêu cầu chờ Checker duyệt.</p>

          <table class="data-table contact-table">
            <thead>
              <tr><th>Loại LH</th><th>Loại thông tin</th><th>Giá trị</th><th>Quốc gia</th><th>Mặc định</th><th v-if="canManageContacts">Thao tác</th></tr>
            </thead>
            <tbody>
              <tr v-for="c in contacts" :key="c.contactId">
                <td>{{ addTypeLabel(c.addType) }}</td>
                <td>{{ infoTypeLabel(c.addType, c.infoType) }}</td>
                <td>{{ c.contact }}</td>
                <td>{{ c.countryId || '—' }}</td>
                <td>{{ c.isDefault === 'Y' ? '✓' : '' }}</td>
                <td v-if="canManageContacts" class="actions-cell">
                  <button class="btn btn-ghost btn-sm" title="Sửa" @click="openEditContact(c)">✏️</button>
                  <button class="btn btn-ghost btn-sm" title="Đặt mặc định" :disabled="c.isDefault === 'Y'" @click="setDefaultContact(c)">⭐</button>
                  <button class="btn btn-ghost btn-sm" title="Xóa" @click="deleteContact(c)">🗑️</button>
                </td>
              </tr>
              <tr v-if="contacts.length === 0">
                <td :colspan="canManageContacts ? 6 : 5" class="empty-state">Chưa có thông tin liên hệ</td>
              </tr>
            </tbody>
          </table>

          <!-- Form thêm/sửa liên hệ -->
          <fieldset v-if="showContactForm" class="form-section contact-form">
            <legend class="form-section-title">{{ contactEditing ? 'Sửa liên hệ' : 'Thêm liên hệ' }}</legend>
            <div class="form-grid">
              <ValidationInput class="fld" label="Loại liên hệ" required>
                <select v-model="contactForm.addType" class="form-control" @change="onAddTypeChange">
                  <option v-for="o in addTypeOptions" :key="o.id" :value="o.id">{{ o.vi }}</option>
                </select>
              </ValidationInput>
              <ValidationInput class="fld" label="Loại thông tin" required>
                <select v-model="contactForm.infoType" class="form-control">
                  <option v-for="o in contactInfoOptions" :key="o.id" :value="o.id">{{ o.vi }}</option>
                </select>
              </ValidationInput>
              <ValidationInput class="fld" label="Giá trị" required v-model="contactForm.contact" :error="contactError" />
              <ValidationInput class="fld" label="Quốc gia" v-model="contactForm.countryId" />
              <ValidationInput v-if="contactForm.addType === 'F'" class="fld" label="Fax attention" v-model="contactForm.faxAttention" />
              <div class="fld checkbox-fld">
                <label><input type="checkbox" :checked="contactForm.isDefault === 'Y'" @change="contactForm.isDefault = $event.target.checked ? 'Y' : 'N'" /> Mặc định</label>
              </div>
            </div>
            <div class="contact-form-actions">
              <button class="btn btn-outline btn-sm" @click="showContactForm = false">Hủy</button>
              <button class="btn btn-primary btn-sm" @click="saveContact">Lưu</button>
            </div>
          </fieldset>
        </template>
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

      <!-- Thanh hành động hàng loạt -->
      <div v-if="canBulk && selectedRows.length > 0" class="bulk-bar">
        <span>Đã chọn {{ selectedRows.length }} bản ghi</span>
        <template v-if="authStore.isChecker || authStore.isAdmin">
          <button class="btn btn-success btn-sm" @click="bulkApprove">Duyệt</button>
          <button class="btn btn-danger btn-sm" @click="bulkReject">Từ chối</button>
        </template>
        <button v-if="authStore.isMaker || authStore.isAdmin" class="btn btn-outline btn-sm" @click="bulkDelete">Yêu cầu xóa</button>
        <button class="btn btn-ghost btn-sm" @click="selectedRows = []">Bỏ chọn</button>
      </div>

      <div class="grid-content">
        <table class="grid-table">
          <thead>
            <tr>
              <th v-if="canBulk" class="checkbox-col">
                <input type="checkbox" :checked="isAllPageSelected" @change="toggleAllPage" />
              </th>
              <th v-for="col in grid.visibleColumns.value" :key="col.key">{{ col.label }}</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="cli in grid.paged.value" :key="cli.custId || cli.clientId" @click="selectClient(cli)">
              <td v-if="canBulk" class="checkbox-col" @click.stop>
                <input type="checkbox" :checked="selectedRows.includes(cli.custId)" @change="toggleRow(cli.custId)" />
              </td>
              <td v-for="col in grid.visibleColumns.value" :key="col.key" :class="{ 'font-weight-bold': col.key === 'custId' }">
                <span v-if="col.key === 'recordStatus'" class="badge" :class="recordStatusBadge(cli.recordStatus)">
                  {{ recordStatusLabel(cli.recordStatus) }}
                </span>
                <span v-else>{{ cellValue(cli, col.key) }}</span>
              </td>
            </tr>
            <tr v-if="grid.paged.value.length === 0">
              <td :colspan="grid.visibleColumns.value.length + (canBulk ? 1 : 0)" class="empty-state">Không tìm thấy bản ghi nào</td>
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

    <!-- Popup Audit trail -->
    <div v-if="showAuditModal" class="modal-overlay" @click.self="showAuditModal = false">
      <div class="modal-container audit-modal">
        <div class="modal-header">
          <h3>🕓 Lịch sử thay đổi (Audit trail)</h3>
          <button class="modal-close" @click="showAuditModal = false">✕</button>
        </div>
        <div class="modal-body">
          <table class="data-table audit-table">
            <thead>
              <tr><th>Thời gian</th><th>Hành động</th><th>Trạng thái</th><th>Maker</th><th>Checker</th><th>Mô tả</th></tr>
            </thead>
            <tbody>
              <tr v-for="row in auditRows" :key="row.mtTranId">
                <td>{{ formatAuditDate(row.actionDate) }}</td>
                <td>{{ auditTypeLabel(row.mtlType) }}</td>
                <td>{{ auditStatusLabel(row.mtlStatus) }}</td>
                <td>{{ row.maker || '—' }}</td>
                <td>{{ row.checker || '—' }}</td>
                <td>{{ row.description || '—' }}</td>
              </tr>
              <tr v-if="auditRows.length === 0">
                <td colspan="6" class="empty-state">Chưa có lịch sử thay đổi</td>
              </tr>
            </tbody>
          </table>
        </div>
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
import { institutionTypesByScope } from '@/constants/institutionType'

// Loại hình khách hàng (URD)
const registrationTypes = [
  { id: 'LOCAL_RETAIL', vi: 'Cá nhân trong nước' },
  { id: 'FOREIGN_RETAIL', vi: 'Cá nhân nước ngoài' },
  { id: 'LOCAL_INSTITUTION', vi: 'Tổ chức trong nước' },
  { id: 'FOREIGN_INSTITUTION', vi: 'Tổ chức nước ngoài' }
]

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

// Các computed điều kiện theo loại hình khách hàng
const isReadonly = computed(() => mode.value === 'view')
const isInstitution = computed(() => (formData.value.registrationType || '').includes('INSTITUTION'))
const isForeign = computed(() => (formData.value.registrationType || '').includes('FOREIGN'))
const isIndividual = computed(() => (formData.value.registrationType || '').includes('RETAIL'))
const institutionOptions = computed(() =>
  institutionTypesByScope(isForeign.value ? 'FOREIGN' : 'DOMESTIC')
)

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
    clientId: cli.custId || cli.clientId,
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

// ===== Tab Liên hệ: danh mục & CRUD =====
const addTypeOptions = [
  { id: 'A', vi: 'Địa chỉ' },
  { id: 'S', vi: 'Số điện thoại' },
  { id: 'E', vi: 'Email' },
  { id: 'F', vi: 'Fax' }
]
const infoTypeByAddType = {
  A: [{ id: 'PER', vi: 'Thường trú/Trụ sở' }, { id: 'CON', vi: 'Liên lạc' }, { id: 'BIL', vi: 'Hóa đơn' }],
  S: [{ id: 'HOM', vi: 'Số nhà' }, { id: 'OFC', vi: 'Công ty' }, { id: 'MOB', vi: 'Di động' }],
  E: [{ id: 'EML', vi: 'Email' }],
  F: [{ id: 'FAX', vi: 'Fax' }]
}
const addTypeLabel = (t) => (addTypeOptions.find(o => o.id === t) || {}).vi || t || '—'
const infoTypeLabel = (addType, infoType) => {
  const list = infoTypeByAddType[addType] || []
  return (list.find(o => o.id === infoType) || {}).vi || infoType || '—'
}
const canManageContacts = computed(() => authStore.isMaker || authStore.isAdmin)

const showContactForm = ref(false)
const contactEditing = ref(false)
const contactError = ref('')
const contactForm = ref({ contactId: '', custId: '', addType: 'A', infoType: 'PER', contact: '', countryId: 'VN', faxAttention: '', isDefault: 'N', description: '' })
const contactInfoOptions = computed(() => infoTypeByAddType[contactForm.value.addType] || [])

const onAddTypeChange = () => {
  const opts = infoTypeByAddType[contactForm.value.addType] || []
  contactForm.value.infoType = opts.length ? opts[0].id : ''
}
const openAddContact = () => {
  contactForm.value = { contactId: '', custId: formData.value.custId, addType: 'A', infoType: 'PER', contact: '', countryId: 'VN', faxAttention: '', isDefault: 'N', description: '' }
  contactError.value = ''
  contactEditing.value = false
  showContactForm.value = true
}
const openEditContact = (c) => {
  contactForm.value = { ...c }
  contactError.value = ''
  contactEditing.value = true
  showContactForm.value = true
}
const saveContact = async () => {
  if (!contactForm.value.contact?.trim()) { contactError.value = 'Bắt buộc nhập'; return }
  try {
    if (contactEditing.value) {
      await contactStore.update(contactForm.value.contactId, contactForm.value)
    } else {
      const payload = { ...contactForm.value, contactId: `C${formData.value.custId}-${Date.now()}`.slice(0, 50) }
      await contactStore.create(payload)
    }
    notify.success('Đã gửi yêu cầu liên hệ (chờ duyệt)')
    showContactForm.value = false
    await loadContactsFor(formData.value.custId)
  } catch (e) {
    notify.error(e.response?.data?.message || e.response?.data || 'Lưu liên hệ thất bại')
  }
}
const deleteContact = async (c) => {
  if (!confirm('Gửi yêu cầu xóa liên hệ này?')) return
  try {
    await contactStore.delete(c.contactId)
    notify.success('Đã gửi yêu cầu xóa (chờ duyệt)')
    await loadContactsFor(formData.value.custId)
  } catch (e) {
    notify.error('Xóa liên hệ thất bại')
  }
}
const setDefaultContact = async (c) => {
  try {
    await contactStore.setDefault(c.contactId)
    notify.success('Đã đặt liên hệ mặc định')
    await loadContactsFor(formData.value.custId)
  } catch (e) {
    notify.error('Đặt mặc định thất bại')
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
  const f = formData.value
  if (mode.value === 'add' && !f.clientId?.trim()) e.clientId = 'Bắt buộc nhập (hoặc bấm #)'
  if (!f.name?.trim()) e.name = 'Bắt buộc nhập'
  if (!f.registrationType) e.registrationType = 'Bắt buộc chọn'
  if (f.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(f.email)) e.email = 'Email không hợp lệ'

  if (isIndividual.value) {
    if (!f.gender) e.gender = 'Bắt buộc với KH cá nhân'
    if (!f.dateOfBirth) e.dateOfBirth = 'Bắt buộc với KH cá nhân'
    else if (!isAdult(f.dateOfBirth)) e.dateOfBirth = 'Khách hàng phải đủ 18 tuổi'
  }
  if (isInstitution.value && !f.institutionType) e.institutionType = 'Bắt buộc với KH tổ chức'
  if (isForeign.value && !f.investorCode?.trim()) e.investorCode = 'Bắt buộc với KH nước ngoài'

  errors.value = e
  return Object.keys(e).length === 0
}

// Kiểm tra đủ 18 tuổi
const isAdult = (dob) => {
  const d = new Date(dob)
  if (isNaN(d)) return false
  const now = new Date()
  let age = now.getFullYear() - d.getFullYear()
  const m = now.getMonth() - d.getMonth()
  if (m < 0 || (m === 0 && now.getDate() < d.getDate())) age--
  return age >= 18
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
    if (!validate()) { notify.warn('Vui lòng kiểm tra lại các trường bắt buộc'); return }
    try {
        if (mode.value === 'add') {
          await clientStore.create(formData.value)
          notify.success('Tạo khách hàng thành công (chờ duyệt)')
        } else {
          await clientStore.update(formData.value.custId || formData.value.clientId, formData.value)
          notify.success('Cập nhật thành công (chờ duyệt)')
        }
        mode.value = 'view'
        await clientStore.fetchAll()
    } catch (e) {
        notify.error(e.response?.data?.message || 'Lưu thất bại')
    }
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
  } else if (action === 'approve') {
    await approveClient()
  } else if (action === 'reject') {
    await rejectClient()
  } else if (action === 'delete') {
    await requestDeleteClient()
  } else if (action === 'audit') {
    await openAudit()
  } else if (action === 'copy') {
    copyClient()
  } else {
    notify.info(`Chức năng "${action}" đang được phát triển`)
  }
}

// Copy record: sao chép dữ liệu bản ghi hiện tại sang chế độ thêm mới (URD)
const copyClient = () => {
  if (!formData.value.custId && !formData.value.clientId) {
    notify.warn('Vui lòng chọn một bản ghi để sao chép')
    return
  }
  const src = { ...formData.value }
  // Xóa các trường định danh/tự sinh để tạo bản ghi mới
  delete src.custId
  delete src.recordStatus
  delete src.custodyCd
  delete src.fatca
  delete src.createdDate
  delete src.createdBy
  delete src.createdDateDisplay
  delete src.approveBy
  delete src.approveDate
  src.clientId = ''
  formData.value = src
  selectedIds.value = []
  contacts.value = []
  currentTab.value = 'general'
  mode.value = 'add'
  notify.info('Đã sao chép dữ liệu. Nhập Mã KH mới rồi bấm Lưu.')
}

// Lấy ID bản ghi đang chọn
const currentClientId = () => formData.value.custId || formData.value.clientId || selectedIds.value[0]

const approveClient = async () => {
  const id = currentClientId()
  if (!id) { notify.warn('Vui lòng chọn một bản ghi'); return }
  if (!confirm('Xác nhận DUYỆT bản ghi này?')) return
  try {
    await ClientService.approve(id)
    notify.success('Đã duyệt thành công')
    await clientStore.fetchAll()
  } catch (e) {
    notify.error(e.response?.data?.message || 'Duyệt thất bại')
  }
}

const rejectClient = async () => {
  const id = currentClientId()
  if (!id) { notify.warn('Vui lòng chọn một bản ghi'); return }
  const reason = prompt('Nhập lý do từ chối (bắt buộc):')
  if (reason === null) return
  if (!reason.trim()) { notify.warn('Lý do từ chối là bắt buộc'); return }
  try {
    await ClientService.reject(id, reason.trim())
    notify.success('Đã từ chối bản ghi')
    await clientStore.fetchAll()
  } catch (e) {
    notify.error(e.response?.data?.message || 'Từ chối thất bại')
  }
}

const requestDeleteClient = async () => {
  const id = currentClientId()
  if (!id) { notify.warn('Vui lòng chọn một bản ghi'); return }
  if (!confirm('Gửi yêu cầu XÓA bản ghi đã duyệt này?')) return
  try {
    await ClientService.requestDelete(id)
    notify.success('Đã gửi yêu cầu xóa (chờ duyệt)')
    await clientStore.fetchAll()
  } catch (e) {
    notify.error(e.response?.data?.message || 'Yêu cầu xóa thất bại')
  }
}

// ===== Chọn nhiều bản ghi (bulk) =====
const selectedRows = ref([])
const canBulk = computed(() => authStore.isChecker || authStore.isMaker || authStore.isAdmin)
const isAllPageSelected = computed(() => {
  const ids = grid.paged.value.map(c => c.custId)
  return ids.length > 0 && ids.every(id => selectedRows.value.includes(id))
})
const toggleRow = (id) => {
  if (selectedRows.value.includes(id)) selectedRows.value = selectedRows.value.filter(x => x !== id)
  else selectedRows.value = [...selectedRows.value, id]
}
const toggleAllPage = () => {
  const ids = grid.paged.value.map(c => c.custId)
  if (isAllPageSelected.value) selectedRows.value = selectedRows.value.filter(id => !ids.includes(id))
  else selectedRows.value = [...new Set([...selectedRows.value, ...ids])]
}

// Chạy 1 thao tác theo lô và báo cáo kết quả
const runBulk = async (ids, fn, verb) => {
  const results = await Promise.allSettled(ids.map(id => fn(id)))
  const ok = results.filter(r => r.status === 'fulfilled').length
  const fail = results.length - ok
  if (fail === 0) notify.success(`${verb} thành công ${ok} bản ghi`)
  else notify.warn(`${verb}: thành công ${ok}, thất bại ${fail}`)
  selectedRows.value = []
  await clientStore.fetchAll()
}
const bulkApprove = async () => {
  if (!confirm(`Duyệt ${selectedRows.value.length} bản ghi đã chọn?`)) return
  await runBulk([...selectedRows.value], (id) => ClientService.approve(id), 'Duyệt')
}
const bulkReject = async () => {
  const reason = prompt('Nhập lý do từ chối (áp dụng cho tất cả bản ghi đã chọn):')
  if (reason === null) return
  if (!reason.trim()) { notify.warn('Lý do từ chối là bắt buộc'); return }
  await runBulk([...selectedRows.value], (id) => ClientService.reject(id, reason.trim()), 'Từ chối')
}
const bulkDelete = async () => {
  if (!confirm(`Gửi yêu cầu xóa ${selectedRows.value.length} bản ghi đã chọn?`)) return
  await runBulk([...selectedRows.value], (id) => ClientService.requestDelete(id), 'Yêu cầu xóa')
}

const auditRows = ref([])
const showAuditModal = ref(false)
const formatAuditDate = (iso) => iso ? new Date(iso).toLocaleString('vi-VN') : '—'
const auditTypeLabel = (t) => ({ I: 'Thêm', U: 'Sửa', D: 'Xóa' }[t] || t || '—')
const auditStatusLabel = (s) => ({ N: 'Chờ duyệt', A: 'Đã duyệt', R: 'Từ chối', C: 'Đã hủy' }[s] || s || '—')
const openAudit = async () => {
  const id = currentClientId()
  if (!id) { notify.warn('Vui lòng chọn một bản ghi'); return }
  try {
    const res = await ClientService.getAudit(id)
    auditRows.value = res.data || []
    showAuditModal.value = true
  } catch (e) {
    notify.error('Không tải được lịch sử thay đổi')
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

/* Modal Audit trail */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}
.modal-container {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
  width: min(900px, 92vw);
  max-height: 85vh;
  display: flex;
  flex-direction: column;
}
.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 16px;
  border-bottom: 1px solid #e5e7eb;
}
.modal-header h3 { margin: 0; font-size: 16px; }
.modal-close {
  border: none;
  background: transparent;
  font-size: 18px;
  cursor: pointer;
  color: #6b7280;
}
.modal-body { padding: 12px 16px; overflow: auto; }
.audit-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 13px;
}
.audit-table th, .audit-table td {
  border: 1px solid #e5e7eb;
  padding: 6px 8px;
  text-align: left;
}
.audit-table thead th { background: #f9fafb; }

/* Form lưới 2 cột */
.form-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 10px 16px;
}
.fld { min-width: 0; }
.checkbox-fld { display: flex; align-items: center; }
.checkbox-fld label { display: flex; align-items: center; gap: 6px; font-size: 13px; }

/* Thanh hành động hàng loạt */
.bulk-bar {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  background: #eff6ff;
  border-bottom: 1px solid #bfdbfe;
  font-size: 13px;
}
.checkbox-col { width: 36px; text-align: center; }

/* Biến thể nút (phòng khi chưa có ở global) */
.btn-success { background: #10b981; color: #fff; border: 1px solid #10b981; }
.btn-danger { background: #ef4444; color: #fff; border: 1px solid #ef4444; }
.btn-primary { background: #2563eb; color: #fff; border: 1px solid #2563eb; }
.btn-sm { padding: 4px 10px; font-size: 12px; }

/* Tab Liên hệ */
.contact-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 6px;
}
.contact-note { font-size: 12px; color: #6b7280; margin: 0 0 10px; }
.contact-table { width: 100%; border-collapse: collapse; font-size: 13px; }
.contact-table th, .contact-table td { border: 1px solid #e5e7eb; padding: 6px 8px; text-align: left; }
.contact-table thead th { background: #f9fafb; }
.actions-cell { display: flex; gap: 4px; }
.contact-form { margin-top: 12px; }
.contact-form-actions { display: flex; justify-content: flex-end; gap: 8px; margin-top: 10px; }
</style>