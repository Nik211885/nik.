import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap, switchMap } from 'rxjs';
import { HttpClient } from '@angular/common/http';

/**
 * Language API endpoints
 */
const ApiLanguage = {

  /**
   * Get language dictionary by language code
   *
   * @param lang - Language code (e.g., 'en', 'vi')
   */
  GET_LANGUAGE: (lang: string) => `languages?lang=${lang}`
};

/**
 * LanguageService
 *
 * Responsible for managing application internationalization (i18n).
 *
 * Features:
 * - Persist selected language in localStorage
 * - Load translation dictionary from backend
 * - Provide reactive language stream
 * - Provide translation utility method
 * - Support reactive API calls based on language changes
 *
 * Usage:
 * - Initialize in AppComponent
 * - Use `translate()` in pipes/components
 * - Use `withLanguage()` for reactive API calls
 */
@Injectable({
  providedIn: 'root',
})
export class LanguageService {

  /**
   * LocalStorage key for persisting language
   */
  private readonly STORAGE_KEY = 'app_language';

  /**
   * Default fallback language
   */
  private readonly DEFAULT_LANG = 'en';

  /**
   * Current language value (sync access)
   */
  private currentLang = this.DEFAULT_LANG;

  /**
   * Reactive stream for current language
   */
  private currentLanguageSubject = new BehaviorSubject<string>(this.DEFAULT_LANG);

  /**
   * Public observable for language changes
   */
  currentLanguage$ = this.currentLanguageSubject.asObservable();

  /**
   * Translation dictionary store
   */
  private dictionarySubject = new BehaviorSubject<Record<string, string>>({});

  /**
   * Public observable for dictionary updates
   */
  lang$ = this.dictionarySubject.asObservable();

  constructor(private http: HttpClient) {}

  /**
   * Initialize language system
   *
   * Flow:
   * 1. Read saved language from localStorage
   * 2. Fallback to default language if not found
   * 3. Emit current language
   * 4. Load translation dictionary from API
   *
   * @returns Observable of translation dictionary
   */
  init(): Observable<Record<string, string>> {

    const savedLang =
      localStorage.getItem(this.STORAGE_KEY) ?? this.DEFAULT_LANG;

    this.currentLang = savedLang;
    this.currentLanguageSubject.next(savedLang);

    return this.loadLanguage();
  }

  /**
   * Change application language
   *
   * Behavior:
   * - Skip if language is unchanged
   * - Update localStorage
   * - Emit new language
   * - Reload translation dictionary
   *
   * @param lang - New language code
   * @returns Observable of updated dictionary
   */
  changeLanguage(lang: string): Observable<Record<string, string>> {

    if (lang === this.currentLang) {
      return this.lang$;
    }

    this.currentLang = lang;

    /**
     * Persist selected language
     */
    localStorage.setItem(this.STORAGE_KEY, lang);

    /**
     * Notify subscribers (triggers reactive flows)
     */
    this.currentLanguageSubject.next(lang);

    return this.loadLanguage();
  }

  /**
   * Wrap API calls to automatically react to language changes
   *
   * Behavior:
   * - Re-executes API call when language changes
   * - Cancels previous request using switchMap
   *
   * Use case:
   * - Fetch localized data from backend
   *
   * @template T
   * @param factory - Function returning API observable based on language
   * @returns Observable<T>
   *
   * @example
   * this.languageService.withLanguage(lang =>
   *   this.http.get(`/products?lang=${lang}`)
   * )
   */
  withLanguage<T>(factory: (lang: string) => Observable<T>): Observable<T> {

    return this.currentLanguage$.pipe(
      switchMap(lang => factory(lang))
    );
  }

  /**
   * Get current language synchronously
   *
   * @returns Current language code
   */
  getCurrentLanguage(): string {
    return this.currentLang;
  }

  /**
   * Translate a key using current dictionary
   *
   * Behavior:
   * - Returns translated value if exists
   * - Falls back to key if not found
   *
   * @param key - Translation key
   * @returns Translated string
   */
  translate(key: string): string {
    return this.dictionarySubject.getValue()[key] ?? key;
  }

  /**
   * Load translation dictionary from backend
   *
   * @private
   * @returns Observable of dictionary
   */
  private loadLanguage(): Observable<Record<string, string>> {

    return this.http
      .get<Record<string, string>>(
        ApiLanguage.GET_LANGUAGE(this.currentLang)
      )
      .pipe(
        tap(data => this.dictionarySubject.next(data))
      );
  }
}
