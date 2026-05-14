import {Component, OnInit} from '@angular/core';
import {PostCardComponent} from '../../shared/components/post-card/post-card.component';
import {ApplicationTitle} from '../../app.message';
import {LanguagePipe} from '../../shared/pipes/language.pipe';
import {HeroSectionComponent} from '../../shared/components/hero-section/hero-section.component';
import {ArticleService} from '../../core/services/article.service';
import {ArticleModel} from '../../shared/models/article.model';

@Component({
  selector: 'app-home',
  imports: [
    PostCardComponent,
    LanguagePipe,
    HeroSectionComponent,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  articles: ArticleModel[] = [];

  constructor(private articleService: ArticleService) {}

  ngOnInit(): void {
    this.articleService.getTopArticles().subscribe(articles => this.articles = articles);
  }

  protected readonly ApplicationTitle = ApplicationTitle;
}
