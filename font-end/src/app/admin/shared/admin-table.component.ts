import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableColumn } from '../models/admin.model';
import { AdminMessage } from '../../app.message';
import { LanguagePipe } from '../../shared/pipes/language.pipe';

@Component({
  selector: 'app-admin-table',
  standalone: true,
  imports: [CommonModule, FormsModule, LanguagePipe],
  styles: [`
    .sort-icon { font-size:.7rem; opacity:.4; transition:opacity .15s; }
    th:hover .sort-icon { opacity:.8; }
    .sort-icon.active { opacity:1; color:#90caf9; }

    /* checkbox column */
    .cb-col {
      width:40px; min-width:40px;
      padding:0 !important;
      vertical-align:middle;
    }

    /* compact filter row */
    .filter-row th {
      background:#f8f9fa; padding:.25rem .5rem;
    }
    .filter-row input.col-filter {
      font-size:.75rem; padding:.2rem .4rem; height:26px;
      border:1px solid #ced4da; border-radius:4px;
      background:#fff; width:100%; outline:none;
      transition:border-color .15s, box-shadow .15s;
    }
    .filter-row input.col-filter:focus {
      border-color:#86b7fe; box-shadow:0 0 0 2px rgba(13,110,253,.15);
    }

    /* action bar */
    .action-bar {
      display:flex; align-items:center; gap:.75rem;
      border:1px solid #dee2e6; border-radius:8px;
      padding:.35rem .85rem; margin-bottom:.75rem;
      background:#f8f9fa; transition:background .15s, border-color .15s;
      min-height:38px;
    }
    .action-bar.has-selection { background:#fff3cd; border-color:#ffc107; }
    .action-bar .sel-count { font-size:.8rem; color:#6c757d; min-width:5ch; }
    .action-bar.has-selection .sel-count { color:#856404; font-weight:600; }
    .action-bar .action-btn {
      width:30px; height:30px; padding:0;
      display:inline-flex; align-items:center; justify-content:center;
      border-radius:6px; transition:opacity .15s;
    }
    .action-bar .action-btn:disabled { opacity:.28; cursor:not-allowed; }

    .page-link { cursor:pointer; }
    tr.row-selected { background:rgba(13,110,253,.06) !important; }
  `],
  template: `
    <!-- Action bar — always visible, buttons dim when not applicable -->
    <div class="action-bar" [class.has-selection]="selectedKeys.size > 0">
      <span class="sel-count">{{ selectedKeys.size }} {{ AdminMessage.TABLE_SELECTED | language }}</span>
      <div class="d-flex gap-1 ms-auto">
        <button class="btn btn-sm btn-outline-secondary action-btn"
                [disabled]="selectedKeys.size === 0"
                [title]="AdminMessage.TABLE_DESELECT | language"
                (click)="clearSelection()">
          <i class="bi bi-x-lg"></i>
        </button>
        @if (showActions) {
          <button class="btn btn-sm btn-outline-warning action-btn"
                  [disabled]="selectedKeys.size !== 1"
                  [title]="AdminMessage.TABLE_ACTION_EDIT | language"
                  (click)="emitEdit()">
            <i class="bi bi-pencil"></i>
          </button>
          @if (!hideDelete) {
            <button class="btn btn-sm btn-danger action-btn"
                    [disabled]="selectedKeys.size === 0"
                    [title]="AdminMessage.TABLE_ACTION_DELETE | language"
                    (click)="emitDelete()">
              <i class="bi bi-trash3"></i>
            </button>
          }
        }
      </div>
    </div>

    <!-- Toolbar: count + page-size -->
    <div class="d-flex justify-content-between align-items-center mb-2">
      <small class="text-muted">
        @if (!loading) {
          @if (filteredData.length === 0) { 0 } @else { {{ fromIndex }}–{{ toIndex }} }
          &nbsp;/&nbsp;{{ filteredData.length }}
          @if (filteredData.length !== data.length) {
            <span class="ms-1 text-secondary">({{ data.length }})</span>
          }
        }
      </small>
      <div class="d-flex align-items-center gap-2">
        <small class="text-muted">{{ AdminMessage.TABLE_PER_PAGE | language }}</small>
        <select class="form-select form-select-sm" style="width:80px"
                [(ngModel)]="pageSize" (ngModelChange)="onPageSizeChange()">
          @for (opt of pageSizeOptions; track opt) {
            <option [value]="opt">{{ opt }}</option>
          }
        </select>
      </div>
    </div>

    <!-- Table (horizontal scroll) -->
    <div style="overflow-x:auto">
      <table class="table table-hover align-middle mb-0 border" style="min-width:600px">
        <thead class="table-dark">

          <!-- Sort header row -->
          <tr>
            <th class="cb-col">
              <div style="display:flex;justify-content:center;align-items:center;width:100%;height:100%">
                <input type="checkbox" class="form-check-input" style="float:none;margin:0"
                       [checked]="allSelected"
                       [indeterminate]="someSelected"
                       (change)="toggleAll()">
              </div>
            </th>
            @for (col of columns; track $index) {
              <th class="fw-semibold"
                  [style.cursor]="isSortable(col) ? 'pointer' : 'default'"
                  (click)="isSortable(col) && toggleSort(col.key)">
                <div class="d-flex align-items-center gap-1 text-nowrap">
                  {{ col.label | language }}
                  @if (isSortable(col)) {
                    <i class="bi sort-icon"
                       [class.active]="sortKey === col.key"
                       [class.bi-arrow-down-up]="sortKey !== col.key"
                       [class.bi-arrow-up]="sortKey === col.key && sortDir === 'asc'"
                       [class.bi-arrow-down]="sortKey === col.key && sortDir === 'desc'"></i>
                  }
                </div>
              </th>
            }
          </tr>

          <!-- Per-column filter row -->
          <tr class="filter-row">
            <th class="cb-col"></th>
            @for (col of columns; track $index) {
              <th>
                @if (col.type !== 'image') {
                  <input class="col-filter"
                         [(ngModel)]="filterMap[col.key]"
                         (ngModelChange)="onFilterChange()">
                }
              </th>
            }
          </tr>

        </thead>
        <tbody>
          @if (loading) {
            <tr>
              <td [colSpan]="columns.length + 1" class="text-center py-5">
                <div class="spinner-border text-primary" role="status">
                  <span class="visually-hidden">{{ AdminMessage.TABLE_LOADING | language }}</span>
                </div>
              </td>
            </tr>
          }
          @if (!loading && pagedData.length === 0) {
            <tr>
              <td [colSpan]="columns.length + 1" class="text-center py-5 text-muted">
                <i class="bi bi-inbox fs-3 d-block mb-2"></i>
                {{ AdminMessage.TABLE_EMPTY_DATA | language }}
              </td>
            </tr>
          }
          @for (row of pagedData; track $index) {
            <tr [class.row-selected]="isSelected(row)" style="cursor:pointer" (click)="toggleRow(row)">
              <td class="cb-col" (click)="$event.stopPropagation()">
                <div style="display:flex;justify-content:center;align-items:center;width:100%;height:100%">
                  <input type="checkbox" class="form-check-input" style="float:none;margin:0;cursor:pointer"
                         [checked]="isSelected(row)"
                         (change)="toggleRow(row)">
                </div>
              </td>
              @for (col of columns; track $index) {
                <td>
                  @switch (col.type) {
                    @case ('image') {
                      <img [src]="row[col.key]"
                           style="width:48px;height:48px;object-fit:cover;border-radius:6px;border:1px solid #dee2e6"
                           onerror="this.style.display='none'">
                    }
                    @case ('date') {
                      <small class="text-muted">{{ formatDate(row[col.key]) }}</small>
                    }
                    @case ('number') {
                      <span class="badge bg-secondary">{{ row[col.key] ?? 0 }}</span>
                    }
                    @case ('truncate') {
                      <span [title]="row[col.key]" class="d-inline-block"
                            style="max-width:280px;overflow:hidden;text-overflow:ellipsis;white-space:nowrap">
                        {{ row[col.key] }}
                      </span>
                    }
                    @default {
                      <span>{{ row[col.key] }}</span>
                    }
                  }
                </td>
              }
            </tr>
          }
        </tbody>
      </table>
    </div>

    <!-- Pagination -->
    @if (!loading && pageCount > 1) {
      <div class="d-flex justify-content-center mt-3">
        <nav>
          <ul class="pagination pagination-sm mb-0">
            <li class="page-item" [class.disabled]="pageNum === 1">
              <button class="page-link" (click)="goToPage(pageNum - 1)">&laquo;</button>
            </li>
            @for (p of pageRange; track p) {
              <li class="page-item" [class.active]="p === pageNum">
                <button class="page-link" (click)="goToPage(p)">{{ p }}</button>
              </li>
            }
            <li class="page-item" [class.disabled]="pageNum === pageCount">
              <button class="page-link" (click)="goToPage(pageNum + 1)">&raquo;</button>
            </li>
          </ul>
        </nav>
      </div>
    }
  `
})
export class AdminTableComponent implements OnChanges {
  @Input() columns: TableColumn[] = [];
  @Input() data: any[] = [];
  @Input() loading = false;
  @Input() showActions = true;
  @Input() hideDelete = false;
  @Input() rowKey = 'id';
  @Output() editClick = new EventEmitter<any>();
  @Output() deleteClick = new EventEmitter<any[]>();
  protected readonly AdminMessage = AdminMessage;

  filterMap: { [key: string]: string } = {};
  sortKey = '';
  sortDir: 'asc' | 'desc' = 'asc';
  pageNum = 1;
  pageSize = 10;
  readonly pageSizeOptions = [10, 25, 50, 100];
  selectedKeys = new Set<any>();

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['data']) {
      this.pageNum = 1;
      this.selectedKeys = new Set();
    }
  }

  // ── Filter / sort / page ───────────────────────────────────────────────

  get filteredData(): any[] {
    let result = this.data;
    for (const [key, val] of Object.entries(this.filterMap)) {
      if (!val?.trim()) continue;
      const lower = val.toLowerCase();
      result = result.filter(row => String(row[key] ?? '').toLowerCase().includes(lower));
    }
    if (this.sortKey) {
      result = [...result].sort((a, b) => {
        const av = String(a[this.sortKey] ?? '');
        const bv = String(b[this.sortKey] ?? '');
        const cmp = av.localeCompare(bv, undefined, { numeric: true, sensitivity: 'base' });
        return this.sortDir === 'asc' ? cmp : -cmp;
      });
    }
    return result;
  }

  get pageCount(): number {
    return Math.max(1, Math.ceil(this.filteredData.length / this.pageSize));
  }

  get pagedData(): any[] {
    const start = (this.pageNum - 1) * this.pageSize;
    return this.filteredData.slice(start, start + this.pageSize);
  }

  get fromIndex(): number { return (this.pageNum - 1) * this.pageSize + 1; }
  get toIndex(): number { return Math.min(this.pageNum * this.pageSize, this.filteredData.length); }

  get pageRange(): number[] {
    const max = 7, half = Math.floor(max / 2);
    let start = Math.max(1, this.pageNum - half);
    let end = Math.min(this.pageCount, start + max - 1);
    if (end - start < max - 1) start = Math.max(1, end - max + 1);
    return Array.from({ length: end - start + 1 }, (_, i) => start + i);
  }

  toggleSort(key: string): void {
    if (this.sortKey === key) this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc';
    else { this.sortKey = key; this.sortDir = 'asc'; }
    this.pageNum = 1;
  }

  onFilterChange(): void { this.pageNum = 1; }
  onPageSizeChange(): void { this.pageNum = 1; }

  goToPage(p: number): void {
    const c = Math.max(1, Math.min(p, this.pageCount));
    if (c !== this.pageNum) this.pageNum = c;
  }

  isSortable(col: TableColumn): boolean { return col.type !== 'image'; }

  formatDate(val: string): string {
    if (!val) return '—';
    return new Date(val).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });
  }

  // ── Selection ──────────────────────────────────────────────────────────

  isSelected(row: any): boolean { return this.selectedKeys.has(row[this.rowKey]); }

  get allSelected(): boolean {
    return this.pagedData.length > 0 && this.pagedData.every(r => this.selectedKeys.has(r[this.rowKey]));
  }

  get someSelected(): boolean {
    return !this.allSelected && this.pagedData.some(r => this.selectedKeys.has(r[this.rowKey]));
  }

  toggleRow(row: any): void {
    const key = row[this.rowKey];
    const next = new Set(this.selectedKeys);
    next.has(key) ? next.delete(key) : next.add(key);
    this.selectedKeys = next;
  }

  toggleAll(): void {
    const next = new Set(this.selectedKeys);
    if (this.allSelected) this.pagedData.forEach(r => next.delete(r[this.rowKey]));
    else this.pagedData.forEach(r => next.add(r[this.rowKey]));
    this.selectedKeys = next;
  }

  clearSelection(): void { this.selectedKeys = new Set(); }

  get selectedRows(): any[] {
    return this.filteredData.filter(r => this.selectedKeys.has(r[this.rowKey]));
  }

  emitEdit(): void {
    const rows = this.selectedRows;
    if (rows.length === 1) this.editClick.emit(rows[0]);
  }

  emitDelete(): void {
    const rows = this.selectedRows;
    if (rows.length > 0) this.deleteClick.emit(rows);
  }
}
