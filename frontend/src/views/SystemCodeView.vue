<template>
  <div class="main-content">
    <div class="form-panel">
      <Toolbar
        title="Tham Số Hệ Thống"
        :features="toolbarFeatures"
        @action="handleToolbarAction"
      />
     
      <div v-if="!selectedCode && mode === 'view'" class="alert-info mt-4 p-3">
        Vui lòng chọn một danh mục bên phải để xem chi tiết.
      </div>
      <div v-else class="form-section mt-4">
        <legend class="form-section-title">Thông tin Danh Mục</legend>
        <div class="row">
          <div class="col col-6">
            <ValidationInput label="Mã Tham số" required v-model="formData.id" :disabled="mode !== 'add'" :error="errors.id" />
          </div>
          <div class="col col-6">
            <ValidationInput label="Tên Danh mục" required v-model="formData.name" :disabled="mode === 'view'" :error="errors.name" />
          </div>
        </div>
        <div class="row mt-2">
          <div class="col col-12">
            <ValidationInput label="Mô tả" v-model="formData.description" :disabled="mode === 'view'" />
          </div>
        </div>
        <div class="row mt-2">
          <div class="col col-12">
            <label>
              <input type="checkbox" v-model="formData.isActive" :disabled="mode === 'view'"> Hoạt động
            </label>
          </div>
        </div>
      </div>
    </div>
   
    <div class="grid-panel" :class="{ collapsed: isGridCollapsed }">
      <div class="grid-panel-header">
        <span>Tham số ({{ codes.length }})</span>
        <button class="btn btn-outline" style="padding: 0 10px;" @click="isGridCollapsed = !isGridCollapsed" title="Đóng/Mở lưới">
          <i class="fas" :class="isGridCollapsed ? 'fa-chevron-left' : 'fa-chevron-right'"></i>
          <span v-if="!isGridCollapsed" style="margin-left:5px">Thu gọn</span>
        </button>
      </div>
     
      <div class="grid-content">
        <table class="grid-table">
          <thead>
            <tr>
              <th>SystemCodeId</th>
              <th>Tên Danh mục</th>
              <th>Trạng thái</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="code in codes"
              :key="code.id"
              :class="{ active: selectedCode?.id === code.id }"
              @click="selectCode(code)"
            >
              <td class="font-weight-bold">{{ code.id }}</td>
              <td>{{ code.name }}</td>
              <td>
                <span class="status-badge" :class="code.isActive ? 'success' : 'secondary'">
                  {{ code.isActive ? 'active' : 'blocked' }}
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
import { mockSystemCodes } from '@/data/mockData'

const isGridCollapsed = ref(false)
const mode = ref('view')
const codes = ref(mockSystemCodes)
const selectedCode = ref(null)
const formData = ref({})
const errors = ref({})

const toolbarFeatures = computed(() => {
  if (mode.value === 'add' || mode.value === 'edit') return ['Save', 'Cancel']
  return ['Add', 'Edit', 'Delete', 'Refresh']
})

const handleToolbarAction = (action) => {
  if (action === 'add') {
    mode.value = 'add'
    formData.value = { isActive: true }
  } else if (action === 'cancel') {
    mode.value = 'view'
    if (selectedCode.value) selectCode(selectedCode.value)
  } else if (action === 'edit') {
    if (selectedCode.value) mode.value = 'edit'
  } else if (action === 'save') {
    if (validateForm()) {
      alert('Lưu thành công!')
      mode.value = 'view'
    }
  }
}

const validateForm = () => {
  errors.value = {}
  let isValid = true
  if (!formData.value.id) { errors.value.id = 'Không được để trống'; isValid = false }
  if (!formData.value.name) { errors.value.name = 'Không được để trống'; isValid = false }
  return isValid
}

const selectCode = (code) => {
  if (mode.value !== 'view') return
  selectedCode.value = code
  formData.value = { ...code }
}
</script>

<style scoped>
.status-badge {
  padding: 3px 9px;
  border-radius: 9999px;
  font-size: 11.5px;
  font-weight: 500;
}
.status-badge.success { background: #10b981; color: white; }
.status-badge.secondary { background: #6b7280; color: white; }
</style>