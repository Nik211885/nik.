import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { QuillModule } from 'ngx-quill';
import { ContentTranslationAdminService } from '../../../services/content-translation.admin.service';
import { LanguagePipe } from '../../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../../app.message';
import { ENTITY_FIELDS, FieldDef } from '../content-translations.component';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-translation-editor-page',
  standalone: true,
  imports: [CommonModule, FormsModule, QuillModule, LanguagePipe],
  templateUrl: './translation-editor-page.component.html',
})
export class TranslationEditorPageComponent implements OnInit {
  readonly AdminMessage = AdminMessage;

  readonly editorModules = {
    toolbar: [
      [{ header: [1, 2, 3, 4, false] }],
      ['bold', 'italic', 'underline', 'strike'],
      [{ list: 'ordered' }, { list: 'bullet' }],
      ['blockquote', 'code-block'],
      ['link', 'image'],
      [{ align: [] }],
      ['clean'],
    ],
  };

  entityType = '';
  entityId = '';
  langCode = '';
  sourceTitle = '';

  fields: FieldDef[] = [];
  sourceValues: Record<string, string> = {};
  translationValues: Record<string, string> = {};

  loading = true;
  saving = false;
  saved = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private svc: ContentTranslationAdminService,
    private toast: ToastService,
  ) {}

  ngOnInit(): void {
    this.entityType  = this.route.snapshot.paramMap.get('entityType') ?? '';
    this.entityId    = this.route.snapshot.paramMap.get('entityId') ?? '';
    this.langCode    = this.route.snapshot.paramMap.get('lang') ?? '';
    this.sourceTitle = this.route.snapshot.queryParamMap.get('title') ?? '';
    this.fields      = ENTITY_FIELDS[this.entityType] ?? [];

    forkJoin({
      source:      this.svc.getSource(this.entityType, this.entityId),
      translation: this.svc.getEntityTranslation(this.entityType, this.entityId, this.langCode),
    }).subscribe({
      next: ({ source, translation }) => {
        this.sourceValues      = source.fields;
        this.translationValues = { ...translation.fields };
        this.loading = false;
      },
      error: () => { this.loading = false; },
    });
  }

  save(): void {
    this.saving = true;
    this.saved  = false;
    this.svc.upsert({
      entityType: this.entityType,
      entityId:   this.entityId,
      langCode:   this.langCode,
      fields:     this.translationValues,
    }).subscribe({
      next: () => { this.saving = false; this.saved = true; this.toast.success(AdminMessage.TOAST_SAVE_SUCCESS); },
      error: () => { this.saving = false; this.toast.error(AdminMessage.TOAST_SAVE_ERROR); },
    });
  }

  back(): void {
    this.router.navigate(['/admin/content-translations']);
  }
}
