import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface WorkExperience {
  id: string;
  company: string;
  role: string;
  startDate: string;
  endDate: string | null;
  description: string | null;
  techTags: string[];
  order: number;
  isPublished: boolean;
}

export interface Skill {
  id: string;
  name: string;
  category: string;
  order: number;
  isPublished: boolean;
}

export interface CreateWorkExperienceRequest {
  company: string;
  role: string;
  startDate: string;
  endDate: string | null;
  description: string | null;
  techTags: string | null;
  order: number;
  isPublished: boolean;
}

export interface UpdateWorkExperienceRequest extends CreateWorkExperienceRequest {
  id: string;
}

export interface Project {
  id: string;
  name: string;
  description: string | null;
  techTags: string[];
  demoUrl: string | null;
  repoUrl: string | null;
  order: number;
  isPublished: boolean;
}

export interface CreateProjectRequest {
  name: string;
  description: string | null;
  techTags: string | null;
  demoUrl: string | null;
  repoUrl: string | null;
  order: number;
  isPublished: boolean;
}

export interface UpdateProjectRequest extends CreateProjectRequest {
  id: string;
}

export interface CreateSkillRequest {
  name: string;
  category: string;
  order: number;
  isPublished: boolean;
}

export interface UpdateSkillRequest extends CreateSkillRequest {
  id: string;
}

@Injectable({ providedIn: 'root' })
export class CareerService {
  constructor(private http: HttpClient) {}

  getPublishedWorkExperiences(): Observable<WorkExperience[]> {
    return this.http.get<WorkExperience[]>('public-api/careers/work-experiences');
  }

  getAllWorkExperiences(): Observable<WorkExperience[]> {
    return this.http.get<WorkExperience[]>('api/careers/work-experiences');
  }

  createWorkExperience(req: CreateWorkExperienceRequest): Observable<WorkExperience> {
    return this.http.post<WorkExperience>('api/careers/work-experiences/create', req);
  }

  updateWorkExperience(req: UpdateWorkExperienceRequest): Observable<WorkExperience> {
    return this.http.put<WorkExperience>('api/careers/work-experiences/update', req);
  }

  deleteWorkExperience(id: string): Observable<void> {
    return this.http.delete<void>(`api/careers/work-experiences/delete/${id}`);
  }

  getPublishedSkills(): Observable<Skill[]> {
    return this.http.get<Skill[]>('public-api/careers/skills');
  }

  getAllSkills(): Observable<Skill[]> {
    return this.http.get<Skill[]>('api/careers/skills');
  }

  createSkill(req: CreateSkillRequest): Observable<Skill> {
    return this.http.post<Skill>('api/careers/skills/create', req);
  }

  updateSkill(req: UpdateSkillRequest): Observable<Skill> {
    return this.http.put<Skill>('api/careers/skills/update', req);
  }

  deleteSkill(id: string): Observable<void> {
    return this.http.delete<void>(`api/careers/skills/delete/${id}`);
  }

  getPublishedProjects(): Observable<Project[]> {
    return this.http.get<Project[]>('public-api/careers/projects');
  }

  getAllProjects(): Observable<Project[]> {
    return this.http.get<Project[]>('api/careers/projects');
  }

  createProject(req: CreateProjectRequest): Observable<Project> {
    return this.http.post<Project>('api/careers/projects/create', req);
  }

  updateProject(req: UpdateProjectRequest): Observable<Project> {
    return this.http.put<Project>('api/careers/projects/update', req);
  }

  deleteProject(id: string): Observable<void> {
    return this.http.delete<void>(`api/careers/projects/delete/${id}`);
  }
}
