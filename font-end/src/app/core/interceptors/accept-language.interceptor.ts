import {HttpInterceptorFn} from '@angular/common/http';
import {inject} from '@angular/core';
import {LanguageService} from '../services/language.service';

/**
 * acceptLanguageInterceptor
 *
 * HTTP Interceptor responsible for attaching the `Accept-Language` header
 * to every outgoing HTTP request.
 *
 * Purpose:
 * - Inform backend about current user language
 * - Enable server-side localization (i18n)
 *
 * Behavior:
 * - Reads current language from LanguageService
 * - Adds `Accept-Language` header if not already present
 * - Preserves existing header if explicitly set
 *
 * Notes:
 * - Uses Angular functional interceptor (Angular 15+)
 * - HttpRequest is immutable → must clone before modifying
 */
export const acceptLanguageInterceptor: HttpInterceptorFn = (req, next) => {

  /**
   * Inject LanguageService to get current language
   */
  const languageService = inject(LanguageService);

  /**
   * Get current application language (e.g., 'en', 'vi')
   */
  const lang = languageService.getCurrentLanguage();

  /**
   * Skip if request already contains Accept-Language header
   *
   * This allows manual override per request
   */
  if (req.headers.has('Accept-Language')) {
    return next(req);
  }

  /**
   * Clone request and attach Accept-Language header
   *
   * Note:
   * - HttpRequest is immutable → cannot modify directly
   */
  const cloned = req.clone({
    setHeaders: {
      'Accept-Language': lang,
    }
  });

  /**
   * Forward modified request to next handler
   */
  return next(cloned);
};
