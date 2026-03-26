import {AfterViewInit, Component} from '@angular/core';
import {ToastComponent} from './shared/components/toast/toast.component';
import {LoadingComponent} from './shared/components/loadding/loading.component';
import {MainLayoutComponent} from './layout/main-layout/main-layout.component';
import AOS from 'aos';
import 'aos/dist/aos.css';

@Component({
  selector: 'app-root',
  imports: [ToastComponent, LoadingComponent, MainLayoutComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
  standalone: true
})
export class App implements AfterViewInit {
  private resizeHandler!: () => void;
  private scrollHandler!: () => void;
  private clickHandler!: (e: MouseEvent) => void;
  private carouselTimer: any;
  private mutationObserver!: MutationObserver;
  private fallbackTimer: any;
  private called = false;

  ngAfterViewInit(): void {
    AOS.init({ duration: 800, easing: 'ease-in-out' });

    const run = () => {
      if (this.called) return;
      this.called = true;
      this.mutationObserver?.disconnect();

      this.initLoader();
      this.initFullHeight();
      this.initParallax();
      this.initBurgerMenu();
      this.initMobileMenuOutsideClick();
      this.initCarousel();
      this.initScrollAnimate();
      this.initLightbox();
    };

    this.mutationObserver = new MutationObserver(() => {
      const ready =
        document.querySelector('.home-slider') ||
        document.querySelectorAll('.ftco-animate').length > 0;
      if (ready) run();
    });

    this.mutationObserver.observe(document.body, { childList: true, subtree: true });
    this.fallbackTimer = setTimeout(run, 2000);
  }

  ngOnDestroy(): void {
    window.removeEventListener('resize', this.resizeHandler);
    window.removeEventListener('scroll', this.scrollHandler);
    document.removeEventListener('click', this.clickHandler as EventListener);
    this.mutationObserver?.disconnect();
    clearInterval(this.carouselTimer);
    clearTimeout(this.fallbackTimer);
  }

  private initLoader(): void {
    setTimeout(() => {
      const loader = document.getElementById('ftco-loader');
      if (loader) loader.classList.remove('show');
    }, 1);
  }

  private initFullHeight(): void {
    this.resizeHandler = () => {
      document.querySelectorAll<HTMLElement>('.js-fullheight').forEach(el => {
        el.style.height = window.innerHeight + 'px';
      });
    };
    this.resizeHandler();
    window.addEventListener('resize', this.resizeHandler);
  }

  private initParallax(): void {
    const els = document.querySelectorAll<HTMLElement>('[data-stellar-background-ratio]');
    if (!els.length) return;

    const applyParallax = () => {
      const scrollY = window.scrollY || window.pageYOffset;
      els.forEach(el => {
        const ratio = parseFloat(el.getAttribute('data-stellar-background-ratio') || '0.5') || 0.5;
        el.style.backgroundPosition = `center ${scrollY * ratio}px`;
      });
    };

    applyParallax();
    window.addEventListener('scroll', applyParallax, { passive: true });
  }

  private initScrollAnimate(): void {
    const elements = document.querySelectorAll<HTMLElement>('.ftco-animate');
    if (!elements.length) return;

    const observer = new IntersectionObserver(entries => {
      let delay = 0;
      entries.forEach(entry => {
        if (entry.isIntersecting && !entry.target.classList.contains('ftco-animated')) {
          const el = entry.target as HTMLElement;
          el.classList.add('item-animate');
          setTimeout(() => {
            const effect = el.dataset['animateEffect'];
            const cls =
              effect === 'fadeIn'      ? 'fadeIn'      :
                effect === 'fadeInLeft'  ? 'fadeInLeft'  :
                  effect === 'fadeInRight' ? 'fadeInRight' : 'fadeInUp';
            el.classList.add(cls, 'ftco-animated');
            el.classList.remove('item-animate');
          }, delay);
          delay += 50;
        }
      });
    }, { threshold: 0.05 });

    elements.forEach(el => observer.observe(el));
  }

  private initBurgerMenu(): void {
    document.querySelectorAll<HTMLElement>('.js-colorlib-nav-toggle').forEach(btn => {
      btn.addEventListener('click', e => {
        e.preventDefault();
        const isOpen = document.body.classList.contains('offcanvas');
        btn.classList.toggle('active', !isOpen);
        document.body.classList.toggle('offcanvas', !isOpen);
      });
    });
  }

  private initMobileMenuOutsideClick(): void {
    const closeMenu = () => {
      document.body.classList.remove('offcanvas');
      document.querySelectorAll('.js-colorlib-nav-toggle').forEach(btn => {
        btn.classList.remove('active');
      });
    };

    this.clickHandler = (e: MouseEvent) => {
      const aside   = document.getElementById('colorlib-aside');
      const toggles = document.querySelectorAll('.js-colorlib-nav-toggle');
      const inside  =
        (aside && (aside === e.target || aside.contains(e.target as Node))) ||
        Array.from(toggles).some(btn => btn === e.target || btn.contains(e.target as Node));
      if (!inside && document.body.classList.contains('offcanvas')) closeMenu();
    };

    this.scrollHandler = () => {
      if (document.body.classList.contains('offcanvas')) closeMenu();
    };

    document.addEventListener('click', this.clickHandler as EventListener);
    window.addEventListener('scroll', this.scrollHandler, { passive: true });
  }

  private initCarousel(): void {
    const slider = document.querySelector<HTMLElement>('.home-slider');
    if (!slider) return;

    const slides  = Array.from(slider.children) as HTMLElement[];
    const total   = slides.length;
    let   current = 0;

    slides.forEach(s => {
      s.style.cssText = 'position:absolute;inset:0;opacity:0;transition:opacity 0.8s ease';
    });
    slider.style.position = 'relative';
    slider.style.overflow = 'hidden';

    const goTo = (index: number) => {
      slides[current].style.opacity   = '0';
      slides[current].style.position  = 'absolute';
      current = (index + total) % total;
      slides[current].style.opacity   = '1';
      slides[current].style.position  = 'relative';
    };

    goTo(0);
    this.carouselTimer = setInterval(() => goTo(current + 1), 5000);
  }

  private initLightbox(): void {
    const imageLinks = Array.from(document.querySelectorAll<HTMLAnchorElement>('.image-popup'));

    if (imageLinks.length) {
      const overlay  = document.createElement('div');
      const img      = document.createElement('img');
      const btnClose = document.createElement('button');
      const btnPrev  = document.createElement('button');
      const btnNext  = document.createElement('button');

      overlay.style.cssText =
        'display:none;position:fixed;inset:0;background:rgba(0,0,0,.88);' +
        'z-index:9999;align-items:center;justify-content:center;cursor:zoom-out';
      img.style.cssText = 'max-width:90vw;max-height:90vh;border-radius:4px';
      btnClose.textContent = '×';
      btnClose.style.cssText =
        'position:absolute;top:16px;right:24px;background:none;border:none;' +
        'color:#fff;font-size:40px;cursor:pointer;line-height:1';

      [btnPrev, btnNext].forEach((b, i) => {
        b.textContent = i === 0 ? '‹' : '›';
        b.style.cssText =
          `position:absolute;${i === 0 ? 'left:16px' : 'right:16px'};` +
          'top:50%;transform:translateY(-50%);background:none;border:none;' +
          'color:#fff;font-size:56px;cursor:pointer;line-height:1';
      });

      overlay.append(btnClose, btnPrev, img, btnNext);
      document.body.appendChild(overlay);

      let currentIndex = 0;

      const openAt = (index: number) => {
        currentIndex = (index + imageLinks.length) % imageLinks.length;
        img.src = imageLinks[currentIndex].href || imageLinks[currentIndex].dataset['src'] || '';
        overlay.style.display = 'flex';
        document.body.style.overflow = 'hidden';
      };
      const closeOverlay = () => {
        overlay.style.display = 'none';
        document.body.style.overflow = '';
        img.src = '';
      };

      imageLinks.forEach((link, i) => {
        link.addEventListener('click', e => { e.preventDefault(); openAt(i); });
      });
      btnClose.addEventListener('click', closeOverlay);
      btnPrev.addEventListener('click',  e => { e.stopPropagation(); openAt(currentIndex - 1); });
      btnNext.addEventListener('click',  e => { e.stopPropagation(); openAt(currentIndex + 1); });
      overlay.addEventListener('click',  e => { if (e.target === overlay) closeOverlay(); });
      document.addEventListener('keydown', e => {
        if (overlay.style.display !== 'flex') return;
        if (e.key === 'Escape')     closeOverlay();
        if (e.key === 'ArrowLeft')  openAt(currentIndex - 1);
        if (e.key === 'ArrowRight') openAt(currentIndex + 1);
      });
    }

    const iframeLinks = document.querySelectorAll<HTMLAnchorElement>('.popup-youtube, .popup-vimeo, .popup-gmaps');
    if (!iframeLinks.length) return;

    const iframeOverlay = document.createElement('div');
    const iframeWrap    = document.createElement('div');
    const iframe        = document.createElement('iframe');
    const iframeClose   = document.createElement('button');

    iframeOverlay.style.cssText =
      'display:none;position:fixed;inset:0;background:rgba(0,0,0,.85);' +
      'z-index:9999;align-items:center;justify-content:center';
    iframeWrap.style.cssText = 'position:relative;width:90vw;max-width:900px;aspect-ratio:16/9';
    iframe.style.cssText = 'width:100%;height:100%;border:none;border-radius:4px';
    iframe.setAttribute('allowfullscreen', '');
    iframeClose.textContent = '×';
    iframeClose.style.cssText =
      'position:absolute;top:-40px;right:0;background:none;border:none;' +
      'color:#fff;font-size:36px;cursor:pointer;line-height:1';

    iframeWrap.append(iframeClose, iframe);
    iframeOverlay.appendChild(iframeWrap);
    document.body.appendChild(iframeOverlay);

    const closeIframe = () => {
      iframeOverlay.style.display = 'none';
      iframe.src = '';
      document.body.style.overflow = '';
    };

    iframeLinks.forEach(link => {
      link.addEventListener('click', e => {
        e.preventDefault();
        if (window.innerWidth < 700) { window.open(link.href, '_blank'); return; }
        iframe.src = link.href;
        iframeOverlay.style.display = 'flex';
        document.body.style.overflow = 'hidden';
      });
    });
    iframeClose.addEventListener('click', closeIframe);
    iframeOverlay.addEventListener('click', e => { if (e.target === iframeOverlay) closeIframe(); });
    document.addEventListener('keydown', e => {
      if (e.key === 'Escape' && iframeOverlay.style.display === 'flex') closeIframe();
    });
  }
}
