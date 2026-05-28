import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface ProvinceMapModel {
  id: string;
  name: string;
  code: string;
  tripCount: number;
}

export interface TripPhotoMapModel {
  id: string;
  url: string;
  caption: string | null;
  order: number;
}

export interface TripMapModel {
  id: string;
  provinceId: string;
  title: string;
  date: string;
  story: string | null;
  photos: TripPhotoMapModel[];
}

@Injectable({ providedIn: 'root' })
export class VietnamMapService {
  constructor(private http: HttpClient) {}

  getProvinces() {
    return this.http.get<ProvinceMapModel[]>('api/provinces');
  }

  getTripsByProvince(provinceId: string) {
    return this.http.get<TripMapModel[]>(`api/trips?provinceId=${provinceId}`);
  }
}
