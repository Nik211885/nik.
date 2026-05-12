import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TagItem } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class TagAdminService {
  private readonly base = 'api/tags';

  constructor(private http: HttpClient) {}

  getAll(): Observable<TagItem[]> {
    return this.http.get<TagItem[]>(this.base);
  }

  create(body: { name: string; title: string; description: string; image: string }): Observable<TagItem> {
    return this.http.post<TagItem>(`${this.base}/create`, body);
  }

  update(body: { id: string; name: string; title: string; description: string; image: string }): Observable<TagItem> {
    return this.http.put<TagItem>(`${this.base}/update`, body);
  }

  delete(ids: string[]): Observable<void> {
    const params = ids.reduce((p, id) => p.append('ids', id), new HttpParams());
    return this.http.delete<void>(`${this.base}/delete`, { params });
  }
}
