import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { VenueService } from '../../../core/services/venue.service';
import { AuthService } from '../../../core/services/auth.service';
import { VenueDto } from '../../../core/models';

@Component({
  selector: 'app-venue-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="main-content page-enter">
      <div class="page-header">
        <div><h1>Venues</h1><p>Event locations and spaces</p></div>
        <a *ngIf="auth.isAdmin()" routerLink="/venues/new" class="btn btn-primary">+ New Venue</a>
      </div>
      <div class="loading-container" *ngIf="loading()"><div class="spinner"></div></div>
      <div class="grid-auto" *ngIf="!loading()">
        <div class="card" *ngFor="let v of venues()">
          <h3 style="color:var(--text-primary)">{{ v.name }}</h3>
          <p class="text-sm" style="margin:.25rem 0">📍 {{ v.address }}</p>
          <p class="text-xs text-muted">Capacity: {{ v.capacity | number }} people</p>
          <div class="flex gap-1 mt-2" *ngIf="auth.isAdmin()">
            <a [routerLink]="['/venues', v.id, 'edit']" class="btn btn-secondary btn-sm">Edit</a>
            <button class="btn btn-danger btn-sm" (click)="delete(v.id)">Delete</button>
          </div>
        </div>
      </div>
      <div *ngIf="!loading() && !venues().length" class="empty-state">
        <div class="empty-state-icon">🏛️</div>
        <h3>No venues yet</h3>
      </div>
    </div>
  `
})
export class VenueListComponent implements OnInit {
  private svc = inject(VenueService);
  auth = inject(AuthService);
  venues = signal<VenueDto[]>([]);
  loading = signal(true);
  ngOnInit() { this.svc.getAll().subscribe({ next: d => { this.venues.set(d); this.loading.set(false); }, error: () => this.loading.set(false) }); }
  delete(id: number) {
    if (!confirm('Delete venue?')) return;
    this.svc.delete(id).subscribe(() => this.venues.update(v => v.filter(x => x.id !== id)));
  }
}
