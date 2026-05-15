import {Component, EventEmitter, Input, Output} from '@angular/core';
import {ApplicationTitle} from '../../../app.message';
import {LanguagePipe} from '../../pipes/language.pipe';
import {PostCommentModel} from '../../models/post-comment.model';
import {AppDatePipe} from '../../pipes/app-date.pipe';

@Component({
  selector: 'li[app-post-comment]',
  imports: [LanguagePipe, AppDatePipe],
  templateUrl: './post-comment.component.html',
  styleUrl: './post-comment.component.css',
})
export class PostCommentComponent {
  @Input() comment!: PostCommentModel;
  @Output() replyClicked = new EventEmitter<PostCommentModel>();
  protected readonly ApplicationTitle = ApplicationTitle;
  protected readonly defaultAvatar = 'https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=80&h=80&fit=crop';
}
