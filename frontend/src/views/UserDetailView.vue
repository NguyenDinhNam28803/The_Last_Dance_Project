<template>
  <div class="user-detail">
    <div class="page-header">
      <div>
        <h1 class="page-title">Chi tiết người dùng</h1>
        <p class="page-subtitle">{{ user?.name }} ({{ user?.custId }})</p>
      </div>
      <button class="btn btn-outline" @click="$router.push('/users')">← Quay lại</button>
    </div>

    <div v-if="user" class="detail-layout">
      <!-- User Info Card -->
      <div class="card">
        <div class="card-header">
          <h2>👤 Thông tin cá nhân</h2>
          <span class="badge" :class="user.status === 'Active' ? 'badge-active' : 'badge-inactive'">
            {{ user.status }}
          </span>
        </div>
        <div class="card-body">
          <div class="user-profile">
            <div class="profile-avatar">{{ userInitials }}</div>
            <div>
              <h3 class="profile-name">{{ user.name }}</h3>
              <p class="profile-role">
                <span class="badge" :class="getRoleBadge(user.roleId)">{{ user.roleName }}</span>
              </p>
            </div>
          </div>
          <div class="detail-grid" style="margin-top: 24px">
            <div class="detail-item"><span class="label">Customer ID</span><span class="value">{{ user.custId }}</span></div>
            <div class="detail-item"><span class="label">Username</span><span class="value">{{ user.userName }}</span></div>
            <div class="detail-item"><span class="label">Email</span><span class="value">{{ user.email || '—' }}</span></div>
            <div class="detail-item"><span class="label">Số điện thoại</span><span class="value">{{ user.phoneNumber || '—' }}</span></div>
            <div class="detail-item"><span class="label">Giới tính</span><span class="value">{{ user.gender === 'M' ? 'Nam' : user.gender === 'F' ? 'Nữ' : '—' }}</span></div>
            <div class="detail-item"><span class="label">Ngày sinh</span><span class="value">{{ user.dateOfBirth || '—' }}</span></div>
            <div class="detail-item"><span class="label">Quốc tịch</span><span class="value">{{ user.nationality || '—' }}</span></div>
            <div class="detail-item"><span class="label">Record Status</span><span class="value">{{ user.recordStatus }}</span></div>
          </div>
        </div>
      </div>

      <!-- Audit Info -->
      <div class="card">
        <div class="card-header">
          <h2>📝 Lịch sử thay đổi</h2>
        </div>
        <div class="card-body">
          <div class="detail-grid">
            <div class="detail-item"><span class="label">Ngày tạo</span><span class="value">{{ formatDate(user.createdDate) }}</span></div>
            <div class="detail-item"><span class="label">Tạo bởi</span><span class="value">{{ user.createdBy || '—' }}</span></div>
            <div class="detail-item"><span class="label">Chỉnh sửa lần cuối</span><span class="value">{{ formatDate(user.lastChangeDate) }}</span></div>
            <div class="detail-item"><span class="label">Sửa bởi</span><span class="value">{{ user.lastChangeBy || '—' }}</span></div>
          </div>
        </div>
      </div>

      <!-- Contacts -->
      <div class="card" style="grid-column: 1 / -1">
        <div class="card-header">
          <h2>📞 Thông tin liên hệ ({{ userContacts.length }})</h2>
        </div>
        <div class="card-body" style="padding:0;overflow-x:auto">
          <table v-if="userContacts.length" class="data-table">
            <thead>
              <tr><th>ID</th><th>Loại</th><th>Nội dung</th><th>Mặc định</th><th>Mô tả</th></tr>
            </thead>
            <tbody>
              <tr v-for="c in userContacts" :key="c.contactId">
                <td><code>{{ c.contactId }}</code></td>
                <td><span class="badge badge-default">{{ getAddTypeLabel(c.addType) }}</span></td>
                <td>{{ c.contact }}</td>
                <td><span v-if="c.isDefault === 'Y'" class="badge badge-active">Mặc định</span><span v-else>—</span></td>
                <td>{{ c.description || '—' }}</td>
              </tr>
            </tbody>
          </table>
          <div v-else class="empty-state">Chưa có thông tin liên hệ</div>
        </div>
      </div>
    </div>

    <div v-else class="empty-state"><div class="icon">❌</div><p>Không tìm thấy người dùng</p></div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { mockUsers, mockContacts, addTypeLabels } from '../data/mockData'

const route = useRoute()
const user = computed(() => mockUsers.find(u => u.custId === route.params.id))
const userContacts = computed(() => mockContacts.filter(c => c.custId === route.params.id))

const userInitials = computed(() => {
  const name = user.value?.name || ''
  return name.split(' ').map(w => w[0]).join('').slice(0, 2).toUpperCase()
})

function getRoleBadge(roleId) {
  if (roleId === 'ADMIN') return 'badge-active'
  if (roleId === 'MANAGER') return 'badge-default'
  return 'badge-pending'
}

function formatDate(d) { return d ? new Date(d).toLocaleDateString('vi-VN') + ' ' + new Date(d).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }) : '—' }
function getAddTypeLabel(type) { return addTypeLabels[type] || type }
</script>

<style scoped>
.detail-layout {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-lg);
}

@media (max-width: 1024px) { .detail-layout { grid-template-columns: 1fr; } }

.user-profile { display: flex; align-items: center; gap: 16px; }

.profile-avatar {
  width: 60px; height: 60px; border-radius: 14px;
  background: var(--color-primary-gradient); color: white;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.3rem; font-weight: 800;
}

.profile-name { font-size: 1.2rem; font-weight: 700; }
.profile-role { margin-top: 4px; }

code {
  background: var(--color-primary-light); color: var(--color-primary);
  padding: 2px 8px; border-radius: 4px; font-size: 0.8rem; font-weight: 600;
}
</style>
