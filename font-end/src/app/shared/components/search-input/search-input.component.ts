import {Component, Input} from '@angular/core';
import {ApplicationTitle} from '../../../app.message';
import {LanguagePipe} from '../../pipes/language.pipe';

@Component({
  selector: 'app-search-input',
  imports: [
    LanguagePipe
  ],
  templateUrl: './search-input.component.html',
  styleUrl: './search-input.component.css',
})
export class SearchInputComponent {
  @Input() searchDescription: string = "";
}
