export interface ArticleAuthorModel {
  id: string;
  userName: string;
  slug: string;
  avatar?: string;
  bio: string;
}

export interface ArticleTagModel {
  id: string;
  name: string;
  slug: string;
  image: string;
}

export interface ArticleCategoryModel {
  id: string;
  name: string;
  slug: string;
  image: string;
}

export interface ArticleModel {
  id: string;
  title: string;
  description: string;
  content: string;
  image: string;
  slug: string;
  createdDate: string;
  updatedDate?: string;
  countCommentRef: number;
  countLikeRef: number;
  countSee: number;
  countHeartRef: number;
  author: ArticleAuthorModel;
  tags: ArticleTagModel[];
  categories: ArticleCategoryModel[];
}
