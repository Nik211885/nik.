import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  EntityTranslationResponse,
  PaginationResponse,
  TranslationStatusItem,
  UpsertTranslationRequest,
} from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class ContentTranslationAdminService {
  private readonly base = 'api/content-translations';

  constructor(private http: HttpClient) {}

  getStatusList(
    entityType: string,
    langCode: string,
    translated: boolean | null,
    pageNumber: number,
    pageSize: number
  ): Observable<PaginationResponse<TranslationStatusItem>> {
    const params: Record<string, string | number> = { entityType, langCode, pageNumber, pageSize };
    if (translated !== null) params['translated'] = String(translated);
    return this.http.get<PaginationResponse<TranslationStatusItem>>(`${this.base}/status`, { params });
  }

  getEntityTranslation(entityType: string, entityId: string, langCode: string): Observable<EntityTranslationResponse> {
    return this.http.get<EntityTranslationResponse>(this.base, {
      params: { entityType, entityId, langCode },
    });
  }

  getSource(entityType: string, entityId: string): Observable<EntityTranslationResponse> {
    return this.http.get<EntityTranslationResponse>(`${this.base}/source/${entityType}/${entityId}`);
  }

  upsert(request: UpsertTranslationRequest): Observable<void> {
    return this.http.post<void>(`${this.base}/upsert`, request);
  }
}
