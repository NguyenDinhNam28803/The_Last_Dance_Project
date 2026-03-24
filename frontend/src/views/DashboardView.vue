<template>
  <div class="dashboard">
    <div class="page-header">
      <div>
        <h1 class="page-title">Dashboard</h1>
        <p class="page-subtitle">Tổng quan hệ thống</p>
      </div>
    </div>
    
    <div class="stat-grid">
      <div class="stat-card">
        <div class="stat-info">
          <h3>{{ stats.totalUsers }}</h3>
          <p>Tổng người dùng</p>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-info">
          <h3>{{ stats.activeUsers }}</h3>
          <p>Đang hoạt động</p>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-info">
          <h3>{{ stats.pendingRequests }}</h3>
          <p>Chờ phê duyệt</p>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-info">
          <h3>{{ stats.inactiveUsers }}</h3>
          <p>Ngừng hoạt động</p>
        </div>
      </div>
    </div>
    
    <div class="dashboard-grid">
      <div class="card">
        <div class="card-header">
          <h2>Hoạt động gần đây</h2>
        </div>
        <div class="card-body">
          <div class="activity-list">
            <div v-for="log in recentLogs" :key="log.mtTranId" class="activity-item">
              <div class="activity-content">
                <p><strong>{{ log.maker }}</strong> đã {{ getTypeAction(log.mtlType) }} <span class="activity-obj">{{ log.objChange }}</span></p>
                <span class="activity-time">{{ formatDate(log.actionDate) }}</span>
              </div>
              <span class="status-badge" :class="log.mtlStatus === 'A' ? 'success' : 'warning'">
                {{ log.mtlStatus === 'A' ? 'Đã duyệt' : 'Chờ duyệt' }}
              </span>
            </div>
          </div>
        </div>
      </div>
      
      <div class="card">
        <div class="card-header">
          <h2>Thao tác nhanh</h2>
        </div>
        <div class="card-body">
          <div class="quick-actions">
            <router-link to="/users" class="quick-action-item">Quản lý người dùng</router-link>
            <router-link to="/contacts" class="quick-action-item">Thông tin liên hệ</router-link>
            <router-link to="/maker-checker" class="quick-action-item">Phê duyệt yêu cầu</router-link>
            <router-link to="/audit" class="quick-action-item">Nhật ký hệ thống</router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { mockUsers, mockAuditLogs } from '../data/mockData'

const stats = computed(() => ({
  totalUsers: mockUsers.length,
  activeUsers: mockUsers.filter(u => u.status === 'Active').length,
  inactiveUsers: mockUsers.filter(u => u.status === 'Inactive').length,
  pendingRequests: 12 // mock
}))

const recentLogs = computed(() => mockAuditLogs.slice(0, 5))

const getTypeAction = (type) => {
  const actions = { C: 'tạo mới', E: 'chỉnh sửa', D: 'xóa' }
  return actions[type] || 'thao tác'
}

const formatDate = (dateStr) => {
  const d = new Date(dateStr)
  return d.toLocaleDateString('vi-VN')
}
</script>

<style scoped>
.dashboard { padding: 20px; }
.page-title { font-size: 20px; font-weight: 600; color: var(--color-primary); }
.stat-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 16px;
  margin-bottom: 24px;
}
.stat-card {
  background: white;
  padding: 20px;
  border-radius: 6px;
  border: 1px solid var(--color-border);
}
.stat-card h3 { font-size: 28px; margin: 0; font-weight: 700; }
.page-subtitle { color: var(--color-text-secondary); margin-top: 4px; }

.dashboard-grid {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 20px;
}
.card { background: white; border-radius: 6px; border: 1px solid var(--color-border); }
.card-header { padding: 16px 20px; border-bottom: 1px solid var(--color-border); }
.card-header h2 { margin: 0; font-size: 15px; font-weight: 600; }

.status-badge {
  padding: 4px 10px;
  border-radius: 9999px;
  font-size: 12px;
}
.status-badge.success { background: #10b981; color: white; }
.status-badge.warning { background: #f59e0b; color: white; }

.quick-actions {
  display: flex;
  flex-direction: column;
  gap: 8px;
  padding: 10px;
}
.quick-action-item {
  padding: 12px 16px;
  border: 1px solid var(--color-border);
  border-radius: 4px;
  text-decoration: none;
  color: var(--color-text-primary);
}
.quick-action-item:hover {
  background: var(--color-bg-base);
  border-color: var(--color-primary);
}
</style>