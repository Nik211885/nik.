import {
  ApplicationConfig, inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
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

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes,
       withInMemoryScrolling({
         scrollPositionRestoration: 'disabled'
       })
    ),
    provideHttpClient(
       withInterceptors([authInterceptor, acceptLanguageInterceptor, zoneInterceptor])
       ),
    provideAppInitializer(()=> inject(ConfigService).readConfig()),
    provideAppInitializer(() =>inject(LanguageService).init()),
    provideAppInitializer(() => inject(AuthService).initSession()),
    provideAppInitializer(() => { inject(PageViewTrackerService); }),
  ]
};
