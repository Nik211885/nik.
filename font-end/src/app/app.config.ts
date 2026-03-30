import {
  ApplicationConfig, inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners
} from '@angular/core';
import {provideRouter, withInMemoryScrolling} from '@angular/router';

import { routes } from './app.routes';
import {provideHttpClient, withFetch, withInterceptors} from '@angular/common/http';
import {LanguageService} from './core/services/language.service';
import {AuthService} from './core/auth/auth.service';
import {authInterceptor} from './core/interceptors/auth.interceptor';
import {ConfigService} from './core/services/config.service';
import {acceptLanguageInterceptor} from './core/interceptors/accept-language.interceptor';

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
    // provideAppInitializer(() =>inject(LanguageService).init()),
    // provideAppInitializer(()=> inject(AuthService).initSession()),
    // provideAppInitializer(()=> inject(ConfigService).readConfigAuth())
  ]
};
