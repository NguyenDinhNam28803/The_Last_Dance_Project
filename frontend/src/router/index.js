import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth.js'
import MainLayout from '../components/layout/MainLayout.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/LoginView.vue'),
      meta: { requiresGuest: true }
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/RegisterView.vue'),
      meta: { requiresGuest: true }
    },
    {
      path: '/',
      component: MainLayout,
      meta: { requiresAuth: true },
      children: [
        { path: '', redirect: '/clients' },
        { 
          path: 'dashboard', 
          name: 'dashboard', 
          component: () => import('../views/DashboardView.vue') 
        },
        { 
          path: 'users', 
          name: 'users', 
          component: () => import('../views/UserListView.vue'),
          meta: { requiresAdmin: true }
        },
        { 
          path: 'clients', 
          name: 'clients', 
          component: () => import('../views/ClientView.vue') 
        },
        { 
          path: 'contacts', 
          name: 'contacts', 
          component: () => import('@/views/ContactListView.vue') 
        },
        { 
          path: 'audit', 
          name: 'audit', 
          component: () => import('@/views/AuditLogView.vue') 
        },
        { 
          path: 'maker-checker', 
          name: 'maker-checker', 
          component: () => import('@/views/MakerCheckerView.vue') 
        },
        { 
          path: 'system-code', 
          name: 'system-code', 
          component: () => import('@/views/SystemCodeView.vue'),
          meta: { requiresAdmin: true }
        }
      ]
    }
  ]
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const isAuthenticated = authStore.isAuthenticated

  if (to.meta.requiresAuth && !isAuthenticated) {
    next('/login')
  } else if (to.meta.requiresGuest && isAuthenticated) {
    next('/dashboard')
  } else if (to.meta.requiresAdmin && !authStore.isAdmin) {
    next('/dashboard')
  } else if (to.meta.requiresChecker && !authStore.isChecker && !authStore.isAdmin) {
    next('/dashboard')
  } else if (to.meta.requiresMaker && !authStore.isMaker && !authStore.isAdmin) {
    next('/dashboard')
  } else {
    next()
  }
})

export default router
