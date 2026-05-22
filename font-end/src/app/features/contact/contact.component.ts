import { Component, OnInit, inject } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ConfigService } from '../../core/services/config.service';
import { ContactService } from '../../core/services/contact.service';
import { ApplicationTitle } from '../../app.message';
import { LanguagePipe } from '../../shared/pipes/language.pipe';
import { AsyncPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SeoService } from '../../core/services/seo.service';

@Component({
  selector: 'app-contact',
  imports: [LanguagePipe, AsyncPipe, FormsModule],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.css',
})
export class ContactComponent implements OnInit {
  protected readonly configService = inject(ConfigService);
  protected readonly ApplicationTitle = ApplicationTitle;
  private readonly sanitizer = inject(DomSanitizer);
  private readonly seoService = inject(SeoService);

  name = '';
  email = '';
  subject = '';
  message = '';
  submitting = false;
  success = false;
  error = false;
  mapSrc: SafeResourceUrl | null = null;

  constructor(private contactService: ContactService) {}

  ngOnInit(): void {
    this.seoService.set({ title: 'Contact', description: 'Get in touch — send a message or find contact information.', path: '/contact' });

    this.configService.config.subscribe((config) => {
      if (config?.info?.address) {
        const url = `https://maps.google.com/maps?q=${encodeURIComponent(config.info.address)}&output=embed`;
        this.mapSrc = this.sanitizer.bypassSecurityTrustResourceUrl(url);
      }
    });
  }

  submit(): void {
    if (!this.name.trim() || !this.email.trim() || !this.subject.trim() || !this.message.trim()) return;
    this.submitting = true;
    this.success = false;
    this.error = false;

    this.contactService.send({
      name: this.name.trim(),
      email: this.email.trim(),
      subject: this.subject.trim(),
      message: this.message.trim()
    }).subscribe({
      next: () => {
        this.submitting = false;
        this.success = true;
        this.name = '';
        this.email = '';
        this.subject = '';
        this.message = '';
      },
      error: () => {
        this.submitting = false;
        this.error = true;
      }
    });
  }

}
