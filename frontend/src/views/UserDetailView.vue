<template>
  <div class="ud-page">

    <!-- ══ TOP BAR ══════════════════════════════════════ -->
    <div class="top-bar">
      <button class="back-btn" @click="$router.push('/users')">
        <i class="fas fa-arrow-left"></i> Quay lại
      </button>

      <div class="top-bar-center">
        <div class="avatar-lg" :class="avatarClass(user?.roleId)">
          {{ initials(user?.name) }}
        </div>
        <div>
          <div class="top-name">{{ user?.name }}</div>
          <div class="top-meta">
            <span class="role-badge" :class="roleBadgeClass(user?.roleId)">
              {{ user?.roleId || 'NO ROLE' }}
            </span>
            <span class="sep">·</span>
            <span class="status-dot" :class="user?.status === 'Active' ? 'on' : 'off'"></span>
            {{ user?.status }}
            <span class="sep">·</span>
            <span class="cust-id">{{ user?.custId }}</span>
          </div>
        </div>
      </div>

      <div class="top-actions">
        <button class="tbtn outline" @click="startEdit" v-if="!editMode">
          <i class="fas fa-pencil-alt"></i> Chỉnh sửa
        </button>
        <template v-else>
          <button class="tbtn success" @click="saveUser">
            <i class="fas fa-check"></i> Lưu
          </button>
          <button class="tbtn ghost" @click="cancelEdit">
            <i class="fas fa-times"></i> Huỷ
          </button>
        </template>
      </div>
    </div>

    <!-- ══ CONTENT ═══════════════════════════════════════ -->
    <div class="ud-body" v-if="user">
      <div class="col-left">

        <!-- Card: Định danh -->
        <div class="card">
          <div class="card-head">
            <i class="fas fa-id-card"></i> Định danh tài khoản
          </div>
          <div class="card-body">
            <div class="field-row">
              <div class="field-item">
                <span class="fl">Customer ID</span>
                <span class="fv mono">{{ user.custId }}</span>
              </div>
              <div class="field-item">
                <span class="fl">Tên đăng nhập</span>
                <span class="fv bold">{{ user.userName }}</span>
              </div>
            </div>
            <div class="field-row">
              <div class="field-item">
                <span class="fl">Họ và tên</span>
                <template v-if="editMode">
                  <input class="fi" v-model="form.name" :class="{ err: errors.name }" />
                  <span class="err-msg" v-if="errors.name">{{ errors.name }}</span>
                </template>
                <span class="fv" v-else>{{ user.name }}</span>
              </div>
              <div class="field-item">
                <span class="fl">Tên khác</span>
                <template v-if="editMode">
                  <input class="fi" v-model="form.nameOther" placeholder="—" />
                </template>
                <span class="fv muted" v-else>{{ user.nameOther || '—' }}</span>
              </div>
            </div>
            <div class="field-row">
              <div class="field-item">
                <span class="fl">Tên viết tắt</span>
                <template v-if="editMode">
                  <input class="fi" v-model="form.shortName" placeholder="—" />
                </template>
                <span class="fv muted" v-else>{{ user.shortName || '—' }}</span>
              </div>
              <div class="field-item">
                <span class="fl">Quyền (Role)</span>
                <template v-if="editMode">
                  <select class="fi" v-model="form.roleId">
                    <option value="">— Chọn —</option>
                    <option v-for="r in roles" :key="r.value" :value="r.value">{{ r.label }}</option>
                  </select>
                </template>
                <span class="fv" v-else>
                  <span class="role-badge" :class="roleBadgeClass(user.roleId)">
                    {{ user.roleId || 'N/A' }}
                  </span>
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Card: Liên hệ -->
        <div class="card">
          <div class="card-head">
            <i class="fas fa-address-book"></i> Thông tin liên hệ
          </div>
          <div class="card-body">
            <div class="field-row">
              <div class="field-item">
                <span class="fl">Email</span>
                <template v-if="editMode">
                  <input class="fi" type="email" v-model="form.email" :class="{ err: errors.email }" />
                  <span class="err-msg" v-if="errors.email">{{ errors.email }}</span>
                </template>
                <span class="fv" v-else>
                  <a :href="'mailto:' + user.email" class="link" v-if="user.email">{{ user.email }}</a>
                  <span class="muted" v-else>—</span>
                </span>
              </div>
              <div class="field-item">
                <span class="fl">Số điện thoại</span>
                <template v-if="editMode">
                  <input class="fi" v-model="form.phoneNumber" placeholder="09xxxxxxxx" />
                </template>
                <span class="fv" v-else>
                  <a :href="'tel:' + user.phoneNumber" class="link" v-if="user.phoneNumber">{{ user.phoneNumber }}</a>
                  <span class="muted" v-else>—</span>
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Card: Cá nhân -->
        <div class="card">
          <div class="card-head">
            <i class="fas fa-user"></i> Thông tin cá nhân
          </div>
          <div class="card-body">
            <div class="field-row">
              <div class="field-item">
                <span class="fl">Giới tính</span>
                <template v-if="editMode">
                  <select class="fi" v-model="form.gender">
                    <option value="">— Chọn —</option>
                    <option value="M">Nam</option>
                    <option value="F">Nữ</option>
                  </select>
                </template>
                <span class="fv" v-else>
                  {{ user.gender === 'M' ? '♂ Nam' : user.gender === 'F' ? '♀ Nữ' : '—' }}
                </span>
              </div>
              <div class="field-item">
                <span class="fl">Ngày sinh</span>
                <template v-if="editMode">
                  <input class="fi" type="date" v-model="form.dateOfBirth" />
                </template>
                <span class="fv" v-else>{{ formatDate(user.dateOfBirth) }}</span>
              </div>
            </div>
            <div class="field-row">
              <div class="field-item">
                <span class="fl">Quốc tịch</span>
                <template v-if="editMode">
                  <input class="fi" v-model="form.nationality" placeholder="VN, US..." />
                </template>
                <span class="fv muted" v-else>{{ user.nationality || '—' }}</span>
              </div>
              <div class="field-item">
                <span class="fl">Quốc gia cư trú</span>
                <template v-if="editMode">
                  <input class="fi" v-model="form.residentCountryId" placeholder="Mã quốc gia..." />
                </template>
                <span class="fv muted" v-else>{{ user.residentCountryId || '—' }}</span>
              </div>
            </div>
          </div>
        </div>

      </div>

      <div class="col-right">

        <!-- Card: Trạng thái hệ thống -->
        <div class="card">
          <div class="card-head">
            <i class="fas fa-shield-alt"></i> Trạng thái hệ thống
          </div>
          <div class="card-body">
            <div class="status-row">
              <div class="status-block" :class="user.status === 'Active' ? 'ok' : 'bad'">
                <div class="sb-label">Account Status</div>
                <div class="sb-val">
                  <span class="status-dot lg" :class="user.status === 'Active' ? 'on' : 'off'"></span>
                  {{ user.status }}
                </div>
                <template v-if="editMode">
                  <select class="fi mt-8" v-model="form.status">
                    <option value="Active">Active</option>
                    <option value="Inactive">Inactive</option>
                  </select>
                </template>
              </div>
              <div class="status-block" :class="user.recordStatus === '1' ? 'ok' : 'bad'">
                <div class="sb-label">Record Status</div>
                <div class="sb-val">
                  <span class="rec-dot" :class="user.recordStatus === '1' ? 'on' : 'off'"></span>
                  {{ user.recordStatus === '1' ? 'Active (1)' : 'Inactive (0)' }}
                </div>
                <template v-if="editMode">
                  <select class="fi mt-8" v-model="form.recordStatus">
                    <option value="1">Active (1)</option>
                    <option value="0">Inactive (0)</option>
                  </select>
                </template>
              </div>
            </div>
          </div>
        </div>

        <!-- Card: Audit log -->
        <div class="card">
          <div class="card-head">
            <i class="fas fa-history"></i> Lịch sử hệ thống
          </div>
          <div class="card-body">
            <div class="timeline">
              <div class="tl-item">
                <div class="tl-dot created"></div>
                <div class="tl-content">
                  <div class="tl-label">Tạo tài khoản</div>
                  <div class="tl-date">{{ formatDatetime(user.createdDate) }}</div>
                  <div class="tl-by" v-if="user.createdBy">bởi <b>{{ user.createdBy }}</b></div>
                  <div class="tl-by muted" v-else>Người tạo: chưa ghi nhận</div>
                </div>
              </div>
              <div class="tl-item" v-if="user.lastChangeDate">
                <div class="tl-dot updated"></div>
                <div class="tl-content">
                  <div class="tl-label">Cập nhật gần nhất</div>
                  <div class="tl-date">{{ formatDatetime(user.lastChangeDate) }}</div>
                  <div class="tl-by" v-if="user.lastChangeBy">bởi <b>{{ user.lastChangeBy }}</b></div>
                </div>
              </div>
              <div class="tl-item tl-no-line" v-if="!user.lastChangeDate">
                <div class="tl-dot muted-dot"></div>
                <div class="tl-content">
                  <div class="tl-label muted">Chưa có cập nhật</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Card: Raw JSON (dev tool) -->
        <div class="card dev-card">
          <div class="card-head toggle-head" @click="showRaw = !showRaw">
            <span><i class="fas fa-code"></i> Raw API Response</span>
            <i class="fas" :class="showRaw ? 'fa-chevron-up' : 'fa-chevron-down'"></i>
          </div>
          <div class="card-body raw-body" v-if="showRaw">
            <pre>{{ JSON.stringify(user, null, 2) }}</pre>
          </div>
        </div>

      </div>
    </div>

    <!-- Not found -->
    <div v-else class="not-found">
      <i class="fas fa-user-slash"></i>
      <p>Không tìm thấy người dùng</p>
      <button class="tbtn outline" @click="$router.push('/users')">← Quay lại danh sách</button>
    </div>

  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route  = useRoute()
const router = useRouter()

// ── Mock data — thay bằng API call thực ────────────
const allUsers = [
  { custId: '14afcad6-48ae-4374-b', userName: 'Namden135',          name: 'Namden135',              nameOther: null, shortName: null, email: 'namndtb00921@fpt.edu.vn',        phoneNumber: '0808080808', roleId: 'CHECKER', roleName: null, status: 'Active',   recordStatus: '1', gender: null, dateOfBirth: null, nationality: null, residentCountryId: null, createdDate: '2026-03-27T16:06:14.9364172', createdBy: null, lastChangeDate: null, lastChangeBy: null },
  { custId: '247cbcd2-957c-4c8d-b', userName: 'Namden789',          name: 'Namden789',              nameOther: null, shortName: null, email: 'namden@gmail.com',               phoneNumber: '0606060606', roleId: 'ADMIN',   roleName: null, status: 'Active',   recordStatus: '1', gender: null, dateOfBirth: null, nationality: null, residentCountryId: null, createdDate: '2026-03-27T16:36:33.6678712', createdBy: null, lastChangeDate: null, lastChangeBy: null },
  { custId: '74f46864-53ce-439a-b', userName: 'Namden246',          name: 'Namden246',              nameOther: null, shortName: null, email: 'nam@gmail.com',                  phoneNumber: '0707070707', roleId: 'MAKER',   roleName: null, status: 'Active',   recordStatus: '1', gender: null, dateOfBirth: null, nationality: null, residentCountryId: null, createdDate: '2026-03-27T16:10:06.1968369', createdBy: null, lastChangeDate: null, lastChangeBy: null },
  { custId: 'ACC_CHECKER_001',      userName: 'checker_system',     name: 'Checker System Account', nameOther: null, shortName: null, email: 'checker_sys@test.com',           phoneNumber: null,         roleId: 'CHECKER', roleName: null, status: 'Active',   recordStatus: '1', gender: null, dateOfBirth: null, nationality: null, residentCountryId: null, createdDate: '2026-03-27T00:00:00',         createdBy: null, lastChangeDate: null, lastChangeBy: null },
  { custId: 'ACC_MAKER_001',        userName: 'maker_system',       name: 'Maker System Account',   nameOther: null, shortName: null, email: 'maker_sys@test.com',             phoneNumber: null,         roleId: 'MAKER',   roleName: null, status: 'Active',   recordStatus: '1', gender: null, dateOfBirth: null, nationality: null, residentCountryId: null, createdDate: '2026-03-27T00:00:00',         createdBy: null, lastChangeDate: null, lastChangeBy: null },
  { custId: 'ace73354-1606-4a26-8', userName: 'Nguyendinhnam28803', name: 'Nguyendinhnam28803',     nameOther: null, shortName: null, email: 'nguyendinhnam241209@gmail.com',  phoneNumber: '0908651852', roleId: null,      roleName: null, status: 'Active',   recordStatus: '1', gender: null, dateOfBirth: null, nationality: null, residentCountryId: null, createdDate: '2026-03-21T01:24:14.7931554', createdBy: null, lastChangeDate: null, lastChangeBy: null },
]

const roles = [
  { value: 'ADMIN',   label: 'Quản trị hệ thống (ADMIN)'  },
  { value: 'MANAGER', label: 'Quản lý (MANAGER)'           },
  { value: 'MAKER',   label: 'Maker'                        },
  { value: 'CHECKER', label: 'Checker'                      },
  { value: 'USER',    label: 'Người dùng (USER)'            },
]

// ── Resolve user từ route param ─────────────────────
// TODO: thay bằng axios.get(`/api/Customer/${route.params.id}`)
const user = ref(allUsers.find(u => u.custId === route.params.id) ?? null)

// ── Edit mode ───────────────────────────────────────
const editMode = ref(false)
const form     = ref({})
const errors   = ref({})
const showRaw  = ref(false)

const startEdit = () => {
  form.value   = { ...user.value }
  errors.value = {}
  editMode.value = true
}

const cancelEdit = () => {
  editMode.value = false
  errors.value   = {}
}

const saveUser = async () => {
  errors.value = {}
  let ok = true
  if (!form.value.name?.trim())  { errors.value.name  = 'Không được để trống'; ok = false }
  if (!form.value.email?.trim()) { errors.value.email = 'Không được để trống'; ok = false }
  if (!ok) return

  try {
    // TODO: await axios.put(`/api/Customer/${form.value.custId}`, form.value)
    Object.assign(user.value, form.value)
    editMode.value = false
  } catch (e) {
    alert('Lỗi khi lưu: ' + e.message)
  }
}

// ── Helpers ─────────────────────────────────────────
const initials = (name) =>
  (name ?? 'U').split(' ').map(w => w[0]).join('').slice(0, 2).toUpperCase()

const formatDate = (d) => {
  if (!d) return '—'
  return new Date(d).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

const formatDatetime = (d) => {
  if (!d) return '—'
  return new Date(d).toLocaleString('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
  })
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
/* ── Root ────────────────────────────────────────── */
.ud-page {
  display: flex; flex-direction: column;
  min-height: calc(100vh - 56px);
  background: #f0f2f5;
  font-family: 'Segoe UI', system-ui, sans-serif;
  font-size: 13px; color: #1f2937;
}

/* ── Top Bar ─────────────────────────────────────── */
.top-bar {
  display: flex; align-items: center;
  justify-content: space-between;
  padding: 0 24px; height: 64px; flex-shrink: 0;
  background: #0f172a;
  border-bottom: 1px solid #1e293b;
  gap: 16px;
}

.back-btn {
  display: flex; align-items: center; gap: 7px;
  background: transparent; border: 1px solid #334155;
  color: #94a3b8; padding: 6px 13px; border-radius: 6px;
  font-size: 12.5px; cursor: pointer; white-space: nowrap;
  transition: all 0.14s;
}
.back-btn:hover { background: #1e293b; color: #e2e8f0; }

.top-bar-center {
  display: flex; align-items: center; gap: 14px; flex: 1;
}

.avatar-lg {
  width: 42px; height: 42px; border-radius: 10px; flex-shrink: 0;
  display: flex; align-items: center; justify-content: center;
  font-size: 15px; font-weight: 800; color: #fff;
}

.top-name {
  font-size: 15px; font-weight: 700; color: #f1f5f9;
  line-height: 1.3;
}
.top-meta {
  display: flex; align-items: center; gap: 7px;
  font-size: 12px; color: #64748b; margin-top: 3px;
}
.sep { color: #334155; }
.cust-id { font-family: monospace; font-size: 11px; color: #475569; }

.top-actions { display: flex; gap: 7px; flex-shrink: 0; }

/* ── Toolbar buttons ─────────────────────────────── */
.tbtn {
  display: inline-flex; align-items: center; gap: 6px;
  padding: 6px 13px; border-radius: 6px;
  font-size: 12.5px; font-weight: 500; cursor: pointer;
  border: 1px solid transparent; transition: all 0.14s;
}
.tbtn.success  { background: #10b981; color: #fff; border-color: #10b981; }
.tbtn.success:hover  { background: #059669; }
.tbtn.outline  { background: transparent; color: #93c5fd; border-color: #334155; }
.tbtn.outline:hover  { background: #1e293b; }
.tbtn.ghost    { background: transparent; color: #64748b; border-color: #334155; }
.tbtn.ghost:hover    { background: #1e293b; color: #94a3b8; }

/* ── Body layout ─────────────────────────────────── */
.ud-body {
  flex: 1; display: grid;
  grid-template-columns: 1fr 380px;
  gap: 16px; padding: 20px 24px;
  align-items: start;
}

.col-left, .col-right {
  display: flex; flex-direction: column; gap: 16px;
}

/* ── Card ────────────────────────────────────────── */
.card {
  background: #fff; border-radius: 10px;
  border: 1px solid #e5e7eb;
  box-shadow: 0 1px 3px rgba(0,0,0,0.04);
  overflow: hidden;
}

.card-head {
  display: flex; align-items: center; gap: 9px;
  padding: 12px 18px;
  font-size: 12px; font-weight: 700;
  text-transform: uppercase; letter-spacing: 0.6px;
  color: #6b7280; background: #f9fafb;
  border-bottom: 1px solid #f3f4f6;
}
.card-head i { color: #3b82f6; font-size: 13px; }

.card-body { padding: 16px 18px; }

/* ── Field row / item ────────────────────────────── */
.field-row {
  display: grid; grid-template-columns: 1fr 1fr;
  gap: 16px; margin-bottom: 14px;
}
.field-row:last-child { margin-bottom: 0; }

.field-item { display: flex; flex-direction: column; gap: 5px; }

.fl {
  font-size: 10.5px; font-weight: 700;
  text-transform: uppercase; letter-spacing: 0.5px; color: #9ca3af;
}

.fv { font-size: 13px; color: #1f2937; font-weight: 500; }
.fv.bold  { font-weight: 700; color: #1e40af; }
.fv.mono  { font-family: monospace; font-size: 11.5px; color: #374151; background: #f3f4f6; padding: 2px 7px; border-radius: 4px; display: inline-block; }
.fv.muted { color: #9ca3af; font-weight: 400; }

.link { color: #3b82f6; text-decoration: none; }
.link:hover { text-decoration: underline; }

/* Inline editable input */
.fi {
  height: 32px; padding: 0 9px;
  border: 1px solid #d1d5db; border-radius: 5px;
  font-size: 12.5px; color: #111827; background: #fff; outline: none;
  transition: border-color 0.15s, box-shadow 0.15s;
}
.fi:focus  { border-color: #3b82f6; box-shadow: 0 0 0 3px #dbeafe; }
.fi.err    { border-color: #ef4444; }
.mt-8      { margin-top: 8px; }
.err-msg   { font-size: 11px; color: #ef4444; }

/* ── Status blocks ───────────────────────────────── */
.status-row { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }

.status-block {
  padding: 13px 14px; border-radius: 8px; border: 1px solid;
}
.status-block.ok  { background: #f0fdf4; border-color: #bbf7d0; }
.status-block.bad { background: #fef2f2; border-color: #fecaca; }

.sb-label {
  font-size: 10.5px; font-weight: 700; text-transform: uppercase;
  letter-spacing: 0.5px; color: #6b7280; margin-bottom: 7px;
}
.sb-val {
  display: flex; align-items: center; gap: 7px;
  font-size: 13px; font-weight: 600;
}

/* ── Status / role dots & badges ─────────────────── */
.status-dot {
  display: inline-block; border-radius: 50%; vertical-align: middle;
  width: 7px; height: 7px;
}
.status-dot.on  { background: #10b981; box-shadow: 0 0 0 2px #d1fae5; }
.status-dot.off { background: #ef4444; box-shadow: 0 0 0 2px #fee2e2; }
.status-dot.lg  { width: 9px; height: 9px; }

.rec-dot {
  display: inline-block; width: 8px; height: 8px; border-radius: 3px; vertical-align: middle;
}
.rec-dot.on  { background: #10b981; }
.rec-dot.off { background: #ef4444; }

.role-badge {
  padding: 2px 9px; border-radius: 4px;
  font-size: 11px; font-weight: 700; letter-spacing: 0.4px;
}
.rb-admin   { background: #fee2e2; color: #b91c1c; }
.rb-manager { background: #ede9fe; color: #6d28d9; }
.rb-maker   { background: #fef3c7; color: #92400e; }
.rb-checker { background: #d1fae5; color: #065f46; }
.rb-none    { background: #f3f4f6; color: #6b7280; }

/* ── Avatar ──────────────────────────────────────── */
.av-admin   { background: linear-gradient(135deg, #ef4444, #b91c1c); }
.av-manager { background: linear-gradient(135deg, #8b5cf6, #6d28d9); }
.av-maker   { background: linear-gradient(135deg, #f59e0b, #d97706); }
.av-checker { background: linear-gradient(135deg, #10b981, #059669); }
.av-default { background: linear-gradient(135deg, #6b7280, #4b5563); }

/* ── Timeline ────────────────────────────────────── */
.timeline { display: flex; flex-direction: column; gap: 0; }

.tl-item {
  display: flex; gap: 12px;
  padding-bottom: 18px; position: relative;
}
.tl-item:not(.tl-no-line)::before {
  content: ''; position: absolute;
  left: 7px; top: 18px; bottom: 0;
  width: 2px; background: #f3f4f6;
}
.tl-item:last-child { padding-bottom: 0; }

.tl-dot {
  width: 16px; height: 16px; border-radius: 50%;
  flex-shrink: 0; margin-top: 2px; border: 2px solid;
}
.tl-dot.created { background: #dbeafe; border-color: #3b82f6; }
.tl-dot.updated { background: #d1fae5; border-color: #10b981; }
.tl-dot.muted-dot { background: #f3f4f6; border-color: #e5e7eb; }

.tl-label { font-weight: 600; font-size: 12.5px; color: #374151; }
.tl-date  { font-size: 12px; color: #6b7280; margin-top: 2px; }
.tl-by    { font-size: 11.5px; color: #9ca3af; margin-top: 2px; }
.muted    { color: #9ca3af !important; }

/* ── Dev card (Raw JSON) ─────────────────────────── */
.dev-card { border-style: dashed; border-color: #e5e7eb; }
.dev-card .card-head { background: #1e293b; color: #64748b; }
.dev-card .card-head i { color: #38bdf8; }

.toggle-head {
  cursor: pointer; user-select: none; justify-content: space-between;
}
.toggle-head:hover { background: #1a2332; }
.toggle-head .fas:last-child { font-size: 11px; }

.raw-body { padding: 0; }
.raw-body pre {
  margin: 0; padding: 14px 16px;
  font-size: 11px; line-height: 1.6;
  color: #10b981; background: #0f172a;
  overflow-x: auto; max-height: 300px;
}

/* ── Not found ───────────────────────────────────── */
.not-found {
  flex: 1; display: flex; flex-direction: column;
  align-items: center; justify-content: center;
  gap: 14px; color: #9ca3af;
}
.not-found i { font-size: 48px; color: #e5e7eb; }
.not-found p { font-size: 14px; }

/* ── Responsive ──────────────────────────────────── */
@media (max-width: 1024px) {
  .ud-body { grid-template-columns: 1fr; }
}
</style>