import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ContentTranslationAdminService } from '../../services/content-translation.admin.service';
import { LanguageAdminService } from '../../services/language.admin.service';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { LanguageItem, TranslationStatusItem } from '../../models/admin.model';
import { AdminMessage } from '../../../app.message';
import { CONTENT_LANG, DEFAULT_LANG } from '../../../core/services/language.service';

interface EntityTypeOption { value: string; label: string; }
export interface FieldDef { key: string; label: string; multiline: boolean; rich?: boolean; }

export const ENTITY_FIELDS: Record<string, FieldDef[]> = {
  article:   [
    { key: 'title',       label: 'Title',       multiline: false },
    { key: 'description', label: 'Description', multiline: true  },
    { key: 'content',     label: 'Content',     multiline: true,  rich: true },
  ],
  category:  [{ key: 'title', label: 'Title', multiline: false }],
  tag:       [
    { key: 'title',       label: 'Title',       multiline: false },
    { key: 'description', label: 'Description', multiline: true  },
  ],
  album:     [
    { key: 'title',       label: 'Title',       multiline: false },
    { key: 'description', label: 'Description', multiline: true  },
  ],
  heroSlide: [
    { key: 'title',       label: 'Title',       multiline: false },
    { key: 'description', label: 'Description', multiline: true  },
  ],
};

@Component({
  selector: 'app-content-translations-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, PaginationComponent, LanguagePipe],
  templateUrl: './content-translations.component.html',
  styleUrl: './content-translations.component.css',
})
export class ContentTranslationsAdminComponent implements OnInit {
  readonly AdminMessage = AdminMessage;

  readonly entityTypeOptions: EntityTypeOption[] = [
    { value: 'article',   label: 'Article' },
    { value: 'category',  label: 'Category' },
    { value: 'tag',       label: 'Tag' },
    { value: 'album',     label: 'Album' },
    { value: 'heroSlide', label: 'Hero Slide' },
  ];

  languages: LanguageItem[] = [];
  selectedEntityType = 'article';
  selectedLang = DEFAULT_LANG;
  selectedTranslated: boolean | null = null;

  items: TranslationStatusItem[] = [];
  loading = false;
  loadError = false;
  currentPage = 1;
  pageCount = 0;
  pageSize = 20;

  constructor(
    private svc: ContentTranslationAdminService,
    private langSvc: LanguageAdminService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.langSvc.getLanguages().subscribe({
      next: langs => {
        this.languages = langs.filter(l => l.code !== CONTENT_LANG);
        if (this.languages.length > 0) this.selectedLang = this.languages[0].code;
      },
      error: () => {}
    });
    this.load(1);
  }

  load(page: number): void {
    this.loading = true;
    this.loadError = false;
    this.svc.getStatusList(this.selectedEntityType, this.selectedLang, this.selectedTranslated, page, this.pageSize)
      .subscribe({
        next: res => {
          this.items = res.data;
          this.currentPage = res.pageNumber;
          this.pageCount = res.pageCount;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.loadError = true;
        },
      });
  }

  applyFilters(): void { this.load(1); }

  openEditor(item: TranslationStatusItem): void {
    this.router.navigate(
      ['/admin/content-translations/editor', item.entityType, item.entityId, this.selectedLang],
      { queryParams: { title: item.sourceTitle } }
    );
  }

  get currentLang(): string { return this.selectedLang; }
}
