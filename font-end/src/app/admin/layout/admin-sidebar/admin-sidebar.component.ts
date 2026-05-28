import { Component, Input, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule, AsyncPipe } from '@angular/common';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { LanguageService, AvailableLanguage } from '../../../core/services/language.service';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';
import { WallNotificationService } from '../../services/wall-notification.service';

interface NavItem  { label: string; icon: string; route: string; }
interface NavGroup { label: string; items: NavItem[]; }

@Component({
  selector: 'app-admin-sidebar',
  standalone: true,
  imports: [CommonModule, AsyncPipe, RouterLink, RouterLinkActive, LanguagePipe],
  templateUrl: './admin-sidebar.component.html',
  styleUrl: './admin-sidebar.component.css'
})
export class AdminSidebarComponent implements OnInit, OnDestroy {
  @Input() collapsed = false;

  readonly navGroups: NavGroup[] = [
    {
      label: AdminMessage.SIDEBAR_GROUP_OVERVIEW,
      items: [
        { label: AdminMessage.SIDEBAR_NAV_DASHBOARD,  icon: 'bi-speedometer2',      route: '/admin/dashboard' },
      ],
    },
    {
      label: AdminMessage.SIDEBAR_GROUP_CONTENT,
      items: [
        { label: AdminMessage.SIDEBAR_NAV_ARTICLES,   icon: 'bi-file-earmark-text', route: '/admin/articles' },
        { label: AdminMessage.SIDEBAR_NAV_ALBUMS,     icon: 'bi-images',            route: '/admin/albums' },
        { label: AdminMessage.SIDEBAR_NAV_CATEGORIES, icon: 'bi-folder2',           route: '/admin/categories' },
        { label: AdminMessage.SIDEBAR_NAV_TAGS,       icon: 'bi-tags',              route: '/admin/tags' },
        { label: AdminMessage.SIDEBAR_NAV_HERO_SLIDES,   icon: 'bi-collection-play',   route: '/admin/hero-slides' },
        { label: AdminMessage.SIDEBAR_NAV_VIETNAM_MAP,   icon: 'bi-map',               route: '/admin/vietnam-map' },
      ],
    },
    {
      label: AdminMessage.SIDEBAR_GROUP_COMMUNITY,
      items: [
        { label: AdminMessage.SIDEBAR_NAV_COMMENTS,      icon: 'bi-chat-dots',         route: '/admin/comments' },
        { label: AdminMessage.SIDEBAR_NAV_WALL_MESSAGES, icon: 'bi-megaphone',       route: '/admin/wall-messages' },
        { label: AdminMessage.SIDEBAR_NAV_CONTACTS,   icon: 'bi-envelope',          route: '/admin/contacts' },
        { label: AdminMessage.SIDEBAR_NAV_CAREERS,   icon: 'bi-person-badge',      route: '/admin/careers' },
        { label: AdminMessage.SIDEBAR_NAV_USERS,      icon: 'bi-people',            route: '/admin/users' },
      ],
    },
    {
      label: AdminMessage.SIDEBAR_GROUP_I18N,
      items: [
        { label: AdminMessage.SIDEBAR_NAV_LANGUAGES,    icon: 'bi-translate',   route: '/admin/languages' },
        { label: AdminMessage.SIDEBAR_NAV_TRANSLATIONS, icon: 'bi-card-text',   route: '/admin/translations' },
        { label: AdminMessage.SIDEBAR_NAV_CONTENT_TRANS,icon: 'bi-globe',       route: '/admin/content-translations' },
      ],
    },
    {
      label: AdminMessage.SIDEBAR_GROUP_SYSTEM,
      items: [
        { label: AdminMessage.SIDEBAR_NAV_SETTINGS,   icon: 'bi-gear',              route: '/admin/sys-config' },
        { label: AdminMessage.SIDEBAR_NAV_PAGE_VIEWS, icon: 'bi-bar-chart-line',    route: '/admin/page-views' },
      ],
    },
  ];

  protected readonly AdminMessage = AdminMessage;

  availableLanguages: AvailableLanguage[] = [];
  currentLang = '';
  langMenuOpen = false;

  private subs = new Subscription();

  private readonly wallNotifService = inject(WallNotificationService);
  readonly pendingCount$ = this.wallNotifService.pendingCount$;

  constructor(
    private authService: AuthService,
    private router: Router,
    private langService: LanguageService
  ) {}

  ngOnInit(): void {
    this.subs.add(this.langService.availableLanguages$.subscribe(langs => this.availableLanguages = langs));
    this.subs.add(this.langService.currentLanguage$.subscribe(lang => this.currentLang = lang));
  }

  ngOnDestroy(): void { this.subs.unsubscribe(); }

  changeLang(code: string): void {
    this.langMenuOpen = false;
    this.langService.changeLanguage(code).subscribe();
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => this.router.navigate(['/login']),
      error: () => this.router.navigate(['/login'])
    });
  }
}
