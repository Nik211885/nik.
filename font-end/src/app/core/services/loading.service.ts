import { Injectable, signal } from '@angular/core';
import {ApplicationMessage} from '../../app.message';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private _isLoading = signal<boolean>(false);
  private _message = signal<string>(ApplicationMessage.LOADING_PROCESS);

  readonly isLoading = this._isLoading.asReadonly();
  readonly message = this._message.asReadonly();

  show(message: string = ApplicationMessage.LOADING_PROCESS): void {
    this._message.set(message);
    this._isLoading.set(true);
  }

  hide(): void {
    this._isLoading.set(false);
  }
}
