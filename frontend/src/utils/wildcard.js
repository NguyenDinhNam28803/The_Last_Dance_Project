// Tìm kiếm theo ký tự đại diện (*) theo nguyên tắc URD phần Giao diện chung:
//   MIN*   -> bắt đầu bằng "MIN"
//   *MIN   -> kết thúc bằng "MIN"
//   *MIN*  -> chứa "MIN"
//   MIN003 -> khớp chính xác (không có *)
// Tất cả không phân biệt hoa/thường.

// Escape các ký tự đặc biệt của regex, giữ lại '*' để chuyển thành '.*'
function escapeRegex(str) {
  return str.replace(/[.+?^${}()|[\]\\]/g, '\\$&')
}

// So khớp 1 giá trị với 1 mẫu wildcard
export function matchWildcard(pattern, value) {
  if (pattern === null || pattern === undefined || pattern === '') return true
  const val = (value ?? '').toString()
  const escaped = escapeRegex(pattern.toString().trim())
  const regexStr = '^' + escaped.replace(/\*/g, '.*') + '$'
  try {
    return new RegExp(regexStr, 'i').test(val)
  } catch {
    return false
  }
}

// Một bản ghi khớp nếu BẤT KỲ trường nào trong danh sách khớp mẫu
export function matchAnyField(pattern, record, fields) {
  if (!pattern) return true
  return fields.some(f => matchWildcard(pattern, record?.[f]))
}
