import { Component } from '@angular/core';
import {PostCardComponent} from '../../shared/components/post-card/post-card.component';
import {PostCardCompactComponent} from '../../shared/components/post-card-compact/post-card-compact.component';
import {NewsLetterComponent} from '../../shared/components/news-letter/news-letter.component';
import {SidebarStatItemModel} from '../../shared/components/sidebar-list/sidebar-stat-item.model';
import {SidebarListComponent} from '../../shared/components/sidebar-list/sidebar-list.component';
import {ApplicationTitle} from '../../app.message';

@Component({
  selector: 'app-home',
  imports: [
    PostCardComponent,
    PostCardCompactComponent,
    NewsLetterComponent,
    SidebarListComponent
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
  protected readonly ApplicationTitle = ApplicationTitle;
}
