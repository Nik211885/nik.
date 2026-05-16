import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { TagCloudModel } from '../../shared/components/tag-cloud/tag-cloud.model';

@Injectable({ providedIn: 'root' })
export class TagService {
  constructor(private http: HttpClient) {}

  getTags(): Observable<TagCloudModel[]> {
    return this.http
      .get<{ id: string; name: string; title: string; slug: string }[]>('api/tags')
      .pipe(map(tags => tags.map(t => ({ id: t.id, name: t.title || t.name, ref: '/tag/' + t.slug }))));
  }
}
