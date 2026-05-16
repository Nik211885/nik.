import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HeroSlideAdminService } from '../../services/hero-slide.admin.service';
import { AdminConfirmModalComponent } from '../../shared/admin-confirm-modal.component';
import { CloudinaryUploadComponent } from '../../shared/cloudinary-upload/cloudinary-upload.component';
import { HeroSlideItem } from '../../models/admin.model';
import { AdminMessage } from '../../../app.message';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-hero-slides-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminConfirmModalComponent, CloudinaryUploadComponent, LanguagePipe],
  templateUrl: './hero-slides.component.html',
  styleUrl: './hero-slides.component.css'
})
export class HeroSlidesAdminComponent implements OnInit {
  protected readonly AdminMessage = AdminMessage;

  items: HeroSlideItem[] = [];
  loading = false;
  showModal = false;
  showConfirm = false;
  isEditing = false;
  selected: HeroSlideItem | null = null;
  saving = false;
  error = '';

  form: Omit<HeroSlideItem, 'id'> = this.emptyForm();

  constructor(private svc: HeroSlideAdminService, private toast: ToastService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.svc.getAll().subscribe({
      next: d => { this.items = d; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  openCreate(): void {
    this.isEditing = false;
    this.form = this.emptyForm();
    this.error = '';
    this.showModal = true;
  }

  openEdit(item: HeroSlideItem): void {
    this.isEditing = true;
    this.selected = item;
    this.form = { title: item.title, description: item.description, imageUrl: item.imageUrl, orderIndex: item.orderIndex, isActive: item.isActive };
    this.error = '';
    this.showModal = true;
  }

  openDelete(item: HeroSlideItem): void {
    this.selected = item;
    this.showConfirm = true;
  }

  save(): void {
    this.saving = true;
    const obs = this.isEditing
      ? this.svc.update({ id: this.selected!.id, ...this.form })
      : this.svc.create(this.form);

    obs.subscribe({
      next: () => { this.showModal = false; this.saving = false; this.load(); this.toast.success(AdminMessage.TOAST_SAVE_SUCCESS); },
      error: () => { this.error = AdminMessage.ERROR_GENERIC; this.saving = false; this.toast.error(AdminMessage.TOAST_SAVE_ERROR); }
    });
  }

  delete(): void {
    if (!this.selected) return;
    this.svc.delete([this.selected.id]).subscribe({
      next: () => { this.showConfirm = false; this.load(); this.toast.success(AdminMessage.TOAST_DELETE_SUCCESS); },
      error: () => { this.showConfirm = false; this.toast.error(AdminMessage.TOAST_DELETE_ERROR); }
    });
  }

  private emptyForm(): Omit<HeroSlideItem, 'id'> {
    return { title: '', description: '', imageUrl: '', orderIndex: this.items.length + 1, isActive: true };
  }
}
