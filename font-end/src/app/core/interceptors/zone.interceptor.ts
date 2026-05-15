import { HttpInterceptorFn } from '@angular/common/http';
import { ApplicationRef, inject } from '@angular/core';
import { tap } from 'rxjs';

/**
 * Forces change detection after every HTTP response in Angular 21 zoneless mode.
 *
 * Problem: Angular 21 bootstraps with provideZonelessChangeDetectionInternal()
 * by default. In zoneless mode ApplicationRef.tick() is a no-op (skips dirty
 * flag setup). Plain property assignments in subscribe callbacks never trigger
 * view updates.
 *
 * Fix: schedule detectChanges() as a microtask so it runs AFTER the component
 * subscription callback has already assigned the new data to the component's
 * properties. detectChanges() sets lView[FLAGS] |= 1024 and runs a full
 * mode-0 (global) refresh of the component tree, bypassing zoneless guards.
 */
export const zoneInterceptor: HttpInterceptorFn = (req, next) => {
  const appRef = inject(ApplicationRef);

  return next(req).pipe(
    tap({
      next: () => {
        // Defer to microtask so component subscriber runs first and sets its
        // properties, THEN we trigger detectChanges on the full tree.
        queueMicrotask(() => {
          const root = appRef.components[0];
          root?.changeDetectorRef.detectChanges();
        });
      },
    })
  );
};
