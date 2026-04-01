import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {TagCloudComponent} from '../../shared/components/tag-cloud/tag-cloud.component';
import {postModel, statisticsArchives, statisticsCategories} from '../../app-data.fake';
import {SearchInputComponent} from '../../shared/components/search-input/search-input.component';
import {ApplicationTitle} from '../../app.message';
import {SidebarListComponent} from '../../shared/components/sidebar-list/sidebar-list.component';
import {LanguagePipe} from '../../shared/pipes/language.pipe';
import {PostCardCompactComponent} from '../../shared/components/post-card-compact/post-card-compact.component';
import {NewsLetterComponent} from '../../shared/components/news-letter/news-letter.component';
import {ConfigService} from '../../core/services/config.service';
import {AsyncPipe} from '@angular/common';
import {PlantBannerComponent} from '../../shared/components/plant-banner/plant-banner.component';

@Component({
  selector: 'app-post-detail',
  imports: [
    TagCloudComponent,
    SearchInputComponent,
    SidebarListComponent,
    LanguagePipe,
    PostCardCompactComponent,
    NewsLetterComponent,
    AsyncPipe,
    PlantBannerComponent,
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
}
