import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';
import { HeroCarouselModel } from '../../shared/components/hero-carousel/hero-carousel.model';

@Injectable({ providedIn: 'root' })
export class HeroSlideService {
  private cache$: Observable<HeroCarouselModel[]> | null = null;

  constructor(private http: HttpClient) {}

  getSlides(): Observable<HeroCarouselModel[]> {
    if (!this.cache$) {
      this.cache$ = this.http
        .get<{ id: string; title: string; description: string; imageUrl: string }[]>('public-api/hero-slides')
        .pipe(
          map(slides => slides.map(s => ({ id: s.id, img: s.imageUrl, title: s.title, description: s.description }))),
          shareReplay(1)
        );
    }
    return this.cache$;
  }
}
