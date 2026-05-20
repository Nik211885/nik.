import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { HomeComponent } from './features/home/home.component';
import { PhotographyComponent } from './features/photography/photography.component';
import { TravelComponent } from './features/travel/travel.component';
import { FashionComponent } from './features/fashion/fashion.component';
import { AboutComponent } from './features/about/about.component';
import { ContactComponent } from './features/contact/contact.component';
import { PostDetailComponent } from './features/post-detail/post-detail.component';
import { authGuard } from './core/gurads/auth.guard';

export const routes: Routes = [
  // Login — standalone, không có layout
  {
    path: 'login',
    loadComponent: () => import('./features/login/login.component').then(m => m.LoginComponent)
  },

  // Admin panel — lazy-loaded, protected by authGuard
  {
    path: 'admin',
    loadComponent: () => import('./admin/layout/admin-layout.component').then(m => m.AdminLayoutComponent),
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard',    loadComponent: () => import('./admin/features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'articles',     loadComponent: () => import('./admin/features/articles/articles.component').then(m => m.ArticlesAdminComponent) },
      { path: 'albums',       loadComponent: () => import('./admin/features/albums/albums.component').then(m => m.AlbumsAdminComponent) },
      { path: 'categories',   loadComponent: () => import('./admin/features/categories/categories.component').then(m => m.CategoriesAdminComponent) },
      { path: 'tags',         loadComponent: () => import('./admin/features/tags/tags.component').then(m => m.TagsAdminComponent) },
      { path: 'comments',     loadComponent: () => import('./admin/features/comments/comments.component').then(m => m.CommentsAdminComponent) },
      { path: 'files',        loadComponent: () => import('./admin/features/files/files.component').then(m => m.FilesAdminComponent) },
      { path: 'users',        loadComponent: () => import('./admin/features/users/users.component').then(m => m.UsersAdminComponent) },
      { path: 'languages',    loadComponent: () => import('./admin/features/languages/languages.component').then(m => m.LanguagesAdminComponent) },
      { path: 'translations', loadComponent: () => import('./admin/features/translations/translations.component').then(m => m.TranslationsAdminComponent) },
      { path: 'sys-config',              loadComponent: () => import('./admin/features/sys-config/sys-config.component').then(m => m.SysConfigAdminComponent) },
      { path: 'sys-config/editor/:id',   loadComponent: () => import('./admin/features/sys-config/sys-config-editor-page/sys-config-editor-page.component').then(m => m.SysConfigEditorPageComponent) },
      { path: 'contacts',     loadComponent: () => import('./admin/features/contacts/contacts.component').then(m => m.ContactsAdminComponent) },
      { path: 'page-views',   loadComponent: () => import('./admin/features/page-views/page-views.component').then(m => m.PageViewsAdminComponent) },
      { path: 'hero-slides',         loadComponent: () => import('./admin/features/hero-slides/hero-slides.component').then(m => m.HeroSlidesAdminComponent) },
      { path: 'content-translations', loadComponent: () => import('./admin/features/content-translations/content-translations.component').then(m => m.ContentTranslationsAdminComponent) },
      { path: 'content-translations/editor/:entityType/:entityId/:lang', loadComponent: () => import('./admin/features/content-translations/translation-editor-page/translation-editor-page.component').then(m => m.TranslationEditorPageComponent) },
    ]
  },

  // Public site — wrapped in MainLayoutComponent
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '',            component: HomeComponent },
      { path: 'home',        component: HomeComponent },
      { path: 'photography', component: PhotographyComponent },
      { path: 'travel',      component: TravelComponent },
      { path: 'fashion',     component: FashionComponent },
      { path: 'about',       component: AboutComponent },
      { path: 'contact',     component: ContactComponent },
      { path: 'post/:prefix/:slug', component: PostDetailComponent },
      { path: 'sponsor', loadComponent: () => import('./features/sponsor/sponsor.component').then(m => m.SponsorComponent) },
    ]
  },

  { path: '**', redirectTo: '' }
];
