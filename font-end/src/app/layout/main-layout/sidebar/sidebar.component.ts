import { Component } from '@angular/core';
import {AuthService} from '../../../core/auth/auth.service';
import {ConfigService} from '../../../core/services/config.service';
import {AsyncPipe} from '@angular/common';
import {RouterLink, RouterLinkActive} from '@angular/router';
import {DomSanitizer, SafeHtml} from '@angular/platform-browser';

@Component({
  selector: 'app-sidebar',
  imports: [
    AsyncPipe,
    RouterLink,
    RouterLinkActive
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css',
})
export class SidebarComponent {
  yearNow = new Date().getFullYear();
  constructor(protected readonly configService: ConfigService,
              private sanitizer: DomSanitizer) {}

  sanitizeIcon(svg: string) : SafeHtml{
    return this.sanitizer.bypassSecurityTrustHtml(svg);
  }
  onNavClick(): void {
    document.body.classList.remove('offcanvas');
    document.querySelectorAll('.js-colorlib-nav-toggle').forEach(btn =>
      btn.classList.remove('active')
    );
  }
}
