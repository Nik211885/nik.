import { Component, OnInit } from '@angular/core';
import { AdminMessage } from '../../../app.message';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { WallService } from '../../../core/services/wall.service';
import { AppDatePipe } from '../../../shared/pipes/app-date.pipe';

const AVATAR_COLORS = [
  '#f87171', '#fb923c', '#fbbf24', '#34d399',
  '#38bdf8', '#818cf8', '#e879f9', '#f472b6',
];

@Component({
  selector: 'app-wall-messages-admin',
  standalone: true,
  imports: [LanguagePipe, AppDatePipe],
  templateUrl: './wall-messages.component.html',
  styleUrl: './wall-messages.component.css',
})
export class WallMessagesAdminComponent implements OnInit {
  protected readonly AdminMessage = AdminMessage;

  tabs = [
    { label: AdminMessage.WALL_MESSAGES_STATUS_ALL,      value: '' },
    { label: AdminMessage.WALL_MESSAGES_STATUS_PENDING,  value: 'Pending' },
    { label: AdminMessage.WALL_MESSAGES_STATUS_APPROVED, value: 'Approved' },
    { label: AdminMessage.WALL_MESSAGES_STATUS_REJECTED, value: 'Rejected' },
  ];

  messages: any[] = [];
  total = 0;
  page = 1;
  pageSize = 20;
  statusFilter = '';
  loading = false;
  pendingCount = 0;

  constructor(private wallService: WallService) {}

  ngOnInit(): void {
    this.load();
    this.loadPendingCount();
  }

  load(): void {
    this.loading = true;
    this.wallService.getAll(this.page, this.pageSize, this.statusFilter || undefined).subscribe({
      next: (res) => {
        this.messages = res.data;
        this.total = res.totalItems;
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }

  private loadPendingCount(): void {
    this.wallService.getAll(1, 1, 'Pending').subscribe({
      next: (res) => (this.pendingCount = res.totalItems),
    });
  }

  setFilter(status: string): void {
    this.statusFilter = status;
    this.page = 1;
    this.load();
  }

  approve(id: string): void {
    this.wallService.updateStatus(id, 'Approved').subscribe(() => {
      this.load();
      this.loadPendingCount();
    });
  }

  reject(id: string): void {
    this.wallService.updateStatus(id, 'Rejected').subscribe(() => {
      this.load();
      this.loadPendingCount();
    });
  }

  delete(id: string): void {
    this.wallService.delete([id]).subscribe(() => {
      this.load();
      this.loadPendingCount();
    });
  }

  avatarColor(name: string): string {
    return AVATAR_COLORS[this.hash(name) % AVATAR_COLORS.length];
  }

  initial(name: string): string {
    return (name?.trim()[0] ?? '?').toUpperCase();
  }

  private hash(s: string): number {
    let h = 0;
    for (let i = 0; i < s.length; i++) h = s.charCodeAt(i) + ((h << 5) - h);
    return Math.abs(h);
  }
}
