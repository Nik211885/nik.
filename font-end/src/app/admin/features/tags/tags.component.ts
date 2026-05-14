import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TagAdminService } from '../../services/tag.admin.service';
import { AdminTableComponent } from '../../shared/admin-table.component';
import { AdminConfirmModalComponent } from '../../shared/admin-confirm-modal.component';
import { CloudinaryUploadComponent } from '../../shared/cloudinary-upload/cloudinary-upload.component';
import { TagItem, TableColumn } from '../../models/admin.model';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';

@Component({
  selector: 'app-tags-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminTableComponent, AdminConfirmModalComponent, CloudinaryUploadComponent, LanguagePipe],
  templateUrl: './tags.component.html',
  styleUrl: './tags.component.css'
})
export class TagsAdminComponent implements OnInit {
  items: TagItem[] = [];
  loading = false;
  showModal = false;
  showConfirm = false;
  isEditing = false;
  selected: TagItem | null = null;
  deleteItems: TagItem[] = [];
  saving = false;
  error = '';

  form = { name: '', title: '', description: '', image: '' };

  protected readonly AdminMessage = AdminMessage;

  readonly columns: TableColumn[] = [
    { key: 'image',    label: AdminMessage.LABEL_IMAGE,         type: 'image' },
    { key: 'name',     label: AdminMessage.LABEL_NAME,          type: 'text' },
    { key: 'title',    label: AdminMessage.LABEL_TITLE,         type: 'truncate' },
    { key: 'slug',     label: AdminMessage.LABEL_SLUG,          type: 'text' },
    { key: 'countRef', label: AdminMessage.LABEL_ARTICLE_COUNT, type: 'number' },
  ];

  constructor(private svc: TagAdminService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.svc.getAll().subscribe({ next: d => { this.items = d; this.loading = false; }, error: () => { this.loading = false; } });
  }

  openCreate(): void {
    this.isEditing = false;
    this.form = { name: '', title: '', description: '', image: '' };
    this.error = '';
    this.showModal = true;
  }

  openEdit(item: TagItem): void {
    this.isEditing = true;
    this.selected = item;
    this.form = { name: item.name, title: item.title, description: item.description, image: item.image };
    this.error = '';
    this.showModal = true;
  }

  openDelete(items: TagItem[]): void { this.deleteItems = items; this.selected = items[0] ?? null; this.showConfirm = true; }

  save(): void {
    this.saving = true;
    const obs = this.isEditing
      ? this.svc.update({ id: this.selected!.id, ...this.form })
      : this.svc.create(this.form);

    obs.subscribe({
      next: () => { this.showModal = false; this.saving = false; this.load(); },
      error: () => { this.error = AdminMessage.ERROR_GENERIC; this.saving = false; }
    });
  }

  delete(): void {
    this.svc.delete(this.deleteItems.map(i => i.id)).subscribe({
      next: () => { this.showConfirm = false; this.load(); },
      error: () => { this.showConfirm = false; }
    });
  }
}
