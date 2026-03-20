import { ElNotification } from "element-plus";

// 通知类型定义
export type NotificationType = 'success' | 'warning' | 'info' | 'error';

export interface NotificationOptions {
  title: string;
  message: string;
  type?: NotificationType;
  duration?: number;
}

/**
 * 显示通知消息
 */
export function showNotification(options: NotificationOptions): void {
  ElNotification(options);
}

/**
 * 显示成功通知
 */
export function showSuccessNotification(options: NotificationOptions): void {
  ElNotification.success({
    title: options.title,
    message: options.message,
    duration: options.duration ?? 3000,
  });
}

/**
 * 显示警告通知
 */
export function showWarningNotification(options: NotificationOptions): void {
  ElNotification.warning({
    title: options.title,
    message: options.message,
    duration: options.duration ?? 3000,
  });
}

/**
 * 显示错误通知
 */
export function showErrorNotification(options: NotificationOptions): void {
  ElNotification.error({
    title: options.title,
    message: options.message,
    duration: options.duration ?? 3000,
  });
}

/**
 * 显示信息通知
 */
export function showInfoNotification(options: NotificationOptions): void {
  ElNotification.info({
    title: options.title,
    message: options.message,
    duration: options.duration ?? 3000,
  });
}
