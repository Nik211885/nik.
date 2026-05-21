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

  // ── Bulk selection ──────────────────────────────────────────────────────────
  selected = new Set<string>();

  get allSelected(): boolean {
    return this.messages.length > 0 && this.messages.every(m => this.selected.has(m.id));
  }

  get someSelected(): boolean { return this.selected.size > 0; }

  constructor(private wallService: WallService) {}

  ngOnInit(): void {
    this.load();
    this.loadPendingCount();
  }

  load(): void {
    this.loading = true;
    this.selected.clear();
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

  // ── Single actions ──────────────────────────────────────────────────────────
  approve(id: string): void {
    this.wallService.updateStatus(id, 'Approved').subscribe(() => {
      this.load(); this.loadPendingCount();
    });
  }

  reject(id: string): void {
    this.wallService.updateStatus(id, 'Rejected').subscribe(() => {
      this.load(); this.loadPendingCount();
    });
  }

  delete(id: string): void {
    this.wallService.delete([id]).subscribe(() => {
      this.load(); this.loadPendingCount();
    });
  }

  // ── Selection ───────────────────────────────────────────────────────────────
  toggleSelect(id: string): void {
    if (this.selected.has(id)) this.selected.delete(id);
    else this.selected.add(id);
  }

  toggleAll(): void {
    if (this.allSelected) this.selected.clear();
    else this.messages.forEach(m => this.selected.add(m.id));
  }

  // ── Bulk actions ────────────────────────────────────────────────────────────
  bulkApprove(): void {
    const ids = [...this.selected];
    this.wallService.bulkUpdateStatus(ids, 'Approved').subscribe(() => {
      this.load(); this.loadPendingCount();
    });
  }

  bulkReject(): void {
    const ids = [...this.selected];
    this.wallService.bulkUpdateStatus(ids, 'Rejected').subscribe(() => {
      this.load(); this.loadPendingCount();
    });
  }

  bulkDelete(): void {
    const ids = [...this.selected];
    this.wallService.delete(ids).subscribe(() => {
      this.load(); this.loadPendingCount();
    });
  }

  // ── Helpers ─────────────────────────────────────────────────────────────────
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
