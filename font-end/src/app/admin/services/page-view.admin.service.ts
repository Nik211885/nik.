import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PageViewItem, PageViewStats, PaginationResponse } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class PageViewAdminService {
  private readonly base = 'api/page-views';

  constructor(private http: HttpClient) {}

  getAll(pageNumber: number, pageSize: number): Observable<PaginationResponse<PageViewItem>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);
    return this.http.get<PaginationResponse<PageViewItem>>(this.base, { params });
  }

  getStats(period: 'week' | 'month' | 'year'): Observable<PageViewStats> {
    return this.http.get<PageViewStats>(`${this.base}/stats`, { params: { period } });
  }
}
