export interface AlbumModel {
  id: string;
  name: string;
  title: string;
  description: string;
  slug: string;
  countImageRef: number;
  coverUrl: string | null;
  albumId: string | null;
  orderIndex: number;
  children: AlbumModel[] | null;
}

export interface AlbumFileModel {
  id: string;
  albumId: string;
  name: string;
  title: string;
  url: string;
  description: string;
}
