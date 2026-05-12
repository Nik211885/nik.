import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';

interface NavItem {
  label: string;
  icon: string;
  route: string;
}

@Component({
  selector: 'app-admin-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, LanguagePipe],
  templateUrl: './admin-sidebar.component.html',
  styleUrl: './admin-sidebar.component.css'
})
export class AdminSidebarComponent {
  readonly navItems: NavItem[] = [
    { label: AdminMessage.SIDEBAR_NAV_DASHBOARD,    icon: 'bi-speedometer2',      route: '/admin/dashboard' },
    { label: AdminMessage.SIDEBAR_NAV_ARTICLES,     icon: 'bi-file-earmark-text', route: '/admin/articles' },
    { label: AdminMessage.SIDEBAR_NAV_ALBUMS,       icon: 'bi-images',            route: '/admin/albums' },
    { label: AdminMessage.SIDEBAR_NAV_CATEGORIES,   icon: 'bi-folder2',           route: '/admin/categories' },
    { label: AdminMessage.SIDEBAR_NAV_TAGS,         icon: 'bi-tags',              route: '/admin/tags' },
    { label: AdminMessage.SIDEBAR_NAV_COMMENTS,     icon: 'bi-chat-dots',         route: '/admin/comments' },
    { label: AdminMessage.SIDEBAR_NAV_USERS,        icon: 'bi-people',            route: '/admin/users' },
    { label: AdminMessage.SIDEBAR_NAV_LANGUAGES,    icon: 'bi-translate',         route: '/admin/languages' },
    { label: AdminMessage.SIDEBAR_NAV_TRANSLATIONS, icon: 'bi-card-text',         route: '/admin/translations' },
    { label: AdminMessage.SIDEBAR_NAV_SETTINGS,     icon: 'bi-gear',              route: '/admin/sys-config' },
  ];

  protected readonly AdminMessage = AdminMessage;

  constructor(private authService: AuthService, private router: Router) {}

  logout(): void {
    this.authService.logout().subscribe({
      next: () => this.router.navigate(['/login']),
      error: () => this.router.navigate(['/login'])
    });
  }
}
