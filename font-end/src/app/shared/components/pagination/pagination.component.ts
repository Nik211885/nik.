import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
  selector: 'app-pagination',
  imports: [],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.css',
})
export class PaginationComponent {
  private _currentPage = 1;
  private _pageCount = 0;

  pages: number[] = [];

  @Input() set currentPage(val: number) {
    this._currentPage = val;
    this.pages = this.buildPages();
  }
  get currentPage() { return this._currentPage; }

  @Input() set pageCount(val: number) {
    this._pageCount = val;
    this.pages = this.buildPages();
  }
  get pageCount() { return this._pageCount; }

  @Output() pageChange = new EventEmitter<number>();

  go(page: number): void {
    if (page >= 1 && page <= this._pageCount && page !== this._currentPage) {
      this.currentPage = page;
      this.pageChange.emit(page);
    }
  }

  private buildPages(): number[] {
    const total = this._pageCount;
    if (total <= 7) return Array.from({length: total}, (_, i) => i + 1);
    const cur = this._currentPage;
    const set = new Set([1, total, cur, cur - 1, cur + 1].filter(p => p >= 1 && p <= total));
    return Array.from(set).sort((a, b) => a - b);
  }
}
