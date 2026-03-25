import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { map, take } from 'rxjs';

/**
 * Auth Guard (CanActivate)
 *
 * Protects routes by checking authentication state before allowing access.
 *
 * Behavior:
 * - If user is authenticated → allow navigation
 * - If not authenticated → redirect to login page
 *   and preserve the attempted URL for later redirect
 *
 * This guard uses reactive auth state from AuthService.
 */
export const authGuard: CanActivateFn = (route, state) => {

  /**
   * Inject required services
   */
  const authService = inject(AuthService);
  const router      = inject(Router);

  /**
   * Subscribe to authentication state
   * - Take only the latest value
   * - Avoid memory leaks with take(1)
   */
  return authService.authState$.pipe(
    take(1),

    /**
     * Decide whether navigation is allowed
     */
    map(({ isAuthenticated }) => {

      /**
       * Allow access if user is authenticated
       */
      if (isAuthenticated) return true;

      /**
       * Redirect to login page if not authenticated
       * - Preserve attempted URL via query param (returnUrl)
       * - Enables redirect back after successful login
       */
      return router.createUrlTree(['/login'], {
        queryParams: { returnUrl: state.url },
      });
    })
  );
};
