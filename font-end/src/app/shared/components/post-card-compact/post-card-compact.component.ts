import {Component, Input} from '@angular/core';
import {RouterLink} from '@angular/router';
import {AppDatePipe} from '../../pipes/app-date.pipe';
import {ArticleModel} from '../../models/article.model';

@Component({
  selector: 'app-post-card-compact',
  imports: [
    RouterLink,
    AppDatePipe
  ],
  standalone: true,
  templateUrl: './post-card-compact.component.html',
  styleUrl: './post-card-compact.component.css',
})
export class PostCardCompactComponent {
  @Input() post!: ArticleModel;
}
