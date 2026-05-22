import { inject, Injectable, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import {HttpClient} from '@angular/common/http';
import {BehaviorSubject, catchError, Observable, of, skip, switchMap, tap} from 'rxjs';
import {AuthService} from '../auth/auth.service';
import {LanguageService} from './language.service';


const ApiConfig = {
  GET_CONFIG: "public-api/config",
  GET_CONFIG_AUTH: "api/config-auth"
}

@Injectable({
  providedIn: 'root'
})

export class ConfigService {
  private readonly isBrowser = isPlatformBrowser(inject(PLATFORM_ID));

  private config$ = new BehaviorSubject<Config | null>(null);
  private configAuth$ = new BehaviorSubject<ConfigAuth | null>(null);

  public config: Observable<Config | null> = this.config$.asObservable();
  public configAuth: Observable<ConfigAuth | null> = this.configAuth$.asObservable();

  constructor(
    private readonly httpClient: HttpClient,
    private readonly authService: AuthService,
    private readonly langService: LanguageService,
  ) {
    if (this.isBrowser) {
      langService.currentLanguage$.pipe(
        skip(1),
        switchMap(() => this.httpClient.get<Config>(ApiConfig.GET_CONFIG)),
        catchError(() => of(null))
      ).subscribe(res => this.config$.next(res));
    }
  }

  readConfig(): Observable<Config | null> {
    if (!this.isBrowser) return of(null);
    return this.httpClient.get<Config>(ApiConfig.GET_CONFIG).pipe(
      tap(res => this.config$.next(res))
    );
  }

  readConfigAuth() : Observable<ConfigAuth | null>{
    if(!this.authService.isLoggedIn()){
      return of(null)
    }
    return this.httpClient.get<ConfigAuth>(ApiConfig.GET_CONFIG_AUTH)
      .pipe(tap(res=>this.configAuth$.next(res)));
  }
}


export interface Config {
  sidebar: SidebarConfig[];
  social: SocialConfig[];
  categoryCountArchives: CategoryCountArchives[];
  archivesCountAtTime: ArchivesCountAtTime[];
  info: InfoPageConfig;
}

export interface ConfigAuth {}

export interface SidebarConfig {
  nameKey: string;
  ref: string;
}

export interface SocialConfig {
  id: string;
  name: string;
  ref: string;
  icon: string;
}

export interface CategoryCountArchives {
  id: string;
  name: string;
  count: number;
  ref: string;
}

export interface ArchivesCountAtTime {
  time: string;
  count: number;
  ref: string;
}

export interface InfoPageConfig {
  id: string;
  name: string;
  email: string;
  phone: string;
  address: string;
  website: string;
  avatar: string;
  bio: string;
  introduction: string;
}
