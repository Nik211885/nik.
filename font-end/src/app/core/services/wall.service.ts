import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface WallMessageModel {
  id: string;
  name: string;
  message: string;
  source?: string;
  status: string;
  createdDate: string;
  reactionCount: number;
}

@Injectable({ providedIn: 'root' })
export class WallService {
  constructor(private http: HttpClient) {}

  getApproved(): Observable<WallMessageModel[]> {
    return this.http.get<WallMessageModel[]>('public-api/wall-messages');
  }

  create(name: string, message: string, source?: string): Observable<WallMessageModel> {
    return this.http.post<WallMessageModel>('public-api/wall-messages', { name, message, source: source || null });
  }

  react(id: string, deviceId: string): Observable<{ reactionCount: number; reacted: boolean }> {
    return this.http.post<{ reactionCount: number; reacted: boolean }>(
      `public-api/wall-messages/${id}/react`,
      { deviceId }
    );
  }

  getAll(page = 1, pageSize = 20, status?: string): Observable<any> {
    let url = `api/wall-messages?pageNumber=${page}&pageSize=${pageSize}`;
    if (status) url += `&status=${status}`;
    return this.http.get<any>(url);
  }

  updateStatus(id: string, status: string): Observable<any> {
    return this.http.put<any>(`api/wall-messages/${id}/status?status=${status}`, {});
  }

  bulkUpdateStatus(ids: string[], status: string): Observable<void> {
    const params = ids.map(id => `ids=${id}`).join('&');
    return this.http.put<void>(`api/wall-messages/bulk-status?${params}&status=${status}`, {});
  }

  delete(ids: string[]): Observable<void> {
    const params = ids.map(id => `ids=${id}`).join('&');
    return this.http.delete<void>(`api/wall-messages/delete?${params}`);
  }
}
