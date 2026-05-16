import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserAdminService } from '../../services/user.admin.service';
import { AdminTableComponent } from '../../shared/admin-table.component';
import { TableColumn, UserItem } from '../../models/admin.model';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-users-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminTableComponent, LanguagePipe],
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersAdminComponent implements OnInit {
  items: UserItem[] = [];
  loading = false;
  showModal = false;
  selected: UserItem | null = null;
  saving = false;
  error = '';

  form = { userName: '', email: '', phone: '', bio: '' };

  protected readonly AdminMessage = AdminMessage;

  readonly columns: TableColumn[] = [
    { key: 'userName',    label: AdminMessage.LABEL_USERNAME,     type: 'text' },
    { key: 'email',       label: AdminMessage.LABEL_EMAIL,        type: 'text' },
    { key: 'phone',       label: AdminMessage.LABEL_PHONE,        type: 'text' },
    { key: 'bio',         label: AdminMessage.LABEL_BIO,          type: 'truncate' },
    { key: 'slug',        label: AdminMessage.LABEL_SLUG,         type: 'text' },
    { key: 'createdDate', label: AdminMessage.LABEL_CREATED_DATE, type: 'date' },
  ];

  constructor(private svc: UserAdminService, private toast: ToastService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.svc.getAll().subscribe({ next: d => { this.items = d; this.loading = false; }, error: () => { this.loading = false; } });
  }

  openEdit(item: UserItem): void {
    this.selected = item;
    this.form = { userName: item.userName, email: item.email ?? '', phone: item.phone ?? '', bio: item.bio };
    this.error = '';
    this.showModal = true;
  }

  save(): void {
    this.saving = true;
    this.svc.update(this.selected!.id, this.form).subscribe({
      next: () => { this.showModal = false; this.saving = false; this.load(); this.toast.success(AdminMessage.TOAST_SAVE_SUCCESS); },
      error: () => { this.error = AdminMessage.ERROR_GENERIC; this.saving = false; this.toast.error(AdminMessage.TOAST_SAVE_ERROR); }
    });
  }
}
