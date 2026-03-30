import {SidebarStatItemModel} from './shared/components/sidebar-list/sidebar-stat-item.model';
import {TagCloudModel} from './shared/components/tag-cloud/tag-cloud.model';
import {PhotoMasonryGridModel} from './shared/components/photo-masonry-grid/photo-masonry-grid.model';

export const statisticsCategories: SidebarStatItemModel[] = [
  {
    id: '1',
    count: 6,
    name: 'Fashion',
    slug: 'fashion'
  },
  {
    id: '2',
    count: 8,
    name: 'Technology',
    slug: 'technology'
  },
  {
    id: '3',
    count: 2,
    name: 'Travel',
    slug: 'travel'
  },
  {
    id: '4',
    count: 2,
    name: 'Food',
    slug: 'food'
  },
  {
    id: '5',
    count: 7,
    name: 'Photography',
    slug: 'photography'
  }
]

export const listTagCloud: TagCloudModel[] = [
  { id: '1', ref: '/tag/animals', name: 'animals' },
  { id: '2', ref: '/tag/human', name: 'human' },
  { id: '3', ref: '/tag/people', name: 'people' },
  { id: '4', ref: '/tag/cat', name: 'cat' },
  { id: '5', ref: '/tag/dog', name: 'dog' },
  { id: '6', ref: '/tag/nature', name: 'nature' },
  { id: '7', ref: '/tag/leaves', name: 'leaves' },
  { id: '8', ref: '/tag/food', name: 'food' }
];

export const statisticsArchives: SidebarStatItemModel[] = [
  {
    id: '1',
    name: 'October 2018',
    slug: '/archive/2018/10',
    count: 10
  },
  {
    id: '2',
    name: 'September 2018',
    slug: '/archive/2018/09',
    count: 6
  },
  {
    id: '3',
    name: 'August 2018',
    slug: '/archive/2018/08',
    count: 8
  },
  {
    id: '4',
    name: 'July 2018',
    slug: '/archive/2018/07',
    count: 2
  },
  {
    id: '5',
    name: 'June 2018',
    slug: '/archive/2018/06',
    count: 7
  },
  {
    id: '6',
    name: 'May 2018',
    slug: '/archive/2018/05',
    count: 5
  }
];

export const photoLocationMasonryGrid: PhotoMasonryGridModel[] = [
  {
    id: '1',
    ref: 'ha-noi',
    name: 'Hà Nội',
    count: 128,
    image: 'https://images.unsplash.com/photo-1598935898639-81586f7d2129?w=800'
  },
  {
    id: '2',
    ref: 'ha-long',
    name: 'Hạ Long',
    count: 245,
    image: 'https://images.unsplash.com/photo-1528127269322-539801943592?w=800'
  },
  {
    id: '3',
    ref: 'hoi-an',
    name: 'Hội An',
    count: 312,
    image: 'https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800'
  },
  {
    id: '4',
    ref: 'da-nang',
    name: 'Đà Nẵng',
    count: 198,
    image: 'https://images.unsplash.com/photo-1567942712661-82b9b407abbf?w=800'
  },
  {
    id: '5',
    ref: 'sapa',
    name: 'Sa Pa',
    count: 276,
    image: 'https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800'
  },
  {
    id: '6',
    ref: 'ho-chi-minh',
    name: 'Hồ Chí Minh',
    count: 354,
    image: 'https://images.unsplash.com/photo-1583417319070-4a69db38a482?w=800'
  },
  {
    id: '7',
    ref: 'nha-trang',
    name: 'Nha Trang',
    count: 189,
    image: 'https://images.unsplash.com/photo-1596422846543-75c6fc197f07?w=800'
  },
  {
    id: '8',
    ref: 'phu-quoc',
    name: 'Phú Quốc',
    count: 421,
    image: 'https://images.unsplash.com/photo-1537996194471-e657df975ab4?w=800'
  },
  {
    id: '9',
    ref: 'da-lat',
    name: 'Đà Lạt',
    count: 267,
    image: 'https://images.unsplash.com/photo-1571366343168-631c5bcca7a4?w=800'
  },
  {
    id: '10',
    ref: 'hue',
    name: 'Huế',
    count: 143,
    image: 'https://images.unsplash.com/photo-1555400038-63f5ba517a47?w=800'
  },
  {
    id: '11',
    ref: 'mui-ne',
    name: 'Mũi Né',
    count: 176,
    image: 'https://images.unsplash.com/photo-1546412414-8035e1776c9a?w=800'
  },
  {
    id: '12',
    ref: 'ninh-binh',
    name: 'Ninh Bình',
    count: 209,
    image: 'https://images.unsplash.com/photo-1528360983277-13d401cdc186?w=800'
  }
];
