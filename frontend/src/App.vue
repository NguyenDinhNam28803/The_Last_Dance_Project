<template>
  <router-view />
</template>

<script setup>
import { useAuthStore } from './stores/auth'

const auth = useAuthStore()
auth.loadSession()
</script>
