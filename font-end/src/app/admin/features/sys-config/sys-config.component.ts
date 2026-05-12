import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SysConfigAdminService } from '../../services/sys-config.admin.service';
import { AdminTableComponent } from '../../shared/admin-table.component';
import { AdminConfirmModalComponent } from '../../shared/admin-confirm-modal.component';
import { SysConfigItem, TableColumn } from '../../models/admin.model';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';

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
  isEditing = false;
  selected: SysConfigItem | null = null;
  saving = false;
  error = '';

  form = { key: '', value: '' };

  protected readonly AdminMessage = AdminMessage;

  readonly columns: TableColumn[] = [
    { key: 'key',   label: AdminMessage.LABEL_KEY,   type: 'text' },
    { key: 'value', label: AdminMessage.LABEL_VALUE, type: 'truncate' },
  ];

  constructor(private svc: SysConfigAdminService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.svc.getAll().subscribe({ next: d => { this.items = d; this.loading = false; }, error: () => { this.loading = false; } });
  }

  openCreate(): void {
    this.isEditing = false;
    this.form = { key: '', value: '' };
    this.error = '';
    this.showModal = true;
  }

  openEdit(item: SysConfigItem): void {
    this.isEditing = true;
    this.selected = item;
    this.form = { key: item.key, value: typeof item.value === 'object' ? JSON.stringify(item.value) : String(item.value) };
    this.error = '';
    this.showModal = true;
  }

  openDelete(item: SysConfigItem): void { this.selected = item; this.showConfirm = true; }

  save(): void {
    this.saving = true;
    let parsedValue: any;
    try { parsedValue = JSON.parse(this.form.value); } catch { parsedValue = this.form.value; }

    const obs = this.isEditing
      ? this.svc.update({ id: this.selected!.id, key: this.form.key, value: parsedValue })
      : this.svc.create({ key: this.form.key, value: parsedValue });

    obs.subscribe({
      next: () => { this.showModal = false; this.saving = false; this.load(); },
      error: () => { this.error = AdminMessage.ERROR_GENERIC; this.saving = false; }
    });
  }

  delete(): void {
    this.svc.delete([this.selected!.id]).subscribe({
      next: () => { this.showConfirm = false; this.load(); },
      error: () => { this.showConfirm = false; }
    });
  }
}
