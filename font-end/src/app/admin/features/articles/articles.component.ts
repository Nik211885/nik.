import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { QuillModule } from 'ngx-quill';
import { ArticleAdminService } from '../../services/article.admin.service';
import { CategoryAdminService } from '../../services/category.admin.service';
import { TagAdminService } from '../../services/tag.admin.service';
import { AdminTableComponent } from '../../shared/admin-table.component';
import { AdminConfirmModalComponent } from '../../shared/admin-confirm-modal.component';
import { CloudinaryUploadComponent } from '../../shared/cloudinary-upload/cloudinary-upload.component';
import { ArticleItem, CategoryItem, PaginationResponse, TableColumn, TagItem } from '../../models/admin.model';
import { forkJoin } from 'rxjs';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';

@Component({
  selector: 'app-articles-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, QuillModule, AdminTableComponent, AdminConfirmModalComponent, CloudinaryUploadComponent, LanguagePipe],
  templateUrl: './articles.component.html',
  styleUrl: './articles.component.css'
})
export class ArticlesAdminComponent implements OnInit {
  protected readonly AdminMessage = AdminMessage;

  readonly editorModules = {
    toolbar: [
      [{ header: [1, 2, 3, 4, false] }],
      ['bold', 'italic', 'underline', 'strike'],
      [{ list: 'ordered' }, { list: 'bullet' }],
      ['blockquote', 'code-block'],
      ['link', 'image'],
      [{ align: [] }],
      ['clean'],
    ],
  };

  page: PaginationResponse<ArticleItem> = { data: [], totalItems: 0, pageNumber: 1, pageSize: 20, pageCount: 0 };
  categories: CategoryItem[] = [];
  tags: TagItem[] = [];
  loading = false;
  showModal = false;
  showConfirm = false;
  isEditing = false;
  selected: ArticleItem | null = null;
  saving = false;
  error = '';

  form = { title: '', description: '', content: '', image: '', tagIds: [] as string[], categoryIds: [] as string[] };

  readonly columns: TableColumn[] = [
    { key: 'image',          label: AdminMessage.LABEL_IMAGE,         type: 'image' },
    { key: 'title',          label: AdminMessage.LABEL_TITLE,         type: 'truncate' },
    { key: 'slug',           label: AdminMessage.LABEL_SLUG,          type: 'truncate' },
    { key: 'countSee',       label: AdminMessage.LABEL_VIEWS,         type: 'number' },
    { key: 'countLikeRef',   label: AdminMessage.LABEL_LIKE,          type: 'number' },
    { key: 'countCommentRef',label: AdminMessage.LABEL_COMMENT_COUNT, type: 'number' },
    { key: 'createdDate',    label: AdminMessage.LABEL_CREATED_DATE,  type: 'date' },
  ];

  constructor(
    private svc: ArticleAdminService,
    private catSvc: CategoryAdminService,
    private tagSvc: TagAdminService
  ) {}

  ngOnInit(): void { this.loadAll(); }

  loadAll(p = 1): void {
    this.loading = true;
    forkJoin({
      articles:   this.svc.getAll(p, 20),
      categories: this.catSvc.getAll(),
      tags:       this.tagSvc.getAll()
    }).subscribe({
      next: ({ articles, categories, tags }) => {
        this.page = articles;
        this.categories = categories;
        this.tags = tags;
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  openCreate(): void {
    this.isEditing = false;
    this.form = { title: '', description: '', content: '', image: '', tagIds: [], categoryIds: [] };
    this.error = '';
    this.showModal = true;
  }

  openEdit(item: ArticleItem): void {
    this.isEditing = true;
    this.selected = item;
    this.form = {
      title:       item.title,
      description: item.description,
      content:     item.content,
      image:       item.image,
      tagIds:      item.tags.map(t => t.id),
      categoryIds: item.categories.map(c => c.id)
    };
    this.error = '';
    this.showModal = true;
  }

  openDelete(item: ArticleItem): void { this.selected = item; this.showConfirm = true; }

  toggleTag(id: string): void {
    const idx = this.form.tagIds.indexOf(id);
    idx >= 0 ? this.form.tagIds.splice(idx, 1) : this.form.tagIds.push(id);
  }

  toggleCategory(id: string): void {
    const idx = this.form.categoryIds.indexOf(id);
    idx >= 0 ? this.form.categoryIds.splice(idx, 1) : this.form.categoryIds.push(id);
  }

  save(): void {
    this.saving = true;
    const obs = this.isEditing
      ? this.svc.update({ id: this.selected!.id, ...this.form })
      : this.svc.create(this.form);

    obs.subscribe({
      next: () => { this.showModal = false; this.saving = false; this.loadAll(this.page.pageNumber); },
      error: () => { this.error = AdminMessage.ERROR_GENERIC; this.saving = false; }
    });
  }

  delete(): void {
    this.svc.delete([this.selected!.id]).subscribe({
      next: () => { this.showConfirm = false; this.loadAll(this.page.pageNumber); },
      error: () => { this.showConfirm = false; }
    });
  }

  goToPage(p: number): void { if (p >= 1 && p <= this.page.pageCount) this.loadAll(p); }
}
