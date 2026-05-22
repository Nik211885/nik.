import { mergeApplicationConfig, ApplicationConfig } from '@angular/core';
import { provideServerRendering as providePlatformServerRendering } from '@angular/platform-server';
import { provideServerRendering, withRoutes } from '@angular/ssr';
import { appConfig } from './app.config';
import { serverRoutes } from './app.routes.server';

const serverConfig: ApplicationConfig = {
  providers: [
    providePlatformServerRendering(),
    provideServerRendering(withRoutes(serverRoutes)),
  ]
};

export const config = mergeApplicationConfig(appConfig, serverConfig);
