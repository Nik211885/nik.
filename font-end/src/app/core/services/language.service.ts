import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';

/**
 * API helper for language endpoints
 */
const ApiLanguage = {
  /**
   * Generate API endpoint for fetching language data
   *
   * @param lang - Language code
   * @returns API URL string
   */
  GET_LANGUAGE: (lang: string) => `languages?lang=${lang}`
};


/**
 * LanguageService
 *
 * This service is responsible for:
 * - Managing the current application language
 * - Loading language resources from API
 * - Providing translation values for UI
 * - Persisting selected language in localStorage
 */
@Injectable({
  providedIn: 'root',
})
export class LanguageService {

  /**
   * Key used to store language in localStorage
   */
  private readonly APP_LANGUAGE = "app_language";

  /**
   * Current active language (default: 'en')
   */
  private CURRENT_LANGUAGE = "en";

  /**
   * Holds the current language dictionary (key-value pairs)
   */
  private languageSubject = new BehaviorSubject<Record<string, string>>({});

  /**
   * Observable stream for components to subscribe and react to language changes
   */
  lang$ = this.languageSubject.asObservable();

  constructor(private http: HttpClient) {}

  /**
   * Initialize language settings
   * - Load language from localStorage if exists
   * - Otherwise, use default language and store it
   * - Fetch language data from API
   *
   * @returns Observable containing language key-value pairs
   */
  init(): Observable<Record<string, string>> {
    const savedLanguage = localStorage.getItem(this.APP_LANGUAGE);

    if (savedLanguage) {
      this.CURRENT_LANGUAGE = savedLanguage;
    } else {
      localStorage.setItem(this.APP_LANGUAGE, this.CURRENT_LANGUAGE);
    }

    return this.loadLanguages();
  }

  /**
   * Fetch language data from API based on current language
   * and update the internal BehaviorSubject
   *
   * @private
   * @returns Observable containing language dictionary
   */
  private loadLanguages(): Observable<Record<string, string>> {
    return this.http
      .get<Record<string, string>>(ApiLanguage.GET_LANGUAGE(this.CURRENT_LANGUAGE))
      .pipe(
        tap(res => this.languageSubject.next(res))
      );
  }

  /**
   * Change application language
   * - Avoids reloading if the selected language is already active
   * - Updates localStorage
   * - Fetches new language data
   *
   * @param lang - Language code (e.g., 'en', 'vi')
   * @returns Observable of updated language dictionary
   */
  changeLanguage(lang: string) {

    if (lang === this.CURRENT_LANGUAGE) {
      return this.languageSubject.asObservable();
    }

    this.CURRENT_LANGUAGE = lang;
    localStorage.setItem(this.APP_LANGUAGE, lang);

    return this.loadLanguages();
  }

  /**
   * Get current active language
   *
   * @returns Current language code
   */
  getCurrentLanguage(): string {
    return this.CURRENT_LANGUAGE;
  }

  /**
   * Translate a given key using current language dictionary
   * - Returns the key itself if translation is missing
   *
   * @param code - Translation key
   * @returns Translated string or fallback key
   */
  translate(code: string): string {
    const langs = this.languageSubject.getValue();
    return langs[code] ?? code;
  }
}
