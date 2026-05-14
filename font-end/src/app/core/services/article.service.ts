import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {ArticleModel} from '../../shared/models/article.model';
import {PaginationResponse} from '../../admin/models/admin.model';

@Injectable({ providedIn: 'root' })
export class ArticleService {
  constructor(private http: HttpClient) {}

  getTopArticles(): Observable<ArticleModel[]> {
    return this.http.get<ArticleModel[]>('api/articles/top');
  }

  getArticles(pageNumber: number = 1, pageSize: number = 6): Observable<PaginationResponse<ArticleModel>> {
    return this.http.get<PaginationResponse<ArticleModel>>(`api/articles?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }
}
