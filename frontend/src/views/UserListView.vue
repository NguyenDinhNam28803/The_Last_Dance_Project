<template>
  <div class="main-content">
    
    <!-- LEFT PANEL: FORM -->
    <div class="form-panel">
      <Toolbar 
        title="Quản lý Người Dùng" 
        :features="toolbarFeatures" 
        @action="handleToolbarAction" 
      />
      
      <!-- MULTIPLE SELECT WARNING -->
      <div v-if="selectedIds.length > 1" class="alert-info mt-3 p-2 text-center" style="font-size: 12.5px; background: #eef2ff; color: #4338ca; border-radius: 4px; border: 1px dashed #c7d2fe;">
        <i class="fas fa-info-circle"></i> Đang chọn <b>{{ selectedIds.length }}</b> người dùng. Tính năng Sửa bị khóa để thao tác hàng loạt.
      </div>

      <div class="form-section" :class="selectedIds.length > 1 ? 'mt-3' : 'mt-4'" :style="selectedIds.length > 1 ? 'opacity: 0.4; pointer-events: none; filter: grayscale(100%);' : ''">
        <legend class="form-section-title">Thông tin Người dùng</legend>
        <div class="row">
          <div class="col col-4">
            <ValidationInput label="Mã User (CustId)" required v-model="formData.custId" :disabled="mode !== 'add'" :error="errors.custId" />
          </div>
          <div class="col col-4">
            <ValidationInput label="Tên Đăng Nhập" required v-model="formData.username" :disabled="mode !== 'add'" :error="errors.username" />
          </div>
          <div class="col col-4">
            <ValidationInput label="Họ Tên đày đủ" required v-model="formData.name" :disabled="mode === 'view'" :error="errors.name" />
          </div>
        </div>
        
        <div class="row mt-3">
          <div class="col col-4">
            <ValidationInput type="email" label="Email" required v-model="formData.email" :disabled="mode === 'view'" :error="errors.email" />
          </div>
          <div class="col col-4">
            <div class="form-group">
              <label class="form-label required">Quyền (Role)</label>
              <select class="form-control" v-model="formData.roleName" :disabled="mode === 'view'">
                <option value="Admin">Quản trị hệ thống (Admin)</option>
                <option value="Manager">Quản lý (Manager)</option>
                <option value="User">Người dùng (User)</option>
              </select>
            </div>
          </div>
          <div class="col col-4">
            <div class="form-group">
              <label class="form-label">Trạng thái</label>
              <select class="form-control" v-model="formData.status" :disabled="mode === 'view'">
                <option value="Active">Hoạt động</option>
                <option value="Inactive">Khóa</option>
              </select>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- RIGHT PANEL: GRID -->
    <div class="grid-panel" :class="{ collapsed: isGridCollapsed }">
      <div class="grid-panel-header">
        <span><i class="fas fa-users"></i> TÀI KHOẢN ({{ users.length }})</span>
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
                  :checked="users.length > 0 && selectedIds.length === users.length" 
                  @change="e => selectedIds = e.target.checked ? users.map(u => u.custId) : []" 
                />
              </th>
              <th>Username</th>
              <th>Họ tên</th>
              <th>Vai trò</th>
              <th>Trạng thái</th>
            </tr>
          </thead>
          <tbody>
            <tr 
              v-for="user in users" 
              :key="user.custId"
              :class="{ active: selectedUser?.custId === user.custId }"
              @click="selectUser(user)"
            >
              <td @click.stop><input type="checkbox" :value="user.custId" v-model="selectedIds" /></td>
              <td class="text-primary font-weight-bold">{{ user.username }}</td>
              <td>{{ user.name }}</td>
              <td>{{ user.roleName }}</td>
              <td>
                <span class="badge" :class="user.status === 'Active' ? 'bg-success text-white' : 'bg-danger text-white'">{{ user.status }}</span>
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
import { mockUsers } from '@/data/mockData'

const isGridCollapsed = ref(false)
const mode = ref('view') // 'view', 'add', 'edit'

const users = ref(mockUsers)
const selectedUser = ref(null)
const selectedIds = ref([])

const formData = ref({})
const errors = ref({})

// Toolbar Action handler
const toolbarFeatures = computed(() => {
  if (mode.value === 'add' || mode.value === 'edit') return ['Save', 'Cancel']
  
  const features = ['Search', 'Add']
  if (selectedIds.value.length <= 1) {
    features.push('Edit')
  }
  features.push('Delete', 'Refresh')
  return features
})

const handleToolbarAction = (action) => {
  if (action === 'add') {
    mode.value = 'add'
    selectedUser.value = null
    formData.value = { roleName: 'User', status: 'Active' }
    errors.value = {}
  } else if (action === 'cancel') {
    mode.value = 'view'
    if (selectedUser.value) selectUser(selectedUser.value)
    else formData.value = {}
  } else if (action === 'edit') {
    if (selectedUser.value) mode.value = 'edit'
    else alert('Vui lòng chọn 1 bản ghi')
  } else if (action === 'save') {
    if (validateForm()) {
      alert('Lưu người dùng thành công!')
      mode.value = 'view'
    }
  } else if (action === 'delete') {
    if (selectedUser.value) {
      if (confirm('Bạn có chắc muốn xoá user này?')) alert('Đã yêu cầu khoá/xoá!')
    }
  }
}

const validateForm = () => {
  errors.value = {}
  let isValid = true
  if (!formData.value.custId) { errors.value.custId = 'Không được để trống'; isValid = false }
  if (!formData.value.username) { errors.value.username = 'Không được để trống'; isValid = false }
  if (!formData.value.name) { errors.value.name = 'Không được để trống'; isValid = false }
  if (!formData.value.email) { errors.value.email = 'Không được để trống'; isValid = false }
  return isValid
}

const selectUser = (user) => {
  if (mode.value !== 'view') return
  selectedUser.value = user
  formData.value = { ...user }
}
</script>

<style scoped>
.bg-success { background: #10b981; }
.bg-danger { background: #ef4444; }
</style>
