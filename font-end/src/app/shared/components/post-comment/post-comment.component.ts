import {Component, Input} from '@angular/core';
import {ApplicationTitle} from '../../../app.message';
import {LanguagePipe} from '../../pipes/language.pipe';
import {PostCommentModel} from '../../models/post-comment.model';
import {AppDatePipe} from '../../pipes/app-date.pipe';

@Component({
  selector: 'li[app-post-comment]',
  imports: [
    LanguagePipe,
    AppDatePipe
  ],
  templateUrl: './post-comment.component.html',
  styleUrl: './post-comment.component.css',
})
export class PostCommentComponent {
  @Input() comment!: PostCommentModel;
  protected readonly ApplicationTitle = ApplicationTitle;
}
