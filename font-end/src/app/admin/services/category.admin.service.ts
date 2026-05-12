import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CategoryItem } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class CategoryAdminService {
  private readonly base = 'api/categories';

  constructor(private http: HttpClient) {}

  getAll(): Observable<CategoryItem[]> {
    return this.http.get<CategoryItem[]>(this.base);
  }

  create(body: { name: string; title: string; description: string; image: string }): Observable<CategoryItem> {
    return this.http.post<CategoryItem>(`${this.base}/create`, body);
  }

  update(body: { id: string; name: string; title: string; description: string; image: string }): Observable<CategoryItem> {
    return this.http.put<CategoryItem>(`${this.base}/update`, body);
  }

  delete(ids: string[]): Observable<void> {
    const params = ids.reduce((p, id) => p.append('ids', id), new HttpParams());
    return this.http.delete<void>(`${this.base}/delete`, { params });
  }
}
