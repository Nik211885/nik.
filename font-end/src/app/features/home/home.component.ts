import { Component } from '@angular/core';
import {PostCardComponent} from '../../shared/components/post-card/post-card.component';
import {ApplicationTitle} from '../../app.message';
import {LanguagePipe} from '../../shared/pipes/language.pipe';
import {HeroSectionComponent} from '../../shared/components/hero-section/hero-section.component';

@Component({
  selector: 'app-home',
  imports: [
    PostCardComponent,
    LanguagePipe,
    HeroSectionComponent,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  protected readonly ApplicationTitle = ApplicationTitle;
}
