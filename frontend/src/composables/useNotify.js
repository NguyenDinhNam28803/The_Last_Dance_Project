export function useNotify() {
  const ensureContainer = () => {
    let container = document.getElementById('app-notify-container')
    if (!container) {
      container = document.createElement('div')
      container.id = 'app-notify-container'
      container.style.position = 'fixed'
      container.style.right = '20px'
      container.style.top = '20px'
      container.style.zIndex = '9999'
      container.style.display = 'flex'
      container.style.flexDirection = 'column'
      container.style.gap = '10px'
      document.body.appendChild(container)
    }
    return container
  }

  const show = (message, type = 'info', timeout = 3500) => {
    const container = ensureContainer()
    const el = document.createElement('div')
    el.textContent = message
    el.style.padding = '10px 14px'
    el.style.borderRadius = '8px'
    el.style.color = 'white'
    el.style.boxShadow = '0 4px 12px rgba(0,0,0,0.12)'
    el.style.fontSize = '14px'
    el.style.maxWidth = '320px'
    el.style.wordBreak = 'break-word'
    if (type === 'success') el.style.background = '#10b981'
    else if (type === 'error') el.style.background = '#ef4444'
    else if (type === 'warning') el.style.background = '#f59e0b'
    else el.style.background = '#2563eb'

    container.appendChild(el)
    setTimeout(() => {
      el.style.transition = 'opacity 0.3s ease, transform 0.3s ease'
      el.style.opacity = '0'
      el.style.transform = 'translateX(20px)'
      setTimeout(() => container.removeChild(el), 300)
    }, timeout)
  }

  return {
    success: (msg, t) => show(msg, 'success', t),
    error: (msg, t) => show(msg, 'error', t),
    info: (msg, t) => show(msg, 'info', t),
    warn: (msg, t) => show(msg, 'warning', t)
  }
}
