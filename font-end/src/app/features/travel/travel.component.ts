import { Component, DestroyRef, ElementRef, OnInit, ViewChild, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { skip } from 'rxjs';
import { HeroCarouselComponent } from '../../shared/components/hero-carousel/hero-carousel.component';
import { PostCardComponent } from '../../shared/components/post-card/post-card.component';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { PlantBannerComponent } from '../../shared/components/plant-banner/plant-banner.component';
import { ArticleService } from '../../core/services/article.service';
import { HeroSlideService } from '../../core/services/hero-slide.service';
import { LanguageService } from '../../core/services/language.service';
import { HeroCarouselModel } from '../../shared/components/hero-carousel/hero-carousel.model';
import { ArticleModel } from '../../shared/models/article.model';
import { SeoService } from '../../core/services/seo.service';

@Component({
  selector: 'app-travel',
  imports: [HeroCarouselComponent, PostCardComponent, PaginationComponent, PlantBannerComponent],
  templateUrl: './travel.component.html',
  styleUrl: './travel.component.css',
})
export class TravelComponent implements OnInit {
  @ViewChild('articleList') articleList!: ElementRef;

  articles: ArticleModel[] = [];
  slides: HeroCarouselModel[] = [];
  currentPage = 1;
  pageCount = 0;

  private destroyRef = inject(DestroyRef);

  private seoService = inject(SeoService);

  constructor(
    private articleService: ArticleService,
    private heroSlideService: HeroSlideService,
    private langService: LanguageService,
  ) {}

  ngOnInit(): void {
    this.seoService.set({
      title: 'Travel',
      description: 'Travel stories, destinations, and photography from around the world.',
      path: '/travel',
    });

    this.heroSlideService.getSlides().subscribe({ next: s => this.slides = s, error: () => {} });
    this.load(1);

    this.langService.currentLanguage$.pipe(
      skip(1),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(() => {
      this.heroSlideService.getSlides().subscribe({ next: s => this.slides = s, error: () => {} });
      this.load(1);
    });
  }

  load(page: number): void {
    this.currentPage = page;
    this.articleService.getArticles(page, 6, 'travel').subscribe(res => {
      this.articles = res.data;
      this.currentPage = res.pageNumber;
      this.pageCount = res.pageCount;
      this.articleList?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
    });
  }
}
