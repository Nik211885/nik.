import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { FileAdminService } from '../../services/file.admin.service';
import { AdminConfirmModalComponent } from '../../shared/admin-confirm-modal.component';
import { CloudinaryUploadComponent } from '../../shared/cloudinary-upload/cloudinary-upload.component';
import { FileCloudinaryService } from '../../../core/services/file.cloudinary.service';
import { FileItem, PaginationResponse } from '../../models/admin.model';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';

interface BatchItem {
  name: string;
  progress: number;
  done: boolean;
  error?: string;
}

@Component({
  selector: 'app-files-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminConfirmModalComponent, CloudinaryUploadComponent, LanguagePipe],
  templateUrl: './files.component.html',
  styleUrl: './files.component.css'
})
export class FilesAdminComponent implements OnInit, OnDestroy {
  page: PaginationResponse<FileItem> = { data: [], totalItems: 0, pageNumber: 1, pageSize: 30, pageCount: 0 };
  loading = false;
  showModal = false;
  showConfirm = false;
  isEditing = false;
  selected: FileItem | null = null;
  saving = false;
  error = '';

  form = { name: '', title: '', url: '', description: '' };

  isDroppingPage = false;
  batchItems: BatchItem[] = [];
  private dragCounter = 0;
  private subs: Subscription[] = [];

  protected readonly AdminMessage = AdminMessage;

  constructor(
    private svc: FileAdminService,
    private cloudinary: FileCloudinaryService
  ) {}

  ngOnInit(): void { this.load(); }
  ngOnDestroy(): void { this.subs.forEach(s => s.unsubscribe()); }

  load(p = 1): void {
    this.loading = true;
    this.svc.getAll(p, 30).subscribe({
      next: d => { this.page = d; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  openCreate(): void {
    this.isEditing = false;
    this.form = { name: '', title: '', url: '', description: '' };
    this.error = '';
    this.showModal = true;
  }

  openEdit(item: FileItem): void {
    this.isEditing = true;
    this.selected = item;
    this.form = { name: item.name, title: item.title, url: item.url, description: item.description };
    this.error = '';
    this.showModal = true;
  }

  openDelete(item: FileItem): void { this.selected = item; this.showConfirm = true; }

  save(): void {
    if (!this.isEditing && !this.form.url) {
      this.error = AdminMessage.FILES_UPLOAD_REQUIRED;
      return;
    }
    this.saving = true;
    const obs = this.isEditing
      ? this.svc.update({ id: this.selected!.id, ...this.form })
      : this.svc.create(this.form);

    obs.subscribe({
      next: () => { this.showModal = false; this.saving = false; this.load(this.page.pageNumber); },
      error: () => { this.error = AdminMessage.ERROR_GENERIC; this.saving = false; }
    });
  }

  delete(): void {
    this.svc.delete([this.selected!.id]).subscribe({
      next: () => { this.showConfirm = false; this.load(this.page.pageNumber); },
      error: () => { this.showConfirm = false; }
    });
  }

  goToPage(p: number): void {
    if (p >= 1 && p <= this.page.pageCount) this.load(p);
  }

  copyUrl(url: string): void {
    navigator.clipboard.writeText(url).catch(() => {});
  }

  // ─── Page-level drag & drop batch upload ─────────────────────────────────

  @HostListener('document:dragenter', ['$event'])
  onDocDragEnter(e: DragEvent): void {
    if (!e.dataTransfer?.types.includes('Files')) return;
    this.dragCounter++;
    this.isDroppingPage = true;
  }

  @HostListener('document:dragleave', ['$event'])
  onDocDragLeave(e: DragEvent): void {
    if (!e.dataTransfer?.types.includes('Files')) return;
    this.dragCounter = Math.max(0, this.dragCounter - 1);
    if (this.dragCounter === 0) this.isDroppingPage = false;
  }

  @HostListener('document:dragover', ['$event'])
  onDocDragOver(e: DragEvent): void {
    if (e.dataTransfer?.types.includes('Files')) e.preventDefault();
  }

  @HostListener('document:drop', ['$event'])
  onDocDrop(e: DragEvent): void {
    e.preventDefault();
    this.isDroppingPage = false;
    this.dragCounter = 0;
    if (this.showModal) return;

    const files = Array.from(e.dataTransfer?.files ?? []).filter(
      f => f.type.startsWith('image/') || f.type === 'video/mp4'
    );
    if (files.length) this.batchUpload(files);
  }

  private batchUpload(files: File[]): void {
    const items: BatchItem[] = files.map(f => ({ name: f.name, progress: 0, done: false }));
    this.batchItems = [...this.batchItems, ...items];

    files.forEach((file, i) => {
      const item = items[i];
      const sub = this.cloudinary.uploadFile(file).subscribe({
        next: ({ progress, response }) => {
          item.progress = progress;
          if (response?.url) {
            const name = file.name.replace(/\.[^/.]+$/, '');
            this.svc.create({ name, title: '', url: response.url, description: '' }).subscribe({
              next: () => {
                item.done = true;
                this.load(1);
                setTimeout(() => {
                  this.batchItems = this.batchItems.filter(x => x !== item);
                }, 2500);
              },
              error: () => { item.error = AdminMessage.FILES_SAVE_FAILED; }
            });
          }
        },
        error: () => {
          item.progress = 0;
          item.error = AdminMessage.ERROR_GENERIC;
        }
      });
      this.subs.push(sub);
    });
  }
}
