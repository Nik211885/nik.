import { Component } from '@angular/core';
import {ApplicationTitle} from "../../../app.message";
import {LanguagePipe} from '../../pipes/language.pipe';

@Component({
  selector: 'app-input-comment',
  imports: [
    LanguagePipe
  ],
  templateUrl: './input-comment.component.html',
  styleUrl: './input-comment.component.css',
})
export class InputCommentComponent {
    protected readonly ApplicationTitle = ApplicationTitle;
}
