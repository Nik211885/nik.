import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CommentItem } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class CommentAdminService {
  private readonly base = 'api/comments';

  constructor(private http: HttpClient) {}

  getByArticle(articleId: string): Observable<CommentItem[]> {
    return this.http.get<CommentItem[]>(this.base, { params: { articleId } });
  }

  delete(ids: string[]): Observable<void> {
    const params = ids.reduce((p, id) => p.append('ids', id), new HttpParams());
    return this.http.delete<void>(`${this.base}/delete`, { params });
  }
}
