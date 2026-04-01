import {Component, Input, OnInit} from '@angular/core';
import {PostModel} from '../../models/post.model';
import {RouterLink} from '@angular/router';
import {AppDatePipe} from '../../pipes/app-date.pipe';

@Component({
  selector: 'app-post-card-compact',
  imports: [
    RouterLink,
    AppDatePipe
  ],
  standalone: true,
  templateUrl: './post-card-compact.component.html',
  styleUrl: './post-card-compact.component.css',
})
export class PostCardCompactComponent  implements OnInit {
  @Input() post!: PostModel;
  ngOnInit(): void {
    this.post = {
      id: '1',
      image: 'https://smartcom.vn/blog/wp-content/uploads/2024/03/2_1.jpg',
      see: 100,
      comment: 5,
      content: '',
      heart: 3,
      slug: 'one',
      createdAt: new Date(),
      updatedAt: new Date(),
      title: 'The Newest Technology On This Year 2019 ',
      description: 'Even the all-powerful Pointing has no control about the blind texts it is an almost Even the all-powerful Pointing has no control about the blind texts it is an almost',
      writer: {
        id: '3',
        bio: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ducimus itaque, autem necessitatibus voluptate quod mollitia delectus aut, sunt placeat nam vero culpa sapiente consectetur similique, inventore eos fugit cupiditate numquam!',
        slug: 'dave-lewis',
        avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTtJnACR9i1iQyY4t4E4f0-bfAUw994dvubfw&s',
        name: 'Dave Lewis'
      },
      category:{
        id: '4',
        name: 'Technology',
        slug: 'technology',
      }
    }
  }
}
