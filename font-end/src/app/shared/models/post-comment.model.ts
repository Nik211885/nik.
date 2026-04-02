import {UserModel} from './user.model';

export interface PostCommentModel {
  id: string;
  writer: UserModel,
  createdDate: Date,
  content: string,
  commentChild: PostCommentModel[]
}
