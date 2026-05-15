import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { ContactAdminService } from './contact.admin.service';
import { ContactItem } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class ContactNotificationService implements OnDestroy {
  private readonly unreadSubject = new BehaviorSubject<ContactItem[]>([]);

  readonly unreadList$ = this.unreadSubject.asObservable();
  readonly unreadCount$ = this.unreadList$.pipe(map(list => list.length));

  private intervalId: ReturnType<typeof setInterval> | null = null;

  constructor(private contactSvc: ContactAdminService) {
    this.fetch();
    this.intervalId = setInterval(() => this.fetch(), 60_000);
  }

  fetch(): void {
    this.contactSvc.getUnread().subscribe({
      next: list => this.unreadSubject.next(list),
      error: () => {}
    });
  }

  /** Call after marking an item read so the badge updates immediately. */
  removeFromUnread(id: string): void {
    this.unreadSubject.next(this.unreadSubject.getValue().filter(c => c.id !== id));
  }

  ngOnDestroy(): void {
    if (this.intervalId) clearInterval(this.intervalId);
  }
}
