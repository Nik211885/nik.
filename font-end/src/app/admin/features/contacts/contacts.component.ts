import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ContactAdminService } from '../../services/contact.admin.service';
import { AdminTableComponent } from '../../shared/admin-table.component';
import { AdminConfirmModalComponent } from '../../shared/admin-confirm-modal.component';
import { ContactItem, TableColumn } from '../../models/admin.model';
import { AdminMessage } from '../../../app.message';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-contacts-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminTableComponent, AdminConfirmModalComponent, LanguagePipe, PaginationComponent],
  templateUrl: './contacts.component.html',
  styleUrl: './contacts.component.css'
})
export class ContactsAdminComponent implements OnInit {
  items: ContactItem[] = [];
  loading = false;
  currentPage = 1;
  pageCount = 0;
  pageSize = 15;

  showConfirm = false;
  deleteItems: ContactItem[] = [];
  selected: ContactItem | null = null;

  detailItem: ContactItem | null = null;

  protected readonly AdminMessage = AdminMessage;

  readonly columns: TableColumn[] = [
    { key: 'name',        label: AdminMessage.CONTACTS_LABEL_NAME,    type: 'text' },
    { key: 'email',       label: AdminMessage.CONTACTS_LABEL_EMAIL,   type: 'text' },
    { key: 'subject',     label: AdminMessage.CONTACTS_LABEL_SUBJECT, type: 'truncate' },
    { key: 'message',     label: AdminMessage.CONTACTS_LABEL_MESSAGE, type: 'truncate' },
    { key: 'createdDate', label: AdminMessage.LABEL_CREATED_DATE,     type: 'date' },
  ];

  constructor(private svc: ContactAdminService) {}

  ngOnInit(): void {
    this.load(1);
  }

  load(page: number): void {
    this.currentPage = page;
    this.loading = true;
    this.svc.getPage(page, this.pageSize).subscribe({
      next: res => {
        this.items = res.data;
        this.currentPage = res.pageNumber;
        this.pageCount = res.pageCount;
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  openDetail(item: ContactItem): void {
    this.detailItem = item;
    if (!item.isRead) {
      this.svc.markAsRead(item.id).subscribe(updated => {
        const idx = this.items.findIndex(i => i.id === updated.id);
        if (idx !== -1) this.items[idx] = updated;
        if (this.detailItem?.id === updated.id) this.detailItem = updated;
      });
    }
  }

  closeDetail(): void {
    this.detailItem = null;
  }

  openDelete(items: ContactItem[]): void {
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
