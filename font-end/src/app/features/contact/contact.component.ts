import {Component, OnInit, inject, AfterViewInit} from '@angular/core';
import { ConfigService } from '../../core/services/config.service';
import { ApplicationTitle } from '../../app.message';
import { LanguagePipe } from '../../shared/pipes/language.pipe';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-contact',
  imports: [LanguagePipe, AsyncPipe],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.css',
})
export class ContactComponent implements OnInit {
  protected readonly configService = inject(ConfigService);
  protected readonly ApplicationTitle = ApplicationTitle;

  ngOnInit(): void {
    this.configService.config.subscribe((config) => {
      if (config?.infoPageConfig?.address) {
        this.loadGoogleMaps().then(() => {
          this.initMap(config.infoPageConfig.address);
        });
      }
    });
  }
  private loadGoogleMaps(): Promise<void> {
    return new Promise((resolve) => {
      if ((window as any)['google']) {
        resolve();
        return;
      }
      const script = document.createElement('script');
      script.src = 'https://maps.googleapis.com/maps/api/js?key=AIzaSyBVWaKrjvy3MaE7SQ74_uJiULgl1JY0H2s&sensor=false';
      script.onload = () => resolve();
      document.head.appendChild(script);
    });
  }
  private initMap(address: string): void {
    const google = (window as any)['google'];
    const mapOptions = {
      zoom: 15,
      center: new google.maps.LatLng(0, 0),
      scrollwheel: false,
      styles: [
        {
          featureType: 'administrative.country',
          elementType: 'geometry',
          stylers: [{ visibility: 'simplified' }, { hue: '#ff0000' }],
        },
      ],
    };
    const map = new google.maps.Map(document.getElementById('map'), mapOptions);
    fetch(
      `https://maps.googleapis.com/maps/api/geocode/json?address=${encodeURIComponent(address)}&sensor=false&key=AIzaSyBVWaKrjvy3MaE7SQ74_uJiULgl1JY0H2s`
    )
      .then((res) => res.json())
      .then((data) => {
        if (data.results?.length > 0) {
          const p = data.results[0].geometry.location;
          const latlng = new google.maps.LatLng(p.lat, p.lng);
          map.setCenter(latlng);
          new google.maps.Marker({ position: latlng, map });
        }
      });
  }
}
