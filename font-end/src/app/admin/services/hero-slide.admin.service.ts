import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HeroSlideItem } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class HeroSlideAdminService {
  private readonly base = 'api/hero-slides';

  constructor(private http: HttpClient) {}

  getAll(): Observable<HeroSlideItem[]> {
    return this.http.get<HeroSlideItem[]>(this.base);
  }

  create(payload: Omit<HeroSlideItem, 'id'>): Observable<HeroSlideItem> {
    return this.http.post<HeroSlideItem>(`${this.base}/create`, payload);
  }

  update(payload: HeroSlideItem): Observable<HeroSlideItem> {
    return this.http.put<HeroSlideItem>(`${this.base}/update`, payload);
  }

  delete(ids: string[]): Observable<void> {
    const params = ids.map(id => `ids=${encodeURIComponent(id)}`).join('&');
    return this.http.delete<void>(`${this.base}/delete?${params}`);
  }
}
