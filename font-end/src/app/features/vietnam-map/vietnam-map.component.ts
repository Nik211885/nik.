import {
  Component, OnInit, AfterViewInit, OnDestroy,
  ViewChild, ElementRef, inject, PLATFORM_ID,
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { forkJoin } from 'rxjs';
import type { FeatureCollection, Feature } from 'geojson';
import type { PathOptions } from 'leaflet';
import { LanguagePipe } from '../../shared/pipes/language.pipe';
import { SeoService } from '../../core/services/seo.service';
import { VietnamMapService, ProvinceMapModel, TripMapModel } from '../../core/services/vietnam-map.service';
import { ApplicationTitle } from '../../app.message';

// GeoJSON NAME_1 values that differ from our Province.Name in the database
const GEOJSON_NAME_MAP: Record<string, string> = {
  'Hồ Chí Minh city': 'Thành phố Hồ Chí Minh',
  'Thừa Thiên - Huế': 'Thừa Thiên Huế',
  'Đăk Nông': 'Đắk Nông',
};

const UNVISITED_STYLE: PathOptions = { color: '#fff', weight: 1, fillColor: '#cbd5e1', fillOpacity: 0.85 };
const VISITED_STYLE: PathOptions  = { color: '#fff', weight: 1, fillColor: '#3b82f6', fillOpacity: 0.75 };
const HOVER_UNVISITED: PathOptions = { color: '#94a3b8', weight: 2, fillColor: '#94a3b8', fillOpacity: 0.9 };
const HOVER_VISITED: PathOptions   = { color: '#1d4ed8', weight: 2, fillColor: '#2563eb', fillOpacity: 0.9 };

@Component({
  selector: 'app-vietnam-map',
  standalone: true,
  imports: [LanguagePipe],
  templateUrl: './vietnam-map.component.html',
  styleUrl: './vietnam-map.component.css',
})
export class VietnamMapComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('mapEl') mapEl!: ElementRef<HTMLDivElement>;

  private readonly platformId = inject(PLATFORM_ID);
  protected readonly isBrowser = isPlatformBrowser(this.platformId);
  protected readonly ApplicationTitle = ApplicationTitle;

  provinces: ProvinceMapModel[] = [];
  selectedProvince: ProvinceMapModel | null = null;
  trips: TripMapModel[] = [];
  tripsLoading = false;
  drawerOpen = false;
  expandedTripId: string | null = null;

  get visitedCount(): number {
    return this.provinces.filter(p => p.tripCount > 0).length;
  }

  private leafletMap: import('leaflet').Map | null = null;
  private provinceByName = new Map<string, ProvinceMapModel>();

  constructor(
    private http: HttpClient,
    private seoService: SeoService,
    private mapService: VietnamMapService,
  ) {}

  ngOnInit(): void {
    this.seoService.set({
      title: 'Vietnam Map',
      description: 'My Vietnam travel journal — provinces I have visited and stories from each trip.',
      path: '/vietnam-map',
    });
  }

  ngAfterViewInit(): void {
    if (!this.isBrowser) return;

    forkJoin([
      this.mapService.getProvinces(),
      this.http.get<FeatureCollection>('/assets/vietnam-provinces.geojson', {
        headers: { 'X-Skip-Auth-Request': '1' },
      }),
    ]).subscribe({
      next: ([provinces, geoJson]) => {
        this.provinces = provinces;
        this.provinceByName = new Map(provinces.map(p => [p.name, p]));
        this.initLeafletMap(geoJson);
      },
    });
  }

  ngOnDestroy(): void {
    this.leafletMap?.remove();
    this.leafletMap = null;
  }

  private async initLeafletMap(geoJson: FeatureCollection): Promise<void> {
    const L = await import('leaflet');

    this.leafletMap = L.map(this.mapEl.nativeElement, {
      center: [16.5, 107.5],
      zoom: 5,
      zoomControl: true,
      scrollWheelZoom: true,
      attributionControl: false,
    });

    L.geoJSON(geoJson as any, {
      style: (feature) => this.featureStyle(feature),
      onEachFeature: (feature, layer) => this.bindEvents(feature, layer, L),
    }).addTo(this.leafletMap);
  }

  private resolveName(feature?: Feature): string {
    const raw = feature?.properties?.['NAME_1'] as string ?? '';
    return GEOJSON_NAME_MAP[raw] ?? raw;
  }

  private featureStyle(feature?: Feature): PathOptions {
    const name = this.resolveName(feature);
    const province = this.provinceByName.get(name);
    return province?.tripCount ? VISITED_STYLE : UNVISITED_STYLE;
  }

  private bindEvents(feature: Feature, layer: import('leaflet').Layer, L: typeof import('leaflet')): void {
    const name = this.resolveName(feature);
    const province = this.provinceByName.get(name);
    if (!province) return;

    const visited = province.tripCount > 0;
    const base = visited ? VISITED_STYLE : UNVISITED_STYLE;
    const hover = visited ? HOVER_VISITED : HOVER_UNVISITED;

    (layer as import('leaflet').Path).on({
      mouseover: (e: import('leaflet').LeafletMouseEvent) => {
        (e.target as import('leaflet').Path).setStyle(hover);
        (e.target as import('leaflet').Path).bringToFront();
      },
      mouseout: (e: import('leaflet').LeafletMouseEvent) => {
        (e.target as import('leaflet').Path).setStyle(base);
      },
      click: () => this.selectProvince(province),
    });

    (layer as import('leaflet').Path).bindTooltip(name, {
      permanent: false,
      direction: 'center',
      className: 'province-tooltip',
    });
  }

  selectProvince(province: ProvinceMapModel): void {
    this.selectedProvince = province;
    this.drawerOpen = true;
    this.expandedTripId = null;
    this.trips = [];

    if (province.tripCount === 0) return;

    this.tripsLoading = true;
    this.mapService.getTripsByProvince(province.id).subscribe({
      next: trips => { this.trips = trips; this.tripsLoading = false; },
      error: () => (this.tripsLoading = false),
    });
  }

  closeDrawer(): void {
    this.drawerOpen = false;
    setTimeout(() => { this.selectedProvince = null; this.trips = []; }, 300);
  }

  toggleTrip(id: string): void {
    this.expandedTripId = this.expandedTripId === id ? null : id;
  }
}
