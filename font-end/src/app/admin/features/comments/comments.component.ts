import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CommentAdminService } from '../../services/comment.admin.service';
import { AdminTableComponent } from '../../shared/admin-table.component';
import { AdminConfirmModalComponent } from '../../shared/admin-confirm-modal.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { CommentItem, TableColumn } from '../../models/admin.model';
import { AdminMessage } from '../../../app.message';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';

@Component({
  selector: 'app-comments-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminTableComponent, AdminConfirmModalComponent, LanguagePipe, PaginationComponent],
  templateUrl: './comments.component.html',
  styleUrl: './comments.component.css'
})
export class CommentsAdminComponent implements OnInit {
  items: CommentItem[] = [];
  loading = false;
  currentPage = 1;
  pageCount = 0;
  pageSize = 20;

  showConfirm = false;
  selected: CommentItem | null = null;
  deleteItems: CommentItem[] = [];
  articleId = '';

  protected readonly AdminMessage = AdminMessage;

  readonly columns: TableColumn[] = [
    { key: 'authorName',  label: AdminMessage.LABEL_AUTHOR,       type: 'text' },
    { key: 'text',        label: AdminMessage.LABEL_CONTENT,      type: 'truncate' },
    { key: 'articleId',   label: AdminMessage.LABEL_ARTICLE_ID,   type: 'truncate' },
    { key: 'parentId',    label: AdminMessage.LABEL_REPLY_ID,     type: 'text' },
    { key: 'createdDate', label: AdminMessage.LABEL_CREATED_DATE, type: 'date' },
  ];

  constructor(private svc: CommentAdminService) {}

  ngOnInit(): void {
    this.load(1);
  }

  load(page: number): void {
    this.currentPage = page;
    this.loading = true;
    const filter = this.articleId.trim() || undefined;
    this.svc.getPage(page, this.pageSize, filter).subscribe({
      next: res => {
        this.items = res.data;
        this.currentPage = res.pageNumber;
        this.pageCount = res.pageCount;
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  search(): void {
    this.load(1);
  }

  openDelete(items: CommentItem[]): void {
    this.deleteItems = items;
    this.selected = items[0] ?? null;
    this.showConfirm = true;
  }

  delete(): void {
    this.svc.delete(this.deleteItems.map(i => i.id)).subscribe({
      next: () => { this.showConfirm = false; this.load(this.currentPage); },
      error: () => { this.showConfirm = false; }
    });
  }
}
