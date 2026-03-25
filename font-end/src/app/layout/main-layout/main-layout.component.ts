import { Component } from '@angular/core';
import {FooterComponent} from './footer/footer.component';
import {RouterOutlet} from '@angular/router';
import {SidebarComponent} from './sidebar/sidebar.component';

@Component({
  selector: 'app-main-layout',
  imports: [
    FooterComponent,
    RouterOutlet,
    SidebarComponent
  ],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.css',
})
export class MainLayoutComponent {}
