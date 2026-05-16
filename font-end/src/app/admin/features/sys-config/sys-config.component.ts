import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { SysConfigAdminService } from '../../services/sys-config.admin.service';
import { AdminTableComponent } from '../../shared/admin-table.component';
import { AdminConfirmModalComponent } from '../../shared/admin-confirm-modal.component';
import { SysConfigItem, TableColumn } from '../../models/admin.model';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-sys-config-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminTableComponent, AdminConfirmModalComponent, LanguagePipe],
  templateUrl: './sys-config.component.html',
  styleUrl: './sys-config.component.css'
})
export class SysConfigAdminComponent implements OnInit {
  items: SysConfigItem[] = [];
  loading = false;
  showModal = false;
  showConfirm = false;
  selected: SysConfigItem | null = null;
  deleteItems: SysConfigItem[] = [];
  saving = false;
  error = '';

  form = { key: '', value: '' };
  jsonError = '';

  protected readonly AdminMessage = AdminMessage;

  readonly columns: TableColumn[] = [
    { key: 'key',   label: AdminMessage.LABEL_KEY,   type: 'text' },
    { key: 'value', label: AdminMessage.LABEL_VALUE, type: 'truncate' },
  ];

  constructor(private svc: SysConfigAdminService, private router: Router, private toast: ToastService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.svc.getAll().subscribe({ next: d => { this.items = d; this.loading = false; }, error: () => { this.loading = false; } });
  }

  openCreate(): void {
    this.form = { key: '', value: '' };
    this.error = '';
    this.jsonError = '';
    this.showModal = true;
  }

  openEdit(item: SysConfigItem): void {
    this.router.navigate(['/admin/sys-config/editor', item.id]);
  }

  get isJsonLike(): boolean {
    const v = this.form.value.trim();
    return v.startsWith('{') || v.startsWith('[');
  }

  get jsonValid(): boolean {
    if (!this.isJsonLike) return false;
    try { JSON.parse(this.form.value); return true; } catch { return false; }
  }

  formatJson(): void {
    try { this.form.value = JSON.stringify(JSON.parse(this.form.value), null, 2); this.jsonError = ''; }
    catch (e: any) { this.jsonError = e.message; }
  }

  get jsonRows(): { key: string; value: string }[] {
    if (!this.jsonValid) return [];
    try {
      const parsed = JSON.parse(this.form.value);
      if (typeof parsed !== 'object' || Array.isArray(parsed)) return [];
      return Object.entries(parsed).map(([k, v]) => ({
        key: k,
        value: typeof v === 'object' ? JSON.stringify(v) : String(v ?? '')
      }));
    } catch { return []; }
  }

  openDelete(items: SysConfigItem[]): void { this.deleteItems = items; this.selected = items[0] ?? null; this.showConfirm = true; }

  save(): void {
    this.saving = true;
    let parsedValue: any;
    try { parsedValue = JSON.parse(this.form.value); } catch { parsedValue = this.form.value; }

    this.svc.create({ key: this.form.key, value: parsedValue }).subscribe({
      next: () => { this.showModal = false; this.saving = false; this.load(); this.toast.success(AdminMessage.TOAST_SAVE_SUCCESS); },
      error: () => { this.error = AdminMessage.ERROR_GENERIC; this.saving = false; this.toast.error(AdminMessage.TOAST_SAVE_ERROR); }
    });
  }

  delete(): void {
    this.svc.delete(this.deleteItems.map(i => i.id)).subscribe({
      next: () => { this.showConfirm = false; this.load(); this.toast.success(AdminMessage.TOAST_DELETE_SUCCESS); },
      error: () => { this.showConfirm = false; this.toast.error(AdminMessage.TOAST_DELETE_ERROR); }
    });
  }
}
