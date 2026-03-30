import {AfterViewInit, Component, Input, OnDestroy} from '@angular/core';
import {HeroCarouselModel} from './hero-carousel.model';

@Component({
  selector: 'app-hero-carousel',
  imports: [],
  templateUrl: './hero-carousel.component.html',
  styleUrl: './hero-carousel.component.css',
})
export class HeroCarouselComponent implements AfterViewInit, OnDestroy{
  private carouselTimer: any;
  @Input() listCarousel: HeroCarouselModel[] = [
    {
      id: '1',
      img: '/assets/images/img.png',
      title: 'Discover the Place',
      description: 'Find great places to stay, eat, shop, or visit from local experts'
    },
    {
      id: '2',
      img: '/assets/images/img_1.png',
      title: 'Explore and travel',
      description: 'Find great places to stay, eat, shop, or visit from local experts'
    }
  ];
  ngOnDestroy(): void {
    clearInterval(this.carouselTimer);
  }
  ngAfterViewInit(): void {
    this.initCarousel();
  }
  private initCarousel(): void {
    const slider = document.querySelector<HTMLElement>('.home-slider');
    if (!slider) { setTimeout(() => this.initCarousel(), 100); return; }

    const slides = Array.from(slider.querySelectorAll<HTMLElement>('.slider-item'));
    if (!slides.length) { setTimeout(() => this.initCarousel(), 100); return; }

    const total = slides.length;
    let current = 0;
    let isAnimating = false;
    const DURATION = 7000;

    slider.style.display = 'block';
    slider.style.position = 'relative';
    slider.style.overflow = 'hidden';

    slides.forEach((s, i) => {
      s.style.position = 'absolute';
      s.style.inset = '0';
      s.style.height = '100%';
      s.style.width = '100%';
      s.style.backgroundSize = 'cover';
      s.style.backgroundPosition = 'center';
      s.style.transition = 'transform 0.9s cubic-bezier(0.76, 0, 0.24, 1)';
      s.style.zIndex = '0';
      s.style.transform = i === 0 ? 'translateX(0)' : 'translateX(100%)';
    });
    slides[0].style.zIndex = '1';

    const goTo = (index: number, direction: 'next' | 'prev' = 'next') => {
      if (isAnimating) return;
      isAnimating = true;

      const prev = current;
      current = (index + total) % total;

      const incoming = direction === 'next' ? '100%' : '-100%';
      const outgoing = direction === 'next' ? '-30%' : '30%';

      slides[current].style.transition = 'none';
      slides[current].style.transform = `translateX(${incoming})`;
      slides[current].style.zIndex = '2';

      requestAnimationFrame(() => {
        requestAnimationFrame(() => {
          slides[current].style.transition = 'transform 0.9s cubic-bezier(0.76, 0, 0.24, 1)';
          slides[current].style.transform = 'translateX(0)';
          slides[prev].style.zIndex = '1';
          slides[prev].style.transform = `translateX(${outgoing})`;

          setTimeout(() => {
            // Reset TẤT CẢ slides về đúng vị trí tương đối với current
            slides.forEach((s, i) => {
              if (i === current) return;
              s.style.transition = 'none';
              s.style.zIndex = '0';
              s.style.transform = i < current ? 'translateX(-100%)' : 'translateX(100%)';
            });
            isAnimating = false;
          }, 900);
        });
      });

      updateDots();
      updateCounter();
      resetProgress();
    };

    slider.querySelector('.__carousel-ui')?.remove();
    const ui = document.createElement('div');
    ui.className = '__carousel-ui';
    ui.style.cssText = `
    position:absolute;bottom:32px;left:50%;transform:translateX(-50%);
    z-index:10;display:flex;flex-direction:column;align-items:center;gap:12px;
  `;

    const progressWrap = document.createElement('div');
    progressWrap.style.cssText = `
    width:160px;height:2px;background:rgba(255,255,255,0.25);
    border-radius:2px;overflow:hidden;
  `;
    const progressBar = document.createElement('div');
    progressBar.style.cssText = `
    height:100%;width:0%;background:#fff;border-radius:2px;
  `;
    progressWrap.appendChild(progressBar);

    const dotsWrap = document.createElement('div');
    dotsWrap.style.cssText = `display:flex;gap:10px;align-items:center;`;

    const dots = slides.map((_, i) => {
      const dot = document.createElement('button');
      dot.style.cssText = `
      width:6px;height:6px;border-radius:50%;
      border:none;background:rgba(255,255,255,0.4);
      cursor:pointer;padding:0;
      transition:background 0.3s,transform 0.3s;
    `;
      dot.addEventListener('click', (e) => {
        e.stopPropagation();
        if (i === current) return;
        const dir = i > current ? 'next' : 'prev';
        clearInterval(this.carouselTimer);
        goTo(i, dir);
        startTimer();
      });
      dotsWrap.appendChild(dot);
      return dot;
    });

    const counter = document.createElement('span');
    counter.style.cssText = `
    color:rgba(255,255,255,0.6);font-size:11px;
    font-family:monospace;letter-spacing:2px;
  `;

    const updateDots = () => {
      dots.forEach((dot, i) => {
        dot.style.background = i === current ? '#fff' : 'rgba(255,255,255,0.4)';
        dot.style.transform = i === current ? 'scale(1.6)' : 'scale(1)';
      });
    };

    const updateCounter = () => {
      counter.textContent = `${String(current + 1).padStart(2, '0')} / ${String(total).padStart(2, '0')}`;
    };

    ui.append(progressWrap, dotsWrap, counter);
    slider.appendChild(ui);

    let progressRaf: any;
    let progressStart = 0;

    const resetProgress = () => {
      cancelAnimationFrame(progressRaf);
      progressBar.style.transition = 'none';
      progressBar.style.width = '0%';
      requestAnimationFrame(() => {
        progressStart = performance.now();
        const tick = (now: number) => {
          const pct = Math.min(((now - progressStart) / DURATION) * 100, 100);
          progressBar.style.width = pct + '%';
          if (pct < 100) progressRaf = requestAnimationFrame(tick);
        };
        progressRaf = requestAnimationFrame(tick);
      });
    };

    const startTimer = () => {
      clearInterval(this.carouselTimer);
      this.carouselTimer = setInterval(() => goTo(current + 1, 'next'), DURATION);
      resetProgress();
    };

    let startX = 0;
    let isDragging = false;

    const onStart = (x: number) => { startX = x; isDragging = true; };
    const onEnd = (x: number) => {
      if (!isDragging) return;
      isDragging = false;
      const diff = startX - x;
      if (Math.abs(diff) < 50) return;
      const dir = diff > 0 ? 'next' : 'prev';
      clearInterval(this.carouselTimer);
      goTo(dir === 'next' ? current + 1 : current - 1, dir);
      startTimer();
    };

    slider.addEventListener('mousedown', (e) => onStart(e.clientX));
    slider.addEventListener('mouseup', (e) => onEnd(e.clientX));
    slider.addEventListener('mouseleave', () => { isDragging = false; });
    slider.addEventListener('touchstart', (e) => onStart(e.touches[0].clientX), { passive: true });
    slider.addEventListener('touchend', (e) => onEnd(e.changedTouches[0].clientX), { passive: true });

    slider.addEventListener('mouseenter', () => {
      clearInterval(this.carouselTimer);
      cancelAnimationFrame(progressRaf);
    });
    slider.addEventListener('mouseleave', () => startTimer());
    updateDots();
    updateCounter();
    startTimer();
  }
}
