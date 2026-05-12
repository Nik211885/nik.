import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserItem } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class UserAdminService {
  private readonly base = 'api/users';

  constructor(private http: HttpClient) {}

  getAll(): Observable<UserItem[]> {
    return this.http.get<UserItem[]>(this.base);
  }

  getById(id: string): Observable<UserItem> {
    return this.http.get<UserItem>(`${this.base}/${id}`);
  }

  update(body: { userName: string; email?: string; phone?: string; bio: string }): Observable<UserItem> {
    return this.http.put<UserItem>(`${this.base}/update`, body);
  }
}
