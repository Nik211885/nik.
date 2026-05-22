import { Component, DestroyRef, ElementRef, OnInit, ViewChild, inject } from '@angular/core';
import { SeoService } from '../../core/services/seo.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { skip } from 'rxjs';
import { HeroSectionComponent } from '../../shared/components/hero-section/hero-section.component';
import { PostCardComponent } from '../../shared/components/post-card/post-card.component';
import { SearchInputComponent } from '../../shared/components/search-input/search-input.component';
import { ApplicationTitle } from '../../app.message';
import { SidebarListComponent } from '../../shared/components/sidebar-list/sidebar-list.component';
import { SidebarStatItemModel } from '../../shared/components/sidebar-list/sidebar-stat-item.model';
import { LanguagePipe } from '../../shared/pipes/language.pipe';
import { PostCardCompactComponent } from '../../shared/components/post-card-compact/post-card-compact.component';
import { TagCloudComponent } from '../../shared/components/tag-cloud/tag-cloud.component';
import { NewsLetterComponent } from '../../shared/components/news-letter/news-letter.component';
import { ConfigService } from '../../core/services/config.service';
import { AsyncPipe } from '@angular/common';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { ArticleService } from '../../core/services/article.service';
import { LanguageService } from '../../core/services/language.service';
import { ArticleModel } from '../../shared/models/article.model';
import { TagService } from '../../core/services/tag.service';
import { TagCloudModel } from '../../shared/components/tag-cloud/tag-cloud.model';

@Component({
  selector: 'app-fashion',
  imports: [
    HeroSectionComponent,
    PostCardComponent,
    SearchInputComponent,
    SidebarListComponent,
    LanguagePipe,
    PostCardCompactComponent,
    TagCloudComponent,
    NewsLetterComponent,
    AsyncPipe,
    PaginationComponent,
  ],
  templateUrl: './fashion.component.html',
  styleUrl: './fashion.component.css',
})
export class FashionComponent implements OnInit {
  @ViewChild('articleList') articleList!: ElementRef;

  articles: ArticleModel[] = [];
  topArticles: ArticleModel[] = [];
  tags: TagCloudModel[] = [];
  categories: SidebarStatItemModel[] = [];
  archives: SidebarStatItemModel[] = [];
  currentPage = 1;
  pageCount = 0;

  private destroyRef = inject(DestroyRef);

  private seoService = inject(SeoService);

  constructor(
    protected readonly configService: ConfigService,
    private articleService: ArticleService,
    private tagService: TagService,
    private langService: LanguageService,
  ) {}

  ngOnInit(): void {
    this.seoService.set({
      title: 'Fashion',
      description: 'Fashion, lifestyle, and style inspiration.',
      path: '/fashion',
    });

    this.articleService.getTopArticles().subscribe({ next: a => this.topArticles = a.slice(0, 3), error: () => {} });
    this.tagService.getTags().subscribe({ next: t => this.tags = t, error: () => {} });
    this.configService.config.subscribe(config => {
      if (!config) return;
      this.categories = config.categoryCountArchives.map(c => ({ id: c.id, name: c.name, slug: c.ref, count: c.count }));
      this.archives = config.archivesCountAtTime.map((a, i) => ({ id: String(i), name: a.time, slug: a.ref, count: a.count }));
    });
    this.load(1);

    this.langService.currentLanguage$.pipe(
      skip(1),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(() => {
      this.articleService.getTopArticles().subscribe({ next: a => this.topArticles = a.slice(0, 3), error: () => {} });
      this.tagService.getTags().subscribe({ next: t => this.tags = t, error: () => {} });
      this.load(1);
    });
  }

  load(page: number): void {
    this.currentPage = page;
    this.articleService.getArticles(page, 12, 'lifestyle').subscribe(res => {
      this.articles = res.data;
      this.currentPage = res.pageNumber;
      this.pageCount = res.pageCount;
      this.articleList?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
    });
  }

  protected readonly ApplicationTitle = ApplicationTitle;
}
