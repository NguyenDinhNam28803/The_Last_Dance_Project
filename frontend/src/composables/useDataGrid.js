import { ref, computed, watch } from 'vue'
import { matchAnyField } from '@/utils/wildcard'

// Composable lưới dữ liệu dùng chung theo URD phần "Kết quả tìm kiếm":
// - Tìm kiếm wildcard nhiều trường
// - Phân trang 10/20/50/100
// - Cấu hình cột (hiện/ẩn + thứ tự), lưu localStorage theo user
//
// options:
//   source        : Ref<Array> dữ liệu nguồn
//   columns       : [{ key, label, visible=true }]  định nghĩa cột mặc định
//   storageKey    : khóa lưu cấu hình cột (vd 'grid.client')
//   userId        : để tách cấu hình theo từng user
//   searchKeys    : (tùy chọn) các trường được tìm kiếm; mặc định = tất cả key cột
export function useDataGrid(options) {
  const {
    source,
    columns: defaultColumns = [],
    storageKey = 'grid.default',
    userId = 'anon',
    searchKeys = null
  } = options

  const PAGE_SIZES = [10, 20, 50, 100]
  const searchTerm = ref('')
  const pageSize = ref(PAGE_SIZES[0])
  const currentPage = ref(1)

  const fullKey = `${storageKey}:${userId}`

  // ---- Cấu hình cột (nạp từ localStorage nếu có) ----
  function loadColumns() {
    const base = defaultColumns.map((c, i) => ({
      key: c.key,
      label: c.label,
      visible: c.visible !== false,
      order: i
    }))
    try {
      const saved = JSON.parse(localStorage.getItem(fullKey) || 'null')
      if (Array.isArray(saved)) {
        // Merge: giữ định nghĩa hiện tại, áp dụng visible/order đã lưu
        return base
          .map(col => {
            const s = saved.find(x => x.key === col.key)
            return s ? { ...col, visible: s.visible, order: s.order } : col
          })
          .sort((a, b) => a.order - b.order)
      }
    } catch {
      /* bỏ qua cấu hình hỏng */
    }
    return base
  }

  const columns = ref(loadColumns())

  function persistColumns() {
    const payload = columns.value.map(c => ({ key: c.key, visible: c.visible, order: c.order }))
    try {
      localStorage.setItem(fullKey, JSON.stringify(payload))
    } catch {
      /* localStorage không khả dụng -> bỏ qua */
    }
  }

  const visibleColumns = computed(() =>
    columns.value.filter(c => c.visible).sort((a, b) => a.order - b.order)
  )

  function toggleColumn(key) {
    const col = columns.value.find(c => c.key === key)
    if (col) {
      col.visible = !col.visible
      persistColumns()
    }
  }

  function moveColumn(key, direction) {
    const sorted = [...columns.value].sort((a, b) => a.order - b.order)
    const idx = sorted.findIndex(c => c.key === key)
    const swapWith = direction === 'up' ? idx - 1 : idx + 1
    if (idx < 0 || swapWith < 0 || swapWith >= sorted.length) return
    const tmpOrder = sorted[idx].order
    sorted[idx].order = sorted[swapWith].order
    sorted[swapWith].order = tmpOrder
    persistColumns()
  }

  function resetColumns() {
    try {
      localStorage.removeItem(fullKey)
    } catch {
      /* bỏ qua */
    }
    columns.value = loadColumns()
  }

  // ---- Tìm kiếm + phân trang ----
  const keysToSearch = computed(() =>
    searchKeys || defaultColumns.map(c => c.key)
  )

  const filtered = computed(() => {
    const data = source.value || []
    if (!searchTerm.value) return data
    return data.filter(r => matchAnyField(searchTerm.value, r, keysToSearch.value))
  })

  const totalItems = computed(() => filtered.value.length)
  const totalPages = computed(() => Math.max(1, Math.ceil(totalItems.value / pageSize.value)))

  const paged = computed(() => {
    const start = (currentPage.value - 1) * pageSize.value
    return filtered.value.slice(start, start + pageSize.value)
  })

  function goToPage(p) {
    currentPage.value = Math.min(Math.max(1, p), totalPages.value)
  }

  // Reset về trang 1 khi đổi tiêu chí tìm kiếm / kích thước trang / nguồn
  watch([searchTerm, pageSize, () => source.value], () => {
    currentPage.value = 1
  })

  function clearSearch() {
    searchTerm.value = ''
  }

  return {
    PAGE_SIZES,
    searchTerm,
    pageSize,
    currentPage,
    columns,
    visibleColumns,
    filtered,
    paged,
    totalItems,
    totalPages,
    toggleColumn,
    moveColumn,
    resetColumns,
    goToPage,
    clearSearch
  }
}
