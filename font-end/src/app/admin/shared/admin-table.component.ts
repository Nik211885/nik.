import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableColumn } from '../models/admin.model';
import {AdminMessage} from "../../app.message"
import { LanguagePipe } from '../../shared/pipes/language.pipe';

@Component({
  selector: 'app-admin-table',
  standalone: true,
  imports: [CommonModule, LanguagePipe],
  template: `
    <div class="table-responsive">
      <table class="table table-hover align-middle mb-0 border">
        <thead class="table-dark">
          <tr>
            @for (col of columns; track $index) {
              <th class="fw-semibold">{{ col.label | language }}</th>
            }
            @if(showActions){
               <th style="width:110px" class="fw-semibold text-center">{{AdminMessage.TABLE_ACTION | language}}</th>
            }
          </tr>
        </thead>
        <tbody>
          @if(loading){
            <tr>
              <td [colSpan]="columns.length + (showActions ? 1 : 0)" class="text-center py-5">
                <div class="spinner-border text-primary" role="status">
                  <span class="visually-hidden">{{AdminMessage.TABLE_LOADING | language}}</span>
                </div>
              </td>
            </tr>
          }
          @if(!loading && data.length === 0){
            <tr>
              <td [colSpan]="columns.length + (showActions ? 1 : 0)" class="text-center py-5 text-muted">
                <i class="bi bi-inbox fs-3 d-block mb-2"></i>
                {{AdminMessage.TABLE_EMPTY_DATA | language}}
              </td>
            </tr>
          }
          @for (row of data; track $index) {
             <tr>
              @for (col of columns; track $index){
                <td>
                  @switch (col.type) {

                    @case ('image') {
                      <img
                        [src]="row[col.key]"
                        style="width:48px;height:48px;object-fit:cover;border-radius:6px;border:1px solid #dee2e6"
                        onerror="this.style.display='none'">
                    }

                    @case ('date') {
                      <small class="text-muted">
                        {{ formatDate(row[col.key]) }}
                      </small>
                    }

                    @case ('number') {
                      <span class="badge bg-secondary">
                        {{ row[col.key] ?? 0 }}
                      </span>
                    }

                    @case ('truncate') {
                      <span
                        [title]="row[col.key]"
                        class="d-inline-block"
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
              @if(showActions){
                <td class="text-center">
                  <button class="btn btn-sm btn-outline-warning me-1" (click)="editClick.emit(row)" [title]="AdminMessage.TABLE_ACTION_EDIT | language">
                    <i class="bi bi-pencil-square"></i>
                  </button>
                  <button class="btn btn-sm btn-outline-danger" (click)="deleteClick.emit(row)"  [title]="AdminMessage.TABLE_ACTION_DELETE | language">
                    <i class="bi bi-trash3"></i>
                  </button>
                </td>
              }
            </tr>
          }
        </tbody>
      </table>
    </div>
  `
})
export class AdminTableComponent {
  @Input() columns: TableColumn[] = [];
  @Input() data: any[] = [];
  @Input() loading = false;
  @Input() showActions = true;
  @Output() editClick = new EventEmitter<any>();
  @Output() deleteClick = new EventEmitter<any>();
  protected readonly AdminMessage = AdminMessage;

  formatDate(val: string): string {
    if (!val) return '—';
    return new Date(val).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });
  }
}
