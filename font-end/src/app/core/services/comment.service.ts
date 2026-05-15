import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {PostCommentModel} from '../../shared/models/post-comment.model';

@Injectable({providedIn: 'root'})
export class CommentService {
  constructor(private http: HttpClient) {}

  getComments(articleId: string): Observable<PostCommentModel[]> {
    return this.http.get<PostCommentModel[]>(`api/comments?articleId=${articleId}`).pipe(
      map(flat => this.buildTree(flat))
    );
  }

  createComment(payload: {
    articleId: string;
    text: string;
    parentId?: string;
    guestName?: string;
    guestEmail?: string;
    guestWebsite?: string;
  }): Observable<PostCommentModel> {
    return this.http.post<PostCommentModel>('api/comments', payload);
  }

  private buildTree(flat: PostCommentModel[]): PostCommentModel[] {
    const map = new Map(flat.map(c => [c.id, {...c, children: [] as PostCommentModel[]}]));
    const roots: PostCommentModel[] = [];
    for (const c of map.values()) {
      if (c.parentId) {
        map.get(c.parentId)?.children.push(c);
      } else {
        roots.push(c);
      }
    }
    return roots;
  }
}
