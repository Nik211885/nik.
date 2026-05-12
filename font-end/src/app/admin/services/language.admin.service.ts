import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CodeLanguageItem, LanguageItem, TranslateItem } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class LanguageAdminService {
  private readonly baseLang = 'api/languages';
  private readonly baseCode = 'api/code-languages';
  private readonly baseTrans = 'api/translates';

  constructor(private http: HttpClient) {}

  // Languages
  getLanguages(): Observable<LanguageItem[]> {
    return this.http.get<LanguageItem[]>(this.baseLang);
  }

  createLanguage(body: { code: string; name: string }): Observable<LanguageItem> {
    return this.http.post<LanguageItem>(`${this.baseLang}/create`, body);
  }

  updateLanguage(body: { id: string; code: string; name: string }): Observable<LanguageItem> {
    return this.http.put<LanguageItem>(`${this.baseLang}/update`, body);
  }

  deleteLanguage(ids: string[]): Observable<void> {
    const params = ids.reduce((p, id) => p.append('ids', id), new HttpParams());
    return this.http.delete<void>(`${this.baseLang}/delete`, { params });
  }

  // Code Keys
  getCodeKeys(): Observable<CodeLanguageItem[]> {
    return this.http.get<CodeLanguageItem[]>(this.baseCode);
  }

  createCodeKey(body: { code: string }): Observable<CodeLanguageItem> {
    return this.http.post<CodeLanguageItem>(`${this.baseCode}/create`, body);
  }

  deleteCodeKey(ids: string[]): Observable<void> {
    const params = ids.reduce((p, id) => p.append('ids', id), new HttpParams());
    return this.http.delete<void>(`${this.baseCode}/delete`, { params });
  }

  // Translations
  getTranslations(): Observable<TranslateItem[]> {
    return this.http.get<TranslateItem[]>(this.baseTrans);
  }

  createTranslation(body: { codeId: string; languageId: string; value: string }): Observable<TranslateItem> {
    return this.http.post<TranslateItem>(`${this.baseTrans}/create`, body);
  }

  updateTranslation(body: { id: string; value: string }): Observable<TranslateItem> {
    return this.http.put<TranslateItem>(`${this.baseTrans}/update`, body);
  }

  deleteTranslation(ids: string[]): Observable<void> {
    const params = ids.reduce((p, id) => p.append('ids', id), new HttpParams());
    return this.http.delete<void>(`${this.baseTrans}/delete`, { params });
  }
}
