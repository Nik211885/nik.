import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ArticleItem, PaginationResponse } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class ArticleAdminService {
  private readonly base = 'api/articles';

  constructor(private http: HttpClient) {}

  getAll(pageNumber = 1, pageSize = 50): Observable<PaginationResponse<ArticleItem>> {
    const params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);
    return this.http.get<PaginationResponse<ArticleItem>>(this.base, { params });
  }

  create(body: { title: string; description: string; content: string; image: string; tagIds: string[]; categoryIds: string[] }): Observable<ArticleItem> {
    return this.http.post<ArticleItem>(`${this.base}/create`, body);
  }

  update(body: { id: string; title: string; description: string; content: string; image: string; tagIds: string[]; categoryIds: string[] }): Observable<ArticleItem> {
    return this.http.put<ArticleItem>(`${this.base}/update`, body);
  }

  delete(ids: string[]): Observable<void> {
    const params = ids.reduce((p, id) => p.append('ids', id), new HttpParams());
    return this.http.delete<void>(`${this.base}/delete`, { params });
  }
}
