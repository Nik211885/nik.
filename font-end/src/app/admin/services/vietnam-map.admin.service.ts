import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  CreateTripPayload,
  ProvinceModel,
  TripModel,
  TripPhotoModel,
  UpdateTripPayload,
} from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class VietnamMapAdminService {
  constructor(private http: HttpClient) {}

  getProvinces() {
    return this.http.get<ProvinceModel[]>('api/provinces');
  }

  getTripsByProvince(provinceId: string) {
    return this.http.get<TripModel[]>(`api/trips?provinceId=${provinceId}`);
  }

  createTrip(payload: CreateTripPayload) {
    return this.http.post<TripModel>('api/trips/create', payload);
  }

  updateTrip(payload: UpdateTripPayload) {
    return this.http.put<TripModel>('api/trips/update', payload);
  }

  deleteTrip(id: string) {
    return this.http.delete('api/trips/delete', { params: { ids: id } });
  }

  getPhotos(tripId: string) {
    return this.http.get<TripPhotoModel[]>(`api/trip-photos?tripId=${tripId}`);
  }

  addPhoto(tripId: string, url: string, caption?: string) {
    return this.http.post<TripPhotoModel>('api/trip-photos/create', { tripId, url, caption: caption ?? null, order: 0 });
  }

  deletePhoto(id: string) {
    return this.http.delete('api/trip-photos/delete', { params: { ids: id } });
  }
}
