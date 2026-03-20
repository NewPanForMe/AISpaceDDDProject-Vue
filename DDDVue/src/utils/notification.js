import { Notification } from 'element-plus';

/**
 * 显示通知消息
 * @param {Object} options - 通知配置
 * @param {string} options.title - 通知标题
 * @param {string} options.message - 通知消息
 * @param {string} [options.type='info'] - 通知类型: 'success' | 'warning' | 'info' | 'error'
 * @param {number} [options.duration=3000] - 显示时长（毫秒）
 */
export function showNotification({ title, message, type = 'info', duration = 3000 }) {
  Notification({
    title,
    message,
    type,
    duration
  });
}

/**
 * 显示成功通知
 */
export function showSuccessNotification({ title, message, duration = 3000 }) {
  Notification.success({
    title,
    message,
    duration
  });
}

/**
 * 显示警告通知
 */
export function showWarningNotification({ title, message, duration = 3000 }) {
  Notification.warning({
    title,
    message,
    duration
  });
}

/**
 * 显示错误通知
 */
export function showErrorNotification({ title, message, duration = 3000 }) {
  Notification.error({
    title,
    message,
    duration
  });
}

/**
 * 显示信息通知
 */
export function showInfoNotification({ title, message, duration = 3000 }) {
  Notification.info({
    title,
    message,
    duration
  });
}
