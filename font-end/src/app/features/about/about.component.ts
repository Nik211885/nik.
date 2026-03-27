import { Component } from '@angular/core';
import {ApplicationTitle} from "../../app.message";
import {LanguagePipe} from '../../shared/pipes/language.pipe';
import {ConfigService} from '../../core/services/config.service';
import {AsyncPipe} from '@angular/common';

@Component({
  selector: 'app-about',
  imports: [
    LanguagePipe,
    AsyncPipe
  ],
  templateUrl: './about.component.html',
  styleUrl: './about.component.css',
})
export class AboutComponent {
    protected readonly ApplicationTitle = ApplicationTitle;
    constructor(protected readonly configService: ConfigService) {}
}
