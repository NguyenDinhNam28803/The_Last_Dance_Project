<template>
  <div class="main-content">

    <!-- LEFT PANEL -->
    <div class="form-panel">
      <Toolbar
        title="Khách hàng"
        :features="toolbarFeatures"
        @action="handleToolbarAction"
      />

      <div v-if="clientStore.loading" class="text-center p-4">Đang tải...</div>

      <div v-else :style="selectedIds.length > 1 ? 'opacity: 0.4; pointer-events: none; filter: grayscale(100%);' : ''">
        <!-- Tabs -->
        <div class="tabs-header mt-4 mb-4">
          <button class="tab-btn" :class="{ active: currentTab === 'general' }" @click="currentTab = 'general'">Thông tin chung</button>
          <button class="tab-btn" :class="{ active: currentTab === 'contact' }" @click="currentTab = 'contact'">Liên hệ</button>
          <button class="tab-btn" :class="{ active: currentTab === 'system' }" @click="currentTab = 'system'">Hệ thống</button>
        </div>

        <!-- TAB 1: THÔNG TIN CHUNG -->
        <div v-show="currentTab === 'general'" class="tab-content">
          <fieldset class="form-section">
            <legend class="form-section-title">Thông tin tài khoản</legend>
            <div class="row">
              <div class="col col-4">
                <ValidationInput
                  label="Mã khách hàng"
                  v-model="formData.custId"
                  :disabled="true"
                />
              </div>
              <div class="col col-4">
                <ValidationInput
                  label="Tên đăng nhập"
                  required
                  v-model="formData.userName"
                  :disabled="mode === 'view'"
                  :error="errors.userName"
                />
              </div>
            </div>
            <div class="row">
              <div class="col col-4">
                <ValidationInput
                  label="Họ và tên"
                  required
                  v-model="formData.name"
                  :disabled="mode === 'view'"
                  :error="errors.name"
                />
              </div>
              <div class="col col-4">
                <ValidationInput
                  label="Tên khác"
                  v-model="formData.nameOther"
                  :disabled="mode === 'view'"
                />
              </div>
              <div class="col col-4">
                <ValidationInput
                  label="Tên viết tắt"
                  v-model="formData.shortName"
                  :disabled="mode === 'view'"
                />
              </div>
            </div>
          </fieldset>

          <fieldset class="form-section">
            <legend class="form-section-title">Thông tin cá nhân</legend>
            <div class="row">
              <div class="col col-3">
                <div class="form-group">
                  <label class="form-label">Giới tính</label>
                  <select
                    class="form-control"
                    v-model="formData.gender"
                    :disabled="mode === 'view'"
                  >
                    <option value="">-- Chọn --</option>
                    <option value="M">Nam</option>
                    <option value="F">Nữ</option>
                    <option value="O">Khác</option>
                  </select>
                </div>
              </div>
              <div class="col col-3">
                <ValidationInput
                  label="Ngày sinh"
                  type="date"
                  v-model="formData.dateOfBirth"
                  :disabled="mode === 'view'"
                />
              </div>
              <div class="col col-3">
                <ValidationInput
                  label="Quốc tịch"
                  v-model="formData.nationality"
                  :disabled="mode === 'view'"
                />
              </div>
              <div class="col col-3">
                <ValidationInput
                  label="Quốc gia cư trú"
                  v-model="formData.residentCountryId"
                  :disabled="mode === 'view'"
                />
              </div>
            </div>
          </fieldset>
        </div>

        <!-- TAB 2: LIÊN HỆ -->
        <div v-show="currentTab === 'contact'" class="tab-content">
          <fieldset class="form-section">
            <legend class="form-section-title">Thông tin liên hệ</legend>
            <div class="row">
              <div class="col col-6">
                <ValidationInput
                  label="Email"
                  type="email"
                  required
                  v-model="formData.email"
                  :disabled="mode === 'view'"
                  :error="errors.email"
                />
              </div>
              <div class="col col-4">
                <ValidationInput
                  label="Số điện thoại"
                  v-model="formData.phoneNumber"
                  :disabled="mode === 'view'"
                />
              </div>
            </div>
          </fieldset>
          <fieldset class="form-section mt-4">
            <legend class="form-section-title">Danh sách liên hệ</legend>
            <div v-if="contactLoading" class="text-center p-3">Đang tải liên hệ...</div>
            <div v-else>
              <table class="data-table">
                <thead>
                  <tr>
                    <th>Contact ID</th>
                    <th>Loại</th>
                    <th>Info Type</th>
                    <th>Nội dung</th>
                    <th>Mặc định</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="c in contacts" :key="c.contactId">
                    <td><code>{{ c.contactId }}</code></td>
                    <td>{{ addTypeLabels[c.addType] || c.addType }}</td>
                    <td>{{ c.infoType }}</td>
                    <td>{{ c.contact }}</td>
                    <td>{{ c.isDefault === 'Y' ? 'Có' : 'Không' }}</td>
                  </tr>
                  <tr v-if="contacts.length === 0">
                    <td colspan="5" class="text-center text-muted">Không có liên hệ nào</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </fieldset>
        </div>

        <!-- TAB 3: HỆ THỐNG -->
        <div v-show="currentTab === 'system'" class="tab-content">
          <fieldset class="form-section">
            <legend class="form-section-title">Phân quyền & Trạng thái</legend>
            <div class="row">
              <div class="col col-3">
                <div class="form-group">
                  <label class="form-label">Vai trò <span class="required">*</span></label>
                  <select
                    class="form-control"
                    v-model="formData.roleId"
                    :disabled="mode === 'view' || !authStore.isAdmin"
                  >
                    <option value="USER">User</option>
                    <option value="MAKER">Maker</option>
                    <option value="CHECKER">Checker</option>
                    <option value="ADMIN">Admin</option>
                    <option value="GUEST">Guest</option>
                  </select>
                </div>
              </div>
              <div class="col col-3">
                <div class="form-group">
                  <label class="form-label">Trạng thái</label>
                  <select
                    class="form-control"
                    v-model="formData.status"
                    :disabled="mode === 'view' || !authStore.isAdmin"
                  >
                    <option value="Active">Hoạt động</option>
                    <option value="Inactive">Ngừng hoạt động</option>
                  </select>
                </div>
              </div>
              <div class="col col-3">
                <div class="form-group">
                  <label class="form-label">Trạng thái bản ghi</label>
                  <select
                    class="form-control"
                    v-model="formData.recordStatus"
                    :disabled="mode === 'view' || !authStore.isAdmin"
                  >
                    <option value="1">Có hiệu lực</option>
                    <option value="0">Vô hiệu</option>
                  </select>
                </div>
              </div>
            </div>
          </fieldset>

          <fieldset class="form-section">
            <legend class="form-section-title">Thông tin tạo</legend>
            <div class="row">
              <div class="col col-4">
                <ValidationInput
                  label="Ngày tạo"
                  v-model="formData.createdDateDisplay"
                  :disabled="true"
                />
              </div>
              <div class="col col-4">
                <ValidationInput
                  label="Người tạo"
                  v-model="formData.createdBy"
                  :disabled="true"
                />
              </div>
            </div>
          </fieldset>
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
            <tr>
              <th>Tên đăng nhập</th>
              <th>Họ và tên</th>
              <th>Email</th>
              <th>Vai trò</th>
              <th>Trạng thái</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="cli in clientStore.clients"
              :key="cli.custId"
              :class="{ selected: formData.custId === cli.custId }"
              @click="selectClient(cli)"
            >
              <td class="font-weight-bold">{{ cli.userName }}</td>
              <td>{{ cli.name }}</td>
              <td>{{ cli.email }}</td>
              <td>
                <span class="role-tag" :class="`role-${cli.roleId?.toLowerCase()}`">
                  {{ cli.roleId }}
                </span>
              </td>
              <td>
                <span class="status-dot" :class="cli.status === 'Active' ? 'active' : 'inactive'">
                  {{ cli.status === 'Active' ? 'Hoạt động' : 'Ngừng' }}
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
import { ref, computed, onMounted } from 'vue'
import Toolbar from '@/components/common/Toolbar.vue'
import ValidationInput from '@/components/common/ValidationInput.vue'
import { useClientStore } from '@/stores/client'
import { useAuthStore } from '@/stores/auth'
import { useCustomerContactStore } from '@/stores/customerContact'
import { addTypeLabels } from '@/data/mockData'
import { useNotify } from '@/composables/useNotify'

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
  const base = ['Search']
  if (authStore.isMaker || authStore.isAdmin) base.push('Add', 'Edit', 'Delete')
  return base
})

const handleToolbarAction = async (action) => {
  if (action === 'add') {
    formData.value = { status: 'Active', recordStatus: '1', roleId: 'USER' }
    selectedIds.value = []
    mode.value = 'add'
    currentTab.value = 'general'
  } else if (action === 'edit') {
    mode.value = 'edit'
  } else if (action === 'cancel') {
    mode.value = 'view'
    errors.value = {}
    if (selectedIds.value.length) {
      const original = clientStore.clients.find(c => c.custId === selectedIds.value[0])
      if (original) selectClient(original)
    } else {
      formData.value = {}
    }
  } else if (action === 'save') {
    if (!validate()) return
    try {
      // Loại bỏ field display-only trước khi gửi
      const { createdDateDisplay, ...payload } = formData.value
      if (mode.value === 'add') await clientStore.create(payload)
      else await clientStore.update(formData.value.custId, payload)
      mode.value = 'view'
      await clientStore.fetchAll()
    } catch {
      alert('Lưu thất bại, vui lòng thử lại.')
    }
  } else if (action === 'delete') {
    if (!formData.value.custId) return
    if (!confirm(`Xóa khách hàng "${formData.value.userName}"?`)) return
    try {
      await clientStore.delete(formData.value.custId)
      formData.value = {}
      selectedIds.value = []
      await clientStore.fetchAll()
    } catch {
      alert('Xóa thất bại.')
    }
  }
}
</script>

<style scoped>
.role-tag {
  padding: 2px 8px;
  border-radius: 9999px;
  font-size: 11px;
  font-weight: 600;
  text-transform: uppercase;
}
.role-admin    { background: #e0e7ff; color: #3730a3; }
.role-checker  { background: #e0f2fe; color: #0369a1; }
.role-maker    { background: #d1fae5; color: #065f46; }
.role-user     { background: #f3f4f6; color: #374151; }
.role-guest    { background: #f1f5f9; color: #64748b; }

.status-dot {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  font-size: 12px;
}
.status-dot::before {
  content: '';
  display: inline-block;
  width: 7px;
  height: 7px;
  border-radius: 50%;
}
.status-dot.active::before   { background: #10b981; }
.status-dot.inactive::before { background: #9ca3af; }

tr.selected td { background: #eff6ff; }
</style>