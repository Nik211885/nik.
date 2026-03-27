import { Component } from '@angular/core';
import {PostCardComponent} from '../../shared/components/post-card/post-card.component';
import {PostCardCompactComponent} from '../../shared/components/post-card-compact/post-card-compact.component';
import {NewsLetterComponent} from '../../shared/components/news-letter/news-letter.component';
import {SidebarStatItemModel} from '../../shared/components/sidebar-list/sidebar-stat-item.model';
import {SidebarListComponent} from '../../shared/components/sidebar-list/sidebar-list.component';
import {ApplicationTitle} from '../../app.message';
import {PaginationComponent} from '../../shared/components/pagination/pagination.component';
import {ConfigService} from '../../core/services/config.service';
import {AsyncPipe} from '@angular/common';
import {LanguagePipe} from '../../shared/pipes/language.pipe';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [
    PostCardComponent,
    PostCardCompactComponent,
    NewsLetterComponent,
    SidebarListComponent,
    PaginationComponent,
    AsyncPipe,
    LanguagePipe,
    RouterLink
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  sidebarStatItemModel: SidebarStatItemModel[] = [
    {
      id: '1',
      name: 'Travel',
      count: 10,
      slug: 'travel',
    },
    {
      id: '2',
      name: 'Photography',
      count: 4,
      slug: 'photography',
    },
    {
      id: '3',
      name: 'Technology',
      count: 5,
      slug: 'technology',
    }
  ]
  constructor(protected readonly configService: ConfigService) {
  }
  protected readonly ApplicationTitle = ApplicationTitle;
}
