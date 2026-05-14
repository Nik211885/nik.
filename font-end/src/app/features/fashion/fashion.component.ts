import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {HeroSectionComponent} from '../../shared/components/hero-section/hero-section.component';
import {PostCardComponent} from '../../shared/components/post-card/post-card.component';
import {SearchInputComponent} from '../../shared/components/search-input/search-input.component';
import {ApplicationTitle} from '../../app.message';
import {SidebarListComponent} from '../../shared/components/sidebar-list/sidebar-list.component';
import {listTagCloud, statisticsArchives, statisticsCategories} from '../../app-data.fake';
import {LanguagePipe} from '../../shared/pipes/language.pipe';
import {PostCardCompactComponent} from '../../shared/components/post-card-compact/post-card-compact.component';
import {TagCloudComponent} from '../../shared/components/tag-cloud/tag-cloud.component';
import {NewsLetterComponent} from '../../shared/components/news-letter/news-letter.component';
import {ConfigService} from '../../core/services/config.service';
import {AsyncPipe} from '@angular/common';
import {PaginationComponent} from '../../shared/components/pagination/pagination.component';
import {ArticleService} from '../../core/services/article.service';
import {ArticleModel} from '../../shared/models/article.model';

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
  currentPage = 1;
  pageCount = 0;

  constructor(
    protected readonly configService: ConfigService,
    private articleService: ArticleService,
  ) {}

  ngOnInit(): void {
    this.load(1);
  }

  load(page: number): void {
    this.currentPage = page;
    this.articleService.getArticles(page, 12).subscribe(res => {
      this.articles = res.data;
      this.currentPage = res.pageNumber;
      this.pageCount = res.pageCount;
      this.articleList?.nativeElement.scrollIntoView({behavior: 'smooth', block: 'start'});
    });
  }

  protected readonly ApplicationTitle = ApplicationTitle;
  protected readonly statisticsCategories = statisticsCategories;
  protected readonly listTagCloud = listTagCloud;
  protected readonly statisticsArchives = statisticsArchives;
}
