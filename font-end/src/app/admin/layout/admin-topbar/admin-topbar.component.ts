import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { Subscription, interval } from 'rxjs';
import { ContactNotificationService } from '../../services/contact-notification.service';
import { ContactAdminService } from '../../services/contact.admin.service';
import { ContactItem } from '../../models/admin.model';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';

@Component({
  selector: 'app-admin-topbar',
  standalone: true,
  imports: [CommonModule, RouterLink, LanguagePipe],
  templateUrl: './admin-topbar.component.html',
  styleUrl: './admin-topbar.component.css'
})
export class AdminTopbarComponent implements OnInit, OnDestroy {
  now = new Date();
  bellOpen = false;
  unreadList: ContactItem[] = [];
  unreadCount = 0;

  protected readonly AdminMessage = AdminMessage;

  private subs = new Subscription();

  constructor(
    private notifSvc: ContactNotificationService,
    private contactSvc: ContactAdminService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.subs.add(this.notifSvc.unreadList$.subscribe(list => {
      this.unreadList = list;
      this.unreadCount = list.length;
    }));

    // live clock
    this.subs.add(interval(1000).subscribe(() => this.now = new Date()));
  }

  ngOnDestroy(): void { this.subs.unsubscribe(); }

  toggleBell(): void { this.bellOpen = !this.bellOpen; }

  openContact(item: ContactItem): void {
    this.bellOpen = false;
    if (!item.isRead) {
      this.contactSvc.markAsRead(item.id).subscribe(() => {
        this.notifSvc.removeFromUnread(item.id);
      });
    }
    this.router.navigate(['/admin/contacts']);
  }

  closeBell(): void { this.bellOpen = false; }
}
