import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule, FormArray, FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { EventService } from '../../../core/services/event.service';
import { VenueService } from '../../../core/services/venue.service';
import { CategoryService } from '../../../core/services/category.service';
import { TicketService } from '../../../core/services/ticket.service';
import { VenueDto, CategoryDto, CreateEventDto, TicketDto, CreateTicketDto } from '../../../core/models';

@Component({
  selector: 'app-event-form',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink, FormsModule],
  template: `
    <div class="main-content page-enter" style="max-width:720px">
      <a routerLink="/events" class="btn btn-ghost btn-sm mb-2">← Back</a>
      <h1>{{ isEdit ? 'Edit Event' : 'New Event' }}</h1>
      <div class="alert alert-error" *ngIf="error()">{{ error() }}</div>
      <form [formGroup]="form" (ngSubmit)="submit()" class="card mt-2">
        <div class="form-group">
          <label class="form-label">Title</label>
          <input class="form-control" formControlName="title" [class.is-invalid]="invalid('title')" placeholder="Event title">
          <div class="form-error" *ngIf="invalid('title')">Min. 5 characters.</div>
        </div>
        <div class="form-group">
          <label class="form-label">Description</label>
          <textarea class="form-control" formControlName="description" rows="5" [class.is-invalid]="invalid('description')" placeholder="Describe the event (min. 20 chars)"></textarea>
          <div class="form-error" *ngIf="invalid('description')">Min. 20 characters.</div>
        </div>
        <div class="grid-2">
          <div class="form-group">
            <label class="form-label">Date & Time</label>
            <input class="form-control" type="datetime-local" formControlName="date" [class.is-invalid]="invalid('date')">
            <div class="form-error" *ngIf="invalid('date')">Required.</div>
          </div>
          <div class="form-group">
            <label class="form-label">Venue</label>
            <select class="form-control" formControlName="venueId" [class.is-invalid]="invalid('venueId')">
              <option value="">Select venue...</option>
              <option *ngFor="let v of venues()" [value]="v.id">{{ v.name }}</option>
            </select>
            <div class="form-error" *ngIf="invalid('venueId')">Required.</div>
          </div>
        </div>
        <div class="form-group">
          <label class="form-label">Image URL (optional)</label>
          <input class="form-control" formControlName="imagePath" placeholder="https://...">
        </div>
        <div class="form-group">
          <label class="form-label">Categories</label>
          <div class="cats-grid">
            <label class="cat-check" *ngFor="let c of categories()">
              <input type="checkbox" [checked]="isCatSelected(c.id)" (change)="toggleCat(c.id)">
              <span>{{ c.name }}</span>
            </label>
          </div>
        </div>
        <div class="divider"></div>
        <div class="flex" style="gap:1rem;justify-content:flex-end">
          <a routerLink="/events" class="btn btn-secondary">Cancel</a>
          <button type="submit" class="btn btn-primary" [disabled]="loading()">
            {{ loading() ? 'Saving...' : (isEdit ? 'Save changes' : 'Create event') }}
          </button>
        </div>
      </form>

      <!-- Tickets section (edit only) -->
      <div *ngIf="isEdit" class="card mt-2">
        <h3>Tickets</h3>
        <div class="ticket-row" *ngFor="let t of tickets()">
          <span>{{ t.typeName }}</span>
          <span class="text-accent">{{ t.price | currency:'RON':'symbol':'1.2-2' }}</span>
          <button class="btn btn-danger btn-sm btn-icon" (click)="deleteTicket(t.id)" title="Delete">✕</button>
        </div>
        <div class="divider"></div>
        <h4 style="margin-bottom:.75rem">Add Ticket Type</h4>
        <div class="grid-2">
          <div class="form-group">
            <label class="form-label">Name</label>
            <input class="form-control" [(ngModel)]="newTicket.typeName" placeholder="VIP, General..." [ngModelOptions]="{standalone:true}">
          </div>
          <div class="form-group">
            <label class="form-label">Price (RON)</label>
            <input class="form-control" type="number" min="0.01" step="0.01" [(ngModel)]="newTicket.price" [ngModelOptions]="{standalone:true}">
          </div>
        </div>
        <button class="btn btn-secondary btn-sm" (click)="addTicket()">+ Add ticket</button>
      </div>
    </div>
  `,
  styles: [`
    .cats-grid { display: flex; flex-wrap: wrap; gap: 0.5rem; }
    .cat-check { display: flex; align-items: center; gap: 0.4rem; padding: 0.35rem 0.75rem; background: var(--bg-surface); border: 1px solid var(--border); border-radius: 999px; cursor: pointer; font-size: 0.85rem; transition: border-color .15s; }
    .cat-check:hover { border-color: var(--accent); }
    .cat-check input { accent-color: var(--accent); }
    .ticket-row { display: flex; align-items: center; gap: 1rem; padding: 0.6rem 0; border-bottom: 1px solid var(--border); }
    .ticket-row span:first-child { flex: 1; color: var(--text-primary); }
  `]
})
export class EventFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private svc = inject(EventService);
  private venueSvc = inject(VenueService);
  private catSvc = inject(CategoryService);
  private ticketSvc = inject(TicketService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  isEdit = false;
  editId = 0;
  loading = signal(false);
  error = signal('');
  venues = signal<VenueDto[]>([]);
  categories = signal<CategoryDto[]>([]);
  tickets = signal<TicketDto[]>([]);
  selectedCatIds = new Set<number>();
  newTicket = { typeName: '', price: 0 };

  form = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.minLength(20), Validators.maxLength(2000)]],
    date: ['', Validators.required],
    imagePath: [''],
    venueId: ['', Validators.required]
  });

  invalid(f: string): boolean {
    const c = this.form.get(f);
    return !!(c?.invalid && c?.touched);
  }

  isCatSelected(id: number) { return this.selectedCatIds.has(id); }
  toggleCat(id: number) { if (this.selectedCatIds.has(id)) this.selectedCatIds.delete(id); else this.selectedCatIds.add(id); }

  ngOnInit() {
    this.venueSvc.getAll().subscribe(v => this.venues.set(v));
    this.catSvc.getAll().subscribe(c => this.categories.set(c));
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.editId = Number(id);
      this.svc.getById(this.editId).subscribe(e => {
        this.form.patchValue({ title: e.title, description: e.description, date: new Date(e.date).toISOString().slice(0,16), imagePath: e.imagePath ?? '' });
        e.categoryNames.forEach(name => {
          const cat = this.categories().find(c => c.name === name);
          if (cat) this.selectedCatIds.add(cat.id);
        });
      });
      this.ticketSvc.getAll().subscribe(ts => this.tickets.set(ts.filter(t => t.eventId === this.editId)));
    }
  }

  submit() {
    if (this.form.invalid) { 
      this.form.markAllAsTouched(); 
      return; 
    }

    this.loading.set(true);
    const v = this.form.value;

    const dto: CreateEventDto = { 
      title: v.title!, 
      description: v.description!, 
      date: new Date(v.date!).toISOString(), 
      imagePath: v.imagePath || undefined, 
      venueId: Number(v.venueId), 
      categoryIds: [...this.selectedCatIds] 
    };

   const req = (this.isEdit 
    ? this.svc.update(this.editId, dto) 
    : this.svc.create(dto)) as Observable<any>;

    req.subscribe({ 
      next: () => {
        this.router.navigate(['/events']);
      }, 
      error: (e: any) => { 
        this.error.set(e?.error?.message ?? 'Error saving.'); 
        this.loading.set(false); 
      } 
    });
  }

  addTicket() {
    if (!this.newTicket.typeName || this.newTicket.price <= 0) return;
    const dto: CreateTicketDto = { typeName: this.newTicket.typeName, price: this.newTicket.price, eventId: this.editId };
    this.ticketSvc.create(dto).subscribe(t => { this.tickets.update(ts => [...ts, t]); this.newTicket = { typeName: '', price: 0 }; });
  }

  deleteTicket(id: number) {
    if (!confirm('Delete ticket type?')) return;
    this.ticketSvc.delete(id).subscribe(() => this.tickets.update(ts => ts.filter(t => t.id !== id)));
  }
}
