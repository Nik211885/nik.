import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {AsyncPipe, NgClass } from '@angular/common';
import {Toast, ToastService} from '../../../core/services/toast.service';
import {LanguagePipe} from '../../pipes/language.pipe';

@Component({
  selector:        'app-toast',
  standalone:      true,
  imports: [AsyncPipe, NgClass, LanguagePipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './toast.component.html',
  styleUrl: './toast.component.css',
})
export class ToastComponent {
  protected readonly toastService = inject(ToastService);
  protected readonly iconMap: Record<string, string> = {
    success: '✅',
    error:   '❌',
    warning: '⚠️',
    info:    'ℹ️',
  };

  dismiss(id: string): void {
    this.toastService.dismiss(id);
  }

  trackById(_: number, toast: Toast): string {
    return toast.id;
  }
}
