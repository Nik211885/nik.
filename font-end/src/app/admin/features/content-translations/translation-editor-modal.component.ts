import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ContentTranslationAdminService } from '../../services/content-translation.admin.service';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { TranslationStatusItem } from '../../models/admin.model';
import { AdminMessage } from '../../../app.message';

export interface FieldDef { key: string; label: string; multiline: boolean; }

@Component({
  selector: 'app-translation-editor-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, LanguagePipe],
  templateUrl: './translation-editor-modal.component.html',
})
export class TranslationEditorModalComponent implements OnChanges {
  @Input() item!: TranslationStatusItem;
  @Input() langCode!: string;
  @Input() fields: FieldDef[] = [];

  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  readonly AdminMessage = AdminMessage;

  values: Record<string, string> = {};
  saving = false;
  loading = true;

  constructor(private svc: ContentTranslationAdminService) {}

  ngOnChanges(): void {
    if (this.item && this.langCode) {
      this.loading = true;
      this.values = {};
      this.svc.getEntityTranslation(this.item.entityType, this.item.entityId, this.langCode)
        .subscribe({
          next: res => {
            this.values = { ...res.fields };
            this.loading = false;
          },
          error: () => { this.loading = false; },
        });
    }
  }

  save(): void {
    this.saving = true;
    this.svc.upsert({
      entityType: this.item.entityType,
      entityId: this.item.entityId,
      langCode: this.langCode,
      fields: this.values,
    }).subscribe({
      next: () => { this.saving = false; this.saved.emit(); },
      error: () => { this.saving = false; },
    });
  }

  cancel(): void { this.cancelled.emit(); }
}
