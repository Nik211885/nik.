import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  // Login — client-side only
  { path: 'login', renderMode: RenderMode.Client },

  // Admin panel — client-side only (auth-guarded, no SSR needed)
  { path: 'admin', renderMode: RenderMode.Client },
  { path: 'admin/dashboard',             renderMode: RenderMode.Client },
  { path: 'admin/articles',              renderMode: RenderMode.Client },
  { path: 'admin/albums',                renderMode: RenderMode.Client },
  { path: 'admin/categories',            renderMode: RenderMode.Client },
  { path: 'admin/tags',                  renderMode: RenderMode.Client },
  { path: 'admin/comments',              renderMode: RenderMode.Client },
  { path: 'admin/files',                 renderMode: RenderMode.Client },
  { path: 'admin/users',                 renderMode: RenderMode.Client },
  { path: 'admin/languages',             renderMode: RenderMode.Client },
  { path: 'admin/translations',          renderMode: RenderMode.Client },
  { path: 'admin/sys-config',            renderMode: RenderMode.Client },
  { path: 'admin/sys-config/editor/:id', renderMode: RenderMode.Client },
  { path: 'admin/contacts',              renderMode: RenderMode.Client },
  { path: 'admin/page-views',            renderMode: RenderMode.Client },
  { path: 'admin/hero-slides',           renderMode: RenderMode.Client },
  { path: 'admin/content-translations',  renderMode: RenderMode.Client },
  { path: 'admin/wall-messages',         renderMode: RenderMode.Client },
  { path: 'admin/careers',               renderMode: RenderMode.Client },
  { path: 'admin/content-translations/editor/:entityType/:entityId/:lang', renderMode: RenderMode.Client },

  // All other routes — SSR on each request (meta tags in initial HTML = SEO benefit)
  { path: '**', renderMode: RenderMode.Server },
];
