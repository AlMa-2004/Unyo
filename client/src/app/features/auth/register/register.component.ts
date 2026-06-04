import { Component, inject, signal } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  template: `
    <div class="auth-page">
      <div class="auth-card">
        <div class="auth-brand">
          <div class="auth-logo">U</div>
          <div>
            <h1>Create account</h1>
            <p>Join Unyo to discover events</p>
          </div>
        </div>
        <div class="alert alert-error" *ngIf="error()">{{ error() }}</div>
        <div class="alert alert-success" *ngIf="success()">{{ success() }}</div>
        <form [formGroup]="form" (ngSubmit)="submit()">
          <div class="grid-2">
            <div class="form-group">
              <label class="form-label">First Name</label>
              <input class="form-control" formControlName="firstName"
                [class.is-invalid]="form.get('firstName')?.invalid && form.get('firstName')?.touched"
                placeholder="Ion">
              <div class="form-error" *ngIf="form.get('firstName')?.invalid && form.get('firstName')?.touched">Required.</div>
            </div>
            <div class="form-group">
              <label class="form-label">Last Name</label>
              <input class="form-control" formControlName="lastName"
                [class.is-invalid]="form.get('lastName')?.invalid && form.get('lastName')?.touched"
                placeholder="Popescu">
              <div class="form-error" *ngIf="form.get('lastName')?.invalid && form.get('lastName')?.touched">Required.</div>
            </div>
          </div>
          <div class="form-group">
            <label class="form-label">Email</label>
            <input class="form-control" type="email" formControlName="email"
              [class.is-invalid]="form.get('email')?.invalid && form.get('email')?.touched"
              placeholder="you@example.com">
            <div class="form-error" *ngIf="form.get('email')?.invalid && form.get('email')?.touched">Valid email required.</div>
          </div>
          <div class="form-group">
            <label class="form-label">Birth Date</label>
            <input class="form-control" type="date" formControlName="birthDate"
              [class.is-invalid]="form.get('birthDate')?.invalid && form.get('birthDate')?.touched">
            <div class="form-error" *ngIf="form.get('birthDate')?.invalid && form.get('birthDate')?.touched">Required.</div>
          </div>
          <div class="form-group">
            <label class="form-label">Password</label>
            <input class="form-control" type="password" formControlName="password"
              [class.is-invalid]="form.get('password')?.invalid && form.get('password')?.touched"
              placeholder="Min. 6 characters">
            <div class="form-error" *ngIf="form.get('password')?.invalid && form.get('password')?.touched">
              Min. 6 characters required.
            </div>
          </div>
          <button class="btn btn-primary btn-full btn-lg mt-2" type="submit" [disabled]="loading()">
            {{ loading() ? 'Creating account...' : 'Create account' }}
          </button>
        </form>
        <div class="auth-footer">Already have an account? <a routerLink="/login">Sign in</a></div>
      </div>
    </div>
  `,
  styles: [`
    .auth-page { min-height: 100vh; display: flex; align-items: center; justify-content: center; padding: 2rem; background: radial-gradient(ellipse at top, rgba(245,200,66,0.04) 0%, transparent 60%); }
    .auth-card { width: 100%; max-width: 480px; background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius-lg); padding: 2.5rem; }
    .auth-brand { display: flex; align-items: center; gap: 1rem; margin-bottom: 2rem; }
    .auth-logo { width: 48px; height: 48px; background: var(--accent); color: #0d0d0f; border-radius: 12px; display: flex; align-items: center; justify-content: center; font-size: 1.5rem; font-weight: 700; flex-shrink: 0; }
    .auth-brand h1 { font-size: 1.4rem; font-family: var(--font-display); }
    .auth-brand p { font-size: 0.85rem; color: var(--text-muted); margin: 0; }
    .auth-footer { text-align: center; margin-top: 1.5rem; font-size: 0.875rem; color: var(--text-muted); }
  `]
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  loading = signal(false);
  error = signal('');
  success = signal('');

  form = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    birthDate: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading.set(true);
    this.error.set('');
    const v = this.form.value;
    this.auth.register({ ...v, birthDate: new Date(v.birthDate!).toISOString() } as any).subscribe({
      next: () => { this.success.set('Account created! Redirecting to login...'); setTimeout(() => this.router.navigate(['/login']), 1500); },
      error: (e) => {
        const errs = e?.error;
        this.error.set(Array.isArray(errs) ? errs.map((x: any) => x.description).join(' ') : (e?.error?.message ?? 'Registration failed.'));
        this.loading.set(false);
      }
    });
  }
}
