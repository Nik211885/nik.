import { Component } from '@angular/core';
import {ConfigService} from '../../../core/services/config.service';
import {ApplicationTitle} from '../../../app.message';
import {LanguagePipe} from '../../../shared/pipes/language.pipe';
import {AsyncPipe} from '@angular/common';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-footer',
  imports: [
    LanguagePipe,
    AsyncPipe,
    RouterLink
  ],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.css',
})
export class FooterComponent {
  yearNow = new Date().getFullYear();
  constructor(protected readonly configService: ConfigService) {
  }

  protected readonly ApplicationTitle = ApplicationTitle;
}
