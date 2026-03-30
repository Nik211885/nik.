import { Component } from '@angular/core';
import {HeroSectionComponent} from '../../shared/components/hero-section/hero-section.component';
import {PostCardComponent} from '../../shared/components/post-card/post-card.component';
import {SearchInputComponent} from '../../shared/components/search-input/search-input.component';
import {ApplicationTitle} from '../../app.message';
import {SidebarListComponent} from '../../shared/components/sidebar-list/sidebar-list.component';
import {listTagCloud, statisticsArchives, statisticsCategories} from '../../app-data.fake';
import {LanguagePipe} from '../../shared/pipes/language.pipe';
import {PostCardCompactComponent} from '../../shared/components/post-card-compact/post-card-compact.component';
import {TagCloudComponent} from '../../shared/components/tag-cloud/tag-cloud.component';
import {NewsLetterComponent} from '../../shared/components/news-letter/news-letter.component';
import {ConfigService} from '../../core/services/config.service';
import {AsyncPipe} from '@angular/common';

@Component({
  selector: 'app-fashion',
  imports: [
    HeroSectionComponent,
    PostCardComponent,
    PostCardComponent,
    SearchInputComponent,
    SidebarListComponent,
    LanguagePipe,
    PostCardCompactComponent,
    TagCloudComponent,
    NewsLetterComponent,
    AsyncPipe
  ],
  templateUrl: './fashion.component.html',
  styleUrl: './fashion.component.css',
})
export class FashionComponent {
  constructor(protected readonly configService: ConfigService) {
  }
  protected readonly ApplicationTitle = ApplicationTitle;
  protected readonly statisticsCategories = statisticsCategories;
  protected readonly listTagCloud = listTagCloud;
  protected readonly statisticsArchives = statisticsArchives;
}
