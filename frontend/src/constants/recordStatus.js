// Trạng thái bản ghi theo đặc tả URD (vòng đời Maker-Checker)
// Mã trạng thái nghiệp vụ của bản ghi Client (Customer.RecordStatus)
export const RECORD_STATUS = {
  PENDING_INSERT: 'PI',
  PENDING_UPDATE: 'PU',
  PENDING_DELETE: 'PD',
  ACTIVE: 'A',
  REJECTED: 'R',
  DELETED: 'D'
}

// Nhãn hiển thị VN/EN cho từng trạng thái
export const RECORD_STATUS_LABELS = {
  [RECORD_STATUS.PENDING_INSERT]: { vi: 'Chờ duyệt thêm', en: 'Pending Insert' },
  [RECORD_STATUS.PENDING_UPDATE]: { vi: 'Chờ duyệt sửa', en: 'Pending Update' },
  [RECORD_STATUS.PENDING_DELETE]: { vi: 'Chờ duyệt xóa', en: 'Pending Delete' },
  [RECORD_STATUS.ACTIVE]: { vi: 'Đã duyệt', en: 'Active' },
  [RECORD_STATUS.REJECTED]: { vi: 'Từ chối', en: 'Rejected' },
  [RECORD_STATUS.DELETED]: { vi: 'Đã xóa', en: 'Deleted' }
}

// Class CSS badge tương ứng để tô màu trạng thái trên grid
export const RECORD_STATUS_BADGE = {
  [RECORD_STATUS.PENDING_INSERT]: 'badge-warning',
  [RECORD_STATUS.PENDING_UPDATE]: 'badge-warning',
  [RECORD_STATUS.PENDING_DELETE]: 'badge-warning',
  [RECORD_STATUS.ACTIVE]: 'badge-success',
  [RECORD_STATUS.REJECTED]: 'badge-danger',
  [RECORD_STATUS.DELETED]: 'badge-default'
}

// Lấy nhãn hiển thị theo ngôn ngữ (mặc định tiếng Việt)
export function recordStatusLabel(code, lang = 'vi') {
  const entry = RECORD_STATUS_LABELS[code]
  if (!entry) return code || '—'
  return entry[lang] || entry.vi
}

// Lấy class badge tương ứng trạng thái
export function recordStatusBadge(code) {
  return RECORD_STATUS_BADGE[code] || 'badge-default'
}
