import {Component, Input} from '@angular/core';
import {PostCommentModel} from '../../models/post-comment.model';
import {ApplicationTitle} from '../../../app.message';
import {LanguagePipe} from '../../pipes/language.pipe';
import {PostCommentComponent} from '../post-comment/post-comment.component';

@Component({
  selector: 'app-post-comment-list',
  standalone: true,
  imports: [
    LanguagePipe,
    PostCommentComponent
  ],
  templateUrl: './post-comment-list.component.html',
  styleUrl: './post-comment-list.component.css',
})
export class PostCommentListComponent {
  @Input() commentCount: number = 0;
  @Input() commentList: PostCommentModel[] = [];
  protected readonly ApplicationTitle = ApplicationTitle;
}
