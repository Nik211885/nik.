import { Pipe, PipeTransform } from '@angular/core';

/**
 * TruncatePipe
 *
 * Custom Angular pipe used to truncate long text strings.
 *
 * Features:
 * - Limits string length
 * - Appends suffix when truncated
 * - Safe handling for empty/null values
 *
 * Usage:
 * {{ text | truncate }}
 * {{ text | truncate:30 }}
 * {{ text | truncate:30:'...' }}
 */
@Pipe({
  name: 'truncate'
})
export class TruncatePipe implements PipeTransform {

  /**
   * Transform input string into truncated version
   *
   * @param value - Input string
   * @param limit - Maximum number of characters (default: 50)
   * @param suffix - Suffix to append when truncated (default: "...")
   * @returns Truncated string
   *
   * @example
   * "Hello world" | truncate:5 → "Hello..."
   */
  transform(
    value: string,
    limit: number = 50,
    suffix: string = "..."
  ): string {

    /**
     * Return empty string if value is null/undefined
     */
    if (!value) return "";

    /**
     * If string length is within limit, return original value
     */
    if (value.length <= limit) {
      return value;
    }

    /**
     * Truncate string and append suffix
     */
    return value.substring(0, limit) + suffix;
  }
}
