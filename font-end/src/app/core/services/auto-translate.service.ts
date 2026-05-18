import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { franc } from 'franc-min';
import { CONTENT_LANG } from './language.service';

// franc uses ISO 639-3; MyMemory uses ISO 639-1
// Extend this map when adding new languages to the Languages table
const ISO3: Record<string, string> = {
  vie: 'vi', eng: 'en', fra: 'fr', deu: 'de',
  jpn: 'ja', kor: 'ko', zho: 'zh', spa: 'es',
  por: 'pt', ita: 'it', rus: 'ru', ara: 'ar',
  tha: 'th', ind: 'id', msa: 'ms', nld: 'nl',
};

function detectLang(text: string): string {
  const code = franc(text, { minLength: 10 });
  return code !== 'und' ? (ISO3[code] ?? code.slice(0, 2)) : CONTENT_LANG;
}

interface QueueItem {
  text: string;
  from: string;
  to: string;
  key: string;
  resolve: (v: string) => void;
}

@Injectable({ providedIn: 'root' })
export class AutoTranslateService {
  private cache = new Map<string, string>();
  private queue = new Subject<QueueItem>();
  private readonly API = 'https://api.mymemory.translated.net/get';

  constructor(private http: HttpClient) {
    this.queue.pipe(mergeMap(item => this.call(item), 3)).subscribe();
  }

  /**
   * @param text  Text to translate
   * @param to    Target language code
   * @param from  Source language — detected automatically if omitted
   */
  translate(text: string, to: string, from?: string): Observable<string> {
    if (!text?.trim()) return of(text);

    const sourceLang = from ?? detectLang(text);
    if (sourceLang === to) return of(text);

    const key = `${sourceLang}|${to}::${text}`;
    if (this.cache.has(key)) return of(this.cache.get(key)!);

    return new Observable(observer => {
      this.queue.next({
        text, from: sourceLang, to, key,
        resolve: v => { observer.next(v); observer.complete(); }
      });
    });
  }

  private call(item: QueueItem): Observable<void> {
    const url = `${this.API}?q=${encodeURIComponent(item.text)}&langpair=${item.from}|${item.to}`;
    return this.http.get<{ responseData: { translatedText: string } }>(url, {
      headers: { 'X-Skip-Auth-Request': 'true' }
    }).pipe(
      map(r => {
        const raw = r?.responseData?.translatedText ?? item.text;
        const result = raw.startsWith('MYMEMORY WARNING') ? item.text : raw.trim();
        this.cache.set(item.key, result);
        item.resolve(result);
      }),
      catchError(() => {
        this.cache.set(item.key, item.text);
        item.resolve(item.text);
        return of(undefined);
      })
    );
  }
}
