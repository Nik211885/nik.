import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AlbumAdminService } from '../../services/album.admin.service';
import { AdminTableComponent } from '../../shared/admin-table.component';
import { AdminConfirmModalComponent } from '../../shared/admin-confirm-modal.component';
import { AlbumItem, TableColumn } from '../../models/admin.model';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';

@Component({
  selector: 'app-albums-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminTableComponent, AdminConfirmModalComponent, LanguagePipe],
  templateUrl: './albums.component.html',
  styleUrl: './albums.component.css'
})
export class AlbumsAdminComponent implements OnInit {
  items: AlbumItem[] = [];
  loading = false;
  showModal = false;
  showConfirm = false;
  isEditing = false;
  selected: AlbumItem | null = null;
  saving = false;
  error = '';

  form = { name: '', title: '', description: '', albumId: '', orderIndex: 0 };

  protected readonly AdminMessage = AdminMessage;

  readonly columns: TableColumn[] = [
    { key: 'name',          label: AdminMessage.LABEL_NAME,        type: 'text' },
    { key: 'title',         label: AdminMessage.LABEL_TITLE,       type: 'truncate' },
    { key: 'slug',          label: AdminMessage.LABEL_SLUG,        type: 'text' },
    { key: 'countImageRef', label: AdminMessage.LABEL_IMAGE_COUNT, type: 'number' },
    { key: 'orderIndex',    label: AdminMessage.LABEL_ORDER,       type: 'number' },
  ];

  constructor(private svc: AlbumAdminService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.svc.getAll().subscribe({ next: d => { this.items = d; this.loading = false; }, error: () => { this.loading = false; } });
  }

  openCreate(): void {
    this.isEditing = false;
    this.form = { name: '', title: '', description: '', albumId: '', orderIndex: 0 };
    this.error = '';
    this.showModal = true;
  }

  openEdit(item: AlbumItem): void {
    this.isEditing = true;
    this.selected = item;
    this.form = { name: item.name, title: item.title, description: item.description ?? '', albumId: item.albumId ?? '', orderIndex: item.orderIndex };
    this.error = '';
    this.showModal = true;
  }

  openDelete(item: AlbumItem): void { this.selected = item; this.showConfirm = true; }

  save(): void {
    this.saving = true;
    const body = { ...this.form, albumId: this.form.albumId || undefined };
    const obs = this.isEditing
      ? this.svc.update({ id: this.selected!.id, ...body })
      : this.svc.create(body);

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
