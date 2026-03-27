import { Component } from '@angular/core';
import {PostCardComponent} from "../../shared/components/post-card/post-card.component";

@Component({
  selector: 'app-photography',
    imports: [
        PostCardComponent
    ],
  templateUrl: './photography.component.html',
  styleUrl: './photography.component.css',
})
export class PhotographyComponent {}
