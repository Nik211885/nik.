import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {TagCloudComponent} from '../../shared/components/tag-cloud/tag-cloud.component';
import {ApplicationTitle} from '../../app.message';
import {PlantBannerComponent} from '../../shared/components/plant-banner/plant-banner.component';
import {PostCommentListComponent} from '../../shared/components/post-comment-list/post-comment-list.component';
import {InputCommentComponent} from '../../shared/components/input-comment/input-comment.component';
import {ArticleService} from '../../core/services/article.service';
import {CommentService} from '../../core/services/comment.service';
import {ArticleModel} from '../../shared/models/article.model';
import {PostCommentModel} from '../../shared/models/post-comment.model';

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

  constructor(
    private route: ActivatedRoute,
    private articleService: ArticleService,
    private commentService: CommentService,
  ) {}

  ngOnInit(): void {
    const prefix = this.route.snapshot.paramMap.get('prefix') ?? '';
    const name = this.route.snapshot.paramMap.get('slug') ?? '';
    this.articleService.getArticleBySlug(`${prefix}/${name}`).subscribe(res => {
      this.article = res;
      this.loadComments();
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
