import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { TagCloudComponent } from '../../shared/components/tag-cloud/tag-cloud.component';
import { ApplicationTitle } from '../../app.message';
import { PlantBannerComponent } from '../../shared/components/plant-banner/plant-banner.component';
import { PostCommentListComponent } from '../../shared/components/post-comment-list/post-comment-list.component';
import { InputCommentComponent } from '../../shared/components/input-comment/input-comment.component';
import { ArticleService } from '../../core/services/article.service';
import { CommentService } from '../../core/services/comment.service';
import { LanguageService } from '../../core/services/language.service';
import { AutoTranslateService } from '../../core/services/auto-translate.service';
import { CONTENT_LANG, DEFAULT_LANG } from '../../core/services/language.service';
import { ArticleModel } from '../../shared/models/article.model';
import { PostCommentModel } from '../../shared/models/post-comment.model';
import { SeoService } from '../../core/services/seo.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-post-detail',
  imports: [TagCloudComponent, PlantBannerComponent, PostCommentListComponent, InputCommentComponent],
  templateUrl: './post-detail.component.html',
  styleUrl: './post-detail.component.css',
})
export class PostDetailComponent implements OnInit {
  article: ArticleModel | null = null;
  comments: PostCommentModel[] = [];
  replyTo: PostCommentModel | null = null;
  translatedBio: string | null = null;

  private destroyRef = inject(DestroyRef);
  private currentLang = DEFAULT_LANG;

  private seoService = inject(SeoService);

  constructor(
    private route: ActivatedRoute,
    private articleService: ArticleService,
    private commentService: CommentService,
    private langService: LanguageService,
    private autoTranslate: AutoTranslateService,
  ) {}

  ngOnInit(): void {
    const prefix = this.route.snapshot.paramMap.get('prefix') ?? '';
    const name = this.route.snapshot.paramMap.get('slug') ?? '';
    const slug = `${prefix}/${name}`;

    this.langService.currentLanguage$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(lang => { this.currentLang = lang; });

    let firstLoad = true;
    this.langService.withLanguage(() => this.articleService.getArticleBySlug(slug))
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(res => {
        this.article = res;
        this.translateBio(res.author.bio);
        if (firstLoad) { firstLoad = false; this.loadComments(); }
        this.seoService.set({
          title: res.title,
          description: res.description || res.title,
          image: res.image || undefined,
          path: `/post/${res.slug}`,
          type: 'article',
          structuredData: {
            '@context': 'https://schema.org',
            '@type': 'BlogPosting',
            headline: res.title,
            ...(res.description ? { description: res.description } : {}),
            ...(res.image ? { image: res.image } : {}),
            datePublished: res.createdDate,
            dateModified: res.updatedDate || res.createdDate,
            url: `${environment.siteUrl}/post/${res.slug}`,
            author: {
              '@type': 'Person',
              name: res.author.userName,
              url: environment.siteUrl,
            },
            ...(res.tags?.length ? { keywords: res.tags.map(t => t.name).join(', ') } : {}),
          },
        });
      });
  }

  private translateBio(bio: string | null | undefined): void {
    if (!bio) { this.translatedBio = null; return; }
    this.autoTranslate.translate(bio, this.currentLang, CONTENT_LANG).subscribe(t => {
      this.translatedBio = t !== bio ? t : null;
    });
  }

  loadComments(): void {
    if (!this.article) return;
    this.commentService.getComments(this.article.id).subscribe(tree => {
      this.comments = tree;
    });
  }

  protected readonly ApplicationTitle = ApplicationTitle;
}
