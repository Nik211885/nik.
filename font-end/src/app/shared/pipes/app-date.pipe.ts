import { Pipe, PipeTransform } from '@angular/core';
import { LanguageService } from '../../core/services/language.service';

/**
 * Formats a date according to the current app language using the browser Intl API.
 * Must be impure so it re-evaluates when language changes.
 */
@Pipe({
  name: 'appDate',
  pure: false,
})
export class AppDatePipe implements PipeTransform {
  constructor(private langService: LanguageService) {}

  transform(value: string | Date, format: 'blog' | 'detail' = 'blog'): string {
    if (!value) return value as any;
    const date   = new Date(value);
    const locale = this.langService.getCurrentLanguage() === 'vi' ? 'vi-VN' : 'en-US';

    switch (format) {
      case 'blog':
        return date.toLocaleDateString(locale, { day: 'numeric', month: 'short', year: 'numeric' });
      case 'detail':
        return date.toLocaleString(locale, {
          day: '2-digit', month: 'long', year: 'numeric',
          hour: 'numeric', minute: '2-digit',
        });
      default:
        return date.toDateString();
    }
  }
}
