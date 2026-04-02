import {Pipe, PipeTransform} from '@angular/core';

/**
 * AppDatePipe
 *
 * Custom Angular pipe for formatting date values.
 *
 * Features:
 * - Supports multiple date formats
 * - Provides a default "blog-style" format
 * - Accepts both string and Date inputs
 *
 * Usage:
 * {{ dateValue | appDate }}
 * {{ dateValue | appDate:'blog' }}
 * {{ dateValue | appDate:'detail' }}
 */
@Pipe({
  name: 'appDate'
})
export class AppDatePipe implements PipeTransform {

  /**
   * Abbreviated month names used for 'blog' format
   * Example: 'Jan.', 'Feb.', ...
   */
  private shortMonths = [
    'Jan.', 'Feb.', 'Mar.', 'Apr.', 'May.', 'Jun.',
    'Jul.', 'Aug.', 'Sep.', 'Oct.', 'Nov.', 'Dec.'
  ];

  /**
   * Full month names used for 'detail' format
   * Example: 'January', 'February', ...
   */
  private fullMonths = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'
  ];

  /**
   * Transform input date into formatted string
   *
   * @param value  - Date input (string or Date object)
   * @param format - Format type (default: 'blog')
   * @returns Formatted date string
   *
   * Supported formats:
   * - 'blog'   : e.g., "Jan. 1, 2024"
   * - 'detail' : e.g., "October 03, 2018 at 2:21pm"
   * - default  : JavaScript toDateString()
   */
  transform(value: string | Date, format: 'blog' | 'detail' = 'blog'): string {

    /**
     * Return original value if null/undefined
     */
    if (!value) return value as any;

    /**
     * Convert input to Date object
     */
    const date = new Date(value);

    switch (format) {

      /**
       * Blog-style format
       * Example: "Jan. 1, 2024"
       */
      case 'blog':
        return `${this.shortMonths[date.getMonth()]} ${date.getDate()}, ${date.getFullYear()}`;

      /**
       * Detail-style format
       * Example: "October 03, 2018 at 2:21pm"
       */
      case 'detail':
        const month   = this.fullMonths[date.getMonth()];
        const day     = String(date.getDate()).padStart(2, '0');
        const year    = date.getFullYear();
        const hours   = date.getHours() % 12 || 12;
        const minutes = String(date.getMinutes()).padStart(2, '0');
        const ampm    = date.getHours() >= 12 ? 'pm' : 'am';
        return `${month} ${day}, ${year} at ${hours}:${minutes}${ampm}`;

      /**
       * Default fallback format
       * Example: "Wed Oct 03 2018"
       */
      default:
        return date.toDateString();
    }
  }
}
