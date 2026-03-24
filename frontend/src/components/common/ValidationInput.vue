<template>
  <div class="form-group" :class="{ 'has-error': error }">
    <label v-if="label" class="form-label" :class="{ required: required }">
      {{ label }}
    </label>
   
    <div class="input-wrapper">
      <slot>
        <input
          :type="type"
          :value="modelValue"
          @input="$emit('update:modelValue', $event.target.value)"
          @blur="$emit('blur', $event)"
          :placeholder="placeholder"
          :disabled="disabled"
          :readonly="readonly"
          class="form-control"
          :class="{ 'is-invalid': error }"
        />
      </slot>
    </div>
   
    <div v-if="error" class="invalid-feedback">
      {{ error }}
    </div>
  </div>
</template>

<script setup>
defineProps({
  modelValue: { type: [String, Number], default: '' },
  label: String,
  type: { type: String, default: 'text' },
  placeholder: String,
  required: Boolean,
  disabled: Boolean,
  readonly: Boolean,
  error: String
})

defineEmits(['update:modelValue', 'blur'])
</script>

<style scoped>
.input-wrapper { position: relative; }
</style>