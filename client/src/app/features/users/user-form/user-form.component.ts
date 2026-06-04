import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../core/services/user.service';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  template: `
    <div class="main-content page-enter" style="max-width:560px">
      <a routerLink="/users" class="btn btn-ghost btn-sm mb-2">← Back</a>
      <h1>{{ isEdit ? 'Edit User' : 'New User' }}</h1>
      <div class="alert alert-error" *ngIf="error()">{{ error() }}</div>
      <form [formGroup]="form" (ngSubmit)="submit()" class="card mt-2">
        <div class="grid-2">
          <div class="form-group"><label class="form-label">First Name</label><input class="form-control" formControlName="firstName" [class.is-invalid]="inv('firstName')"><div class="form-error" *ngIf="inv('firstName')">Required.</div></div>
          <div class="form-group"><label class="form-label">Last Name</label><input class="form-control" formControlName="lastName" [class.is-invalid]="inv('lastName')"><div class="form-error" *ngIf="inv('lastName')">Required.</div></div>
        </div>
        <div class="form-group"><label class="form-label">Email</label><input class="form-control" type="email" formControlName="email" [class.is-invalid]="inv('email')"><div class="form-error" *ngIf="inv('email')">Valid email required.</div></div>
        <div class="form-group"><label class="form-label">Birth Date</label><input class="form-control" type="date" formControlName="birthDate" [class.is-invalid]="inv('birthDate')"><div class="form-error" *ngIf="inv('birthDate')">Required.</div></div>
        <div class="flex gap-1 mt-2" style="justify-content:flex-end">
          <a routerLink="/users" class="btn btn-secondary">Cancel</a>
          <button type="submit" class="btn btn-primary" [disabled]="loading()">{{ loading() ? 'Saving...' : 'Save' }}</button>
        </div>
      </form>
    </div>
  `
})
export class UserFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private svc = inject(UserService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  isEdit = false; editId = ''; loading = signal(false); error = signal('');
  form = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    birthDate: ['', Validators.required]
  });
  inv(f: string) { const c = this.form.get(f); return !!(c?.invalid && c?.touched); }
  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) { this.isEdit = true; this.editId = id; this.svc.getById(id).subscribe(u => this.form.patchValue({ ...u, birthDate: u.birthDate?.toString().slice(0,10) })); }
  }
  submit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading.set(true);
    
    const v = this.form.value;

    const dto = {
      firstName: v.firstName || '',
      lastName: v.lastName || '',
      email: v.email || '',
      birthDate: v.birthDate || ''
    };

    const req = (this.isEdit 
      ? this.svc.update(this.editId, dto as any)
      : this.svc.create(dto as any)) as Observable<any>;

    req.subscribe({ 
      next: () => this.router.navigate(['/users']), 
      error: (e: any) => { 
        this.error.set(e?.error?.message ?? 'Error saving.'); 
        this.loading.set(false); 
      } 
    });
  }
}
