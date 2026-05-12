import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FileItem, PaginationResponse } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class FileAdminService {
  private readonly base = 'api/files';

  constructor(private http: HttpClient) {}

  getAll(pageNumber = 1, pageSize = 50): Observable<PaginationResponse<FileItem>> {
    const params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);
    return this.http.get<PaginationResponse<FileItem>>(this.base, { params });
  }

  create(body: { name: string; title: string; url: string; description: string }): Observable<FileItem> {
    return this.http.post<FileItem>(`${this.base}/create`, body);
  }

  update(body: { id: string; name: string; title: string; url: string; description: string }): Observable<FileItem> {
    return this.http.put<FileItem>(`${this.base}/update`, body);
  }

  delete(ids: string[]): Observable<void> {
    const params = ids.reduce((p, id) => p.append('ids', id), new HttpParams());
    return this.http.delete<void>(`${this.base}/delete`, { params });
  }
}
