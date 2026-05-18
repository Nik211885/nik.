import { Component, DestroyRef, ElementRef, HostListener, OnInit, ViewChild, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AlbumFileModel, AlbumModel } from '../../shared/models/album.model';
import { AlbumService } from '../../core/services/album.service';
import { LanguageService } from '../../core/services/language.service';
import { LanguagePipe } from '../../shared/pipes/language.pipe';
import { ApplicationTitle } from '../../app.message';

type View = 'albums' | 'children' | 'photos';

@Component({
  selector: 'app-photography',
  imports: [LanguagePipe],
  templateUrl: './photography.component.html',
  styleUrl: './photography.component.css',
})
export class PhotographyComponent implements OnInit {
  view: View = 'albums';
  loading = false;

  rootAlbums: AlbumModel[] = [];
  currentAlbum: AlbumModel | null = null;
  currentChildren: AlbumModel[] = [];
  files: AlbumFileModel[] = [];
  breadcrumbs: { title: string; album: AlbumModel | null }[] = [];

  lightboxIndex: number | null = null;

  @ViewChild('lightboxVideo') lightboxVideo?: ElementRef<HTMLVideoElement>;

  protected readonly ApplicationTitle = ApplicationTitle;

  private destroyRef = inject(DestroyRef);

  isVideoUrl(url: string): boolean {
    return url.includes('/video/upload/') || /\.(mp4|webm|mov)(\?|$)/i.test(url);
  }

  videoThumbnailUrl(url: string): string {
    return url.replace(/\.(mp4|webm|mov)(\?.*)?$/i, '.jpg');
  }

  constructor(private albumService: AlbumService, private langService: LanguageService) {}

  ngOnInit(): void {
    this.langService.withLanguage(() => this.albumService.getParents())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: albums => {
          // leaf albums (no children) first, then albums with children, each group by orderIndex
          this.rootAlbums = [...albums].sort((a, b) => {
            const aLeaf = !a.children?.length ? 0 : 1;
            const bLeaf = !b.children?.length ? 0 : 1;
            if (aLeaf !== bLeaf) return aLeaf - bLeaf;
            return a.orderIndex - b.orderIndex;
          });
          this.loading = false;
        },
        error: () => { this.loading = false; }
      });
  }

  openAlbum(album: AlbumModel): void {
    if (album.children?.length) {
      this.currentAlbum = album;
      this.currentChildren = [...album.children].sort((a, b) => {
        const aLeaf = !a.children?.length ? 0 : 1;
        const bLeaf = !b.children?.length ? 0 : 1;
        if (aLeaf !== bLeaf) return aLeaf - bLeaf;
        return a.orderIndex - b.orderIndex;
      });
      this.breadcrumbs = [{ title: album.title, album }];
      this.view = 'children';
    } else {
      this.openPhotos(album, this.view === 'children' ? [{ title: this.currentAlbum!.title, album: this.currentAlbum }] : []);
    }
  }

  openChildAlbum(album: AlbumModel): void {
    if (album.children?.length) {
      this.breadcrumbs = [...this.breadcrumbs, { title: album.title, album }];
      this.currentChildren = [...album.children].sort((a, b) => {
        const aLeaf = !a.children?.length ? 0 : 1;
        const bLeaf = !b.children?.length ? 0 : 1;
        if (aLeaf !== bLeaf) return aLeaf - bLeaf;
        return a.orderIndex - b.orderIndex;
      });
    } else {
      this.openPhotos(album, this.breadcrumbs);
    }
  }

  private openPhotos(album: AlbumModel, crumbs: { title: string; album: AlbumModel | null }[]): void {
    this.loading = true;
    this.albumService.getFiles(album.id).subscribe({
      next: files => {
        this.files = files;
        this.breadcrumbs = [...crumbs, { title: album.title, album }];
        this.view = 'photos';
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  goBack(): void {
    if (this.view === 'photos' && this.breadcrumbs.length > 1) {
      // back to children view
      this.breadcrumbs = this.breadcrumbs.slice(0, -1);
      this.view = 'children';
    } else if (this.view === 'photos' || this.view === 'children') {
      this.view = 'albums';
      this.currentAlbum = null;
      this.breadcrumbs = [];
    }
  }

  openLightbox(index: number): void {
    this.lightboxIndex = index;
  }

  closeLightbox(): void {
    this.lightboxVideo?.nativeElement.pause();
    this.lightboxIndex = null;
  }

  prevPhoto(): void {
    if (this.lightboxIndex === null) return;
    this.lightboxVideo?.nativeElement.pause();
    this.lightboxIndex = (this.lightboxIndex - 1 + this.files.length) % this.files.length;
  }

  nextPhoto(): void {
    if (this.lightboxIndex === null) return;
    this.lightboxVideo?.nativeElement.pause();
    this.lightboxIndex = (this.lightboxIndex + 1) % this.files.length;
  }

  @HostListener('document:keydown', ['$event'])
  onKeydown(e: KeyboardEvent): void {
    if (this.lightboxIndex === null) return;
    if (e.key === 'ArrowLeft') this.prevPhoto();
    if (e.key === 'ArrowRight') this.nextPhoto();
    if (e.key === 'Escape') this.closeLightbox();
  }
}
