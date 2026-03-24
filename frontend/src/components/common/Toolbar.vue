<template>
  <div class="action-toolbar">
    <h2 class="toolbar-title">{{ title }}</h2>
    <div class="toolbar-actions">
      <button 
        v-for="feature in features" 
        :key="feature"
        class="toolbar-btn"
        @click="$emit('action', feature.toLowerCase())"
      >
        {{ feature }}
      </button>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: String,
  features: Array
})

defineEmits(['action'])
</script>

<style scoped>
.action-toolbar {
  height: var(--toolbar-height);
  background: var(--color-bg-surface);
  border-bottom: 1px solid var(--color-border);
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 20px;
  font-size: 13.5px;
}

.toolbar-title {
  font-size: 16px;
  font-weight: 600;
  color: var(--color-primary);
  margin: 0;
}

.toolbar-actions {
  display: flex;
  gap: 8px;
}

.toolbar-btn {
  padding: 6px 14px;
  background: var(--color-bg-surface);
  border: 1px solid var(--color-border);
  border-radius: 4px;
  font-size: 13px;
  cursor: pointer;
  transition: all 0.2s;
}

.toolbar-btn:hover {
  background: var(--color-primary);
  color: white;
  border-color: var(--color-primary);
}
</style>