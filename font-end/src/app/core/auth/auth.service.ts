import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  BehaviorSubject,
  catchError,
  combineLatest,
  map,
  Observable,
  of,
  switchMap,
  take,
  tap
} from 'rxjs';
import {
  LoginRequest,
  RegisterRequest,
  TokenResponse,
  UserProfile
} from './auth.model';

/**
 * API endpoints for authentication module
 */
const PREFIX = '/api/auth';
const ApiAuth = {
  LOGIN:         `${PREFIX}/login`,
  LOGOUT:        `${PREFIX}/logout`,
  REGISTER:      `${PREFIX}/register`,
  REFRESH_TOKEN: `${PREFIX}/refresh-token`,
  PROFILE:       `${PREFIX}/profile`,
} as const;

/**
 * AuthService
 *
 * Handles authentication logic including:
 * - Login / Register / Logout
 * - Token storage & refresh
 * - User profile management
 * - Authentication state tracking
 */
@Injectable({ providedIn: 'root' })
export class AuthService {

  /**
   * LocalStorage keys for tokens
   */
  private readonly TOKEN_KEY         = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';

  /**
   * Holds current token state
   */
  private readonly token$ = new BehaviorSubject<TokenResponse | null>(this.loadToken());

  /**
   * Holds current authenticated user profile
   */
  private readonly currentUser$ = new BehaviorSubject<UserProfile | null>(null);

  constructor(private http: HttpClient) {}

  /**
   * Authenticate user with credentials
   *
   * @param credential - Login payload
   * @returns Observable containing access & refresh tokens
   */
  public login(credential: LoginRequest): Observable<TokenResponse> {
    return this.http
      .post<TokenResponse>(ApiAuth.LOGIN, credential)
      .pipe(
        tap((res) => this.saveToken(res))
      );
  }

  /**
   * Register a new user
   *
   * @param payload - Registration data
   * @returns Observable<void>
   */
  public register(payload: RegisterRequest): Observable<void> {
    return this.http.post<void>(ApiAuth.REGISTER, payload);
  }

  /**
   * Logout current user
   * - Clears session locally
   * - Calls logout API
   *
   * @returns Observable<void>
   */
  public logout(): Observable<void> {
    return this.http
      .post<void>(ApiAuth.LOGOUT, {})
      .pipe(
        tap(() => this.clearSession())
      );
  }

  /**
   * Refresh access token using refresh token
   *
   * @returns Observable of new token or null if not available
   */
  public refreshToken(): Observable<TokenResponse | null> {
    return this.token$.pipe(
      take(1),
      switchMap((token) => {
        if (!token?.refreshToken) return of(null);

        return this.http
          .post<TokenResponse>(ApiAuth.REFRESH_TOKEN, {
            refreshToken: token.refreshToken,
          })
          .pipe(
            tap((res) => this.saveToken(res))
          );
      })
    );
  }

  /**
   * Initialize user session on app startup
   * - Checks if token exists
   * - Loads user profile
   * - Clears session if error occurs
   *
   * @returns Observable of UserProfile or null
   */
  public initSession(): Observable<UserProfile | null> {
    if (!this.isLoggedIn()) return of(null);

    return this.fetchProfile().pipe(
      catchError(() => {
        this.clearSession();
        return of(null);
      })
    );
  }

  /**
   * Fetch authenticated user profile from API
   *
   * @returns Observable of UserProfile
   */
  public fetchProfile(): Observable<UserProfile> {
    return this.http
      .get<UserProfile>(ApiAuth.PROFILE)
      .pipe(
        tap((profile) => this.currentUser$.next(profile))
      );
  }

  /**
   * Observable stream of token state
   */
  public get tokenStream(): Observable<TokenResponse | null> {
    return this.token$.asObservable();
  }

  /**
   * Observable stream of current user
   */
  public get currentUserStream(): Observable<UserProfile | null> {
    return this.currentUser$.asObservable();
  }

  /**
   * Get current access token
   *
   * @returns Access token or null
   */
  public getAccessToken(): string | null {
    return this.token$.getValue()?.accessToken ?? null;
  }

  /**
   * Check if user is authenticated
   *
   * @returns true if access token exists
   */
  public isLoggedIn(): boolean {
    return !!this.getAccessToken();
  }

  /**
   * Check if current user has a specific role
   *
   * @param role - Role name (e.g., 'ADMIN')
   * @returns boolean
   */
  public hasRole(role: string) {
    if (!role) return false;

    const roles = this.currentUser$.getValue()?.roles;
    return roles ? roles.includes(role) : false;
  }

  /**
   * Combined authentication state
   * Emits:
   * - token
   * - user
   * - isAuthenticated flag
   */
  public readonly authState$ = combineLatest([
    this.token$,
    this.currentUser$
  ]).pipe(
    map(([token, user]) => ({
      token,
      user,
      isAuthenticated: !!token
    }))
  );

  /**
   * Load token from localStorage
   *
   * @private
   * @returns TokenResponse or null
   */
  private loadToken(): TokenResponse | null {
    const accessToken  = localStorage.getItem(this.TOKEN_KEY);
    const refreshToken = localStorage.getItem(this.REFRESH_TOKEN_KEY);

    return accessToken && refreshToken
      ? { accessToken, refreshToken }
      : null;
  }

  /**
   * Save token to localStorage and update state
   *
   * @private
   * @param token - TokenResponse
   */
  private saveToken(token: TokenResponse): void {
    localStorage.setItem(this.TOKEN_KEY, token.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, token.refreshToken);
    this.token$.next(token);
  }

  /**
   * Clear authentication session
   * - Remove tokens
   * - Reset state
   *
   * @private
   */
  private clearSession(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    this.token$.next(null);
    this.currentUser$.next(null);
  }
}
