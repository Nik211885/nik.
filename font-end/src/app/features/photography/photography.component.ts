import { Component } from '@angular/core';
import {PostCardComponent} from "../../shared/components/post-card/post-card.component";
import {PhotoMasonryGridComponent} from '../../shared/components/photo-masonry-grid/photo-masonry-grid.component';
import {photoLocationMasonryGrid} from '../../app-data.fake';

@Component({
  selector: 'app-photography',
  imports: [
    PhotoMasonryGridComponent
  ],
  templateUrl: './photography.component.html',
  styleUrl: './photography.component.css',
})
export class PhotographyComponent {
  protected readonly photoLocationMasonryGrid = photoLocationMasonryGrid;
}
