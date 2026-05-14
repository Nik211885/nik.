import {Component, Input} from '@angular/core';
import {ArticleModel} from '../../models/article.model';
import {RouterLink} from '@angular/router';
import {ApplicationTitle} from '../../../app.message';
import {LanguagePipe} from '../../pipes/language.pipe';
import {AppDatePipe} from '../../pipes/app-date.pipe';

@Component({
  selector: 'app-post-card',
  imports: [
    RouterLink,
    LanguagePipe,
    AppDatePipe
  ],
  standalone: true,
  templateUrl: './post-card.component.html',
  styleUrl: './post-card.component.css',
})
export class PostCardComponent {
  @Input() post!: ArticleModel;

  protected readonly ApplicationTitle = ApplicationTitle;
}
