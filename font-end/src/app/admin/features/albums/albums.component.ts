import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { AlbumAdminService } from '../../services/album.admin.service';
import { FileAdminService } from '../../services/file.admin.service';
import { AdminConfirmModalComponent } from '../../shared/admin-confirm-modal.component';
import { FileCloudinaryService } from '../../../core/services/file.cloudinary.service';
import { AlbumItem, FileItem } from '../../models/admin.model';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';
import { ToastService } from '../../../core/services/toast.service';

interface TreeNode {
  item: AlbumItem;
  level: number;
  hasChildren: boolean;
}

interface BatchItem {
  name: string;
  progress: number;
  done: boolean;
  error?: string;
}

@Component({
  selector: 'app-albums-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminConfirmModalComponent, LanguagePipe],
  templateUrl: './albums.component.html',
  styleUrl: './albums.component.css'
})
export class AlbumsAdminComponent implements OnInit, OnDestroy {
  allAlbums: AlbumItem[] = [];
  loading = false;

  viewMode: 'tree' | 'explorer' = 'tree';

  // Tree state — use plain object so Angular change detection triggers
  expandedMap: { [id: string]: boolean } = {};

  // Explorer drill-down state
  drillStack: AlbumItem[] = [];
  albumFiles: FileItem[] = [];
  filesLoading = false;

  // CRUD
  showModal = false;
  isEditing = false;
  selected: AlbumItem | null = null;
  saving = false;
  error = '';
  form = { name: '', title: '', description: '', albumId: '', orderIndex: 0 };

  // Album delete
  showConfirm = false;
  deleteItems: AlbumItem[] = [];

  // File remove from album
  fileToRemove: FileItem | null = null;
  showFileConfirm = false;

  // File edit (name / title / description)
  showFileEditModal = false;
  editingFile: FileItem | null = null;
  fileForm = { name: '', title: '', description: '' };
  fileSaving = false;
  fileError = '';

  // Cover target — which album in the drillStack gets the cover assignment
  coverTargetAlbumId = '';

  // Drag-drop batch upload
  isDroppingPage = false;
  batchItems: BatchItem[] = [];
  private dragCounter = 0;
  private subs: Subscription[] = [];

  protected readonly AdminMessage = AdminMessage;

  constructor(
    private svc: AlbumAdminService,
    private fileSvc: FileAdminService,
    private cloudinary: FileCloudinaryService,
    private toast: ToastService
  ) {}

  ngOnInit(): void { this.load(); }
  ngOnDestroy(): void { this.subs.forEach(s => s.unsubscribe()); }

  load(): void {
    this.loading = true;
    this.svc.getAll().subscribe({
      next: d => { this.allAlbums = d; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  // ── Album tree helpers ─────────────────────────────────────────────────

  getRoots(): AlbumItem[] {
    return this.allAlbums
      .filter(a => !a.albumId)
      .sort((a, b) => a.orderIndex - b.orderIndex);
  }

  getChildren(parentId: string): AlbumItem[] {
    return this.allAlbums
      .filter(a => a.albumId === parentId)
      .sort((a, b) => a.orderIndex - b.orderIndex);
  }

  // ── Tree View ──────────────────────────────────────────────────────────

  get treeNodes(): TreeNode[] {
    const result: TreeNode[] = [];
    const visit = (item: AlbumItem, level: number) => {
      const children = this.getChildren(item.id);
      result.push({ item, level, hasChildren: children.length > 0 });
      if (this.expandedMap[item.id]) {
        children.forEach(c => visit(c, level + 1));
      }
    };
    this.getRoots().forEach(r => visit(r, 0));
    return result;
  }

  toggleExpand(id: string): void {
    this.expandedMap = { ...this.expandedMap, [id]: !this.expandedMap[id] };
  }

  expandAll(): void {
    const next: { [id: string]: boolean } = {};
    this.allAlbums.forEach(a => {
      if (this.getChildren(a.id).length > 0) next[a.id] = true;
    });
    this.expandedMap = next;
  }

  collapseAll(): void { this.expandedMap = {}; }

  // ── Explorer View ──────────────────────────────────────────────────────

  get currentAlbumId(): string | null {
    return this.drillStack.length ? this.drillStack[this.drillStack.length - 1].id : null;
  }

  get currentAlbum(): AlbumItem | null {
    return this.drillStack.length ? this.drillStack[this.drillStack.length - 1] : null;
  }

  get coverTargetAlbum(): AlbumItem | null {
    return this.drillStack.find(a => a.id === this.coverTargetAlbumId) ?? null;
  }

  get currentLevelAlbums(): AlbumItem[] {
    if (this.currentAlbumId === null) return this.getRoots();
    return this.getChildren(this.currentAlbumId);
  }

  get isAtFileLevel(): boolean {
    return this.currentAlbumId !== null && this.currentLevelAlbums.length === 0;
  }

  drillInto(album: AlbumItem): void {
    this.drillStack = [...this.drillStack, album];
    if (this.getChildren(album.id).length === 0) {
      this.coverTargetAlbumId = album.id;
      this.loadFiles(album.id);
    } else {
      this.albumFiles = [];
    }
  }

  drillTo(index: number): void {
    this.drillStack = this.drillStack.slice(0, index + 1);
    const album = this.drillStack[this.drillStack.length - 1];
    if (this.getChildren(album.id).length === 0) {
      this.coverTargetAlbumId = album.id;
      this.loadFiles(album.id);
    } else {
      this.albumFiles = [];
    }
  }

  drillToRoot(): void {
    this.drillStack = [];
    this.albumFiles = [];
    this.coverTargetAlbumId = '';
  }

  loadFiles(albumId: string): void {
    this.filesLoading = true;
    this.albumFiles = [];
    this.svc.getFiles(albumId).subscribe({
      next: f => { this.albumFiles = f; this.filesLoading = false; },
      error: () => { this.filesLoading = false; }
    });
  }

  confirmRemoveFile(file: FileItem): void {
    this.fileToRemove = file;
    this.showFileConfirm = true;
  }

  removeFile(): void {
    if (!this.currentAlbumId || !this.fileToRemove) return;
    this.svc.removeFiles(this.currentAlbumId, [this.fileToRemove.id]).subscribe({
      next: () => {
        this.albumFiles = this.albumFiles.filter(f => f.id !== this.fileToRemove!.id);
        this.showFileConfirm = false;
        this.fileToRemove = null;
        this.allAlbums = this.allAlbums.map(a =>
          a.id === this.currentAlbumId
            ? { ...a, countImageRef: Math.max(0, a.countImageRef - 1) }
            : a
        );
        this.toast.success(AdminMessage.TOAST_DELETE_SUCCESS);
      },
      error: () => { this.showFileConfirm = false; this.toast.error(AdminMessage.TOAST_DELETE_ERROR); }
    });
  }

  copyUrl(url: string): void {
    navigator.clipboard.writeText(url).catch(() => {});
  }

  isVideoUrl(url: string): boolean {
    return url.includes('/video/upload/') || /\.(mp4|webm|mov)(\?|$)/i.test(url);
  }

  openEditFile(file: FileItem): void {
    this.editingFile = file;
    this.fileForm = { name: file.name, title: file.title ?? '', description: file.description ?? '' };
    this.fileError = '';
    this.showFileEditModal = true;
  }

  saveFileEdit(): void {
    if (!this.editingFile) return;
    this.fileSaving = true;
    this.fileSvc.update({ id: this.editingFile.id, url: this.editingFile.url, ...this.fileForm }).subscribe({
      next: updated => {
        this.albumFiles = this.albumFiles.map(f => f.id === updated.id ? updated : f);
        this.showFileEditModal = false;
        this.fileSaving = false;
        this.toast.success(AdminMessage.TOAST_SAVE_SUCCESS);
      },
      error: () => {
        this.fileError = AdminMessage.ERROR_GENERIC;
        this.fileSaving = false;
        this.toast.error(AdminMessage.TOAST_SAVE_ERROR);
      }
    });
  }

  setCover(file: FileItem): void {
    const album = this.coverTargetAlbum;
    if (!album) return;
    const newFileId = file.id === album.fileDescriptionId ? null : file.id;
    this.svc.setCover(album.id, newFileId).subscribe({
      next: updated => {
        const patch = { fileDescriptionId: updated.fileDescriptionId, coverUrl: updated.coverUrl };
        this.allAlbums = this.allAlbums.map(a => a.id === album.id ? { ...a, ...patch } : a);
        this.drillStack = this.drillStack.map(a => a.id === album.id ? { ...a, ...patch } : a);
        this.toast.success(AdminMessage.TOAST_SAVE_SUCCESS);
      },
      error: () => this.toast.error(AdminMessage.TOAST_SAVE_ERROR)
    });
  }

  // ── Parent dropdown options (indented tree labels) ─────────────────────

  get parentOptions(): { id: string; label: string }[] {
    const result: { id: string; label: string }[] = [];
    const visit = (item: AlbumItem, level: number) => {
      if (this.isEditing && this.selected?.id === item.id) return;
      result.push({ id: item.id, label: '  '.repeat(level * 2) + (level > 0 ? '↳ ' : '') + item.name });
      this.getChildren(item.id).forEach(c => visit(c, level + 1));
    };
    this.getRoots().forEach(r => visit(r, 0));
    return result;
  }

  // ── CRUD ───────────────────────────────────────────────────────────────

  openCreate(): void {
    this.isEditing = false;
    this.selected = null;
    this.form = {
      name: '', title: '', description: '',
      albumId: this.currentAlbumId ?? '',
      orderIndex: 0
    };
    this.error = '';
    this.showModal = true;
  }

  openEdit(item: AlbumItem): void {
    this.isEditing = true;
    this.selected = item;
    this.form = {
      name: item.name, title: item.title, description: item.description ?? '',
      albumId: item.albumId ?? '', orderIndex: item.orderIndex
    };
    this.error = '';
    this.showModal = true;
  }

  openDelete(items: AlbumItem[]): void { this.deleteItems = items; this.selected = items[0] ?? null; this.showConfirm = true; }

  save(): void {
    this.saving = true;
    const body = { ...this.form, albumId: this.form.albumId || undefined };
    const obs = this.isEditing
      ? this.svc.update({ id: this.selected!.id, ...body })
      : this.svc.create(body);

    obs.subscribe({
      next: () => { this.showModal = false; this.saving = false; this.load(); this.toast.success(AdminMessage.TOAST_SAVE_SUCCESS); },
      error: () => { this.error = AdminMessage.ERROR_GENERIC; this.saving = false; this.toast.error(AdminMessage.TOAST_SAVE_ERROR); }
    });
  }

  delete(): void {
    this.svc.delete(this.deleteItems.map(i => i.id)).subscribe({
      next: () => {
        this.showConfirm = false;
        if (this.drillStack.some(a => this.deleteItems.some(d => d.id === a.id))) {
          this.drillStack = [];
          this.albumFiles = [];
        }
        this.load();
        this.toast.success(AdminMessage.TOAST_DELETE_SUCCESS);
      },
      error: () => { this.showConfirm = false; this.toast.error(AdminMessage.TOAST_DELETE_ERROR); }
    });
  }

  // ── Drag & drop batch upload (explorer file level only) ────────────────

  @HostListener('document:dragenter', ['$event'])
  onDocDragEnter(e: DragEvent): void {
    if (!this.isAtFileLevel || !e.dataTransfer?.types.includes('Files')) return;
    this.dragCounter++;
    this.isDroppingPage = true;
  }

  @HostListener('document:dragleave', ['$event'])
  onDocDragLeave(e: DragEvent): void {
    if (!e.dataTransfer?.types.includes('Files')) return;
    this.dragCounter = Math.max(0, this.dragCounter - 1);
    if (this.dragCounter === 0) this.isDroppingPage = false;
  }

  @HostListener('document:dragover', ['$event'])
  onDocDragOver(e: DragEvent): void {
    if (this.isAtFileLevel && e.dataTransfer?.types.includes('Files')) e.preventDefault();
  }

  @HostListener('document:drop', ['$event'])
  onDocDrop(e: DragEvent): void {
    e.preventDefault();
    this.isDroppingPage = false;
    this.dragCounter = 0;
    if (!this.isAtFileLevel || this.showModal) return;

    const files = Array.from(e.dataTransfer?.files ?? []).filter(
      f => f.type.startsWith('image/') || f.type.startsWith('video/')
    );
    if (files.length) this.batchUpload(files);
  }

  onFileInputChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = Array.from(input.files ?? []).filter(
      f => f.type.startsWith('image/') || f.type.startsWith('video/')
    );
    input.value = '';
    if (files.length) this.batchUpload(files);
  }

  private batchUpload(files: File[]): void {
    const albumId = this.currentAlbumId!;
    const items: BatchItem[] = files.map(f => ({ name: f.name, progress: 0, done: false }));
    this.batchItems = [...this.batchItems, ...items];

    files.forEach((file, i) => {
      const item = items[i];
      const sub = this.cloudinary.uploadFile(file).subscribe({
        next: ({ progress, response }) => {
          item.progress = progress;
          if (response?.secure_url) {
            const name = file.name.replace(/\.[^/.]+$/, '');
            this.fileSvc.create({ name, title: '', url: response.secure_url, description: '' }).subscribe({
              next: created => {
                this.svc.addFiles(albumId, [created.id]).subscribe({
                  next: () => {
                    item.done = true;
                    this.loadFiles(albumId);
                    setTimeout(() => {
                      this.batchItems = this.batchItems.filter(x => x !== item);
                    }, 2500);
                  },
                  error: () => { item.error = AdminMessage.FILES_SAVE_FAILED; }
                });
              },
              error: () => { item.error = AdminMessage.FILES_SAVE_FAILED; }
            });
          }
        },
        error: () => {
          item.progress = 0;
          item.error = AdminMessage.ERROR_GENERIC;
        }
      });
      this.subs.push(sub);
    });
  }
}
