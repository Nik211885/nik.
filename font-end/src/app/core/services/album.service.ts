import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AlbumFileModel, AlbumModel } from '../../shared/models/album.model';

@Injectable({ providedIn: 'root' })
export class AlbumService {
  constructor(private http: HttpClient) {}

  getParents(): Observable<AlbumModel[]> {
    return this.http.get<AlbumModel[]>('api/albums/parents?tree=true');
  }

  getFiles(albumId: string): Observable<AlbumFileModel[]> {
    return this.http.get<AlbumFileModel[]>(`api/albums/${albumId}/files`);
  }
}
