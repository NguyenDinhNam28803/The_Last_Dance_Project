<template>
  <div class="um-page">

    <!-- ══════════════ PANEL TRÁI: FORM CHI TIẾT ══════════════ -->
    <div class="um-panel form-panel">

      <!-- Header / Toolbar -->
      <div class="panel-header">
        <div class="panel-title">
          <i class="fas fa-user-cog"></i>
          <span>{{ panelTitle }}</span>
        </div>
        <div class="toolbar">
          <template v-if="mode === 'view'">
            <button class="tbtn primary"   @click="startAdd">    <i class="fas fa-plus"></i>    Thêm   </button>
            <button class="tbtn outline"   @click="startEdit"    :disabled="!selectedUser || selectedIds.length > 1"> <i class="fas fa-pencil-alt"></i> Sửa </button>
            <button class="tbtn danger-o"  @click="confirmDelete" :disabled="selectedIds.length === 0"> <i class="fas fa-trash-alt"></i> Xoá </button>
            <button class="tbtn ghost"     @click="fetchUsers">   <i class="fas fa-sync-alt"></i>        </button>
          </template>
          <template v-else>
            <button class="tbtn success"   @click="saveUser">    <i class="fas fa-check"></i>    Lưu   </button>
            <button class="tbtn ghost"     @click="cancelEdit">  <i class="fas fa-times"></i>    Huỷ   </button>
          </template>
        </div>
      </div>

      <!-- Multi-select banner -->
      <div v-if="selectedIds.length > 1 && mode === 'view'" class="multi-banner">
        <i class="fas fa-layer-group"></i>
        Đang chọn <b>{{ selectedIds.length }}</b> tài khoản —
        <span>Xoá hàng loạt hoặc bỏ chọn để sửa từng bản ghi</span>
      </div>

      <!-- Form body -->
      <div class="panel-body" :class="{ dimmed: selectedIds.length > 1 && mode === 'view' }">

        <div v-if="!selectedUser && mode === 'view'" class="empty-state">
          <div class="empty-icon"><i class="fas fa-user-circle"></i></div>
          <p>Chọn một tài khoản bên phải<br>để xem chi tiết</p>
        </div>

        <template v-else>
          <!-- Section: Định danh -->
          <div class="section-label">Định danh tài khoản</div>
          <div class="form-grid cols-3">
            <div class="field">
              <label>Mã User (CustId)</label>
              <input v-model="formData.custId" :disabled="mode !== 'add'" :class="{ err: errors.custId }" placeholder="UUID hoặc ACC_..." />
              <span class="err-msg" v-if="errors.custId">{{ errors.custId }}</span>
            </div>
            <div class="field">
              <label>Tên đăng nhập <span class="req">*</span></label>
              <input v-model="formData.userName" :disabled="mode !== 'add'" :class="{ err: errors.userName }" placeholder="username..." />
              <span class="err-msg" v-if="errors.userName">{{ errors.userName }}</span>
            </div>
            <div class="field">
              <label>Họ và tên <span class="req">*</span></label>
              <input v-model="formData.name" :disabled="mode === 'view'" :class="{ err: errors.name }" placeholder="Họ và tên đầy đủ..." />
              <span class="err-msg" v-if="errors.name">{{ errors.name }}</span>
            </div>
          </div>

          <!-- Section: Liên hệ -->
          <div class="section-label mt-14">Thông tin liên hệ</div>
          <div class="form-grid cols-3">
            <div class="field">
              <label>Email <span class="req">*</span></label>
              <input v-model="formData.email" type="email" :disabled="mode === 'view'" :class="{ err: errors.email }" placeholder="email@domain.com" />
              <span class="err-msg" v-if="errors.email">{{ errors.email }}</span>
            </div>
            <div class="field">
              <label>Số điện thoại</label>
              <input v-model="formData.phoneNumber" :disabled="mode === 'view'" placeholder="09xxxxxxxx" />
            </div>
            <div class="field">
              <label>Tên viết tắt</label>
              <input v-model="formData.shortName" :disabled="mode === 'view'" placeholder="Viết tắt..." />
            </div>
          </div>

          <!-- Section: Phân quyền -->
          <div class="section-label mt-14">Phân quyền & Trạng thái</div>
          <div class="form-grid cols-3">
            <div class="field">
              <label>Quyền (Role) <span class="req">*</span></label>
              <select v-model="formData.roleId" :disabled="mode === 'view'" :class="{ err: errors.roleId }">
                <option value="">— Chọn quyền —</option>
                <option v-for="role in roles" :key="role.value" :value="role.value">
                  {{ role.label }}
                </option>
              </select>
              <span class="err-msg" v-if="errors.roleId">{{ errors.roleId }}</span>
            </div>
            <div class="field">
              <label>Trạng thái</label>
              <select v-model="formData.status" :disabled="mode === 'view'">
                <option value="Active">Hoạt động</option>
                <option value="Inactive">Khoá</option>
              </select>
            </div>
            <div class="field">
              <label>Record Status</label>
              <select v-model="formData.recordStatus" :disabled="mode === 'view'">
                <option value="1">Active (1)</option>
                <option value="0">Inactive (0)</option>
              </select>
            </div>
          </div>

          <!-- Section: Metadata (readonly) -->
          <template v-if="mode !== 'add'">
            <div class="section-label mt-14">Thông tin hệ thống</div>
            <div class="meta-grid">
              <div class="meta-item">
                <span class="meta-key">Ngày tạo</span>
                <span class="meta-val">{{ formatDate(formData.createdDate) }}</span>
              </div>
              <div class="meta-item">
                <span class="meta-key">Tạo bởi</span>
                <span class="meta-val">{{ formData.createdBy || '—' }}</span>
              </div>
              <div class="meta-item">
                <span class="meta-key">Tên khác</span>
                <span class="meta-val">{{ formData.nameOther || '—' }}</span>
              </div>
            </div>
          </template>

          <!-- Edit button khi ở view mode và có user được chọn -->
          <div v-if="mode === 'view' && selectedUser && selectedIds.length <= 1" class="action-row">
            <button class="tbtn outline" @click="startEdit">
              <i class="fas fa-pencil-alt"></i> Chỉnh sửa
            </button>
          </div>
        </template>
      </div>
    </div>

    <!-- ══════════════ PANEL PHẢI: DANH SÁCH ══════════════ -->
    <div class="um-panel grid-panel" :class="{ collapsed: isCollapsed }">
      <div class="panel-header clickable" @click="isCollapsed = !isCollapsed">
        <div class="panel-title">
          <i class="fas fa-users"></i>
          <span v-if="!isCollapsed">Tài khoản ({{ users.length }})</span>
        </div>
        <i class="fas collapse-icon" :class="isCollapsed ? 'fa-chevron-left' : 'fa-chevron-right'"></i>
      </div>

      <!-- Search bar -->
      <div v-if="!isCollapsed" class="search-bar">
        <i class="fas fa-search"></i>
        <input v-model="searchQuery" placeholder="Tìm theo tên, username, email, role..." />
        <button v-if="searchQuery" class="clear-btn" @click="searchQuery = ''">
          <i class="fas fa-times"></i>
        </button>
      </div>

      <!-- Table -->
      <div v-if="!isCollapsed" class="grid-body">
        <div v-if="loading" class="loading-row">
          <i class="fas fa-spinner fa-spin"></i> Đang tải dữ liệu...
        </div>

        <table v-else class="data-table">
          <thead>
            <tr>
              <th class="check-col">
                <input type="checkbox"
                  :checked="filteredUsers.length > 0 && selectedIds.length === filteredUsers.length"
                  @change="e => selectedIds = e.target.checked ? filteredUsers.map(u => u.custId) : []"
                />
              </th>
              <th @click="sortBy('userName')" class="sortable">
                Username <i class="fas" :class="sortIcon('userName')"></i>
              </th>
              <th @click="sortBy('name')" class="sortable">
                Họ tên <i class="fas" :class="sortIcon('name')"></i>
              </th>
              <th>Role</th>
              <th>Trạng thái</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="user in sortedUsers"
              :key="user.custId"
              :class="{ active: selectedUser?.custId === user.custId, checked: selectedIds.includes(user.custId) }"
              @click="selectUser(user)"
            >
              <td class="check-col" @click.stop>
                <input type="checkbox" :value="user.custId" v-model="selectedIds" />
              </td>
              <td>
                <div class="user-cell">
                  <div class="avatar" :class="avatarClass(user.roleId)">
                    {{ initials(user.name) }}
                  </div>
                  <div>
                    <div class="username">{{ user.userName }}</div>
                    <div class="email-sub">{{ user.email }}</div>
                  </div>
                </div>
              </td>
              <td>{{ user.name }}</td>
              <td>
                <span class="role-badge" :class="roleBadgeClass(user.roleId)">
                  {{ user.roleId || 'N/A' }}
                </span>
              </td>
              <td>
                <span class="status-dot" :class="user.status === 'Active' ? 'on' : 'off'"></span>
                {{ user.status }}
              </td>
            </tr>
            <tr v-if="sortedUsers.length === 0">
              <td colspan="5" class="empty-row">
                <i class="fas fa-search"></i> Không tìm thấy kết quả
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Delete confirm modal -->
    <div v-if="showDeleteModal" class="modal-overlay" @click.self="showDeleteModal = false">
      <div class="modal-box">
        <div class="modal-icon"><i class="fas fa-exclamation-triangle"></i></div>
        <h3>Xác nhận xoá</h3>
        <p v-if="selectedIds.length === 1">
          Xoá tài khoản <b>{{ selectedUser?.userName }}</b>?
        </p>
        <p v-else>
          Xoá <b>{{ selectedIds.length }}</b> tài khoản được chọn?
        </p>
        <p class="modal-warn">Thao tác này không thể hoàn tác.</p>
        <div class="modal-actions">
          <button class="tbtn ghost" @click="showDeleteModal = false">Huỷ</button>
          <button class="tbtn danger" @click="doDelete">Xoá</button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
// import { useUserStore } from '@/stores/user'  // Uncomment khi dùng store

// ── Mock data theo đúng cấu trúc API ───────────────
const mockUsers = [
  { custId: '14afcad6-48ae-4374-b', userName: 'Namden135',           name: 'Namden135',              nameOther: null, shortName: null, email: 'namndtb00921@fpt.edu.vn',         phoneNumber: '0808080808', roleId: 'CHECKER', roleName: null, status: 'Active',   recordStatus: '1', gender: null, dateOfBirth: null, nationality: null, residentCountryId: null, createdDate: '2026-03-27T16:06:14.9364172', createdBy: null },
  { custId: '247cbcd2-957c-4c8d-b', userName: 'Namden789',           name: 'Namden789',              nameOther: null, shortName: null, email: 'namden@gmail.com',                phoneNumber: '0606060606', roleId: 'ADMIN',   roleName: null, status: 'Active',   recordStatus: '1', gender: null, dateOfBirth: null, nationality: null, residentCountryId: null, createdDate: '2026-03-27T16:36:33.6678712', createdBy: null },
  { custId: '74f46864-53ce-439a-b', userName: 'Namden246',           name: 'Namden246',              nameOther: null, shortName: null, email: 'nam@gmail.com',                   phoneNumber: '0707070707', roleId: 'MAKER',   roleName: null, status: 'Active',   recordStatus: '1', gender: null, dateOfBirth: null, nationality: null, residentCountryId: null, createdDate: '2026-03-27T16:10:06.1968369', createdBy: null },
  { custId: 'ACC_CHECKER_001',      userName: 'checker_system',      name: 'Checker System Account', nameOther: null, shortName: null, email: 'checker_sys@test.com',            phoneNumber: null,         roleId: 'CHECKER', roleName: null, status: 'Active',   recordStatus: '1', gender: null, dateOfBirth: null, nationality: null, residentCountryId: null, createdDate: '2026-03-27T00:00:00',         createdBy: null },
  { custId: 'ACC_MAKER_001',        userName: 'maker_system',        name: 'Maker System Account',   nameOther: null, shortName: null, email: 'maker_sys@test.com',              phoneNumber: null,         roleId: 'MAKER',   roleName: null, status: 'Active',   recordStatus: '1', gender: null, dateOfBirth: null, nationality: null, residentCountryId: null, createdDate: '2026-03-27T00:00:00',         createdBy: null },
  { custId: 'ace73354-1606-4a26-8', userName: 'Nguyendinhnam28803',  name: 'Nguyendinhnam28803',     nameOther: null, shortName: null, email: 'nguyendinhnam241209@gmail.com',   phoneNumber: '0908651852', roleId: null,      roleName: null, status: 'Active',   recordStatus: '1', gender: null, dateOfBirth: null, nationality: null, residentCountryId: null, createdDate: '2026-03-21T01:24:14.7931554', createdBy: null },
]

// Roles lấy từ SystemCode API (ROLE)
const roles = [
  { value: 'ADMIN',   label: 'Quản trị hệ thống (ADMIN)'  },
  { value: 'MANAGER', label: 'Quản lý (MANAGER)'           },
  { value: 'MAKER',   label: 'Maker'                        },
  { value: 'CHECKER', label: 'Checker'                      },
  { value: 'USER',    label: 'Người dùng (USER)'            },
]

// ── State ───────────────────────────────────────────
const users         = ref([])
const loading       = ref(false)
const mode          = ref('view')
const selectedUser  = ref(null)
const selectedIds   = ref([])
const formData      = ref({})
const errors        = ref({})
const isCollapsed   = ref(false)
const showDeleteModal = ref(false)
const searchQuery   = ref('')
const sortField     = ref('userName')
const sortDir       = ref('asc')

// ── Computed ────────────────────────────────────────
const panelTitle = computed(() => {
  if (mode.value === 'add')  return 'Thêm tài khoản mới'
  if (mode.value === 'edit') return 'Chỉnh sửa — ' + (selectedUser.value?.userName ?? '')
  return 'Quản lý Người Dùng'
})

const filteredUsers = computed(() => {
  const q = searchQuery.value.toLowerCase().trim()
  if (!q) return users.value
  return users.value.filter(u =>
    (u.userName  ?? '').toLowerCase().includes(q) ||
    (u.name      ?? '').toLowerCase().includes(q) ||
    (u.email     ?? '').toLowerCase().includes(q) ||
    (u.roleId    ?? '').toLowerCase().includes(q)
  )
})

const sortedUsers = computed(() => {
  return [...filteredUsers.value].sort((a, b) => {
    const av = (a[sortField.value] ?? '').toLowerCase()
    const bv = (b[sortField.value] ?? '').toLowerCase()
    return sortDir.value === 'asc' ? av.localeCompare(bv) : bv.localeCompare(av)
  })
})

// ── Fetch ───────────────────────────────────────────
const fetchUsers = async () => {
  loading.value = true
  try {
    // TODO: const res = await axios.get('/api/Customer')
    // users.value = res.data
    await new Promise(r => setTimeout(r, 400))
    users.value = mockUsers
  } finally {
    loading.value = false
  }
}

onMounted(fetchUsers)

// ── CRUD ────────────────────────────────────────────
const selectUser = (user) => {
  if (mode.value !== 'view') return
  selectedUser.value = user
  selectedIds.value  = [user.custId]
  formData.value     = { ...user }
  errors.value       = {}
}

const startAdd = () => {
  mode.value         = 'add'
  selectedUser.value = null
  selectedIds.value  = []
  formData.value     = { status: 'Active', recordStatus: '1', roleId: '' }
  errors.value       = {}
}

const startEdit = () => {
  if (!selectedUser.value || selectedIds.value.length > 1) return
  mode.value     = 'edit'
  formData.value = { ...selectedUser.value }
  errors.value   = {}
}

const cancelEdit = () => {
  mode.value = 'view'
  if (selectedUser.value) formData.value = { ...selectedUser.value }
  errors.value = {}
}

const saveUser = async () => {
  if (!validateForm()) return
  try {
    if (mode.value === 'add') {
      // TODO: await axios.post('/api/Customer', formData.value)
      users.value.push({ ...formData.value })
    } else {
      // TODO: await axios.put(`/api/Customer/${formData.value.custId}`, formData.value)
      const idx = users.value.findIndex(u => u.custId === formData.value.custId)
      if (idx !== -1) users.value[idx] = { ...formData.value }
      selectedUser.value = { ...formData.value }
    }
    mode.value = 'view'
  } catch (e) {
    alert('Lỗi khi lưu: ' + e.message)
  }
}

const validateForm = () => {
  errors.value = {}
  let ok = true
  if (!formData.value.custId?.trim())    { errors.value.custId    = 'Không được để trống'; ok = false }
  if (!formData.value.userName?.trim())  { errors.value.userName  = 'Không được để trống'; ok = false }
  if (!formData.value.name?.trim())      { errors.value.name      = 'Không được để trống'; ok = false }
  if (!formData.value.email?.trim())     { errors.value.email     = 'Email không được để trống'; ok = false }
  if (!formData.value.roleId)            { errors.value.roleId    = 'Vui lòng chọn quyền'; ok = false }
  return ok
}

const confirmDelete = () => {
  if (selectedIds.value.length === 0) return
  showDeleteModal.value = true
}

const doDelete = async () => {
  try {
    // TODO: await Promise.all(selectedIds.value.map(id => axios.delete(`/api/Customer/${id}`)))
    users.value     = users.value.filter(u => !selectedIds.value.includes(u.custId))
    selectedIds.value  = []
    selectedUser.value = null
    formData.value     = {}
    showDeleteModal.value = false
  } catch (e) {
    alert('Lỗi khi xoá: ' + e.message)
  }
}

// ── Sort ─────────────────────────────────────────────
const sortBy = (field) => {
  if (sortField.value === field) sortDir.value = sortDir.value === 'asc' ? 'desc' : 'asc'
  else { sortField.value = field; sortDir.value = 'asc' }
}
const sortIcon = (field) => {
  if (sortField.value !== field) return 'fa-sort'
  return sortDir.value === 'asc' ? 'fa-sort-up' : 'fa-sort-down'
}

// ── Helpers ──────────────────────────────────────────
const initials = (name) => (name ?? 'U').split(' ').map(w => w[0]).join('').slice(0, 2).toUpperCase()

const formatDate = (iso) => {
  if (!iso) return '—'
  return new Date(iso).toLocaleString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' })
}

const avatarClass = (roleId) => ({
  'av-admin':   roleId === 'ADMIN',
  'av-manager': roleId === 'MANAGER',
  'av-maker':   roleId === 'MAKER',
  'av-checker': roleId === 'CHECKER',
  'av-default': !roleId || roleId === 'USER',
})

const roleBadgeClass = (roleId) => ({
  'rb-admin':   roleId === 'ADMIN',
  'rb-manager': roleId === 'MANAGER',
  'rb-maker':   roleId === 'MAKER',
  'rb-checker': roleId === 'CHECKER',
  'rb-none':    !roleId,
})
</script>

<style scoped>
/* ── Root layout ──────────────────────────────────── */
.um-page {
  display: flex;
  height: calc(100vh - 56px);
  background: #f0f2f5;
  font-family: 'Segoe UI', system-ui, sans-serif;
  font-size: 13px;
  color: #1f2937;
}

.um-panel {
  display: flex;
  flex-direction: column;
  background: #fff;
  border-right: 1px solid #e5e7eb;
  overflow: hidden;
  transition: width 0.22s ease;
}

.form-panel  { width: 420px; min-width: 420px; flex-shrink: 0; }
.grid-panel  { flex: 1; min-width: 44px; border-right: none; }
.grid-panel.collapsed { width: 44px !important; min-width: 44px; flex: none; }

/* ── Panel header ─────────────────────────────────── */
.panel-header {
  display: flex; align-items: center; justify-content: space-between;
  padding: 0 14px; height: 46px; flex-shrink: 0;
  background: #0f172a; color: #fff; gap: 10px;
}
.panel-header.clickable { cursor: pointer; user-select: none; }
.panel-header.clickable:hover { background: #1e293b; }

.panel-title {
  display: flex; align-items: center; gap: 9px;
  font-weight: 600; font-size: 13px; letter-spacing: 0.3px; overflow: hidden;
}
.panel-title i { color: #38bdf8; font-size: 14px; flex-shrink: 0; }

.collapse-icon { color: #64748b; font-size: 11px; flex-shrink: 0; }

/* ── Toolbar buttons ──────────────────────────────── */
.toolbar { display: flex; gap: 5px; }

.tbtn {
  display: inline-flex; align-items: center; gap: 5px;
  padding: 5px 11px; border-radius: 5px;
  font-size: 12px; font-weight: 500; cursor: pointer;
  border: 1px solid transparent; transition: all 0.14s; white-space: nowrap;
}
.tbtn:disabled { opacity: 0.4; cursor: not-allowed; }
.tbtn.primary  { background: #3b82f6; color: #fff; }
.tbtn.primary:not(:disabled):hover  { background: #2563eb; }
.tbtn.success  { background: #10b981; color: #fff; }
.tbtn.success:not(:disabled):hover  { background: #059669; }
.tbtn.outline  { background: transparent; color: #93c5fd; border-color: #334155; }
.tbtn.outline:not(:disabled):hover  { background: #1e293b; }
.tbtn.danger-o { background: transparent; color: #fca5a5; border-color: #334155; }
.tbtn.danger-o:not(:disabled):hover { background: #1e293b; }
.tbtn.danger   { background: #ef4444; color: #fff; }
.tbtn.danger:hover   { background: #dc2626; }
.tbtn.ghost    { background: transparent; color: #64748b; border-color: #334155; padding: 5px 9px; }
.tbtn.ghost:hover    { background: #1e293b; color: #94a3b8; }

/* ── Multi banner ─────────────────────────────────── */
.multi-banner {
  background: #eef2ff; color: #4338ca;
  padding: 8px 16px; font-size: 12px;
  border-bottom: 1px solid #c7d2fe;
  display: flex; align-items: center; gap: 7px;
  flex-shrink: 0;
}
.multi-banner i { font-size: 13px; }

/* ── Panel body ───────────────────────────────────── */
.panel-body {
  flex: 1; overflow-y: auto; padding: 18px 20px;
  transition: opacity 0.2s, filter 0.2s;
}
.panel-body.dimmed { opacity: 0.35; pointer-events: none; filter: grayscale(80%); }

/* ── Empty state ──────────────────────────────────── */
.empty-state {
  display: flex; flex-direction: column; align-items: center;
  justify-content: center; height: 100%; gap: 12px;
  color: #9ca3af; text-align: center;
}
.empty-icon i { font-size: 48px; color: #e5e7eb; }
.empty-state p { font-size: 13px; line-height: 1.6; margin: 0; }

/* ── Form ─────────────────────────────────────────── */
.section-label {
  font-size: 10.5px; font-weight: 700; text-transform: uppercase;
  letter-spacing: 0.8px; color: #6b7280;
  padding-bottom: 8px; border-bottom: 1px solid #f3f4f6;
  margin-bottom: 12px;
}
.mt-14 { margin-top: 18px; }

.form-grid { display: grid; gap: 11px; }
.form-grid.cols-3 { grid-template-columns: 1fr 1fr 1fr; }

.field { display: flex; flex-direction: column; gap: 4px; }

.field label {
  font-size: 11.5px; font-weight: 600; color: #4b5563;
}
.req { color: #ef4444; }

.field input,
.field select {
  height: 34px; padding: 0 10px;
  border: 1px solid #d1d5db; border-radius: 5px;
  font-size: 12.5px; color: #111827; background: #fff;
  outline: none; transition: border-color 0.15s, box-shadow 0.15s;
}
.field input:focus,
.field select:focus  { border-color: #3b82f6; box-shadow: 0 0 0 3px #dbeafe; }
.field input:disabled,
.field select:disabled { background: #f9fafb; color: #9ca3af; cursor: not-allowed; }
.field input.err,
.field select.err { border-color: #ef4444; }
.err-msg { font-size: 11px; color: #ef4444; }

/* Metadata grid */
.meta-grid {
  display: grid; grid-template-columns: 1fr 1fr 1fr; gap: 10px;
}
.meta-item {
  display: flex; flex-direction: column; gap: 3px;
  padding: 9px 11px; background: #f9fafb; border-radius: 6px;
}
.meta-key { font-size: 10.5px; color: #9ca3af; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; }
.meta-val { font-size: 12px; color: #374151; font-weight: 500; }

.action-row {
  margin-top: 18px; padding-top: 14px;
  border-top: 1px solid #f3f4f6;
}

/* ── Search bar ───────────────────────────────────── */
.search-bar {
  display: flex; align-items: center; gap: 8px;
  padding: 9px 14px; border-bottom: 1px solid #f3f4f6;
  background: #fafafa; flex-shrink: 0;
}
.search-bar i.fa-search { color: #9ca3af; font-size: 12px; }
.search-bar input {
  flex: 1; border: none; background: transparent; outline: none;
  font-size: 12.5px; color: #374151;
}
.clear-btn {
  background: none; border: none; cursor: pointer;
  color: #9ca3af; padding: 2px 4px; border-radius: 3px;
  font-size: 11px;
}
.clear-btn:hover { color: #ef4444; }

/* ── Table ────────────────────────────────────────── */
.grid-body { flex: 1; overflow-y: auto; }

.loading-row {
  padding: 20px; text-align: center; color: #6b7280;
  display: flex; align-items: center; justify-content: center; gap: 8px;
}

.data-table { width: 100%; border-collapse: collapse; }

.data-table thead th {
  padding: 9px 12px; text-align: left;
  font-size: 11px; font-weight: 700;
  text-transform: uppercase; letter-spacing: 0.5px;
  color: #6b7280; background: #f9fafb;
  border-bottom: 2px solid #e5e7eb;
  white-space: nowrap; position: sticky; top: 0; z-index: 1;
}
.data-table thead th.sortable { cursor: pointer; user-select: none; }
.data-table thead th.sortable:hover { background: #f3f4f6; }
.data-table thead th .fas { margin-left: 4px; font-size: 9px; color: #9ca3af; }
.check-col { width: 40px; text-align: center !important; }

.data-table tbody tr {
  border-bottom: 1px solid #f3f4f6;
  cursor: pointer; transition: background 0.1s;
}
.data-table tbody tr:hover  { background: #f8fafc; }
.data-table tbody tr.active { background: #eff6ff; }
.data-table tbody tr.checked { background: #fefce8; }
.data-table tbody tr.active.checked { background: #eff6ff; }

.data-table td { padding: 9px 12px; vertical-align: middle; }

/* User cell with avatar */
.user-cell { display: flex; align-items: center; gap: 9px; }
.avatar {
  width: 30px; height: 30px; border-radius: 50%;
  display: flex; align-items: center; justify-content: center;
  font-size: 11px; font-weight: 700; flex-shrink: 0; color: #fff;
}
.av-admin   { background: linear-gradient(135deg, #ef4444, #b91c1c); }
.av-manager { background: linear-gradient(135deg, #8b5cf6, #6d28d9); }
.av-maker   { background: linear-gradient(135deg, #f59e0b, #d97706); }
.av-checker { background: linear-gradient(135deg, #10b981, #059669); }
.av-default { background: linear-gradient(135deg, #6b7280, #4b5563); }

.username { font-weight: 600; font-size: 12.5px; color: #1e40af; }
.email-sub { font-size: 11px; color: #9ca3af; }

/* Role badge */
.role-badge {
  padding: 2px 9px; border-radius: 4px;
  font-size: 11px; font-weight: 700; letter-spacing: 0.4px;
}
.rb-admin   { background: #fee2e2; color: #b91c1c; }
.rb-manager { background: #ede9fe; color: #6d28d9; }
.rb-maker   { background: #fef3c7; color: #92400e; }
.rb-checker { background: #d1fae5; color: #065f46; }
.rb-none    { background: #f3f4f6; color: #6b7280; }

/* Status dot */
.status-dot {
  display: inline-block;
  width: 7px; height: 7px; border-radius: 50%;
  margin-right: 5px; vertical-align: middle;
}
.status-dot.on  { background: #10b981; box-shadow: 0 0 0 2px #d1fae5; }
.status-dot.off { background: #ef4444; box-shadow: 0 0 0 2px #fee2e2; }

.empty-row {
  text-align: center; color: #9ca3af;
  padding: 28px !important; font-size: 12.5px;
}
.empty-row .fas { margin-right: 7px; }

/* ── Modal ────────────────────────────────────────── */
.modal-overlay {
  position: fixed; inset: 0;
  background: rgba(0,0,0,0.45); backdrop-filter: blur(3px);
  display: flex; align-items: center; justify-content: center;
  z-index: 200;
}
.modal-box {
  background: #fff; border-radius: 12px;
  padding: 30px 34px; width: 380px;
  text-align: center;
  box-shadow: 0 24px 64px rgba(0,0,0,0.22);
}
.modal-icon { font-size: 38px; color: #ef4444; margin-bottom: 14px; }
.modal-box h3 { font-size: 17px; font-weight: 700; color: #111827; margin: 0 0 8px; }
.modal-box p  { font-size: 13px; color: #4b5563; margin: 0 0 6px; }
.modal-warn   { font-size: 11.5px !important; color: #ef4444 !important; }
.modal-actions { display: flex; gap: 10px; justify-content: center; margin-top: 20px; }
</style>