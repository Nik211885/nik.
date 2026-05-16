import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HeroCarouselModel } from '../../shared/components/hero-carousel/hero-carousel.model';

@Injectable({ providedIn: 'root' })
export class HeroSlideService {
  constructor(private http: HttpClient) {}

  getSlides(): Observable<HeroCarouselModel[]> {
    return this.http
      .get<{ id: string; title: string; description: string; imageUrl: string }[]>('public-api/hero-slides')
      .pipe(map(slides => slides.map(s => ({ id: s.id, img: s.imageUrl, title: s.title, description: s.description }))));
  }
}
