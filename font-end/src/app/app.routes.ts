import { Routes } from '@angular/router';
import {HomeComponent} from './features/home/home.component';
import {PhotographyComponent} from './features/photography/photography.component';
import {TravelComponent} from './features/travel/travel.component';
import {FashionComponent} from './features/fashion/fashion.component';
import {AboutComponent} from './features/about/about.component';
import {ContactComponent} from './features/contact/contact.component';
import {PostDetailComponent} from './features/post-detail/post-detail.component';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'home',
    component: HomeComponent
  },
  {
    path: 'photography',
    component: PhotographyComponent,
  },
  {
    path: 'travel',
    component: TravelComponent,
  },
  {
    path: 'fashion',
    component: FashionComponent,
  },
  {
    path: 'about',
    component: AboutComponent,
  },
  {
    path: 'contact',
    component: ContactComponent,
  },
  {
    path: 'post/:slug',
    component: PostDetailComponent,
  }
];
