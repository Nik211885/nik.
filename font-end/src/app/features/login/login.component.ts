import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { LanguagePipe } from '../../shared/pipes/language.pipe';
import { AdminMessage } from '../../app.message';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, LanguagePipe],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  form = { emailOrUserName: '', password: '' };
  loading = false;
  error = '';
  showPassword = false;

  protected readonly AdminMessage = AdminMessage;

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/admin']);
    }
  }

  onSubmit(): void {
    if (!this.form.emailOrUserName || !this.form.password) return;
    this.loading = true;
    this.error = '';

    this.authService.login(this.form).subscribe({
      next: () => {
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/admin';
        this.router.navigateByUrl(returnUrl);
      },
      error: () => {
        this.error = AdminMessage.LOGIN_ERROR;
        this.loading = false;
      }
    });
  }
}
