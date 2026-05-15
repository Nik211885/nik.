export interface PostCommentModel {
  id: string;
  articleId: string;
  authorId?: string;
  authorName: string;
  authorAvatar?: string;
  guestWebsite?: string;
  createdDate: string;
  text: string;
  parentId?: string;
  children: PostCommentModel[];
}
