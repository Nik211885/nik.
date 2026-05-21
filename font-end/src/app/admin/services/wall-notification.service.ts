import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, interval, Subscription } from 'rxjs';
import { startWith, switchMap } from 'rxjs/operators';
import { WallService } from '../../core/services/wall.service';

@Injectable({ providedIn: 'root' })
export class WallNotificationService implements OnDestroy {
  private pendingSubject = new BehaviorSubject<number>(0);
  readonly pendingCount$ = this.pendingSubject.asObservable();

  private lastCount = -1;
  private sub = new Subscription();

  constructor(private wallService: WallService) {
    this.startPolling();
  }

  private startPolling(): void {
    this.sub.add(
      interval(30_000).pipe(
        startWith(0),
        switchMap(() => this.wallService.getAll(1, 1, 'Pending'))
      ).subscribe({
        next: (res) => {
          const count: number = res.totalItems ?? 0;
          this.pendingSubject.next(count);
          if (this.lastCount >= 0 && count > this.lastCount) {
            this.notify(count);
          }
          this.lastCount = count;
        },
        error: () => {}
      })
    );
  }

  private notify(count: number): void {
    if (!('Notification' in window)) return;
    const show = () =>
      new Notification('Wall Messages', {
        body: `${count} pending message${count > 1 ? 's' : ''} waiting for review`,
        icon: '/favicon.ico',
      });

    if (Notification.permission === 'granted') {
      show();
    } else if (Notification.permission !== 'denied') {
      Notification.requestPermission().then(p => { if (p === 'granted') show(); });
    }
  }

  ngOnDestroy(): void { this.sub.unsubscribe(); }
}
