import { Pipe, PipeTransform } from '@angular/core';
import { LanguageService } from '../../core/services/language.service';

/**
 * LanguagePipe
 *
 * Angular pipe used to translate a given key into the current language.
 *
 * This pipe:
 * - Subscribes to language changes indirectly via LanguageService
 * - Is marked as `pure: false` to allow UI updates when language data changes
 *
 * Usage example in template:
 * ```html
 * {{ 'HELLO' | language }}
 * ```
 */
@Pipe({
  name: 'language',
  pure: false
})
export class LanguagePipe implements PipeTransform {

  /**
   * Inject LanguageService to access translation logic
   *
   * @param languageService - Service handling language state and translations
   */
  constructor(private languageService: LanguageService) {}

  /**
   * Transform a translation key into its localized string
   *
   * @param code - Translation key (e.g., 'HELLO', 'WELCOME_MESSAGE')
   * @returns Translated string or fallback key if not found
   */
  transform(code: string): string {
    return this.languageService.translate(code);
  }
}
