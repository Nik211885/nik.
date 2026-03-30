import { Component } from '@angular/core';
import {ConfigService} from '../../../core/services/config.service';
import {AsyncPipe} from '@angular/common';
import {LanguagePipe} from '../../pipes/language.pipe';
import {RouterLink} from '@angular/router';
import {ApplicationTitle} from '../../../app.message';

@Component({
  selector: 'app-hero-section',
  imports: [
    AsyncPipe,
    LanguagePipe,
    RouterLink
  ],
  templateUrl: './hero-section.component.html',
  styleUrl: './hero-section.component.css',
})
export class HeroSectionComponent {
  constructor(protected readonly configService: ConfigService) {
  }

  protected readonly ApplicationTitle = ApplicationTitle;
}
