import {
  ApplicationConfig, inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection,
} from '@angular/core';
import {provideRouter, withInMemoryScrolling} from '@angular/router';

import { routes } from './app.routes';
import {provideHttpClient, withFetch, withInterceptors} from '@angular/common/http';
import {LanguageService} from './core/services/language.service';
import {authInterceptor} from './core/interceptors/auth.interceptor';
import {ConfigService} from './core/services/config.service';
import {acceptLanguageInterceptor} from './core/interceptors/accept-language.interceptor';
import {PageViewTrackerService} from './core/services/page-view-tracker.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes,
       withInMemoryScrolling({
         scrollPositionRestoration: 'disabled'
       })
    ),
    provideHttpClient(
      withFetch(),
       withInterceptors([authInterceptor, acceptLanguageInterceptor])
       ),
    provideAppInitializer(()=> inject(ConfigService).readConfig()),
    provideAppInitializer(() =>inject(LanguageService).init()),
    provideAppInitializer(() => { inject(PageViewTrackerService); }),
    // provideAppInitializer(()=> inject(AuthService).initSession()),
    // provideAppInitializer(()=> inject(ConfigService).readConfigAuth())
  ]
};
