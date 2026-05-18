import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule, AsyncPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Subscription, forkJoin, interval, timer } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { ArticleAdminService } from '../../services/article.admin.service';
import { AlbumAdminService } from '../../services/album.admin.service';
import { CategoryAdminService } from '../../services/category.admin.service';
import { TagAdminService } from '../../services/tag.admin.service';
import { UserAdminService } from '../../services/user.admin.service';
import { FileAdminService } from '../../services/file.admin.service';
import { ContactAdminService } from '../../services/contact.admin.service';
import { ContactNotificationService } from '../../services/contact-notification.service';
import { ContactItem } from '../../models/admin.model';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';

interface StatCard { label: string; value: number; icon: string; color: string; route: string; }

function getBrowserName(ua: string): string {
  if (ua.includes('Edg/')) return 'Microsoft Edge';
  if (ua.includes('OPR/') || ua.includes('Opera')) return 'Opera';
  if (ua.includes('Chrome/')) return 'Google Chrome';
  if (ua.includes('Firefox/')) return 'Mozilla Firefox';
  if (ua.includes('Safari/') && !ua.includes('Chrome')) return 'Safari';
  return 'Unknown Browser';
}

function getOsName(ua: string): string {
  if (ua.includes('Windows NT 10')) return 'Windows 10/11';
  if (ua.includes('Windows')) return 'Windows';
  if (ua.includes('Mac OS X')) return 'macOS';
  if (ua.includes('Linux')) return 'Linux';
  if (ua.includes('Android')) return 'Android';
  if (ua.includes('iPhone') || ua.includes('iPad')) return 'iOS';
  return 'Unknown OS';
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, AsyncPipe, RouterLink, LanguagePipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit, OnDestroy {
  stats: StatCard[] = [
    { label: AdminMessage.DASHBOARD_STAT_ARTICLES,   value: 0, icon: 'bi-file-earmark-text', color: '#0f3460', route: '/admin/articles' },
    { label: AdminMessage.DASHBOARD_STAT_ALBUMS,     value: 0, icon: 'bi-images',             color: '#e94560', route: '/admin/albums' },
    { label: AdminMessage.DASHBOARD_STAT_CATEGORIES, value: 0, icon: 'bi-folder2',            color: '#533483', route: '/admin/categories' },
    { label: AdminMessage.DASHBOARD_STAT_TAGS,       value: 0, icon: 'bi-tags',               color: '#05668d', route: '/admin/tags' },
    { label: AdminMessage.DASHBOARD_STAT_FILES,      value: 0, icon: 'bi-paperclip',          color: '#028090', route: '/admin/files' },
    { label: AdminMessage.DASHBOARD_STAT_USERS,      value: 0, icon: 'bi-people',             color: '#02c39a', route: '/admin/users' },
    { label: AdminMessage.DASHBOARD_STAT_CONTACTS,   value: 0, icon: 'bi-envelope',           color: '#f4a261', route: '/admin/contacts' },
  ];

  loading = true;
  now = new Date();
  sessionStart = new Date();

  // Session info
  browser = getBrowserName(navigator.userAgent);
  os = getOsName(navigator.userAgent);
  timezone = Intl.DateTimeFormat().resolvedOptions().timeZone;
  language = navigator.language;

  // Recent contacts
  recentContacts: ContactItem[] = [];

  protected readonly AdminMessage = AdminMessage;

  private subs = new Subscription();

  constructor(
    private articleSvc: ArticleAdminService,
    private albumSvc: AlbumAdminService,
    private categorySvc: CategoryAdminService,
    private tagSvc: TagAdminService,
    private userSvc: UserAdminService,
    private fileSvc: FileAdminService,
    private contactSvc: ContactAdminService,
    public notifSvc: ContactNotificationService,
  ) {}

  ngOnInit(): void {
    // live clock
    this.subs.add(interval(1000).subscribe(() => this.now = new Date()));

    // unread contacts for recent messages panel
    this.subs.add(this.notifSvc.unreadList$.subscribe(list => {
      this.recentContacts = list.slice(0, 6);
    }));

    this.subs.add(
      timer(0, 30_000).pipe(
        switchMap(() => forkJoin({
          articles:   this.articleSvc.getAll(1, 1),
          albums:     this.albumSvc.getAll(),
          categories: this.categorySvc.getAll(),
          tags:       this.tagSvc.getAll(),
          files:      this.fileSvc.getAll(1, 1),
          users:      this.userSvc.getAll(),
          contacts:   this.contactSvc.getPage(1, 1),
        }))
      ).subscribe({
        next: (res) => {
          this.stats[0].value = res.articles.totalItems;
          this.stats[1].value = res.albums.length;
          this.stats[2].value = res.categories.length;
          this.stats[3].value = res.tags.length;
          this.stats[4].value = res.files.totalItems;
          this.stats[5].value = res.users.length;
          this.stats[6].value = res.contacts.totalItems;
          this.loading = false;
        },
        error: () => { this.loading = false; }
      })
    );
  }

  ngOnDestroy(): void { this.subs.unsubscribe(); }

  get greeting(): string {
    const h = this.now.getHours();
    if (h < 12) return 'Good morning';
    if (h < 18) return 'Good afternoon';
    return 'Good evening';
  }

  get sessionDuration(): string {
    const diff = Math.floor((this.now.getTime() - this.sessionStart.getTime()) / 1000);
    const m = Math.floor(diff / 60);
    const s = diff % 60;
    if (m === 0) return `${s}s`;
    return `${m}m ${s}s`;
  }

  openContact(item: ContactItem): void {
    if (!item.isRead) {
      this.contactSvc.markAsRead(item.id).subscribe(() => {
        this.notifSvc.removeFromUnread(item.id);
      });
    }
  }
}
