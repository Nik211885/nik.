import {Component, Input} from '@angular/core';
import {RouterLink} from '@angular/router';
import {PhotoMasonryGridModel} from './photo-masonry-grid.model';
import {ApplicationTitle} from '../../../app.message';
import {LanguagePipe} from '../../pipes/language.pipe';

@Component({
  selector: 'app-photo-masonry-grid',
  imports: [
    RouterLink,
    LanguagePipe
  ],
  templateUrl: './photo-masonry-grid.component.html',
  styleUrl: './photo-masonry-grid.component.css',
})
export class PhotoMasonryGridComponent {
  @Input() photo: PhotoMasonryGridModel = {
    id: '1',
    count: 40,
    name: 'HaNoi',
    ref: 'hanoi-album',
    image: '/assets/images/img.png'
  }
  protected readonly ApplicationTitle = ApplicationTitle;
}
