import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SysConfigAdminService } from '../../../services/sys-config.admin.service';
import { LanguagePipe } from '../../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../../app.message';
import { ToastService } from '../../../../core/services/toast.service';
import { CONTENT_LANG, DEFAULT_LANG } from '../../../../core/services/language.service';

type EditorType = 'info' | 'social' | 'sidebar' | 'json';
type ViewMode   = 'form' | 'json';

interface InfoLang {
  name: string; email: string; phone: string; address: string;
  website: string; avatar: string; bio: string; introduction: string;
}
interface SocialItem  { id: string; name: string; ref: string; icon: string; }
interface SidebarItem { nameKey: string; ref: string; }
interface GenericProp { key: string; value: string; }

function emptyInfoLang(): InfoLang {
  return { name: '', email: '', phone: '', address: '', website: '', avatar: '', bio: '', introduction: '' };
}

@Component({
  selector: 'app-sys-config-editor-page',
  standalone: true,
  imports: [CommonModule, FormsModule, LanguagePipe],
  templateUrl: './sys-config-editor-page.component.html',
})
export class SysConfigEditorPageComponent implements OnInit {
  readonly AdminMessage = AdminMessage;
  readonly CONTENT_LANG = CONTENT_LANG;
  readonly DEFAULT_LANG  = DEFAULT_LANG;

  configId  = '';
  configKey = '';
  editorType: EditorType = 'json';
  viewMode: ViewMode = 'form';
  activeLang = CONTENT_LANG;

  // structured forms — keyed by CONTENT_LANG and DEFAULT_LANG
  infoForm: Record<string, InfoLang> = {
    [CONTENT_LANG]: emptyInfoLang(),
    [DEFAULT_LANG]:  emptyInfoLang(),
  };
  socialItems:  SocialItem[]  = [];
  sidebarItems: SidebarItem[] = [];
  genericProps: GenericProp[] = [];

  // raw JSON
  jsonValue = '';
  jsonError = '';

  loading = true;
  saving  = false;
  saved   = false;
  error   = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private svc: SysConfigAdminService,
    private toast: ToastService,
  ) {}

  ngOnInit(): void {
    this.configId = this.route.snapshot.paramMap.get('id') ?? '';
    this.svc.getAll().subscribe({
      next: configs => {
        const config = configs.find(c => c.id === this.configId);
        if (config) {
          this.configKey = config.key;
          this.editorType = this.detectType(config.key);
          this.viewMode = this.editorType !== 'json' ? 'form' : 'json';
          const raw = typeof config.value === 'string' ? JSON.parse(config.value) : config.value;
          this.parseRawIntoForms(raw);
          this.jsonValue = JSON.stringify(raw, null, 2);
        }
        this.loading = false;
      },
      error: () => { this.loading = false; },
    });
  }

  private detectType(key: string): EditorType {
    if (key === 'config.info')    return 'info';
    if (key === 'config.social')  return 'social';
    if (key === 'config.sidebar') return 'sidebar';
    return 'json';
  }

  private parseRawIntoForms(raw: any): void {
    switch (this.editorType) {
      case 'info':
        this.infoForm = {
          [CONTENT_LANG]: { ...emptyInfoLang(), ...(raw?.[CONTENT_LANG] ?? {}) },
          [DEFAULT_LANG]:  { ...emptyInfoLang(), ...(raw?.[DEFAULT_LANG] ?? {}) },
        };
        break;
      case 'social':
        this.socialItems = Array.isArray(raw) ? raw.map((s: any) => ({ ...s })) : [];
        break;
      case 'sidebar':
        this.sidebarItems = Array.isArray(raw) ? raw.map((s: any) => ({ ...s })) : [];
        break;
      default:
        if (raw && typeof raw === 'object' && !Array.isArray(raw))
          this.genericProps = Object.entries(raw).map(([k, v]) => ({ key: k, value: typeof v === 'object' ? JSON.stringify(v) : String(v ?? '') }));
        else
          this.genericProps = [];
    }
  }

  // ── Mode toggle ────────────────────────────────────────────────────────────

  switchMode(mode: ViewMode): void {
    if (mode === this.viewMode) return;
    if (mode === 'json') {
      this.jsonValue = JSON.stringify(this.buildValue(), null, 2);
      this.jsonError = '';
    } else {
      if (!this.trySyncJsonToForm()) return;
    }
    this.viewMode = mode;
  }

  private trySyncJsonToForm(): boolean {
    try {
      const raw = JSON.parse(this.jsonValue);
      this.parseRawIntoForms(raw);
      this.jsonError = '';
      return true;
    } catch (e: any) {
      this.jsonError = e.message;
      return false;
    }
  }

  // ── Build value for save ───────────────────────────────────────────────────

  private buildValue(): any {
    if (this.viewMode === 'json') {
      return JSON.parse(this.jsonValue);
    }
    switch (this.editorType) {
      case 'info':    return this.infoForm;
      case 'social':  return this.socialItems;
      case 'sidebar': return this.sidebarItems;
      default: {
        const obj: Record<string, any> = {};
        for (const p of this.genericProps) {
          try { obj[p.key] = JSON.parse(p.value); } catch { obj[p.key] = p.value; }
        }
        return obj;
      }
    }
  }

  // ── Helpers ────────────────────────────────────────────────────────────────

  get infoLang(): InfoLang {
    return this.infoForm[this.activeLang] ?? emptyInfoLang();
  }

  get jsonValid(): boolean {
    try { JSON.parse(this.jsonValue); return true; } catch { return false; }
  }

  formatJson(): void {
    try { this.jsonValue = JSON.stringify(JSON.parse(this.jsonValue), null, 2); this.jsonError = ''; }
    catch (e: any) { this.jsonError = e.message; }
  }

  addSocial(): void   { this.socialItems.push({ id: String(Date.now()), name: '', ref: '', icon: '' }); }
  removeSocial(i: number): void  { this.socialItems.splice(i, 1); }

  addSidebar(): void  { this.sidebarItems.push({ nameKey: '', ref: '' }); }
  removeSidebar(i: number): void { this.sidebarItems.splice(i, 1); }

  addGenericProp(): void  { this.genericProps.push({ key: '', value: '' }); }
  removeGenericProp(i: number): void { this.genericProps.splice(i, 1); }

  // ── Save ───────────────────────────────────────────────────────────────────

  save(): void {
    this.error = '';
    let value: any;
    try { value = this.buildValue(); }
    catch (e: any) { this.jsonError = e.message; return; }

    this.saving = true;
    this.saved  = false;
    this.svc.update({ id: this.configId, key: this.configKey, value }).subscribe({
      next: () => { this.saving = false; this.saved = true; this.toast.success(AdminMessage.TOAST_SAVE_SUCCESS); },
      error: () => { this.saving = false; this.error = AdminMessage.ERROR_GENERIC; this.toast.error(AdminMessage.TOAST_SAVE_ERROR); },
    });
  }

  back(): void { this.router.navigate(['/admin/sys-config']); }
}
