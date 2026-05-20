import { Component } from '@angular/core';
import { ApplicationTitle } from '../../app.message';
import { LanguagePipe } from '../../shared/pipes/language.pipe';

@Component({
  selector: 'app-sponsor',
  standalone: true,
  imports: [LanguagePipe],
  templateUrl: './sponsor.component.html',
  styleUrl: './sponsor.component.css',
})
export class SponsorComponent {
  protected readonly ApplicationTitle = ApplicationTitle;

  readonly qrUrl =
    'https://img.vietqr.io/image/CAKE-0388080661-compact2.png?accountName=LE%20KHAC%20NINH&addInfo=Ung%20ho%20Nik%20App';
  readonly bankName = 'CAKE by VPBank';
  readonly accountName = 'LÊ KHẮC NINH';
  readonly accountNumber = '0388080661';

  readonly payApps = [
    { name: 'MoMo',    color: '#ae2070', deepLink: 'momo://payment?phone=0388080661',     web: 'https://momo.vn' },
    { name: 'ZaloPay', color: '#0068ff', deepLink: 'zalopay://transfer?phone=0388080661', web: 'https://zalopay.vn' },
    { name: 'CAKE',    color: '#ff6b00', deepLink: 'cake://',                              web: 'https://cake.vn' },
  ];

  isMobile = /Mobi|Android|iPhone/i.test(navigator.userAgent);
  copied = false;

  copyAccount(): void {
    navigator.clipboard.writeText(this.accountNumber).then(() => {
      this.copied = true;
      setTimeout(() => (this.copied = false), 2000);
    });
  }

  openApp(deepLink: string, web: string): void {
    const start = Date.now();
    window.location.href = deepLink;
    setTimeout(() => {
      if (Date.now() - start < 2500) window.open(web, '_blank');
    }, 1500);
  }
}
