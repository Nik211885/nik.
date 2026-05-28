import { inject, Injectable } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';
import { DOCUMENT } from '@angular/common';
import { environment } from '../../../environments/environment';

export interface SeoConfig {
  title?: string;
  description: string;
  image?: string;
  path?: string;
  type?: 'website' | 'article' | 'profile';
  structuredData?: object | object[];
  lang?: string;
}

const SITE_NAME = 'Nik.';
const SITE_FULL_TITLE = 'Nik. — Stories, Travel & Creativity';
const DEFAULT_OG_IMAGE = `${environment.siteUrl}/assets/images/og-default.jpg`;
const SUPPORTED_LANGS = ['en', 'vi'];

@Injectable({ providedIn: 'root' })
export class SeoService {
  private readonly meta = inject(Meta);
  private readonly titleSvc = inject(Title);
  private readonly doc = inject(DOCUMENT);

  set(config: SeoConfig): void {
    const title = config.title ? `${config.title} | ${SITE_NAME}` : SITE_FULL_TITLE;
    const description = config.description;
    const image = config.image ?? DEFAULT_OG_IMAGE;
    const url = config.path
      ? `${environment.siteUrl}${config.path === '/' ? '' : config.path}`
      : environment.siteUrl;
    const canonicalUrl = url || environment.siteUrl;
    const type = config.type ?? 'website';

    this.titleSvc.setTitle(title);

    this.meta.updateTag({ name: 'description', content: description });
    this.meta.updateTag({ name: 'robots', content: 'index, follow' });

    this.meta.updateTag({ property: 'og:title', content: title });
    this.meta.updateTag({ property: 'og:description', content: description });
    this.meta.updateTag({ property: 'og:image', content: image });
    this.meta.updateTag({ property: 'og:url', content: canonicalUrl });
    this.meta.updateTag({ property: 'og:type', content: type });
    this.meta.updateTag({ property: 'og:site_name', content: SITE_NAME });
    this.meta.updateTag({ property: 'og:locale', content: 'en_US' });

    this.meta.updateTag({ name: 'twitter:card', content: 'summary_large_image' });
    this.meta.updateTag({ name: 'twitter:title', content: title });
    this.meta.updateTag({ name: 'twitter:description', content: description });
    this.meta.updateTag({ name: 'twitter:image', content: image });

    this.setCanonical(canonicalUrl);
    this.setHreflang(canonicalUrl);

    if (config.lang) {
      this.doc.documentElement.setAttribute('lang', config.lang);
    }

    if (config.structuredData) {
      const data = Array.isArray(config.structuredData)
        ? config.structuredData
        : [config.structuredData];
      this.setJsonLd(data);
    }
  }

  setJsonLd(data: object[]): void {
    this.doc.querySelectorAll('script[type="application/ld+json"]').forEach(s => s.remove());
    data.forEach(item => {
      const script = this.doc.createElement('script');
      script.type = 'application/ld+json';
      script.text = JSON.stringify(item);
      this.doc.head.appendChild(script);
    });
  }

  private setCanonical(url: string): void {
    let link = this.doc.querySelector<HTMLLinkElement>('link[rel="canonical"]');
    if (!link) {
      link = this.doc.createElement('link');
      link.rel = 'canonical';
      this.doc.head.appendChild(link);
    }
    link.href = url;
  }

  private setHreflang(url: string): void {
    this.doc.querySelectorAll('link[rel="alternate"][hreflang]').forEach(el => el.remove());

    SUPPORTED_LANGS.forEach(lang => {
      const link = this.doc.createElement('link');
      link.rel = 'alternate';
      link.setAttribute('hreflang', lang);
      link.href = url;
      this.doc.head.appendChild(link);
    });

    const xDefault = this.doc.createElement('link');
    xDefault.rel = 'alternate';
    xDefault.setAttribute('hreflang', 'x-default');
    xDefault.href = url;
    this.doc.head.appendChild(xDefault);
  }
}
