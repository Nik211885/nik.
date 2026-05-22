import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PostCardComponent } from '../../shared/components/post-card/post-card.component';
import { ApplicationTitle } from '../../app.message';
import { LanguagePipe } from '../../shared/pipes/language.pipe';
import { HeroSectionComponent } from '../../shared/components/hero-section/hero-section.component';
import { ArticleService } from '../../core/services/article.service';
import { LanguageService } from '../../core/services/language.service';
import { ArticleModel } from '../../shared/models/article.model';
import { SeoService } from '../../core/services/seo.service';
import { ConfigService } from '../../core/services/config.service';
import { take } from 'rxjs';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-home',
  imports: [PostCardComponent, LanguagePipe, HeroSectionComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  articles: ArticleModel[] = [];

  private destroyRef = inject(DestroyRef);
  private seoService = inject(SeoService);
  private configService = inject(ConfigService);

  constructor(
    private articleService: ArticleService,
    private langService: LanguageService,
  ) {}

  ngOnInit(): void {
    this.seoService.set({
      description: 'Personal blog and portfolio — stories, travel, fashion, and creativity.',
      path: '/',
      structuredData: {
        '@context': 'https://schema.org',
        '@type': 'WebSite',
        name: 'Nik.',
        url: environment.siteUrl,
        potentialAction: {
          '@type': 'SearchAction',
          target: `${environment.siteUrl}/travel?q={search_term_string}`,
          'query-input': 'required name=search_term_string',
        },
      },
    });

    this.configService.config.pipe(take(1)).subscribe(config => {
      if (!config?.info) return;
      const info = config.info;
      this.seoService.set({
        description: info.introduction || info.bio || 'Personal blog and portfolio.',
        image: info.avatar || undefined,
        path: '/',
        structuredData: [
          {
            '@context': 'https://schema.org',
            '@type': 'WebSite',
            name: 'Nik.',
            url: environment.siteUrl,
            potentialAction: {
              '@type': 'SearchAction',
              target: `${environment.siteUrl}/travel?q={search_term_string}`,
              'query-input': 'required name=search_term_string',
            },
          },
          {
            '@context': 'https://schema.org',
            '@type': 'Person',
            name: info.name,
            url: environment.siteUrl,
            email: info.email,
            image: info.avatar,
            description: info.bio,
          },
        ],
      });
    });

    this.langService.withLanguage(() => this.articleService.getTopArticles())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(articles => this.articles = articles);
  }

  protected readonly ApplicationTitle = ApplicationTitle;
}
