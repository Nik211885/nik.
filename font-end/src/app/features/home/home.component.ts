import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PostCardComponent } from '../../shared/components/post-card/post-card.component';
import { ApplicationTitle } from '../../app.message';
import { LanguagePipe } from '../../shared/pipes/language.pipe';
import { HeroSectionComponent } from '../../shared/components/hero-section/hero-section.component';
import { ArticleService } from '../../core/services/article.service';
import { LanguageService } from '../../core/services/language.service';
import { ArticleModel } from '../../shared/models/article.model';

@Component({
  selector: 'app-home',
  imports: [PostCardComponent, LanguagePipe, HeroSectionComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  articles: ArticleModel[] = [];

  private destroyRef = inject(DestroyRef);

  constructor(
    private articleService: ArticleService,
    private langService: LanguageService,
  ) {}

  ngOnInit(): void {
    this.langService.withLanguage(() => this.articleService.getTopArticles())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(articles => this.articles = articles);
  }

  protected readonly ApplicationTitle = ApplicationTitle;
}
