import {Component, signal} from '@angular/core';
import { RouterOutlet} from '@angular/router';
import {LanguageService} from './core/services/language.service';
import {LanguagePipe} from './shared/pipes/language.pipe';
import {LoadingService} from './core/services/loading.service';
import {ToastService} from './core/services/toast.service';
import {ToastComponent} from './shared/components/toast/toast.component';
import {LoadingComponent} from './shared/components/loadding/loading.component';
import {MainLayoutComponent} from './layout/main-layout/main-layout.component';

@Component({
  selector: 'app-root',
  imports: [LanguagePipe, ToastComponent, LoadingComponent, RouterOutlet, MainLayoutComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
  standalone: true
})
export class App {

}
