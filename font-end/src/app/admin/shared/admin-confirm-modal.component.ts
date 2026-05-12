import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LanguagePipe } from '../../shared/pipes/language.pipe';
import {AdminMessage} from "../../app.message"

@Component({
  selector: 'app-admin-confirm-modal',
  standalone: true,
  imports: [CommonModule, LanguagePipe],
  template: `
    @if (show) {
    <div class="admin-overlay" (click)="cancelled.emit()">
      <div class="admin-confirm-box" (click)="$event.stopPropagation()">

        <div class="text-center mb-3">
          <i class="bi bi-exclamation-triangle-fill text-warning" style="font-size:2.8rem"></i>
        </div>

        <h5 class="text-center fw-semibold mb-2">
          {{AdminMessage.DIAGLOG_CONFIRM_DELETE_TITLE | language}}
        </h5>

        <p class="text-center text-muted mb-4">
          {{AdminMessage.DIAGLOG_CONFIRM_DELETE_LABEL | language}}
          <strong class="text-dark">{{ itemName }}</strong>?
          <br>
          <small>{{AdminMessage.DIAGLOG_CONFIRM_DELETE_MESSAGE | language}}</small>
        </p>

        <div class="d-flex gap-2">
          <button
            class="btn btn-outline-secondary flex-fill"
            (click)="cancelled.emit()">
            <i class="bi bi-x-lg me-1"></i>{{AdminMessage.DIAGLOG_CONFIRM_DELETE_ACTION_CANCEL | language}}
          </button>

          <button
            class="btn btn-danger flex-fill"
            (click)="confirmed.emit()">
            <i class="bi bi-trash3 me-1"></i>{{AdminMessage.DIAGLOG_CONFIRM_DELETE_ACTION_DELETE | language}}
          </button>
        </div>

      </div>
    </div>
  }
  `,
  styles: [`
    .admin-overlay {
      position: fixed; inset: 0; background: rgba(0,0,0,.45);
      z-index: 1055; display: flex; align-items: center; justify-content: center;
    }
    .admin-confirm-box {
      background: #fff; border-radius: 14px; padding: 2rem;
      width: 420px; max-width: 92vw; box-shadow: 0 20px 60px rgba(0,0,0,.2);
    }
  `]
})
export class AdminConfirmModalComponent {
  protected readonly AdminMessage = AdminMessage;
  @Input() show = false;
  @Input() itemName = '';
  @Output() confirmed = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();
}
