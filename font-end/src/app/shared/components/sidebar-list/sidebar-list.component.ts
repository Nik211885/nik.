import {Component, Input} from '@angular/core';
import {SidebarStatItemModel} from './sidebar-stat-item.model';
import {LanguagePipe} from '../../pipes/language.pipe';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-sidebar-list',
  imports: [
    LanguagePipe,
    RouterLink
  ],
  templateUrl: './sidebar-list.component.html',
  styleUrl: './sidebar-list.component.css',
})
export class SidebarListComponent {
  @Input() title: string = 'Sidebar List';
  @Input() statistics: SidebarStatItemModel[] = [];
}
