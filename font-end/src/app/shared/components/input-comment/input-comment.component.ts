import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {ApplicationTitle} from '../../../app.message';
import {LanguagePipe} from '../../pipes/language.pipe';
import {PostCommentModel} from '../../models/post-comment.model';
import {AuthService} from '../../../core/auth/auth.service';
import {CommentService} from '../../../core/services/comment.service';
import {UserProfile} from '../../../core/auth/auth.model';

@Component({
  selector: 'app-input-comment',
  imports: [LanguagePipe, FormsModule],
  templateUrl: './input-comment.component.html',
  styleUrl: './input-comment.component.css',
})
export class InputCommentComponent implements OnInit {
  @Input() articleId!: string;
  @Input() replyTo: PostCommentModel | null = null;
  @Output() commentPosted = new EventEmitter<void>();
  @Output() cancelReply = new EventEmitter<void>();

  currentUser: UserProfile | null = null;
  text = '';
  guestName = '';
  guestEmail = '';
  guestWebsite = '';
  submitting = false;

  constructor(
    private authService: AuthService,
    private commentService: CommentService,
  ) {}

  ngOnInit(): void {
    this.authService.currentUserStream.subscribe(u => this.currentUser = u);
  }

  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  submit(): void {
    if (!this.text.trim() || this.submitting) return;
    if (!this.isLoggedIn && (!this.guestName.trim() || !this.guestEmail.trim())) return;

    this.submitting = true;
    this.commentService.createComment({
      articleId: this.articleId,
      text: this.text.trim(),
      parentId: this.replyTo?.id,
      guestName: this.isLoggedIn ? undefined : this.guestName.trim(),
      guestEmail: this.isLoggedIn ? undefined : this.guestEmail.trim(),
      guestWebsite: this.isLoggedIn ? undefined : (this.guestWebsite.trim() || undefined),
    }).subscribe({
      next: () => {
        this.text = '';
        this.guestName = '';
        this.guestEmail = '';
        this.guestWebsite = '';
        this.submitting = false;
        this.commentPosted.emit();
        if (this.replyTo) this.cancelReply.emit();
      },
      error: () => { this.submitting = false; }
    });
  }

  protected readonly ApplicationTitle = ApplicationTitle;
}
