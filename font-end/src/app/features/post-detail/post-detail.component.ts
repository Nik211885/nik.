import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {TagCloudComponent} from '../../shared/components/tag-cloud/tag-cloud.component';
import {postComment, postModel, statisticsArchives, statisticsCategories} from '../../app-data.fake';
import {ApplicationTitle} from '../../app.message';
import {ConfigService} from '../../core/services/config.service';
import {PlantBannerComponent} from '../../shared/components/plant-banner/plant-banner.component';
import {PostCommentComponent} from '../../shared/components/post-comment/post-comment.component';
import {PostCommentListComponent} from '../../shared/components/post-comment-list/post-comment-list.component';
import {InputCommentComponent} from '../../shared/components/input-comment/input-comment.component';

@Component({
  selector: 'app-post-detail',
  imports: [
    TagCloudComponent,
    PlantBannerComponent,
    PostCommentListComponent,
    InputCommentComponent,
  ],
  templateUrl: './post-detail.component.html',
  styleUrl: './post-detail.component.css',
})
export class PostDetailComponent implements OnInit {
  private slug: string = "";

  constructor(private router: ActivatedRoute, protected  configService: ConfigService) {}

  ngOnInit(): void {
        this.slug = this.router.snapshot.paramMap.get("slug") ?? "";
    }

  protected readonly postModel = postModel;
  protected readonly ApplicationTitle = ApplicationTitle;
  protected readonly statisticsCategories = statisticsCategories;
  protected readonly statisticsArchives = statisticsArchives;
  protected readonly postComment = postComment;
}
