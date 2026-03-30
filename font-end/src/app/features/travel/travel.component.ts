import {AfterViewInit, Component, OnInit} from '@angular/core';
import {HeroCarouselComponent} from '../../shared/components/hero-carousel/hero-carousel.component';
import {PostCardComponent} from '../../shared/components/post-card/post-card.component';
import {PaginationComponent} from '../../shared/components/pagination/pagination.component';
import {SearchInputComponent} from '../../shared/components/search-input/search-input.component';
import {ApplicationTitle} from '../../app.message';
import {SidebarListComponent} from '../../shared/components/sidebar-list/sidebar-list.component';
import {listTagCloud, statisticsArchives, statisticsCategories} from '../../app-data.fake';
import {LanguagePipe} from '../../shared/pipes/language.pipe';
import {PostCardCompactComponent} from '../../shared/components/post-card-compact/post-card-compact.component';
import {TagCloudComponent} from '../../shared/components/tag-cloud/tag-cloud.component';
import {AsyncPipe} from '@angular/common';
import {NewsLetterComponent} from '../../shared/components/news-letter/news-letter.component';
import {ConfigService} from '../../core/services/config.service';

@Component({
  selector: 'app-travel',
  imports: [
    HeroCarouselComponent,
    PostCardComponent,
    PaginationComponent,
    SearchInputComponent,
    SidebarListComponent,
    LanguagePipe,
    PostCardCompactComponent,
    TagCloudComponent,
    AsyncPipe,
    NewsLetterComponent
  ],
  templateUrl: './travel.component.html',
  styleUrl: './travel.component.css',
})
export class TravelComponent {
  constructor(protected readonly configService: ConfigService) {
  }
  protected readonly ApplicationTitle = ApplicationTitle;
  protected readonly statisticsCategories = statisticsCategories;
  protected readonly listTagCloud = listTagCloud;
  protected readonly statisticsArchives = statisticsArchives;
}
