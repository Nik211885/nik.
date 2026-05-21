import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LanguagePipe } from '../../../shared/pipes/language.pipe';
import { AdminMessage } from '../../../app.message';
import {
  CareerService, WorkExperience, Skill, Project,
  CreateWorkExperienceRequest, UpdateWorkExperienceRequest,
  CreateSkillRequest, UpdateSkillRequest,
  CreateProjectRequest, UpdateProjectRequest,
} from '../../../core/services/career.service';

type Tab = 'experience' | 'skills' | 'projects';

@Component({
  selector: 'app-careers-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, LanguagePipe],
  templateUrl: './careers.component.html',
  styleUrl: './careers.component.css',
})
export class CareersAdminComponent implements OnInit {
  protected readonly AdminMessage = AdminMessage;

  activeTab: Tab = 'experience';

  // ── Work Experience ──────────────────────────────────────────────────────
  experiences: WorkExperience[] = [];
  expLoading = false;
  expModalOpen = false;
  expEditing: WorkExperience | null = null;
  expForm: CreateWorkExperienceRequest = this.emptyExpForm();
  expDeleteId: string | null = null;
  expConfirmOpen = false;

  // ── Projects ─────────────────────────────────────────────────────────────
  projects: Project[] = [];
  projectLoading = false;
  projectModalOpen = false;
  projectEditing: Project | null = null;
  projectForm: CreateProjectRequest = this.emptyProjectForm();
  projectDeleteId: string | null = null;
  projectConfirmOpen = false;

  // ── Skills ───────────────────────────────────────────────────────────────
  skills: Skill[] = [];
  skillLoading = false;
  skillModalOpen = false;
  skillEditing: Skill | null = null;
  skillForm: CreateSkillRequest = this.emptySkillForm();
  skillDeleteId: string | null = null;
  skillConfirmOpen = false;

  constructor(private careerService: CareerService) {}

  ngOnInit(): void {
    this.loadExperiences();
    this.loadSkills();
    this.loadProjects();
  }

  // ── Experience CRUD ──────────────────────────────────────────────────────
  loadExperiences(): void {
    this.expLoading = true;
    this.careerService.getAllWorkExperiences().subscribe({
      next: data => { this.experiences = data; this.expLoading = false; },
      error: () => { this.expLoading = false; },
    });
  }

  openExpCreate(): void {
    this.expEditing = null;
    this.expForm = this.emptyExpForm();
    this.expModalOpen = true;
  }

  openExpEdit(exp: WorkExperience): void {
    this.expEditing = exp;
    this.expForm = {
      company: exp.company,
      role: exp.role,
      startDate: exp.startDate.substring(0, 10),
      endDate: exp.endDate ? exp.endDate.substring(0, 10) : null,
      description: exp.description,
      techTags: exp.techTags.join(', '),
      order: exp.order,
      isPublished: exp.isPublished,
    };
    this.expModalOpen = true;
  }

  saveExp(): void {
    if (this.expEditing) {
      const req: UpdateWorkExperienceRequest = { id: this.expEditing.id, ...this.expForm };
      this.careerService.updateWorkExperience(req).subscribe(() => {
        this.expModalOpen = false;
        this.loadExperiences();
      });
    } else {
      this.careerService.createWorkExperience(this.expForm).subscribe(() => {
        this.expModalOpen = false;
        this.loadExperiences();
      });
    }
  }

  confirmDeleteExp(id: string): void {
    this.expDeleteId = id;
    this.expConfirmOpen = true;
  }

  deleteExp(): void {
    if (!this.expDeleteId) return;
    this.careerService.deleteWorkExperience(this.expDeleteId).subscribe(() => {
      this.expConfirmOpen = false;
      this.expDeleteId = null;
      this.loadExperiences();
    });
  }

  private emptyExpForm(): CreateWorkExperienceRequest {
    return { company: '', role: '', startDate: '', endDate: null, description: null, techTags: null, order: 0, isPublished: true };
  }

  // ── Projects CRUD ─────────────────────────────────────────────────────────
  loadProjects(): void {
    this.projectLoading = true;
    this.careerService.getAllProjects().subscribe({
      next: data => { this.projects = data; this.projectLoading = false; },
      error: () => { this.projectLoading = false; },
    });
  }

  openProjectCreate(): void {
    this.projectEditing = null;
    this.projectForm = this.emptyProjectForm();
    this.projectModalOpen = true;
  }

  openProjectEdit(proj: Project): void {
    this.projectEditing = proj;
    this.projectForm = {
      name: proj.name,
      description: proj.description,
      techTags: proj.techTags.join(', '),
      demoUrl: proj.demoUrl,
      repoUrl: proj.repoUrl,
      order: proj.order,
      isPublished: proj.isPublished,
    };
    this.projectModalOpen = true;
  }

  saveProject(): void {
    if (this.projectEditing) {
      const req: UpdateProjectRequest = { id: this.projectEditing.id, ...this.projectForm };
      this.careerService.updateProject(req).subscribe(() => {
        this.projectModalOpen = false;
        this.loadProjects();
      });
    } else {
      this.careerService.createProject(this.projectForm).subscribe(() => {
        this.projectModalOpen = false;
        this.loadProjects();
      });
    }
  }

  confirmDeleteProject(id: string): void {
    this.projectDeleteId = id;
    this.projectConfirmOpen = true;
  }

  deleteProject(): void {
    if (!this.projectDeleteId) return;
    this.careerService.deleteProject(this.projectDeleteId).subscribe(() => {
      this.projectConfirmOpen = false;
      this.projectDeleteId = null;
      this.loadProjects();
    });
  }

  private emptyProjectForm(): CreateProjectRequest {
    return { name: '', description: null, techTags: null, demoUrl: null, repoUrl: null, order: 0, isPublished: true };
  }

  // ── Skills CRUD ───────────────────────────────────────────────────────────
  loadSkills(): void {
    this.skillLoading = true;
    this.careerService.getAllSkills().subscribe({
      next: data => { this.skills = data; this.skillLoading = false; },
      error: () => { this.skillLoading = false; },
    });
  }

  openSkillCreate(): void {
    this.skillEditing = null;
    this.skillForm = this.emptySkillForm();
    this.skillModalOpen = true;
  }

  openSkillEdit(skill: Skill): void {
    this.skillEditing = skill;
    this.skillForm = { name: skill.name, category: skill.category, order: skill.order, isPublished: skill.isPublished };
    this.skillModalOpen = true;
  }

  saveSkill(): void {
    if (this.skillEditing) {
      const req: UpdateSkillRequest = { id: this.skillEditing.id, ...this.skillForm };
      this.careerService.updateSkill(req).subscribe(() => {
        this.skillModalOpen = false;
        this.loadSkills();
      });
    } else {
      this.careerService.createSkill(this.skillForm).subscribe(() => {
        this.skillModalOpen = false;
        this.loadSkills();
      });
    }
  }

  confirmDeleteSkill(id: string): void {
    this.skillDeleteId = id;
    this.skillConfirmOpen = true;
  }

  deleteSkill(): void {
    if (!this.skillDeleteId) return;
    this.careerService.deleteSkill(this.skillDeleteId).subscribe(() => {
      this.skillConfirmOpen = false;
      this.skillDeleteId = null;
      this.loadSkills();
    });
  }

  private emptySkillForm(): CreateSkillRequest {
    return { name: '', category: '', order: 0, isPublished: true };
  }

  get skillsByCategory(): Record<string, Skill[]> {
    return this.skills.reduce((g, s) => {
      (g[s.category] ??= []).push(s);
      return g;
    }, {} as Record<string, Skill[]>);
  }

  skillCategories(): string[] {
    return Object.keys(this.skillsByCategory);
  }
}
