import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LanguageAdminService } from '../../services/language.admin.service';
import { AdminTableComponent } from '../../shared/admin-table.component';
import { AdminConfirmModalComponent } from '../../shared/admin-confirm-modal.component';
import { LanguageItem, TableColumn } from '../../models/admin.model';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';

@Component({
  selector: 'app-languages-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminTableComponent, AdminConfirmModalComponent, LanguagePipe],
  templateUrl: './languages.component.html',
  styleUrl: './languages.component.css'
})
export class LanguagesAdminComponent implements OnInit {
  items: LanguageItem[] = [];
  loading = false;
  showModal = false;
  showConfirm = false;
  isEditing = false;
  selected: LanguageItem | null = null;
  deleteItems: LanguageItem[] = [];
  saving = false;
  error = '';

  form = { code: '', name: '' };

  protected readonly AdminMessage = AdminMessage;

  readonly columns: TableColumn[] = [
    { key: 'code', label: AdminMessage.LABEL_LANG_CODE, type: 'text' },
    { key: 'name', label: AdminMessage.LABEL_NAME,      type: 'text' },
  ];

  constructor(private svc: LanguageAdminService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.svc.getLanguages().subscribe({ next: d => { this.items = d; this.loading = false; }, error: () => { this.loading = false; } });
  }

  openCreate(): void {
    this.isEditing = false;
    this.form = { code: '', name: '' };
    this.error = '';
    this.showModal = true;
  }

  openEdit(item: LanguageItem): void {
    this.isEditing = true;
    this.selected = item;
    this.form = { code: item.code, name: item.name };
    this.error = '';
    this.showModal = true;
  }

  openDelete(items: LanguageItem[]): void { this.deleteItems = items; this.selected = items[0] ?? null; this.showConfirm = true; }

  save(): void {
    this.saving = true;
    const obs = this.isEditing
      ? this.svc.updateLanguage({ id: this.selected!.id, ...this.form })
      : this.svc.createLanguage(this.form);

    obs.subscribe({
      next: () => { this.showModal = false; this.saving = false; this.load(); },
      error: () => { this.error = AdminMessage.ERROR_GENERIC; this.saving = false; }
    });
  }

  delete(): void {
    this.svc.deleteLanguage(this.deleteItems.map(i => i.id)).subscribe({
      next: () => { this.showConfirm = false; this.load(); },
      error: () => { this.showConfirm = false; }
    });
  }
}
