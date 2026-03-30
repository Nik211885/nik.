import {Component, Input} from '@angular/core';
import {ApplicationTitle} from '../../../app.message';
import {LanguagePipe} from '../../pipes/language.pipe';
import {NewsLetterModel} from './news-letter.model';

@Component({
  selector: 'app-news-letter',
  imports: [
    LanguagePipe
  ],
  templateUrl: './news-letter.component.html',
  styleUrl: './news-letter.component.css',
})
export class NewsLetterComponent {
  @Input() newsletterModel: NewsLetterModel = {
    title: ApplicationTitle.NEWS_LETTER_TITLE,
    content: ApplicationTitle.NEW_LETTER_CONTENT,
    inputDescription: ApplicationTitle.EMAIL_ADDRESS,
    submitButtonText: ApplicationTitle.SUBSCRIBE
  };
  protected readonly ApplicationTitle = ApplicationTitle;
}
