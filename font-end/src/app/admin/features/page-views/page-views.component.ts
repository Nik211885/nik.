import {
  Component, OnInit, OnDestroy, AfterViewInit,
  ViewChild, ElementRef, ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Chart, registerables } from 'chart.js';
import * as XLSX from 'xlsx';
import { PageViewAdminService } from '../../services/page-view.admin.service';
import { PageViewItem, PageViewStats, TableColumn } from '../../models/admin.model';
import { AdminTableComponent } from '../../shared/admin-table.component';
import { AdminMessage } from '../../../app.message';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

Chart.register(...registerables);

type Period = 'week' | 'month' | 'year';

const PIE_COLORS = ['#0f3460','#e94560','#028090','#533483','#f5a623','#2ecc71','#e67e22'];

@Component({
  selector: 'app-page-views-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminTableComponent, LanguagePipe, PaginationComponent],
  templateUrl: './page-views.component.html',
  styleUrl: './page-views.component.css'
})
export class PageViewsAdminComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('chartCanvas')   chartCanvas!:   ElementRef<HTMLCanvasElement>;
  @ViewChild('browserCanvas') browserCanvas!: ElementRef<HTMLCanvasElement>;
  @ViewChild('osCanvas')      osCanvas!:      ElementRef<HTMLCanvasElement>;
  @ViewChild('hourlyCanvas')  hourlyCanvas!:  ElementRef<HTMLCanvasElement>;

  protected readonly AdminMessage = AdminMessage;

  activeTab: 'table' | 'chart' = 'chart';
  period: Period = 'week';

  // Table
  items: PageViewItem[] = [];
  tableLoading = false;
  currentPage = 1;
  pageCount = 0;
  pageSize = 20;

  // Chart / stats
  stats: PageViewStats | null = null;
  statsLoading = false;

  private chartTraffic:  Chart | null = null;
  private chartBrowser:  Chart | null = null;
  private chartOs:       Chart | null = null;
  private chartHourly:   Chart | null = null;

  readonly columns: TableColumn[] = [
    { key: 'ipAddress',   label: AdminMessage.PV_LABEL_IP,       type: 'text' },
    { key: 'path',        label: AdminMessage.PV_LABEL_PATH,      type: 'truncate' },
    { key: 'browser',     label: AdminMessage.PV_LABEL_BROWSER,   type: 'text' },
    { key: 'os',          label: AdminMessage.PV_LABEL_OS,        type: 'text' },
    { key: 'referer',     label: AdminMessage.PV_LABEL_REFERER,   type: 'truncate' },
    { key: 'createdDate', label: AdminMessage.LABEL_CREATED_DATE, type: 'date' },
  ];

  constructor(private svc: PageViewAdminService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadStats();
    this.loadTable(1);
  }

  ngAfterViewInit(): void {
    if (this.activeTab === 'chart' && this.stats) this.renderAllCharts();
  }

  ngOnDestroy(): void { this.destroyAllCharts(); }

  switchTab(tab: 'table' | 'chart'): void {
    this.activeTab = tab;
    if (tab === 'chart') {
      this.cdr.detectChanges();
      setTimeout(() => this.renderAllCharts(), 100);
    }
  }

  setPeriod(p: Period): void {
    this.period = p;
    this.loadStats();
  }

  loadTable(page: number): void {
    this.currentPage = page;
    this.tableLoading = true;
    this.svc.getAll(page, this.pageSize).subscribe({
      next: res => {
        this.items = res.data;
        this.currentPage = res.pageNumber;
        this.pageCount = res.pageCount;
        this.tableLoading = false;
      },
      error: () => { this.tableLoading = false; }
    });
  }

  loadStats(): void {
    this.statsLoading = true;
    this.svc.getStats(this.period).subscribe({
      next: stats => {
        this.stats = stats;
        this.statsLoading = false;
        if (this.activeTab === 'chart') {
          this.cdr.detectChanges();
          setTimeout(() => this.renderAllCharts(), 100);
        }
      },
      error: () => { this.statsLoading = false; }
    });
  }

  // ── Charts ──────────────────────────────────────────────────────────────

  private destroyAllCharts(): void {
    [this.chartTraffic, this.chartBrowser, this.chartOs, this.chartHourly].forEach(c => {
      if (c) { c.destroy(); }
    });
    this.chartTraffic = this.chartBrowser = this.chartOs = this.chartHourly = null;
  }

  renderAllCharts(): void {
    if (!this.stats) return;
    this.destroyAllCharts();
    this.renderTrafficChart();
    this.renderBrowserChart();
    this.renderOsChart();
    this.renderHourlyChart();
  }

  private renderTrafficChart(): void {
    if (!this.chartCanvas || !this.stats) return;
    const labels  = this.stats.chart.map(d => d.label);
    const views   = this.stats.chart.map(d => d.views);
    const uniques = this.stats.chart.map(d => d.uniqueIps);

    this.chartTraffic = new Chart(this.chartCanvas.nativeElement, {
      type: 'bar',
      data: {
        labels,
        datasets: [
          {
            label: 'Page Views',
            data: views,
            backgroundColor: 'rgba(14,52,96,0.75)',
            borderColor: '#0f3460',
            borderWidth: 1,
            borderRadius: 6,
            order: 2
          },
          {
            label: 'Unique IPs',
            data: uniques,
            type: 'line',
            borderColor: '#e94560',
            backgroundColor: 'rgba(233,69,96,0.12)',
            pointBackgroundColor: '#e94560',
            pointRadius: 4,
            fill: true,
            tension: 0.4,
            order: 1
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        interaction: { mode: 'index', intersect: false },
        plugins: {
          legend: { position: 'top', labels: { usePointStyle: true, padding: 16 } },
          tooltip: { padding: 10, cornerRadius: 8 }
        },
        scales: {
          x: { grid: { display: false }, ticks: { font: { size: 12 } } },
          y: { beginAtZero: true, grid: { color: 'rgba(0,0,0,.05)' }, ticks: { precision: 0, font: { size: 12 } } }
        }
      }
    });
  }

  private renderBrowserChart(): void {
    if (!this.browserCanvas || !this.stats?.browsers?.length) return;
    const labels = this.stats.browsers.map(b => b.label);
    const counts = this.stats.browsers.map(b => b.count);

    this.chartBrowser = new Chart(this.browserCanvas.nativeElement, {
      type: 'doughnut',
      data: {
        labels,
        datasets: [{
          data: counts,
          backgroundColor: PIE_COLORS.slice(0, labels.length),
          borderWidth: 2,
          borderColor: '#fff',
          hoverOffset: 6
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { position: 'right', labels: { usePointStyle: true, padding: 12, font: { size: 12 } } },
          tooltip: { padding: 10, cornerRadius: 8 }
        },
        cutout: '60%'
      }
    });
  }

  private renderOsChart(): void {
    if (!this.osCanvas || !this.stats?.operatingSystems?.length) return;
    const labels = this.stats.operatingSystems.map(o => o.label);
    const counts = this.stats.operatingSystems.map(o => o.count);

    this.chartOs = new Chart(this.osCanvas.nativeElement, {
      type: 'doughnut',
      data: {
        labels,
        datasets: [{
          data: counts,
          backgroundColor: PIE_COLORS.slice(0, labels.length).reverse(),
          borderWidth: 2,
          borderColor: '#fff',
          hoverOffset: 6
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { position: 'right', labels: { usePointStyle: true, padding: 12, font: { size: 12 } } },
          tooltip: { padding: 10, cornerRadius: 8 }
        },
        cutout: '60%'
      }
    });
  }

  private renderHourlyChart(): void {
    if (!this.hourlyCanvas || !this.stats?.hourlyDistribution) return;
    const labels = this.stats.hourlyDistribution.map(h => `${String(h.hour).padStart(2,'0')}:00`);
    const views  = this.stats.hourlyDistribution.map(h => h.views);
    const maxVal = Math.max(...views, 1);

    this.chartHourly = new Chart(this.hourlyCanvas.nativeElement, {
      type: 'bar',
      data: {
        labels,
        datasets: [{
          label: 'Views',
          data: views,
          backgroundColor: views.map(v =>
            v === maxVal ? '#e94560' : v >= maxVal * 0.6 ? 'rgba(233,69,96,0.55)' : 'rgba(15,52,96,0.45)'
          ),
          borderRadius: 4,
          borderSkipped: false
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false },
          tooltip: {
            padding: 10,
            cornerRadius: 8,
            callbacks: { title: (items) => `${items[0].label}` }
          }
        },
        scales: {
          x: { grid: { display: false }, ticks: { font: { size: 10 }, maxRotation: 0 } },
          y: { beginAtZero: true, grid: { color: 'rgba(0,0,0,.05)' }, ticks: { precision: 0, font: { size: 11 } } }
        }
      }
    });
  }

  // ── Excel export ─────────────────────────────────────────────────────────

  exportTableExcel(): void {
    const rows = this.items.map(i => ({
      IP: i.ipAddress,
      Path: i.path,
      Browser: i.browser,
      OS: i.os,
      Referer: i.referer ?? '',
      Time: new Date(i.createdDate).toLocaleString('vi-VN')
    }));
    const ws = XLSX.utils.json_to_sheet(rows);
    ws['!cols'] = [{ wch: 16 }, { wch: 30 }, { wch: 14 }, { wch: 14 }, { wch: 30 }, { wch: 20 }];
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Page Views');
    XLSX.writeFile(wb, `page-views-${new Date().toISOString().slice(0, 10)}.xlsx`);
  }

  exportChartExcel(): void {
    if (!this.stats) return;

    const wsTraffic = XLSX.utils.json_to_sheet(
      this.stats.chart.map(d => ({ Period: d.label, 'Page Views': d.views, 'Unique IPs': d.uniqueIps }))
    );
    const wsBrowsers = XLSX.utils.json_to_sheet(
      this.stats.browsers.map(b => ({ Browser: b.label, Views: b.count }))
    );
    const wsOs = XLSX.utils.json_to_sheet(
      this.stats.operatingSystems.map(o => ({ OS: o.label, Views: o.count }))
    );
    const wsHourly = XLSX.utils.json_to_sheet(
      this.stats.hourlyDistribution.map(h => ({ Hour: `${String(h.hour).padStart(2,'0')}:00`, Views: h.views }))
    );
    const wsSummary = XLSX.utils.json_to_sheet([
      { Metric: 'Total Views', Value: this.stats.totalViews },
      { Metric: 'Unique IPs',  Value: this.stats.uniqueIps },
    ]);

    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, wsTraffic,  'Traffic');
    XLSX.utils.book_append_sheet(wb, wsBrowsers, 'Browsers');
    XLSX.utils.book_append_sheet(wb, wsOs,        'OS');
    XLSX.utils.book_append_sheet(wb, wsHourly,    'Peak Hours');
    XLSX.utils.book_append_sheet(wb, wsSummary,   'Summary');
    XLSX.writeFile(wb, `traffic-report-${this.period}-${new Date().toISOString().slice(0,10)}.xlsx`);
  }
}
