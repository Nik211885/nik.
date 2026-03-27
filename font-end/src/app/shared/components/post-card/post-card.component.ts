import {Component, Input, OnInit} from '@angular/core';
import {PostModel} from '../../models/post.model';
import {RouterLink} from '@angular/router';
import {ApplicationTitle} from '../../../app.message';
import {LanguagePipe} from '../../pipes/language.pipe';
import {AppDatePipe} from '../../pipes/app-date.pipe';
import {TruncatePipe} from '../../pipes/truncate.pipe';

@Component({
  selector: 'app-post-card',
  imports: [
    RouterLink,
    LanguagePipe,
    AppDatePipe
  ],
  standalone: true,
  templateUrl: './post-card.component.html',
  styleUrl: './post-card.component.css',
})
export class PostCardComponent implements OnInit {
  @Input() post!: PostModel;
  ngOnInit(): void {
      this.post = {
        id: '1',
        image: 'https://smartcom.vn/blog/wp-content/uploads/2024/03/2_1.jpg',
        see: 100,
        comment: 5,
        heart: 3,
        slug: 'one',
        createdAt: new Date(),
        updatedAt: new Date(),
        title: 'The Newest Technology On This Year 2019 ',
        description: 'Even the all-powerful Pointing has no control about the blind texts it is an almost Even the all-powerful Pointing has no control about the blind texts it is an almost',
        writer: {
          id: '3',
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

  protected readonly ApplicationTitle = ApplicationTitle;
}

