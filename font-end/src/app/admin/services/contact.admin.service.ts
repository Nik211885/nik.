import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ContactItem, PaginationResponse } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class ContactAdminService {
  private readonly base = 'api/contacts';

  constructor(private http: HttpClient) {}

  getUnread(): Observable<ContactItem[]> {
    return this.http.get<ContactItem[]>(`${this.base}/unread`);
  }

  getPage(pageNumber: number, pageSize: number): Observable<PaginationResponse<ContactItem>> {
    return this.http.get<PaginationResponse<ContactItem>>(this.base, {
      params: { pageNumber: pageNumber.toString(), pageSize: pageSize.toString() }
    });
  }

  markAsRead(id: string): Observable<ContactItem> {
    return this.http.put<ContactItem>(`${this.base}/${id}/read`, {});
  }

  delete(ids: string[]): Observable<void> {
    const params = ids.reduce((p, id) => p.append('ids', id), new HttpParams());
    return this.http.delete<void>(`${this.base}/delete`, { params });
  }
}
