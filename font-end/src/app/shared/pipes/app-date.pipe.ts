import { Pipe, PipeTransform } from '@angular/core';

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
 */
@Pipe({
  name: 'appDate'
})
export class AppDatePipe implements PipeTransform {

  /**
   * Transform input date into formatted string
   *
   * @param value - Date input (string or Date object)
   * @param format - Format type (default: 'blog')
   * @returns Formatted date string
   *
   * Supported formats:
   * - 'blog': e.g., "Jan. 1, 2024"
   * - default: JavaScript toDateString()
   */
  transform(value: string | Date, format: string = 'blog'): string {

    /**
     * Return original value if null/undefined
     */
    if (!value) return value as any;

    /**
     * Convert input to Date object
     */
    const date = new Date(value);

    /**
     * Month abbreviations
     */
    const months = [
      'Jan.', 'Feb.', 'Mar.', 'Apr.', 'May.', 'Jun.',
      'Jul.', 'Aug.', 'Sep.', 'Oct.', 'Nov.', 'Dec.'
    ];

    /**
     * Format output based on selected format
     */
    switch (format) {

      /**
       * Blog-style format
       * Example: "Jan. 1, 2024"
       */
      case 'blog':
        return `${months[date.getMonth()]} ${date.getDate()}, ${date.getFullYear()}`;

      /**
       * Default fallback format
       */
      default:
        return date.toDateString();
    }
  }
}
