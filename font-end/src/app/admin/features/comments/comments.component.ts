import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CommentAdminService } from '../../services/comment.admin.service';
import { AdminTableComponent } from '../../shared/admin-table.component';
import { AdminConfirmModalComponent } from '../../shared/admin-confirm-modal.component';
import { CommentItem, TableColumn } from '../../models/admin.model';
import { AdminMessage } from '../../../app.message';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';

@Component({
  selector: 'app-comments-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminTableComponent, AdminConfirmModalComponent, LanguagePipe],
  templateUrl: './comments.component.html',
  styleUrl: './comments.component.css'
})
export class CommentsAdminComponent implements OnInit {
  items: CommentItem[] = [];
  loading = false;
  showConfirm = false;
  selected: CommentItem | null = null;
  deleteItems: CommentItem[] = [];
  articleId = '';

  protected readonly AdminMessage = AdminMessage;

  readonly columns: TableColumn[] = [
    { key: 'text',        label: AdminMessage.LABEL_CONTENT,      type: 'truncate' },
    { key: 'articleId',   label: AdminMessage.LABEL_ARTICLE_ID,   type: 'truncate' },
    { key: 'parentId',    label: AdminMessage.LABEL_REPLY_ID,     type: 'text' },
    { key: 'createdDate', label: AdminMessage.LABEL_CREATED_DATE, type: 'date' },
  ];

  constructor(private svc: CommentAdminService) {}

  ngOnInit(): void {}

  search(): void {
    if (!this.articleId.trim()) return;
    this.loading = true;
    this.svc.getByArticle(this.articleId.trim()).subscribe({
      next: d => { this.items = d; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  openDelete(items: CommentItem[]): void { this.deleteItems = items; this.selected = items[0] ?? null; this.showConfirm = true; }

  delete(): void {
    this.svc.delete(this.deleteItems.map(i => i.id)).subscribe({
      next: () => { this.showConfirm = false; this.search(); },
      error: () => { this.showConfirm = false; }
    });
  }
}
