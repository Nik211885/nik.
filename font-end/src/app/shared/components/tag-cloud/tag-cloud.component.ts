import {Component, Input} from '@angular/core';
import {RouterLink} from '@angular/router';
import {TagCloudModel} from './tag-cloud.model';

@Component({
  selector: 'app-tag-cloud',
  imports: [
    RouterLink
  ],
  templateUrl: './tag-cloud.component.html',
  styleUrl: './tag-cloud.component.css',
})
export class TagCloudComponent {
  @Input() tagCloud: TagCloudModel = {
    id: '1',
    name: 'animals',
    ref: 'animals'
  };
}
