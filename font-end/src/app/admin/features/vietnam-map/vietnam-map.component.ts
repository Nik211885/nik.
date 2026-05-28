import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { QuillModule } from 'ngx-quill';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';
import { AdminConfirmModalComponent } from '../../shared/admin-confirm-modal.component';
import { CloudinaryUploadComponent } from '../../shared/cloudinary-upload/cloudinary-upload.component';
import { VietnamMapAdminService } from '../../services/vietnam-map.admin.service';
import {
  ProvinceModel,
  TripModel,
  TripPhotoModel,
  CreateTripPayload,
  UpdateTripPayload,
} from '../../models/admin.model';

@Component({
  selector: 'app-vietnam-map-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, QuillModule, LanguagePipe, AdminConfirmModalComponent, CloudinaryUploadComponent],
  templateUrl: './vietnam-map.component.html',
  styleUrl: './vietnam-map.component.css',
})
export class VietnamMapAdminComponent implements OnInit {
  protected readonly AdminMessage = AdminMessage;

  readonly editorModules = {
    toolbar: [
      [{ header: [1, 2, 3, 4, false] }],
      ['bold', 'italic', 'underline', 'strike'],
      [{ list: 'ordered' }, { list: 'bullet' }],
      ['blockquote', 'code-block'],
      ['link', 'image'],
      [{ align: [] }],
      ['clean'],
    ],
  };

  // ── Province list ────────────────────────────────────────────────────────
  provinces: ProvinceModel[] = [];
  selectedProvince: ProvinceModel | null = null;

  // ── Trip list ────────────────────────────────────────────────────────────
  trips: TripModel[] = [];
  tripsLoading = false;

  // ── Trip form ────────────────────────────────────────────────────────────
  formOpen = false;
  editing: TripModel | null = null;
  saving = false;
  form: CreateTripPayload = this.emptyForm();

  // ── Photo panel ──────────────────────────────────────────────────────────
  photoTripId: string | null = null;
  photoTrip: TripModel | null = null;
  photos: TripPhotoModel[] = [];
  photosLoading = false;
  newPhotoUrl = '';
  newPhotoCaption = '';
  photoUploading = false;

  // ── Delete ───────────────────────────────────────────────────────────────
  deleteId: string | null = null;
  deleteType: 'trip' | 'photo' = 'trip';
  confirmOpen = false;

  constructor(private svc: VietnamMapAdminService) {}

  ngOnInit(): void {
    this.svc.getProvinces().subscribe(data => { this.provinces = data; });
  }

  selectProvince(province: ProvinceModel): void {
    this.selectedProvince = province;
    this.formOpen = false;
    this.editing = null;
    this.closePhotoPanel();
    this.loadTrips(province.id);
  }

  private loadTrips(provinceId: string): void {
    this.tripsLoading = true;
    this.svc.getTripsByProvince(provinceId).subscribe({
      next: data => { this.trips = data; this.tripsLoading = false; },
      error: () => { this.tripsLoading = false; },
    });
  }

  // ── Trip form ────────────────────────────────────────────────────────────

  openCreate(): void {
    this.editing = null;
    this.form = this.emptyForm();
    this.formOpen = true;
    this.closePhotoPanel();
  }

  openEdit(trip: TripModel): void {
    this.editing = trip;
    this.form = { provinceId: trip.provinceId, title: trip.title, date: trip.date, story: trip.story };
    this.formOpen = true;
    this.closePhotoPanel();
  }

  closeForm(): void {
    this.formOpen = false;
    this.editing = null;
  }

  save(): void {
    if (!this.selectedProvince) return;
    this.saving = true;

    if (this.editing) {
      const payload: UpdateTripPayload = { id: this.editing.id, title: this.form.title, date: this.form.date, story: this.form.story };
      this.svc.updateTrip(payload).subscribe({
        next: updated => {
          const idx = this.trips.findIndex(t => t.id === updated.id);
          if (idx !== -1) this.trips[idx] = { ...updated, photos: this.trips[idx].photos ?? [] };
          this.closeForm();
          this.saving = false;
        },
        error: () => { this.saving = false; },
      });
    } else {
      this.form.provinceId = this.selectedProvince.id;
      this.svc.createTrip(this.form).subscribe({
        next: created => {
          this.trips.unshift({ ...created, photos: [] });
          const p = this.provinces.find(p => p.id === this.selectedProvince!.id);
          if (p) p.tripCount++;
          this.closeForm();
          this.saving = false;
        },
        error: () => { this.saving = false; },
      });
    }
  }

  // ── Photo panel ──────────────────────────────────────────────────────────

  openPhotoPanel(trip: TripModel): void {
    this.photoTripId = trip.id;
    this.photoTrip = trip;
    this.formOpen = false;
    this.editing = null;
    this.newPhotoUrl = '';
    this.newPhotoCaption = '';
    this.photosLoading = true;
    this.svc.getPhotos(trip.id).subscribe({
      next: photos => { this.photos = photos; this.photosLoading = false; },
      error: () => { this.photosLoading = false; },
    });
  }

  closePhotoPanel(): void {
    this.photoTripId = null;
    this.photoTrip = null;
    this.photos = [];
    this.newPhotoUrl = '';
    this.newPhotoCaption = '';
  }

  onPhotoUploaded(url: string): void {
    this.newPhotoUrl = url;
  }

  addPhoto(): void {
    if (!this.photoTripId || !this.newPhotoUrl) return;
    this.photoUploading = true;
    this.svc.addPhoto(this.photoTripId, this.newPhotoUrl, this.newPhotoCaption || undefined).subscribe({
      next: photo => {
        this.photos.push(photo);
        // sync photos on the trip object in the list
        const trip = this.trips.find(t => t.id === this.photoTripId);
        if (trip) trip.photos = [...(trip.photos ?? []), photo];
        this.newPhotoUrl = '';
        this.newPhotoCaption = '';
        this.photoUploading = false;
      },
      error: () => { this.photoUploading = false; },
    });
  }

  confirmDeletePhoto(id: string): void {
    this.deleteId = id;
    this.deleteType = 'photo';
    this.confirmOpen = true;
  }

  // ── Delete ───────────────────────────────────────────────────────────────

  confirmDelete(id: string): void {
    this.deleteId = id;
    this.deleteType = 'trip';
    this.confirmOpen = true;
  }

  cancelDelete(): void {
    this.deleteId = null;
    this.confirmOpen = false;
  }

  doDelete(): void {
    if (!this.deleteId) return;
    const id = this.deleteId;
    this.confirmOpen = false;
    this.deleteId = null;

    if (this.deleteType === 'photo') {
      this.svc.deletePhoto(id).subscribe(() => {
        this.photos = this.photos.filter(p => p.id !== id);
        const trip = this.trips.find(t => t.id === this.photoTripId);
        if (trip) trip.photos = (trip.photos ?? []).filter(p => p.id !== id);
      });
    } else {
      this.svc.deleteTrip(id).subscribe(() => {
        this.trips = this.trips.filter(t => t.id !== id);
        const p = this.provinces.find(p => p.id === this.selectedProvince?.id);
        if (p && p.tripCount > 0) p.tripCount--;
        if (this.photoTripId === id) this.closePhotoPanel();
      });
    }
  }

  private emptyForm(): CreateTripPayload {
    return { provinceId: '', title: '', date: new Date().toISOString().substring(0, 10), story: null };
  }
}
