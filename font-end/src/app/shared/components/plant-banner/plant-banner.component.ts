import { Component } from '@angular/core';
import {AsyncPipe} from "@angular/common";
import {LanguagePipe} from "../../pipes/language.pipe";
import {NewsLetterComponent} from "../news-letter/news-letter.component";
import {PostCardCompactComponent} from "../post-card-compact/post-card-compact.component";
import {SearchInputComponent} from "../search-input/search-input.component";
import {SidebarListComponent} from "../sidebar-list/sidebar-list.component";
import {TagCloudComponent} from "../tag-cloud/tag-cloud.component";
import {statisticsArchives, statisticsCategories} from "../../../app-data.fake";
import {ApplicationTitle} from "../../../app.message";
import {ConfigService} from '../../../core/services/config.service';

@Component({
  selector: 'app-plant-banner',
  imports: [
    AsyncPipe,
    LanguagePipe,
    NewsLetterComponent,
    PostCardCompactComponent,
    SearchInputComponent,
    SidebarListComponent,
    TagCloudComponent
  ],
  templateUrl: './plant-banner.component.html',
  styleUrl: './plant-banner.component.css',
})
export class PlantBannerComponent {
  protected readonly statisticsArchives = statisticsArchives;
  protected readonly ApplicationTitle = ApplicationTitle;
  protected readonly statisticsCategories = statisticsCategories;
  constructor(protected readonly configService: ConfigService) {
  }
}
