import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SysConfigItem } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class SysConfigAdminService {
  private readonly base = 'api/sys-configs';

  constructor(private http: HttpClient) {}

  getAll(): Observable<SysConfigItem[]> {
    return this.http.get<SysConfigItem[]>(this.base);
  }

  create(body: { key: string; value: any }): Observable<SysConfigItem> {
    return this.http.post<SysConfigItem>(`${this.base}/create`, body);
  }

  update(body: { id: string; key: string; value: any }): Observable<SysConfigItem> {
    return this.http.put<SysConfigItem>(`${this.base}/update`, body);
  }

  delete(ids: string[]): Observable<void> {
    const params = ids.reduce((p, id) => p.append('ids', id), new HttpParams());
    return this.http.delete<void>(`${this.base}/delete`, { params });
  }
}
