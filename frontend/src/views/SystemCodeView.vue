<template>
  <div class="sc-page">

    <!-- ═══════════════ PANEL 1: FORM ═══════════════ -->
    <div class="sc-panel form-panel">
      <div class="panel-header">
        <span class="panel-title">
          <i class="fas fa-sliders-h"></i>
          {{ valueMode !== 'view' ? 'Giá Trị' : (codeMode !== 'view' ? 'Tham Số' : 'Tham Số Hệ Thống') }}
        </span>
        <div class="toolbar-actions">
          <!-- Code toolbar -->
          <template v-if="valueMode === 'view'">
            <template v-if="codeMode === 'view'">
              <button class="btn-action primary" @click="startAddCode" title="Thêm mới">
                <i class="fas fa-plus"></i> Thêm
              </button>
              <button class="btn-action ghost" @click="fetchCodes" title="Làm mới">
                <i class="fas fa-sync-alt"></i>
              </button>
            </template>
            <template v-else>
              <button class="btn-action success" @click="saveCode">
                <i class="fas fa-check"></i> Lưu
              </button>
              <button class="btn-action ghost" @click="cancelCode">
                <i class="fas fa-times"></i> Huỷ
              </button>
            </template>
          </template>
          <!-- Value toolbar -->
          <template v-else>
            <button class="btn-action success" @click="saveValue">
              <i class="fas fa-check"></i> Lưu
            </button>
            <button class="btn-action ghost" @click="cancelValue">
              <i class="fas fa-times"></i> Huỷ
            </button>
          </template>
        </div>
      </div>

      <div class="panel-body">
        <!-- Placeholder khi chưa chọn gì -->
        <div v-if="!selectedCode && codeMode === 'view' && valueMode === 'view'" class="empty-hint">
          <i class="fas fa-arrow-right pulse"></i>
          <p>Chọn một danh mục bên phải để xem chi tiết</p>
        </div>

        <!-- Form SystemCode -->
        <template v-if="(selectedCode || codeMode === 'add') && valueMode === 'view'">
          <div class="section-label">Thông tin Danh Mục</div>
          <div class="form-grid">
            <div class="field">
              <label>Mã Tham số <span class="req">*</span></label>
              <input
                v-model="codeForm.systemCodeId"
                :disabled="codeMode !== 'add'"
                :class="{ 'has-error': codeErrors.systemCodeId }"
                placeholder="VD: ROLE, STATUS..."
              />
              <span v-if="codeErrors.systemCodeId" class="err-msg">{{ codeErrors.systemCodeId }}</span>
            </div>
            <div class="field">
              <label>Tên Danh mục <span class="req">*</span></label>
              <input
                v-model="codeForm.name"
                :disabled="codeMode === 'view'"
                :class="{ 'has-error': codeErrors.name }"
                placeholder="Nhập tên danh mục..."
              />
              <span v-if="codeErrors.name" class="err-msg">{{ codeErrors.name }}</span>
            </div>
            <div class="field full">
              <label>Mô tả</label>
              <input
                v-model="codeForm.description"
                :disabled="codeMode === 'view'"
                placeholder="Mô tả danh mục..."
              />
            </div>
            <div class="field full">
              <label class="toggle-label">
                <div class="toggle" :class="{ on: codeForm.isActive === 'Y' }" @click="codeMode !== 'view' && toggleActive()">
                  <div class="thumb"></div>
                </div>
                <span>{{ codeForm.isActive === 'Y' ? 'Đang hoạt động' : 'Không hoạt động' }}</span>
              </label>
            </div>
          </div>

          <!-- Edit button khi ở view mode -->
          <div v-if="codeMode === 'view' && selectedCode" class="edit-hint">
            <button class="btn-action outline" @click="startEditCode">
              <i class="fas fa-pencil-alt"></i> Chỉnh sửa
            </button>
          </div>
        </template>

        <!-- Form Value -->
        <template v-if="valueMode !== 'view'">
          <div class="section-label">
            {{ valueMode === 'add' ? 'Thêm giá trị mới' : 'Chỉnh sửa giá trị' }}
            <span class="tag">{{ selectedCode?.systemCodeId }}</span>
          </div>
          <div class="form-grid">
            <div class="field">
              <label>Mã giá trị <span class="req">*</span></label>
              <input
                v-model="valueForm.codeValue"
                :disabled="valueMode === 'edit'"
                :class="{ 'has-error': valueErrors.codeValue }"
                placeholder="VD: ADMIN, USER..."
              />
              <span v-if="valueErrors.codeValue" class="err-msg">{{ valueErrors.codeValue }}</span>
            </div>
            <div class="field">
              <label>Thứ tự</label>
              <input v-model.number="valueForm.orderBy" type="number" min="1" placeholder="1" />
            </div>
            <div class="field full">
              <label>Tên hiển thị (VI) <span class="req">*</span></label>
              <input
                v-model="valueForm.displayValue"
                :class="{ 'has-error': valueErrors.displayValue }"
                placeholder="Tên tiếng Việt..."
              />
              <span v-if="valueErrors.displayValue" class="err-msg">{{ valueErrors.displayValue }}</span>
            </div>
            <div class="field full">
              <label>Tên hiển thị (EN)</label>
              <input v-model="valueForm.displayValueEn" placeholder="English name..." />
            </div>
            <div class="field full">
              <label class="toggle-label">
                <div class="toggle" :class="{ on: valueForm.isDefault === 'Y' }" @click="valueForm.isDefault = valueForm.isDefault === 'Y' ? 'N' : 'Y'">
                  <div class="thumb"></div>
                </div>
                <span>Giá trị mặc định</span>
              </label>
            </div>
          </div>
        </template>
      </div>
    </div>

    <!-- ═══════════════ PANEL 2: DANH SÁCH SYSTEM CODE ═══════════════ -->
    <div class="sc-panel list-panel" :class="{ collapsed: panel2Collapsed }">
      <div class="panel-header" @click="panel2Collapsed = !panel2Collapsed" style="cursor:pointer">
        <span class="panel-title">
          <i class="fas fa-list"></i>
          <span v-if="!panel2Collapsed">Danh mục ({{ codes.length }})</span>
        </span>
        <i class="fas collapse-icon" :class="panel2Collapsed ? 'fa-chevron-right' : 'fa-chevron-left'"></i>
      </div>

      <div v-if="!panel2Collapsed" class="panel-body no-pad">
        <div v-if="loading" class="loading-row">
          <i class="fas fa-spinner fa-spin"></i> Đang tải...
        </div>
        <div
          v-for="code in codes"
          :key="code.systemCodeId"
          class="code-row"
          :class="{ active: selectedCode?.systemCodeId === code.systemCodeId }"
          @click="selectCode(code)"
        >
          <div class="code-id">{{ code.systemCodeId }}</div>
          <div class="code-name">{{ code.name }}</div>
          <div class="code-meta">
            <span class="badge" :class="code.isActive === 'Y' ? 'active' : 'inactive'">
              {{ code.isActive === 'Y' ? 'Active' : 'Blocked' }}
            </span>
            <span class="val-count">{{ code.values?.length || 0 }} giá trị</span>
          </div>
        </div>
        <div v-if="!loading && codes.length === 0" class="empty-hint small">
          Chưa có danh mục nào
        </div>
      </div>
    </div>

    <!-- ═══════════════ PANEL 3: VALUES ═══════════════ -->
    <div class="sc-panel values-panel" :class="{ collapsed: panel3Collapsed, hidden: !selectedCode }">
      <div class="panel-header">
        <span class="panel-title">
          <i class="fas fa-tag"></i>
          <span v-if="!panel3Collapsed">
            Giá trị — <b>{{ selectedCode?.systemCodeId }}</b>
            ({{ selectedValues.length }})
          </span>
        </span>
        <div style="display:flex;gap:6px;align-items:center">
          <button
            v-if="!panel3Collapsed && valueMode === 'view'"
            class="btn-action primary small"
            @click="startAddValue"
          >
            <i class="fas fa-plus"></i> Thêm
          </button>
          <button class="btn-action ghost icon-only" @click="panel3Collapsed = !panel3Collapsed">
            <i class="fas" :class="panel3Collapsed ? 'fa-chevron-left' : 'fa-chevron-right'"></i>
          </button>
        </div>
      </div>

      <div v-if="!panel3Collapsed" class="panel-body no-pad">
        <div v-if="selectedValues.length === 0" class="empty-hint small">
          <i class="fas fa-inbox"></i>
          <p>Chưa có giá trị nào</p>
        </div>

        <table v-else class="val-table">
          <thead>
            <tr>
              <th>#</th>
              <th>Mã</th>
              <th>Tên (VI)</th>
              <th>Tên (EN)</th>
              <th>Mặc định</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="val in selectedValues"
              :key="val.id"
              :class="{ editing: editingValueId === val.id }"
            >
              <td class="order-cell">{{ val.orderBy }}</td>
              <td><span class="code-chip">{{ val.codeValue }}</span></td>
              <td>{{ val.displayValue }}</td>
              <td class="muted">{{ val.displayValueEn }}</td>
              <td>
                <span v-if="val.isDefault === 'Y'" class="default-dot" title="Mặc định">
                  <i class="fas fa-check-circle"></i>
                </span>
              </td>
              <td class="action-cell">
                <button class="icon-btn edit" @click="startEditValue(val)" title="Sửa">
                  <i class="fas fa-pencil-alt"></i>
                </button>
                <button class="icon-btn delete" @click="confirmDeleteValue(val)" title="Xoá">
                  <i class="fas fa-trash-alt"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Delete confirm modal -->
    <div v-if="deleteTarget" class="modal-overlay" @click.self="deleteTarget = null">
      <div class="modal-box">
        <div class="modal-icon danger"><i class="fas fa-exclamation-triangle"></i></div>
        <h3>Xác nhận xoá</h3>
        <p>Bạn có chắc muốn xoá giá trị <b>{{ deleteTarget.codeValue }}</b>?</p>
        <div class="modal-actions">
          <button class="btn-action ghost" @click="deleteTarget = null">Huỷ</button>
          <button class="btn-action danger" @click="doDeleteValue">Xoá</button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

// ── State ──────────────────────────────────────────
const codes        = ref([])
const loading      = ref(false)
const selectedCode = ref(null)

const codeMode   = ref('view')   // view | add | edit
const codeForm   = ref({})
const codeErrors = ref({})

const valueMode      = ref('view')  // view | add | edit
const valueForm      = ref({})
const valueErrors    = ref({})
const editingValueId = ref(null)
const deleteTarget   = ref(null)

const panel2Collapsed = ref(false)
const panel3Collapsed = ref(false)

// ── Computed ───────────────────────────────────────
const selectedValues = computed(() =>
  selectedCode.value?.values ?? []
)

// ── API calls (thay bằng axios/fetch thực tế) ──────
const fetchCodes = async () => {
  loading.value = true
  try {
    // TODO: thay bằng API thực
    // const res = await axios.get('/api/SystemCode')
    // codes.value = res.data
    codes.value = [
      {
        systemCodeId: 'ROLE', name: 'Role definitions',
        description: 'Application roles', isActive: 'Y',
        values: [
          { id: 1001, systemCodeId: 'ROLE', codeValue: 'ADMIN', displayValue: 'Administrator', displayValueEn: 'Administrator', orderBy: 1, isDefault: 'N' },
          { id: 1002, systemCodeId: 'ROLE', codeValue: 'USER',  displayValue: 'User',          displayValueEn: 'User',          orderBy: 2, isDefault: 'Y' },
          { id: 1003, systemCodeId: 'ROLE', codeValue: 'MANAGER', displayValue: 'Manager',     displayValueEn: 'Manager',       orderBy: 3, isDefault: 'N' },
          { id: 1004, systemCodeId: 'ROLE', codeValue: 'MAKER',   displayValue: 'Maker',       displayValueEn: 'Maker',         orderBy: 4, isDefault: 'N' },
          { id: 1005, systemCodeId: 'ROLE', codeValue: 'CHECKER', displayValue: 'Checker',     displayValueEn: 'Checker',       orderBy: 5, isDefault: 'N' },
        ]
      },
      { systemCodeId: 'STATUS', name: 'Transaction Status', description: 'Statuses for MTTRAN', isActive: 'Y', values: [] },
    ]
  } finally {
    loading.value = false
  }
}

onMounted(fetchCodes)

// ── SystemCode CRUD ────────────────────────────────
const selectCode = (code) => {
  if (codeMode.value !== 'view' || valueMode.value !== 'view') return
  selectedCode.value = code
  codeForm.value     = { ...code }
  codeErrors.value   = {}
  panel3Collapsed.value = false
}

const startAddCode = () => {
  selectedCode.value = null
  codeMode.value     = 'add'
  codeForm.value     = { isActive: 'Y' }
  codeErrors.value   = {}
}

const startEditCode = () => {
  codeMode.value   = 'edit'
  codeForm.value   = { ...selectedCode.value }
  codeErrors.value = {}
}

const cancelCode = () => {
  codeMode.value = 'view'
  if (selectedCode.value) codeForm.value = { ...selectedCode.value }
  codeErrors.value = {}
}

const saveCode = async () => {
  if (!validateCode()) return
  try {
    if (codeMode.value === 'add') {
      // TODO: await axios.post('/api/SystemCode', codeForm.value)
      codes.value.push({ ...codeForm.value, values: [] })
    } else {
      // TODO: await axios.put(`/api/SystemCode/${codeForm.value.systemCodeId}`, codeForm.value)
      const idx = codes.value.findIndex(c => c.systemCodeId === codeForm.value.systemCodeId)
      if (idx !== -1) codes.value[idx] = { ...codes.value[idx], ...codeForm.value }
      selectedCode.value = codes.value[idx]
    }
    codeMode.value = 'view'
  } catch (e) {
    alert('Lỗi khi lưu: ' + e.message)
  }
}

const validateCode = () => {
  codeErrors.value = {}
  let ok = true
  if (!codeForm.value.systemCodeId?.trim()) { codeErrors.value.systemCodeId = 'Không được để trống'; ok = false }
  if (!codeForm.value.name?.trim())         { codeErrors.value.name = 'Không được để trống'; ok = false }
  return ok
}

const toggleActive = () => {
  codeForm.value.isActive = codeForm.value.isActive === 'Y' ? 'N' : 'Y'
}

// ── Value CRUD ─────────────────────────────────────
const startAddValue = () => {
  valueMode.value      = 'add'
  editingValueId.value = null
  valueErrors.value    = {}
  valueForm.value      = {
    systemCodeId: selectedCode.value.systemCodeId,
    codeValue: '', displayValue: '', displayValueEn: '',
    orderBy: (selectedValues.value.length + 1),
    isDefault: 'N'
  }
}

const startEditValue = (val) => {
  valueMode.value      = 'edit'
  editingValueId.value = val.id
  valueErrors.value    = {}
  valueForm.value      = { ...val }
}

const cancelValue = () => {
  valueMode.value      = 'view'
  editingValueId.value = null
  valueErrors.value    = {}
}

const saveValue = async () => {
  if (!validateValue()) return
  try {
    if (valueMode.value === 'add') {
      // TODO: await axios.post('/api/SystemCodeValue', valueForm.value)
      const newVal = { ...valueForm.value, id: Date.now() }
      selectedCode.value.values.push(newVal)
    } else {
      // TODO: await axios.put(`/api/SystemCodeValue/${valueForm.value.id}`, valueForm.value)
      const idx = selectedCode.value.values.findIndex(v => v.id === valueForm.value.id)
      if (idx !== -1) selectedCode.value.values[idx] = { ...valueForm.value }
    }
    cancelValue()
  } catch (e) {
    alert('Lỗi khi lưu: ' + e.message)
  }
}

const validateValue = () => {
  valueErrors.value = {}
  let ok = true
  if (!valueForm.value.codeValue?.trim())    { valueErrors.value.codeValue = 'Không được để trống'; ok = false }
  if (!valueForm.value.displayValue?.trim()) { valueErrors.value.displayValue = 'Không được để trống'; ok = false }
  return ok
}

const confirmDeleteValue = (val) => { deleteTarget.value = val }

const doDeleteValue = async () => {
  try {
    // TODO: await axios.delete(`/api/SystemCodeValue/${deleteTarget.value.id}`)
    selectedCode.value.values = selectedCode.value.values.filter(v => v.id !== deleteTarget.value.id)
    deleteTarget.value = null
    if (editingValueId.value === deleteTarget.value?.id) cancelValue()
  } catch (e) {
    alert('Lỗi khi xoá: ' + e.message)
  }
}
</script>

<style scoped>
/* ── Layout ────────────────────────────────────────── */
.sc-page {
  display: flex;
  gap: 0;
  height: calc(100vh - 56px);
  background: #f0f2f5;
  font-family: 'Segoe UI', system-ui, sans-serif;
  font-size: 13px;
}

.sc-panel {
  display: flex;
  flex-direction: column;
  background: #fff;
  border-right: 1px solid #e5e7eb;
  transition: width 0.2s ease;
  overflow: hidden;
}

.form-panel  { width: 340px; min-width: 340px; }
.list-panel  { width: 260px; min-width: 44px; flex-shrink: 0; }
.values-panel {
  flex: 1;
  border-right: none;
  opacity: 1;
  transition: opacity 0.2s, width 0.2s;
}
.values-panel.hidden { opacity: 0; pointer-events: none; }
.sc-panel.collapsed { width: 44px !important; min-width: 44px !important; }

/* ── Panel Header ─────────────────────────────────── */
.panel-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 14px;
  height: 46px;
  background: #1a2340;
  color: #fff;
  flex-shrink: 0;
  gap: 8px;
}

.panel-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 600;
  font-size: 13px;
  letter-spacing: 0.3px;
  white-space: nowrap;
  overflow: hidden;
}

.panel-title i { color: #7dd3fc; font-size: 14px; }
.collapse-icon { font-size: 11px; color: #94a3b8; flex-shrink: 0; }

/* ── Toolbar actions ──────────────────────────────── */
.toolbar-actions { display: flex; gap: 6px; }

.btn-action {
  display: inline-flex; align-items: center; gap: 5px;
  padding: 5px 12px; border-radius: 5px;
  font-size: 12px; font-weight: 500; cursor: pointer;
  border: 1px solid transparent; transition: all 0.15s;
}
.btn-action.primary  { background: #3b82f6; color: #fff; border-color: #3b82f6; }
.btn-action.primary:hover  { background: #2563eb; }
.btn-action.success  { background: #10b981; color: #fff; border-color: #10b981; }
.btn-action.success:hover  { background: #059669; }
.btn-action.ghost    { background: transparent; color: #cbd5e1; border-color: #475569; }
.btn-action.ghost:hover    { background: #334155; }
.btn-action.outline  { background: transparent; color: #3b82f6; border-color: #3b82f6; }
.btn-action.outline:hover  { background: #eff6ff; }
.btn-action.danger   { background: #ef4444; color: #fff; border-color: #ef4444; }
.btn-action.danger:hover   { background: #dc2626; }
.btn-action.small    { padding: 4px 9px; font-size: 11.5px; }
.btn-action.icon-only { padding: 5px 8px; }

/* ── Panel body ───────────────────────────────────── */
.panel-body { flex: 1; overflow-y: auto; padding: 16px; }
.panel-body.no-pad { padding: 0; }

/* ── Form ─────────────────────────────────────────── */
.section-label {
  font-size: 11px; font-weight: 700; letter-spacing: 0.8px;
  text-transform: uppercase; color: #6b7280;
  padding-bottom: 10px; border-bottom: 1px solid #f3f4f6;
  margin-bottom: 14px;
  display: flex; align-items: center; gap: 8px;
}
.section-label .tag {
  background: #dbeafe; color: #1d4ed8;
  padding: 1px 7px; border-radius: 4px;
  font-size: 10.5px; letter-spacing: 0;
  text-transform: none; font-weight: 600;
}

.form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
.field { display: flex; flex-direction: column; gap: 4px; }
.field.full { grid-column: 1 / -1; }

.field label {
  font-size: 11.5px; font-weight: 600; color: #4b5563;
}
.req { color: #ef4444; }

.field input {
  height: 34px; padding: 0 10px;
  border: 1px solid #d1d5db; border-radius: 6px;
  font-size: 13px; color: #111827;
  transition: border-color 0.15s, box-shadow 0.15s;
  outline: none;
}
.field input:focus { border-color: #3b82f6; box-shadow: 0 0 0 3px #dbeafe; }
.field input:disabled { background: #f9fafb; color: #9ca3af; cursor: not-allowed; }
.field input.has-error { border-color: #ef4444; }

.err-msg { font-size: 11px; color: #ef4444; }

/* Toggle switch */
.toggle-label { display: flex; align-items: center; gap: 10px; cursor: pointer; }
.toggle {
  width: 38px; height: 20px; border-radius: 99px;
  background: #d1d5db; position: relative;
  transition: background 0.2s; flex-shrink: 0;
}
.toggle.on { background: #10b981; }
.toggle .thumb {
  position: absolute; top: 2px; left: 2px;
  width: 16px; height: 16px; border-radius: 50%;
  background: white; transition: left 0.2s;
  box-shadow: 0 1px 3px rgba(0,0,0,0.2);
}
.toggle.on .thumb { left: 20px; }

.edit-hint { margin-top: 16px; padding-top: 14px; border-top: 1px solid #f3f4f6; }

/* ── Empty hint ───────────────────────────────────── */
.empty-hint {
  display: flex; flex-direction: column;
  align-items: center; justify-content: center;
  height: 100%; gap: 10px; color: #9ca3af;
  text-align: center; padding: 40px 20px;
}
.empty-hint i { font-size: 28px; color: #d1d5db; }
.empty-hint p { font-size: 12.5px; margin: 0; }
.empty-hint.small { padding: 30px 16px; }
.empty-hint.small i { font-size: 22px; }

@keyframes pulse { 0%,100%{transform:translateX(0)} 50%{transform:translateX(6px)} }
.pulse { animation: pulse 1.2s ease-in-out infinite; }

/* ── Code list rows ───────────────────────────────── */
.loading-row {
  padding: 14px 16px; color: #6b7280; font-size: 12.5px;
  display: flex; align-items: center; gap: 8px;
}

.code-row {
  padding: 10px 14px; border-bottom: 1px solid #f3f4f6;
  cursor: pointer; transition: background 0.1s;
}
.code-row:hover { background: #f8fafc; }
.code-row.active { background: #eff6ff; border-left: 3px solid #3b82f6; }

.code-id {
  font-weight: 700; font-size: 12px;
  color: #1e40af; letter-spacing: 0.5px; margin-bottom: 2px;
}
.code-name { color: #374151; font-size: 12.5px; margin-bottom: 5px; }
.code-meta { display: flex; align-items: center; gap: 8px; }

.badge {
  padding: 1px 8px; border-radius: 99px;
  font-size: 10.5px; font-weight: 600;
}
.badge.active   { background: #d1fae5; color: #065f46; }
.badge.inactive { background: #f3f4f6; color: #6b7280; }
.val-count { font-size: 10.5px; color: #9ca3af; }

/* ── Values table ─────────────────────────────────── */
.val-table {
  width: 100%; border-collapse: collapse; font-size: 12.5px;
}
.val-table thead th {
  padding: 9px 12px; text-align: left;
  font-size: 11px; font-weight: 700;
  text-transform: uppercase; letter-spacing: 0.5px;
  color: #6b7280; background: #f9fafb;
  border-bottom: 1px solid #e5e7eb;
  white-space: nowrap;
}
.val-table tbody tr {
  border-bottom: 1px solid #f3f4f6;
  transition: background 0.1s;
}
.val-table tbody tr:hover { background: #f8fafc; }
.val-table tbody tr.editing { background: #fffbeb; }
.val-table td { padding: 9px 12px; color: #374151; }

.order-cell { color: #9ca3af; font-weight: 600; width: 36px; }
.muted { color: #9ca3af; }

.code-chip {
  background: #eff6ff; color: #1d4ed8;
  padding: 2px 8px; border-radius: 4px;
  font-size: 11.5px; font-weight: 600; letter-spacing: 0.3px;
}

.default-dot i { color: #10b981; font-size: 14px; }

.action-cell {
  width: 60px; text-align: right;
  opacity: 0; transition: opacity 0.15s;
}
.val-table tbody tr:hover .action-cell { opacity: 1; }

.icon-btn {
  background: none; border: none; cursor: pointer;
  padding: 4px 5px; border-radius: 4px; font-size: 12px;
  transition: background 0.15s, color 0.15s;
}
.icon-btn.edit   { color: #3b82f6; }
.icon-btn.edit:hover   { background: #eff6ff; }
.icon-btn.delete { color: #ef4444; }
.icon-btn.delete:hover { background: #fef2f2; }

/* ── Modal ────────────────────────────────────────── */
.modal-overlay {
  position: fixed; inset: 0;
  background: rgba(0,0,0,0.4); backdrop-filter: blur(2px);
  display: flex; align-items: center; justify-content: center;
  z-index: 100;
}
.modal-box {
  background: white; border-radius: 12px;
  padding: 28px 32px; width: 360px;
  text-align: center; box-shadow: 0 20px 60px rgba(0,0,0,0.2);
}
.modal-icon { font-size: 36px; margin-bottom: 12px; }
.modal-icon.danger { color: #ef4444; }
.modal-box h3 { font-size: 16px; font-weight: 700; color: #111827; margin: 0 0 8px; }
.modal-box p  { font-size: 13px; color: #6b7280; margin: 0 0 20px; }
.modal-actions { display: flex; gap: 10px; justify-content: center; }
</style>