import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApplicationTitle } from '../../app.message';
import { LanguagePipe } from '../../shared/pipes/language.pipe';
import { WallService, WallMessageModel } from '../../core/services/wall.service';
import { AppDatePipe } from '../../shared/pipes/app-date.pipe';

const DEVICE_ID_KEY = 'wall_device_id';
const REACTED_IDS_KEY = 'wall_reacted_ids';

function getOrCreateDeviceId(): string {
  let id = localStorage.getItem(DEVICE_ID_KEY);
  if (!id) {
    id = crypto.randomUUID();
    localStorage.setItem(DEVICE_ID_KEY, id);
  }
  return id;
}

const TAPE_COLORS = [
  '#f7c948', '#f09fca', '#8fe5b4', '#9fc3f7',
  '#f7a06e', '#b39ff7', '#6ec8f7', '#f4b3a3',
];

const PAPER_TINTS = [
  '#fdfaf3', '#fdfaf3', // cream (weighted)
  '#fcfcf8',            // cool white
  '#fef9e7',            // pale yellow
  '#f9fdf9',            // pale mint
  '#fdf8fc',            // pale lavender
];

@Component({
  selector: 'app-wall',
  standalone: true,
  imports: [LanguagePipe, FormsModule, AppDatePipe],
  templateUrl: './wall.component.html',
  styleUrl: './wall.component.css',
})
export class WallComponent implements OnInit {
  protected readonly ApplicationTitle = ApplicationTitle;
  readonly MAX_CHARS = 300;

  messages: WallMessageModel[] = [];
  loading = true;
  name = '';
  text = '';
  source = '';
  submitting = false;
  submitResult: 'pending' | 'approved' | null = null;

  private deviceId = getOrCreateDeviceId();
  private reactedIds = new Set<string>(
    JSON.parse(localStorage.getItem(REACTED_IDS_KEY) ?? '[]') as string[]
  );

  constructor(private wallService: WallService) {}

  ngOnInit(): void {
    this.wallService.getApproved().subscribe({
      next: msgs => { this.messages = msgs; this.loading = false; },
      error: ()   => { this.loading = false; },
    });
  }

  get charsLeft(): number { return this.MAX_CHARS - this.text.length; }

  submit(): void {
    if (!this.name.trim() || !this.text.trim() || this.submitting) return;
    this.submitting = true;
    this.wallService.create(this.name.trim(), this.text.trim(), this.source.trim() || undefined).subscribe({
      next: (res) => {
        this.submitting = false;
        this.submitResult = res.status === 'Approved' ? 'approved' : 'pending';
        if (res.status === 'Approved') this.messages.unshift(res);
        this.name = ''; this.text = ''; this.source = '';
        setTimeout(() => (this.submitResult = null), 5000);
      },
      error: () => (this.submitting = false),
    });
  }

  hasReacted(id: string): boolean {
    return this.reactedIds.has(id);
  }

  react(id: string): void {
    this.wallService.react(id, this.deviceId).subscribe({
      next: (res) => {
        const msg = this.messages.find(m => m.id === id);
        if (msg) msg.reactionCount = res.reactionCount;
        if (res.reacted) {
          this.reactedIds.add(id);
        } else {
          this.reactedIds.delete(id);
        }
        localStorage.setItem(REACTED_IDS_KEY, JSON.stringify([...this.reactedIds]));
      },
    });
  }

  tapeColor(id: string): string {
    return TAPE_COLORS[this.hash(id) % TAPE_COLORS.length];
  }

  tapeAngle(id: string): string {
    const deg = (this.hash(id + 't') % 7) - 3;
    return `translateX(-50%) rotate(${deg}deg)`;
  }

  paperColor(id: string): string {
    return PAPER_TINTS[this.hash(id + 'c') % PAPER_TINTS.length];
  }

  noteRotate(id: string): string {
    const deg = (this.hash(id + 'r') % 19) - 9;
    return `rotate(${deg}deg)`;
  }

  private hash(s: string): number {
    let h = 0;
    for (let i = 0; i < s.length; i++) h = s.charCodeAt(i) + ((h << 5) - h);
    return Math.abs(h);
  }
}
