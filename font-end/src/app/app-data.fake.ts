import {SidebarStatItemModel} from './shared/components/sidebar-list/sidebar-stat-item.model';
import {TagCloudModel} from './shared/components/tag-cloud/tag-cloud.model';

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
