import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AlbumItem, FileItem } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class AlbumAdminService {
  private readonly base = 'api/albums';

  constructor(private http: HttpClient) {}

  getAll(): Observable<AlbumItem[]> {
    return this.http.get<AlbumItem[]>(this.base);
  }

  getTree(): Observable<AlbumItem[]> {
    return this.http.get<AlbumItem[]>(`${this.base}/tree`);
  }

  create(body: { name: string; title: string; description: string; albumId?: string; orderIndex?: number }): Observable<AlbumItem> {
    return this.http.post<AlbumItem>(`${this.base}/create`, body);
  }

  update(body: { id: string; name: string; title: string; description: string; albumId?: string; orderIndex?: number; fileDescriptionId?: string }): Observable<AlbumItem> {
    return this.http.put<AlbumItem>(`${this.base}/update`, body);
  }

  setCover(albumId: string, fileId: string | null): Observable<AlbumItem> {
    return this.http.put<AlbumItem>(`${this.base}/set-cover`, { albumId, fileId });
  }

  delete(ids: string[]): Observable<void> {
    const params = ids.reduce((p, id) => p.append('ids', id), new HttpParams());
    return this.http.delete<void>(`${this.base}/delete`, { params });
  }

  getFiles(albumId: string): Observable<FileItem[]> {
    return this.http.get<FileItem[]>(`${this.base}/${albumId}/files`);
  }

  addFiles(albumId: string, fileIds: string[]): Observable<any> {
    return this.http.post(`${this.base}/files/add`, { albumId, fileIds });
  }

  removeFiles(albumId: string, fileIds: string[]): Observable<void> {
    return this.http.delete<void>(`${this.base}/files/remove`, { body: { albumId, fileIds } });
  }
}
