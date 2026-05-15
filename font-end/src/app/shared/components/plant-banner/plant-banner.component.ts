import { Component, OnInit } from '@angular/core';
import {AsyncPipe} from "@angular/common";
import {LanguagePipe} from "../../pipes/language.pipe";
import {NewsLetterComponent} from "../news-letter/news-letter.component";
import {PostCardCompactComponent} from "../post-card-compact/post-card-compact.component";
import {SearchInputComponent} from "../search-input/search-input.component";
import {SidebarListComponent} from "../sidebar-list/sidebar-list.component";
import {TagCloudComponent} from "../tag-cloud/tag-cloud.component";
import {ApplicationTitle} from "../../../app.message";
import {ConfigService} from '../../../core/services/config.service';
import {TagService} from '../../../core/services/tag.service';
import {TagCloudModel} from '../tag-cloud/tag-cloud.model';
import {SidebarStatItemModel} from '../sidebar-list/sidebar-stat-item.model';
import {ArticleService} from '../../../core/services/article.service';
import {ArticleModel} from '../../models/article.model';

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

  constructor(
    protected readonly configService: ConfigService,
    private tagService: TagService,
    private articleService: ArticleService,
  ) {}

  ngOnInit(): void {
    this.articleService.getTopArticles().subscribe({ next: a => this.topArticles = a.slice(0, 4), error: () => {} });
    this.tagService.getTags().subscribe({ next: t => this.tags = t, error: () => {} });
    this.configService.config.subscribe(config => {
      if (!config) return;
      this.categories = config.categoryCountArchives.map(c => ({ id: c.id, name: c.name, slug: c.ref, count: c.count }));
      this.archives = config.archivesCountAtTime.map((a, i) => ({ id: String(i), name: a.time, slug: a.ref, count: a.count }));
    });
  }
}
