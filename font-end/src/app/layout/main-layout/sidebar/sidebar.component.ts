import { Component, OnInit, OnDestroy } from '@angular/core';
import { AsyncPipe, UpperCasePipe } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Subscription } from 'rxjs';
import { ConfigService } from '../../../core/services/config.service';
import { LanguageService, AvailableLanguage } from '../../../core/services/language.service';

@Component({
  selector: 'app-sidebar',
  imports: [AsyncPipe, RouterLink, RouterLinkActive, UpperCasePipe],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css',
})
export class SidebarComponent implements OnInit, OnDestroy {
  yearNow = new Date().getFullYear();

  availableLanguages: AvailableLanguage[] = [];
  currentLang = '';
  langMenuOpen = false;

  private subs = new Subscription();

  constructor(
    protected readonly configService: ConfigService,
    private sanitizer: DomSanitizer,
    private langService: LanguageService
  ) {}

  ngOnInit(): void {
    this.subs.add(this.langService.availableLanguages$.subscribe(l => this.availableLanguages = l));
    this.subs.add(this.langService.currentLanguage$.subscribe(l => this.currentLang = l));
  }

  ngOnDestroy(): void { this.subs.unsubscribe(); }

  get currentLangName(): string {
    return this.availableLanguages.find(l => l.code === this.currentLang)?.name ?? this.currentLang.toUpperCase();
  }

  changeLang(code: string): void {
    this.langMenuOpen = false;
    this.langService.changeLanguage(code).subscribe();
  }

  sanitizeIcon(svg: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(svg);
  }

  onNavClick(): void {
    document.body.classList.remove('offcanvas');
    document.querySelectorAll('.js-colorlib-nav-toggle').forEach(btn =>
      btn.classList.remove('active')
    );
  }
}
