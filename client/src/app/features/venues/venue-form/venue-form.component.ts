import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { VenueService } from '../../../core/services/venue.service';

@Component({
  selector: 'app-venue-form',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  template: `
    <div class="main-content page-enter" style="max-width:560px">
      <a routerLink="/venues" class="btn btn-ghost btn-sm mb-2">← Back</a>
      <h1>{{ isEdit ? 'Edit Venue' : 'New Venue' }}</h1>
      <div class="alert alert-error" *ngIf="error()">{{ error() }}</div>
      <form [formGroup]="form" (ngSubmit)="submit()" class="card mt-2">
        <div class="form-group"><label class="form-label">Name</label><input class="form-control" formControlName="name" [class.is-invalid]="inv('name')" placeholder="Venue name"><div class="form-error" *ngIf="inv('name')">Min 3 chars.</div></div>
        <div class="form-group"><label class="form-label">Address</label><input class="form-control" formControlName="address" [class.is-invalid]="inv('address')" placeholder="Full address"><div class="form-error" *ngIf="inv('address')">Min 5 chars.</div></div>
        <div class="form-group"><label class="form-label">Capacity</label><input class="form-control" type="number" formControlName="capacity" [class.is-invalid]="inv('capacity')" placeholder="500"><div class="form-error" *ngIf="inv('capacity')">1-100000.</div></div>
        <div class="form-group"><label class="form-label">Image URL (optional)</label><input class="form-control" formControlName="imagePath" placeholder="https://..."></div>
        <div class="flex gap-1 mt-2" style="justify-content:flex-end">
          <a routerLink="/venues" class="btn btn-secondary">Cancel</a>
          <button type="submit" class="btn btn-primary" [disabled]="loading()">{{ loading() ? 'Saving...' : 'Save' }}</button>
        </div>
      </form>
    </div>
  `
})
export class VenueFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private svc = inject(VenueService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  isEdit = false; editId = 0; loading = signal(false); error = signal('');
  form = this.fb.group({ name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(150)]], address: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(250)]], capacity: [null as number | null, [Validators.required, Validators.min(1), Validators.max(100000)]], imagePath: [''] });
  inv(f: string) { const c = this.form.get(f); return !!(c?.invalid && c?.touched); }
  ngOnInit() { const id = this.route.snapshot.paramMap.get('id'); if (id) { this.isEdit = true; this.editId = Number(id); this.svc.getById(this.editId).subscribe(v => this.form.patchValue(v as any)); } }
  

  submit() {
  if (this.form.invalid) { 
    this.form.markAllAsTouched(); 
    return; 
  }
  
  this.loading.set(true);
  const v = this.form.value;

  // 2. Forțăm tipul ca Observable<any> pentru a elimina TS2349
  const req = (this.isEdit 
    ? this.svc.update(this.editId, v as any) 
    : this.svc.create(v as any)) as Observable<any>;

  // 3. Specificăm explicit tipul (e: any) pentru a elimina TS7006
  req.subscribe({ 
    next: () => this.router.navigate(['/venues']), 
    error: (e: any) => { 
      this.error.set(e?.error?.message ?? 'Error saving.'); 
      this.loading.set(false); 
    } 
  });
}
}
