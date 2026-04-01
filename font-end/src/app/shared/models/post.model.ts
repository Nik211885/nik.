import {CategoryModel} from './category.model';
import {UserModel} from './user.model';

export interface PostModel{
  id: string;
  slug: string;
  image: string;
  title: string;
  content: string;
  description: string;
  createdAt: Date;
  updatedAt?: Date;
  heart: number;
  see: number;
  comment: number;
  category: CategoryModel;
  writer: UserModel
}
