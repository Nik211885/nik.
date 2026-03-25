import {
  HttpErrorResponse,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import {
  BehaviorSubject,
  Observable,
  catchError,
  filter,
  switchMap,
  take,
  throwError,
} from 'rxjs';
import { inject } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { environment } from '../../../environments/environment';
import { TokenResponse } from '../auth/auth.model';

/**
 * Custom headers used to control interceptor behavior
 */
export const HEADER_SKIP_REQUEST    = 'X-Skip-Auth-Request';
export const HEADER_SKIP_BEAR_TOKEN = 'X-Skip-Auth-Bear-Token';

/**
 * Flag to indicate whether token refresh is in progress
 */
let isRefreshing = false;

/**
 * Subject used to queue requests while refreshing token
 */
const refreshTokenSubject$ = new BehaviorSubject<TokenResponse | null>(null);

/**
 * Auth Interceptor
 *
 * Responsibilities:
 * - Attach base API URL
 * - Attach Bearer token to requests
 * - Handle 401 Unauthorized errors
 * - Automatically refresh token
 * - Queue pending requests during token refresh
 */
export const authInterceptor: HttpInterceptorFn = (
  request: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<any> => {

  const authService = inject(AuthService);

  /**
   * Skip entire interceptor if flagged
   */
  if (shouldSkipRequest(request)) {
    return next(removeHeader(request, HEADER_SKIP_REQUEST));
  }

  /**
   * Add base API URL if needed
   */
  request = addBaseUrl(request);

  /**
   * Skip attaching Bearer token if flagged
   */
  if (shouldSkipBearerToken(request)) {
    return next(removeHeader(request, HEADER_SKIP_BEAR_TOKEN));
  }

  /**
   * Attach token and handle errors
   */
  return next(attachBearerToken(request, authService)).pipe(
    catchError((error: HttpErrorResponse) => {

      /**
       * Handle Unauthorized (401) by refreshing token
       */
      if (error.status === 401) {
        return handle401(request, next, authService);
      }

      return throwError(() => error);
    })
  );
};

/**
 * Handle 401 Unauthorized error
 *
 * Strategy:
 * - If not refreshing → call refresh token API
 * - If refreshing → queue request until token is ready
 *
 * @param request - Original HTTP request
 * @param next - HTTP handler
 * @param authService - AuthService instance
 * @returns Observable<any>
 */
function handle401(
  request: HttpRequest<unknown>,
  next: HttpHandlerFn,
  authService: AuthService
): Observable<any> {

  /**
   * If no refresh in progress → start refresh flow
   */
  if (!isRefreshing) {
    isRefreshing = true;
    refreshTokenSubject$.next(null);

    return authService.refreshToken().pipe(
      switchMap((token) => {
        isRefreshing = false;

        /**
         * If refresh fails → logout user
         */
        if (!token) {
          authService.logout().subscribe();
          return throwError(() => new Error('Session expired'));
        }

        /**
         * Notify queued requests and retry original request
         */
        refreshTokenSubject$.next(token);
        return next(attachBearerToken(request, authService));
      }),
      catchError((err) => {
        isRefreshing = false;
        authService.logout().subscribe();
        return throwError(() => err);
      })
    );
  }

  /**
   * If refresh is already in progress → wait for new token
   */
  return refreshTokenSubject$.pipe(
    /**
     * Wait until token is available
     */
    filter((token) => token !== null),

    /**
     * Take the first emitted token
     */
    take(1),

    /**
     * Retry original request with new token
     */
    switchMap(() => next(attachBearerToken(request, authService)))
  );
}

/**
 * Add base API URL to request if not absolute URL
 *
 * @param request - HTTP request
 * @returns Updated request
 */
function addBaseUrl(request: HttpRequest<unknown>): HttpRequest<unknown> {
  if (request.url.startsWith('http')) return request;

  const baseUrl = environment.baseApiUrl.replace(/\/$/, '');
  const path    = request.url.startsWith('/') ? request.url : `/${request.url}`;

  return request.clone({ url: `${baseUrl}${path}` });
}

/**
 * Attach Bearer token to Authorization header
 *
 * @param request - HTTP request
 * @param authService - AuthService instance
 * @returns Updated request with Authorization header
 */
function attachBearerToken(
  request: HttpRequest<unknown>,
  authService: AuthService
): HttpRequest<unknown> {

  const token = authService.getAccessToken();
  if (!token) return request;

  return request.clone({
    setHeaders: { Authorization: `Bearer ${token}` },
  });
}

/**
 * Remove a specific header from request
 *
 * @param request - HTTP request
 * @param headerName - Header to remove
 * @returns Updated request
 */
function removeHeader(
  request: HttpRequest<unknown>,
  headerName: string
): HttpRequest<unknown> {
  if (!request.headers.has(headerName)) return request;
  return request.clone({ headers: request.headers.delete(headerName) });
}

/**
 * Determine whether to skip entire interceptor
 *
 * @param request - HTTP request
 * @returns boolean
 */
function shouldSkipRequest(request: HttpRequest<unknown>): boolean {
  return (
    request.headers.has(HEADER_SKIP_REQUEST) ||
    request.url.includes('cloudinary.com')
  );
}

/**
 * Determine whether to skip attaching Bearer token
 *
 * @param request - HTTP request
 * @returns boolean
 */
function shouldSkipBearerToken(request: HttpRequest<unknown>): boolean {
  return (
    request.headers.has(HEADER_SKIP_BEAR_TOKEN) ||
    request.url.includes('public-api')
  );
}

