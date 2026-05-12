export interface TableColumn {
  key: string;
  label: string;
  type?: 'text' | 'date' | 'number' | 'image' | 'truncate';
}

export interface PaginationRequest {
  pageNumber?: number;
  pageSize?: number;
}

export interface PaginationResponse<T> {
  data: T[];
  totalItems: number;
  pageNumber: number;
  pageSize: number;
  pageCount: number;
}

export interface AlbumItem {
  id: string;
  name: string;
  title: string;
  description: string;
  slug: string;
  albumId?: string;
  orderIndex: number;
  countImageRef: number;
  children?: AlbumItem[];
}

export interface ArticleItem {
  id: string;
  title: string;
  description: string;
  content: string;
  image: string;
  slug: string;
  countCommentRef: number;
  countLikeRef: number;
  countSee: number;
  countHeartRef: number;
  createdDate: string;
  updatedDate: string;
  author: { id: string; userName: string; slug: string };
  tags: { id: string; name: string; slug: string; image: string }[];
  categories: { id: string; name: string; slug: string; image: string }[];
}

export interface CategoryItem {
  id: string;
  name: string;
  title: string;
  description: string;
  image: string;
  slug: string;
  countRef: number;
}

export interface TagItem {
  id: string;
  name: string;
  title: string;
  description: string;
  image: string;
  slug: string;
  countRef: number;
}

export interface CommentItem {
  id: string;
  text: string;
  articleId: string;
  parentId?: string;
  createdDate: string;
  author?: { id: string; userName: string };
}

export interface FileItem {
  id: string;
  name: string;
  title: string;
  url: string;
  description: string;
}

export interface UserItem {
  id: string;
  userName: string;
  email?: string;
  phone?: string;
  bio: string;
  slug: string;
  createdDate: string;
  updatedDate: string;
}

export interface LanguageItem {
  id: string;
  code: string;
  name: string;
}

export interface CodeLanguageItem {
  id: string;
  code: string;
}

export interface TranslateItem {
  id: string;
  codeId: string;
  languageId: string;
  value: string;
  code?: string;
  language?: string;
}

export interface SysConfigItem {
  id: string;
  key: string;
  value: any;
}
