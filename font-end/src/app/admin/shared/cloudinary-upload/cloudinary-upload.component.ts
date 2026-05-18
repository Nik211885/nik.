import { Component, ElementRef, EventEmitter, Input, OnDestroy, Output, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { FileCloudinaryService, UploadProgress } from '../../../core/services/file.cloudinary.service';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';

@Component({
  selector: 'app-cloudinary-upload',
  standalone: true,
  imports: [CommonModule, FormsModule, LanguagePipe],
  templateUrl: './cloudinary-upload.component.html',
  styleUrl: './cloudinary-upload.component.css'
})
export class CloudinaryUploadComponent implements OnDestroy {
  @Input() value = '';
  @Output() valueChange = new EventEmitter<string>();
  @Input() showManualInput = true;

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  isDragging = false;
  uploading = false;
  progress = 0;
  error = '';
  showUrlInput = false;
  manualUrl = '';

  protected readonly AdminMessage = AdminMessage;

  get isVideo(): boolean {
    return !!this.value && (
      this.value.includes('/video/upload/') ||
      /\.(mp4|webm|mov)(\?|$)/i.test(this.value)
    );
  }

  private cloudinary = inject(FileCloudinaryService);
  private sub?: Subscription;

  ngOnDestroy(): void { this.sub?.unsubscribe(); }

  onDragOver(e: DragEvent): void {
    e.preventDefault();
    e.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(e: DragEvent): void {
    e.preventDefault();
    e.stopPropagation();
    const rect = (e.currentTarget as HTMLElement).getBoundingClientRect();
    if (
      e.clientX < rect.left || e.clientX >= rect.right ||
      e.clientY < rect.top  || e.clientY >= rect.bottom
    ) {
      this.isDragging = false;
    }
  }

  onDrop(e: DragEvent): void {
    e.preventDefault();
    e.stopPropagation();
    this.isDragging = false;
    const file = e.dataTransfer?.files[0];
    if (file) this.upload(file);
  }

  onFileSelect(e: Event): void {
    const file = (e.target as HTMLInputElement).files?.[0];
    if (file) this.upload(file);
    (e.target as HTMLInputElement).value = '';
  }

  openPicker(): void {
    this.fileInput?.nativeElement.click();
  }

  clear(): void {
    this.value = '';
    this.valueChange.emit('');
    this.error = '';
    this.progress = 0;
  }

  applyManualUrl(): void {
    const url = this.manualUrl.trim();
    if (!url) return;
    this.value = url;
    this.valueChange.emit(url);
    this.showUrlInput = false;
    this.manualUrl = '';
    this.error = '';
  }

  private upload(file: File): void {
    this.uploading = true;
    this.progress = 0;
    this.error = '';

    this.sub?.unsubscribe();
    this.sub = this.cloudinary.uploadFile(file).subscribe({
      next: ({ progress, response }: UploadProgress) => {
        this.progress = progress;
        if (response?.secure_url) {
          this.value = response.secure_url;
          this.valueChange.emit(response.secure_url);
          this.uploading = false;
        }
      },
      error: () => {
        this.error = AdminMessage.UPLOAD_ERROR;
        this.uploading = false;
        if (this.showManualInput) this.showUrlInput = true;
      }
    });
  }
}
