import { Component, DestroyRef, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ApplicationTitle } from '../../../app.message';
import { LanguagePipe } from '../../pipes/language.pipe';
import { PostCommentModel } from '../../models/post-comment.model';
import { AppDatePipe } from '../../pipes/app-date.pipe';
import { AutoTranslateService } from '../../../core/services/auto-translate.service';
import { LanguageService } from '../../../core/services/language.service';

@Component({
  selector: 'li[app-post-comment]',
  imports: [LanguagePipe, AppDatePipe],
  templateUrl: './post-comment.component.html',
  styleUrl: './post-comment.component.css',
})
export class PostCommentComponent implements OnInit {
  @Input() comment!: PostCommentModel;
  @Output() replyClicked = new EventEmitter<PostCommentModel>();

  protected readonly ApplicationTitle = ApplicationTitle;
  protected readonly defaultAvatar = 'https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=80&h=80&fit=crop';

  translatedText: string | null = null;
  isTranslated = false;

  private destroyRef = inject(DestroyRef);

  constructor(
    private autoTranslate: AutoTranslateService,
    private langSvc: LanguageService,
  ) {}

  ngOnInit(): void {
    this.langSvc.currentLanguage$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(lang => {
        this.autoTranslate.translate(this.comment.text, lang).subscribe(t => {
          this.translatedText = t !== this.comment.text ? t : null;
          this.isTranslated = this.translatedText !== null;
        });
      });
  }
}
