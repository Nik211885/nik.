import { Component, OnInit, OnDestroy } from '@angular/core';
import { ApplicationTitle } from '../../app.message';
import { LanguagePipe } from '../../shared/pipes/language.pipe';
import { ConfigService } from '../../core/services/config.service';
import { AsyncPipe, KeyValuePipe } from '@angular/common';
import { CareerService, WorkExperience, Skill, Project } from '../../core/services/career.service';
import { LanguageService } from '../../core/services/language.service';
import { Subscription, switchMap } from 'rxjs';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [LanguagePipe, AsyncPipe, KeyValuePipe],
  templateUrl: './about.component.html',
  styleUrl: './about.component.css',
})
export class AboutComponent implements OnInit, OnDestroy {
  protected readonly ApplicationTitle = ApplicationTitle;

  workExperiences: WorkExperience[] = [];
  skillGroups: Record<string, Skill[]> = {};
  projects: Project[] = [];

  private subs = new Subscription();

  constructor(
    protected readonly configService: ConfigService,
    private careerService: CareerService,
    private langService: LanguageService,
  ) {}

  ngOnInit(): void {
    this.subs.add(
      this.langService.currentLanguage$.pipe(
        switchMap(() => this.careerService.getPublishedWorkExperiences())
      ).subscribe(data => {
        this.workExperiences = data;
      })
    );

    this.careerService.getPublishedSkills().subscribe(data => {
      this.skillGroups = data.reduce((groups, skill) => {
        (groups[skill.category] ??= []).push(skill);
        return groups;
      }, {} as Record<string, Skill[]>);
    });

    this.subs.add(
      this.langService.currentLanguage$.pipe(
        switchMap(() => this.careerService.getPublishedProjects())
      ).subscribe(data => {
        this.projects = data;
      })
    );
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  formatYear(dateStr: string): string {
    return new Date(dateStr).getFullYear().toString();
  }

  hasSkills(): boolean {
    return Object.keys(this.skillGroups).length > 0;
  }
}
