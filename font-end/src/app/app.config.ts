import {
  ApplicationConfig, inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import {provideRouter, withInMemoryScrolling} from '@angular/router';

import { routes } from './app.routes';
import {provideHttpClient, withInterceptors} from '@angular/common/http';
import {LanguageService} from './core/services/language.service';
import {authInterceptor} from './core/interceptors/auth.interceptor';
import {ConfigService} from './core/services/config.service';
import {acceptLanguageInterceptor} from './core/interceptors/accept-language.interceptor';
import {zoneInterceptor} from './core/interceptors/zone.interceptor';
import {PageViewTrackerService} from './core/services/page-view-tracker.service';
import {AuthService} from './core/auth/auth.service';
import { catchError, of } from 'rxjs';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideClientHydration(withEventReplay()),
    provideRouter(routes,
       withInMemoryScrolling({
         scrollPositionRestoration: 'disabled'
       })
    ),
    provideHttpClient(
       withInterceptors([authInterceptor, acceptLanguageInterceptor, zoneInterceptor])
       ),
    provideAppInitializer(() => inject(ConfigService).readConfig().pipe(catchError(() => of(null)))),
    provideAppInitializer(() => inject(LanguageService).init().pipe(catchError(() => of(null)))),
    provideAppInitializer(() => inject(AuthService).initSession().pipe(catchError(() => of(null)))),
    provideAppInitializer(() => { inject(PageViewTrackerService); }),
  ]
};
