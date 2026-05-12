import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ArticleAdminService } from '../../services/article.admin.service';
import { AlbumAdminService } from '../../services/album.admin.service';
import { CategoryAdminService } from '../../services/category.admin.service';
import { TagAdminService } from '../../services/tag.admin.service';
import { UserAdminService } from '../../services/user.admin.service';
import { FileAdminService } from '../../services/file.admin.service';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';

interface StatCard { label: string; value: number; icon: string; color: string; route: string; }

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, LanguagePipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  stats: StatCard[] = [
    { label: AdminMessage.DASHBOARD_STAT_ARTICLES,   value: 0, icon: 'bi-file-earmark-text', color: '#0f3460', route: '/admin/articles' },
    { label: AdminMessage.DASHBOARD_STAT_ALBUMS,     value: 0, icon: 'bi-images',             color: '#e94560', route: '/admin/albums' },
    { label: AdminMessage.DASHBOARD_STAT_CATEGORIES, value: 0, icon: 'bi-folder2',            color: '#533483', route: '/admin/categories' },
    { label: AdminMessage.DASHBOARD_STAT_TAGS,       value: 0, icon: 'bi-tags',               color: '#05668d', route: '/admin/tags' },
    { label: AdminMessage.DASHBOARD_STAT_FILES,      value: 0, icon: 'bi-paperclip',          color: '#028090', route: '/admin/files' },
    { label: AdminMessage.DASHBOARD_STAT_USERS,      value: 0, icon: 'bi-people',             color: '#02c39a', route: '/admin/users' },
  ];
  loading = true;

  protected readonly AdminMessage = AdminMessage;

  constructor(
    private articleSvc: ArticleAdminService,
    private albumSvc: AlbumAdminService,
    private categorySvc: CategoryAdminService,
    private tagSvc: TagAdminService,
    private userSvc: UserAdminService,
    private fileSvc: FileAdminService,
  ) {}

  ngOnInit(): void {
    forkJoin({
      articles:   this.articleSvc.getAll(1, 1),
      albums:     this.albumSvc.getAll(),
      categories: this.categorySvc.getAll(),
      tags:       this.tagSvc.getAll(),
      files:      this.fileSvc.getAll(1, 1),
      users:      this.userSvc.getAll(),
    }).subscribe({
      next: (res) => {
        this.stats[0].value = res.articles.totalItems;
        this.stats[1].value = res.albums.length;
        this.stats[2].value = res.categories.length;
        this.stats[3].value = res.tags.length;
        this.stats[4].value = res.files.totalItems;
        this.stats[5].value = res.users.length;
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }
}
