import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {LoadingService} from '../../../core/services/loading.service';
import {LanguagePipe} from '../../pipes/language.pipe';

@Component({
  selector: 'app-loading',
  standalone: true,
  imports: [CommonModule, LanguagePipe],
  templateUrl: './loading.component.html',
  styleUrl: './loading.component.css',
})
export class LoadingComponent {
  protected loadingService = inject(LoadingService);
}
