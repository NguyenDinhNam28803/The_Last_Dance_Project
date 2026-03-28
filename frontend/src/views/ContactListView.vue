<template>
  <div class="contact-list">
    <div class="page-header">
      <div>
        <h1 class="page-title">Thông tin liên hệ</h1>
        <p class="page-subtitle">Quản lý thông tin liên hệ khách hàng</p>
      </div>
      <button v-if="authStore.isMaker || authStore.isAdmin" class="btn btn-primary" @click="openCreateModal">➕ Thêm liên hệ</button>
    </div>

    <!-- Loading State -->
    <div v-if="contactStore.loading" class="text-center">Đang tải...</div>

    <!-- Filter -->
    <div v-else class="filter-bar">
      <div class="search-input">
        <span class="search-icon">🔍</span>
        <input v-model="search" type="text" class="form-control" placeholder="Tìm theo CustId, nội dung liên hệ..." />
      </div>
      <select v-model="filterType" class="form-control" style="width:160px;flex:none">
        <option value="">Tất cả loại</option>
        <option value="A">📍 Địa chỉ</option>
        <option value="S">📱 SMS/SĐT</option>
        <option value="E">📧 Email</option>
        <option value="F">📠 Fax</option>
      </select>
    </div>

    <!-- Table -->
    <div class="card">
      <div class="card-body" style="padding:0;overflow-x:auto">
        <table class="data-table">
          <thead>
            <tr>
              <th>Contact ID</th>
              <th>Cust ID</th>
              <th>Loại</th>
              <th>Nội dung</th>
              <th>Quốc gia</th>
              <th>Mặc định</th>
              <th>Mô tả</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="c in filteredContacts" :key="c.contactId">
              <td><code>{{ c.contactId }}</code></td>
              <td><code>{{ c.custId }}</code></td>
              <td><span class="badge badge-default">{{ getAddTypeLabel(c.addType) }}</span></td>
              <td><strong>{{ c.contact }}</strong></td>
              <td>{{ c.countryId }}</td>
              <td>
                <span v-if="c.isDefault === 'Y'" class="badge badge-active">✓ Mặc định</span>
                <span v-else class="badge badge-pending">—</span>
              </td>
              <td>{{ c.description || '—' }}</td>
              <td>
                <div v-if="authStore.isMaker || authStore.isAdmin" class="actions-cell">
                  <button class="btn btn-ghost btn-sm" @click="openEditModal(c)" title="Sửa">✏️</button>
                  <button class="btn btn-ghost btn-sm" @click="handleSetDefault(c)" title="Đặt mặc định" :disabled="c.isDefault === 'Y'">⭐</button>
                  <button class="btn btn-ghost btn-sm" @click="handleDelete(c)" title="Xóa">🗑️</button>
                </div>
                <div v-else class="text-muted small">Chỉ xem</div>
              </td>
            </tr>
            <tr v-if="filteredContacts.length === 0">
              <td colspan="8" class="empty-state">Không tìm thấy liên hệ nào</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="showModal" class="modal-overlay" @click.self="showModal = false">
      <div class="modal-container">
        <div class="modal-header">
          <h3>{{ isEditing ? '✏️ Sửa liên hệ' : '➕ Thêm liên hệ mới' }}</h3>
          <button class="modal-close" @click="showModal = false">✕</button>
        </div>
        <div class="modal-body">
          <template v-if="!isEditing">
            <div class="form-group">
              <label class="form-label">Contact ID <span class="required">*</span></label>
              <input v-model="modalForm.contactId" type="text" class="form-control" placeholder="VD: CT009" />
            </div>
            <div class="form-group">
              <label class="form-label">Customer ID <span class="required">*</span></label>
              <input v-model="modalForm.custId" type="text" class="form-control" placeholder="VD: CUST001" />
            </div>
          </template>
          <div class="form-row">
            <div class="form-group">
              <label class="form-label">Quốc gia <span class="required">*</span></label>
              <input v-model="modalForm.countryId" type="text" class="form-control" placeholder="VD: VN" />
            </div>
            <div class="form-group">
              <label class="form-label">Loại liên hệ <span class="required">*</span></label>
              <select v-model="modalForm.addType" class="form-control">
                <option value="A">📍 Địa chỉ</option>
                <option value="S">📱 SMS/SĐT</option>
                <option value="E">📧 Email</option>
                <option value="F">📠 Fax</option>
              </select>
            </div>
          </div>
          <div class="form-group">
            <label class="form-label">Info Type <span class="required">*</span></label>
            <select v-model="modalForm.infoType" class="form-control">
              <option value="TEP">TEP - Điện thoại</option>
              <option value="HOM">HOM - Nhà riêng</option>
              <option value="OFC">OFC - Văn phòng</option>
              <option value="OFT">OFT - Fax VP</option>
              <option value="MTL">MTL - Di động</option>
              <option value="EML">EML - Email</option>
              <option value="ARB">ARB - Khác</option>
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">Nội dung <span class="required">*</span></label>
            <input v-model="modalForm.contact" type="text" class="form-control" placeholder="Địa chỉ / SĐT / Email..." />
          </div>
          <div v-if="modalForm.addType === 'F'" class="form-group">
            <label class="form-label">Fax Attention</label>
            <input v-model="modalForm.faxAttention" type="text" class="form-control" placeholder="Người nhận fax" />
          </div>
          <div class="form-row">
            <div class="form-group">
              <label class="form-label">Mặc định</label>
              <select v-model="modalForm.isDefault" class="form-control">
                <option value="N">Không</option>
                <option value="Y">Có</option>
              </select>
            </div>
            <div class="form-group">
              <label class="form-label">Mô tả</label>
              <input v-model="modalForm.description" type="text" class="form-control" placeholder="Mô tả thêm" />
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-outline" @click="showModal = false">Hủy</button>
          <button class="btn btn-primary" @click="saveContact">💾 {{ isEditing ? 'Cập nhật' : 'Thêm mới' }}</button>
        </div>
      </div>
    </div>

    <div v-if="toast" class="toast" :class="'toast-' + toast.type">{{ toast.message }}</div>
  </div>
</template>

<script setup>
import { ref, computed, reactive, onMounted } from 'vue'
import { useCustomerContactStore } from '@/stores/customerContact'
import { useAuthStore } from '@/stores/auth'
import { addTypeLabels } from '../data/mockData'
import { useNotify } from '@/composables/useNotify'

const contactStore = useCustomerContactStore()
const authStore = useAuthStore()
const notify = useNotify()
const search = ref('')
const filterType = ref('')
const showModal = ref(false)
const isEditing = ref(false)

const modalForm = reactive({
  contactId: '', custId: '', countryId: 'VN', addType: 'A', infoType: 'TEP',
  contact: '', faxAttention: '', isDefault: 'N', description: ''
})

onMounted(() => contactStore.fetchAll())

const filteredContacts = computed(() => {
  return contactStore.contacts.filter(c => {
    const matchSearch = !search.value || [c.custId, c.contact, c.contactId].some(v => v?.toLowerCase().includes(search.value.toLowerCase()))
    const matchType = !filterType.value || c.addType === filterType.value
    return matchSearch && matchType
  })
})

function getAddTypeLabel(type) { return addTypeLabels[type] || type }

function openCreateModal() {
  isEditing.value = false
  Object.assign(modalForm, { contactId: '', custId: '', countryId: 'VN', addType: 'A', infoType: 'TEP', contact: '', faxAttention: '', isDefault: 'N', description: '' })
  showModal.value = true
}

function openEditModal(c) {
  isEditing.value = true
  Object.assign(modalForm, { ...c })
  showModal.value = true
}

async function saveContact() {
  try {
    if (isEditing.value) {
      await contactStore.update(modalForm.contactId, modalForm)
      notify.success('Cập nhật liên hệ thành công!')
    } else {
      await contactStore.create(modalForm)
      notify.success('Thêm liên hệ thành công!')
    }
    await contactStore.fetchAll()
    showModal.value = false
  } catch (err) {
    notify.error('Lỗi: ' + (contactStore.error || 'Thất bại'))
  }
}

async function handleSetDefault(c) {
  try {
    await contactStore.setDefault(c.contactId)
    await contactStore.fetchAll()
    notify.success('Đã đặt mặc định thành công!')
  } catch (err) {
    showToast('Lỗi: ' + (contactStore.error || 'Thất bại'), 'error')
  }
}

async function handleDelete(c) {
    if (confirm('Bạn có chắc muốn xóa liên hệ này?')) {
    try {
      await contactStore.delete(c.contactId)
      await contactStore.fetchAll()
      notify.success('Xóa liên hệ thành công!')
    } catch (err) {
      notify.error('Lỗi: ' + (contactStore.error || 'Thất bại'))
    }
  }
}

// Notifications handled by useNotify composable
</script>

<style scoped>
.form-row { display: grid; grid-template-columns: 1fr 1fr; gap: var(--spacing-md); }

code {
  background: var(--color-primary-light); color: var(--color-primary);
  padding: 2px 8px; border-radius: 4px; font-size: 0.8rem; font-weight: 600;
}
</style>
