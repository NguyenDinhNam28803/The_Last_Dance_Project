<template>
  <div class="main-content">
    
    <!-- LEFT PANEL: THÔNG TIN CHI TIẾT (FORM) -->
    <div class="form-panel">
      <Toolbar 
        title="Tài khoản" 
        :features="toolbarFeatures" 
        @action="handleToolbarAction" 
      />
      
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
        <fieldset class="form-section">
          <legend class="form-section-title">Thông tin khách hàng</legend>
          <div class="row">
            <div class="col col-3">
              <ValidationInput label="Mã khách hàng" required v-model="formData.clientId" :disabled="mode !== 'add'" :error="errors.clientId" />
            </div>
            <div class="col col-4">
              <ValidationInput label="Tên khách hàng" required v-model="formData.name" :disabled="mode === 'view'" :error="errors.name" />
            </div>
          </div>
          <!-- (rest of the fields simplified for brevity, assume similar pattern) -->
        </fieldset>
      </div>
      
      <!-- TAB 2: LIÊN HỆ -->
      <div v-show="currentTab === 'contact'" class="tab-content">
        <table class="data-table">
          <thead>
            <tr><th>Loại LH</th><th>Giá trị</th></tr>
          </thead>
          <tbody>
            <tr v-for="(c, i) in contacts" :key="i">
              <td>{{ c.contactType }}</td>
              <td>{{ c.detailInfo }}</td>
            </tr>
          </tbody>
        </table>
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
            <tr><th>Mã KH</th><th>Tên khách hàng</th></tr>
          </thead>
          <tbody>
            <tr v-for="cli in clientStore.clients" :key="cli.clientId" @click="selectClient(cli)">
              <td class="font-weight-bold">{{ cli.clientId }}</td>
              <td>{{ cli.name }}</td>
            </tr>
          </tbody>
        </table>
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

const clientStore = useClientStore()
const authStore = useAuthStore()

const isGridCollapsed = ref(false)
const currentTab = ref('general')
const mode = ref('view')
const selectedIds = ref([])
const formData = ref({})
const errors = ref({})
const contacts = ref([])

onMounted(() => clientStore.fetchAll())

const toolbarFeatures = computed(() => {
  if (mode.value === 'add' || mode.value === 'edit') return ['Save', 'Cancel']
  
  const base = ['Search']
  if (authStore.isMaker || authStore.isAdmin) {
    base.push('Add', 'Edit', 'Delete')
  }
  return base
})

const handleToolbarAction = async (action) => {
  if (action === 'add') {
    mode.value = 'add'
    formData.value = {}
  } else if (action === 'save') {
    try {
        if (mode.value === 'add') await clientStore.create(formData.value)
        else await clientStore.update(formData.value.clientId, formData.value)
        alert('Lưu thành công!')
        mode.value = 'view'
        await clientStore.fetchAll()
    } catch (e) { alert('Lỗi') }
  }
}

const selectClient = (cli) => {
  formData.value = { ...cli }
  contacts.value = cli.contacts || []
}
</script>