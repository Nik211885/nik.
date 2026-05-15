import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';
import { TagCloudModel } from '../../shared/components/tag-cloud/tag-cloud.model';

@Injectable({ providedIn: 'root' })
export class TagService {
  private cache$: Observable<TagCloudModel[]> | null = null;

  constructor(private http: HttpClient) {}

  getTags(): Observable<TagCloudModel[]> {
    if (!this.cache$) {
      this.cache$ = this.http
        .get<{ id: string; name: string; slug: string }[]>('api/tags')
        .pipe(
          map(tags => tags.map(t => ({ id: t.id, name: t.name, ref: '/tag/' + t.slug }))),
          shareReplay(1)
        );
    }
    return this.cache$;
  }
}
