import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {HeroCarouselComponent} from '../../shared/components/hero-carousel/hero-carousel.component';
import {PostCardComponent} from '../../shared/components/post-card/post-card.component';
import {PaginationComponent} from '../../shared/components/pagination/pagination.component';
import {PlantBannerComponent} from '../../shared/components/plant-banner/plant-banner.component';
import {ArticleService} from '../../core/services/article.service';
import {ArticleModel} from '../../shared/models/article.model';

@Component({
  selector: 'app-travel',
  imports: [HeroCarouselComponent, PostCardComponent, PaginationComponent, PlantBannerComponent],
  templateUrl: './travel.component.html',
  styleUrl: './travel.component.css',
})
export class TravelComponent implements OnInit {
  @ViewChild('articleList') articleList!: ElementRef;

  articles: ArticleModel[] = [];
  currentPage = 1;
  pageCount = 0;

  constructor(private articleService: ArticleService) {}

  ngOnInit(): void {
    this.load(1);
  }

  load(page: number): void {
    this.currentPage = page;
    this.articleService.getArticles(page, 6).subscribe(res => {
      this.articles = res.data;
      this.currentPage = res.pageNumber;
      this.pageCount = res.pageCount;
      this.articleList?.nativeElement.scrollIntoView({behavior: 'smooth', block: 'start'});
    });
  }
}
