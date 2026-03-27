import { Component } from '@angular/core';
import {ApplicationTitle} from '../../../app.message';
import {LanguagePipe} from '../../pipes/language.pipe';

@Component({
  selector: 'app-news-letter',
  imports: [
    LanguagePipe
  ],
  templateUrl: './news-letter.component.html',
  styleUrl: './news-letter.component.css',
})
export class NewsLetterComponent {
  protected readonly ApplicationTitle = ApplicationTitle;
}
