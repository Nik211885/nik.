import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AsyncPipe } from "@angular/common";
import { skip } from 'rxjs';
import { LanguagePipe } from "../../pipes/language.pipe";
import { NewsLetterComponent } from "../news-letter/news-letter.component";
import { PostCardCompactComponent } from "../post-card-compact/post-card-compact.component";
import { SearchInputComponent } from "../search-input/search-input.component";
import { SidebarListComponent } from "../sidebar-list/sidebar-list.component";
import { TagCloudComponent } from "../tag-cloud/tag-cloud.component";
import { ApplicationTitle } from "../../../app.message";
import { ConfigService } from '../../../core/services/config.service';
import { TagService } from '../../../core/services/tag.service';
import { TagCloudModel } from '../tag-cloud/tag-cloud.model';
import { SidebarStatItemModel } from '../sidebar-list/sidebar-stat-item.model';
import { ArticleService } from '../../../core/services/article.service';
import { ArticleModel } from '../../models/article.model';
import { LanguageService } from '../../../core/services/language.service';

@Component({
  selector: 'app-plant-banner',
  imports: [
    AsyncPipe,
    LanguagePipe,
    NewsLetterComponent,
    PostCardCompactComponent,
    SearchInputComponent,
    SidebarListComponent,
    TagCloudComponent
  ],
  templateUrl: './plant-banner.component.html',
  styleUrl: './plant-banner.component.css',
})
export class PlantBannerComponent implements OnInit {
  tags: TagCloudModel[] = [];
  categories: SidebarStatItemModel[] = [];
  archives: SidebarStatItemModel[] = [];
  topArticles: ArticleModel[] = [];

  protected readonly ApplicationTitle = ApplicationTitle;

  private readonly destroyRef = inject(DestroyRef);

  constructor(
    protected readonly configService: ConfigService,
    private tagService: TagService,
    private articleService: ArticleService,
    private langService: LanguageService,
  ) {}

  ngOnInit(): void {
    this.loadArticlesAndTags();
    this.configService.config.subscribe(config => {
      if (!config) return;
      this.categories = config.categoryCountArchives.map(c => ({ id: c.id, name: c.name, slug: c.ref, count: c.count }));
      this.archives = config.archivesCountAtTime.map((a, i) => ({ id: String(i), name: a.time, slug: a.ref, count: a.count }));
    });

    this.langService.currentLanguage$.pipe(
      skip(1),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(() => this.loadArticlesAndTags());
  }

  private loadArticlesAndTags(): void {
    this.articleService.getTopArticles().subscribe({ next: a => this.topArticles = a.slice(0, 4), error: () => {} });
    this.tagService.getTags().subscribe({ next: t => this.tags = t, error: () => {} });
  }
}
