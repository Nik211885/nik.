import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

/**
 * Default display duration for toast (in milliseconds)
 */
const DEFAULT_DURATION = 4000;

/**
 * Supported toast types
 */
export type ToastType = 'success' | 'error' | 'warning' | 'info';

/**
 * Toast model
 *
 * Represents a single notification item
 */
export interface Toast {

  /**
   * Unique identifier
   */
  id: string;

  /**
   * Type of toast (affects UI style)
   */
  type: ToastType;

  /**
   * Main title text
   */
  title: string;

  /**
   * Optional detailed message
   */
  message?: string;

  /**
   * Duration before auto-dismiss (ms)
   * - 0 means persistent (manual close required)
   */
  duration: number;

  /**
   * Whether user can manually close the toast
   */
  closeable: boolean;
}

/**
 * Options for creating a toast
 */
export type ToastOptions = Partial<
  Pick<Toast, 'message' | 'duration' | 'closeable'>
>;

/**
 * ToastService
 *
 * Manages toast notifications across the application.
 *
 * Features:
 * - Create different types of toasts (success, error, warning, info)
 * - Auto-dismiss after duration
 * - Manual dismiss support
 * - Limit number of visible toasts
 * - Reactive stream for UI rendering
 */
@Injectable({
  providedIn: 'root',
})
export class ToastService {

  /**
   * Internal state holding active toasts
   */
  private readonly toasts$ = new BehaviorSubject<Toast[]>([]);

  /**
   * Public observable stream of toasts
   *
   * Used by components to render UI reactively
   */
  public readonly toasts: Observable<Toast[]> = this.toasts$.asObservable();

  /**
   * Show success toast
   *
   * @param title - Toast title
   * @param options - Optional settings
   */
  success(title: string, options?: ToastOptions): void {
    this.add('success', title, options);
  }

  /**
   * Show error toast
   *
   * Default behavior:
   * - duration = 0 (persistent)
   *
   * @param title - Toast title
   * @param options - Optional settings
   */
  error(title: string, options?: ToastOptions): void {
    this.add('error', title, { duration: 0, ...options });
  }

  /**
   * Show warning toast
   *
   * @param title - Toast title
   * @param options - Optional settings
   */
  warning(title: string, options?: ToastOptions): void {
    this.add('warning', title, options);
  }

  /**
   * Show info toast
   *
   * @param title - Toast title
   * @param options - Optional settings
   */
  info(title: string, options?: ToastOptions): void {
    this.add('info', title, options);
  }

  /**
   * Dismiss a specific toast by ID
   *
   * @param id - Toast ID
   */
  dismiss(id: string): void {
    this.toasts$.next(
      this.toasts$.getValue().filter((t) => t.id !== id)
    );
  }

  /**
   * Remove all active toasts
   */
  dismissAll(): void {
    this.toasts$.next([]);
  }

  /**
   * Add a new toast to the list
   *
   * Behavior:
   * - Generates unique ID
   * - Applies default options
   * - Limits max visible toasts (last 5)
   * - Auto-dismiss if duration > 0
   *
   * @private
   * @param type - Toast type
   * @param title - Toast title
   * @param options - Optional settings
   */
  private add(type: ToastType, title: string, options?: ToastOptions): void {
    const toast: Toast = {
      id: this.generateId(),
      type,
      title,
      message: options?.message,
      duration: options?.duration ?? DEFAULT_DURATION,
      closeable: options?.closeable ?? true,
    };

    /**
     * Keep only last 5 toasts (queue behavior)
     */
    const current = this.toasts$.getValue();
    const updated = [...current, toast].slice(-5);

    this.toasts$.next(updated);

    /**
     * Auto-dismiss toast after duration
     */
    if (toast.duration > 0) {
      setTimeout(() => this.dismiss(toast.id), toast.duration);
    }
  }

  /**
   * Generate unique toast ID
   *
   * @private
   * @returns string
   */
  private generateId(): string {
    return `toast_${Date.now()}_${Math.random().toString(36).slice(2, 7)}`;
  }
}
