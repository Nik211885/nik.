import { Injectable, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class PageViewTrackerService implements OnDestroy {
  private sub = new Subscription();

  constructor(private http: HttpClient, private router: Router) {
    this.sub.add(
      this.router.events
        .pipe(filter(e => e instanceof NavigationEnd))
        .subscribe((e: NavigationEnd) => this.track(e.urlAfterRedirects))
    );
  }

  private track(path: string): void {
    // Skip admin routes — we only track public site visits
    if (path.startsWith('/admin') || path.startsWith('/login')) return;

    const headers = new HttpHeaders({ 'X-Skip-Auth-Bear-Token': 'true' });
    this.http.post('api/page-views', { path, referer: document.referrer || null }, { headers })
      .subscribe({ error: () => {} });
  }

  ngOnDestroy(): void { this.sub.unsubscribe(); }
}
