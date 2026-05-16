import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LanguageAdminService } from '../../services/language.admin.service';
import { AdminTableComponent } from '../../shared/admin-table.component';
import { AdminConfirmModalComponent } from '../../shared/admin-confirm-modal.component';
import { CodeLanguageItem, LanguageItem, TableColumn, TranslateItem } from '../../models/admin.model';
import { forkJoin } from 'rxjs';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-translations-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminTableComponent, AdminConfirmModalComponent, LanguagePipe],
  templateUrl: './translations.component.html',
  styleUrl: './translations.component.css'
})
export class TranslationsAdminComponent implements OnInit {
  items: TranslateItem[] = [];
  languages: LanguageItem[] = [];
  codeKeys: CodeLanguageItem[] = [];
  loading = false;
  showModal = false;
  showConfirm = false;
  showKeyModal = false;
  isEditing = false;
  selected: TranslateItem | null = null;
  deleteItems: TranslateItem[] = [];
  saving = false;
  error = '';

  form = { codeId: '', languageId: '', value: '' };
  keyForm = { code: '' };

  filterLang = '';

  protected readonly AdminMessage = AdminMessage;

  readonly columns: TableColumn[] = [
    { key: 'code',     label: AdminMessage.LABEL_KEY,      type: 'text' },
    { key: 'language', label: AdminMessage.LABEL_LANGUAGE, type: 'text' },
    { key: 'value',    label: AdminMessage.LABEL_VALUE,    type: 'truncate' },
  ];

  constructor(private svc: LanguageAdminService, private toast: ToastService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    forkJoin({
      translations: this.svc.getTranslations(),
      languages:    this.svc.getLanguages(),
      keys:         this.svc.getCodeKeys()
    }).subscribe({
      next: ({ translations, languages, keys }) => {
        this.languages = languages;
        this.codeKeys = keys;
        this.items = translations.map(t => ({
          ...t,
          code:     keys.find(k => k.id === t.codeId)?.code ?? t.codeId,
          language: languages.find(l => l.id === t.languageId)?.code ?? t.languageId
        }));
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  get filteredItems(): TranslateItem[] {
    if (!this.filterLang) return this.items;
    return this.items.filter(t => t.languageId === this.filterLang);
  }

  openCreate(): void {
    this.isEditing = false;
    this.form = { codeId: '', languageId: '', value: '' };
    this.error = '';
    this.showModal = true;
  }

  openEdit(item: TranslateItem): void {
    this.isEditing = true;
    this.selected = item;
    this.form = { codeId: item.codeId, languageId: item.languageId, value: item.value };
    this.error = '';
    this.showModal = true;
  }

  openDelete(items: TranslateItem[]): void { this.deleteItems = items; this.selected = items[0] ?? null; this.showConfirm = true; }

  openKeyModal(): void { this.keyForm = { code: '' }; this.showKeyModal = true; }

  save(): void {
    this.saving = true;
    const obs = this.isEditing
      ? this.svc.updateTranslation({ id: this.selected!.id, value: this.form.value })
      : this.svc.createTranslation(this.form);

    obs.subscribe({
      next: () => { this.showModal = false; this.saving = false; this.load(); this.toast.success(AdminMessage.TOAST_SAVE_SUCCESS); },
      error: () => { this.error = AdminMessage.ERROR_GENERIC; this.saving = false; this.toast.error(AdminMessage.TOAST_SAVE_ERROR); }
    });
  }

  saveKey(): void {
    this.saving = true;
    this.svc.createCodeKey(this.keyForm).subscribe({
      next: () => { this.showKeyModal = false; this.saving = false; this.load(); this.toast.success(AdminMessage.TOAST_SAVE_SUCCESS); },
      error: () => { this.saving = false; this.toast.error(AdminMessage.TOAST_SAVE_ERROR); }
    });
  }

  delete(): void {
    this.svc.deleteTranslation(this.deleteItems.map(i => i.id)).subscribe({
      next: () => { this.showConfirm = false; this.load(); this.toast.success(AdminMessage.TOAST_DELETE_SUCCESS); },
      error: () => { this.showConfirm = false; this.toast.error(AdminMessage.TOAST_DELETE_ERROR); }
    });
  }
}
