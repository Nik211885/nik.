import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CommentItem, PaginationResponse } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class CommentAdminService {
  private readonly base = 'api/comments';

  constructor(private http: HttpClient) {}

  getPage(page: number, pageSize: number, articleId?: string): Observable<PaginationResponse<CommentItem>> {
    let params: Record<string, string | number> = { pageNumber: page, pageSize };
    if (articleId) params['articleId'] = articleId;
    return this.http.get<PaginationResponse<CommentItem>>(`${this.base}/admin`, { params });
  }

  delete(ids: string[]): Observable<void> {
    const params = ids.reduce((p, id) => p.append('ids', id), new HttpParams());
    return this.http.delete<void>(`${this.base}/delete`, { params });
  }
}
