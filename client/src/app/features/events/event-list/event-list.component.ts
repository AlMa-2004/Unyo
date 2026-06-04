import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { EventService } from '../../../core/services/event.service';
import { AuthService } from '../../../core/services/auth.service';
import { EventDto } from '../../../core/models';

@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="main-content page-enter">
      <div class="page-header">
        <div>
          <h1>Events</h1>
          <p>Discover upcoming experiences</p>
        </div>
        <a *ngIf="auth.isVendor()" routerLink="/events/new" class="btn btn-primary">+ New Event</a>
      </div>

      <div class="filters mb-2">
        <input class="form-control" [(ngModel)]="searchTerm" placeholder="Search events..." style="max-width:320px">
      </div>

      <div class="loading-container" *ngIf="loading()">
        <div class="spinner"></div>
        <span>Loading events...</span>
      </div>

      <div *ngIf="!loading() && filtered().length === 0" class="empty-state">
        <div class="empty-state-icon">🎭</div>
        <h3>No events found</h3>
        <p>Try adjusting your search.</p>
      </div>

      <div class="grid-auto" *ngIf="!loading() && filtered().length > 0">
        <div class="card card-event" *ngFor="let e of filtered()">
          <div class="event-image" *ngIf="e.imagePath">
          <img [src]="e.imagePath" [alt]="e.title">
        </div>
          <div class="event-date-chip">{{ e.date | date:'MMM d, y' }}</div>
          <h3 class="event-title">{{ e.title }}</h3>
          <p class="event-venue">📍 {{ e.venueName }}</p>
          <p class="event-desc">{{ e.description | slice:0:120 }}{{ e.description.length > 120 ? '...' : '' }}</p>
          <div class="event-cats" *ngIf="e.categoryNames.length">
            <span class="badge badge-accent" *ngFor="let c of e.categoryNames">{{ c }}</span>
          </div>
          <div class="event-footer">
          <div>
            <span class="text-xs text-muted">Remaining: </span>
            <span [ngClass]="{'text-danger': e.currentRegistrations >= e.capacity, 'text-success': e.currentRegistrations < e.capacity}">
              {{ e.currentRegistrations >= e.capacity ? 'Sold Out!' : (e.capacity - e.currentRegistrations) + ' spots left!' }}
            </span>
          </div>
            <div>
              <span class="text-xs text-muted">From </span>
              <span class="event-price" *ngIf="e.availableTickets.length">{{ minPrice(e) | currency:'RON':'symbol':'1.0-0' }}</span>
              <span class="text-xs text-muted" *ngIf="!e.availableTickets.length">Free</span>
            </div>
            <div class="event-actions">
              <a [routerLink]="['/events', e.id]" class="btn btn-secondary btn-sm">Details</a>
              
              <ng-container *ngIf="canManage(e)">
                <a [routerLink]="['/events', e.id, 'edit']" class="btn btn-ghost btn-sm">Edit</a>
                <button class="btn btn-danger btn-sm" (click)="delete(e.id)">Del</button>
              </ng-container>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .filters { display: flex; gap: 1rem; flex-wrap: wrap; }
    .card-event { display: flex; flex-direction: column; gap: 0.5rem; }
    .event-date-chip { font-size: 0.75rem; color: var(--accent); font-weight: 600; letter-spacing: 0.05em; text-transform: uppercase; }
    .event-title { font-size: 1.1rem; color: var(--text-primary); margin: 0; font-family: var(--font-display); }
    .event-venue { font-size: 0.82rem; color: var(--text-muted); }
    .event-desc { font-size: 0.875rem; color: var(--text-secondary); flex: 1; }
    .event-cats { display: flex; flex-wrap: wrap; gap: 0.35rem; }
    .event-footer { display: flex; align-items: center; justify-content: space-between; flex-wrap: wrap; gap: 0.5rem; margin-top: auto; padding-top: 0.75rem; border-top: 1px solid var(--border); }
    .event-price { font-weight: 700; color: var(--text-primary); font-size: 1rem; }
    .event-actions { display: flex; gap: 0.35rem; }
  `]
})
export class EventListComponent implements OnInit {
  private svc = inject(EventService);
  auth = inject(AuthService);
  events = signal<EventDto[]>([]);
  loading = signal(true);
  searchTerm = '';

  get filtered() {
    return () => this.events().filter(e =>
      !this.searchTerm || e.title.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      e.venueName.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  minPrice(e: EventDto): number {
    return Math.min(...e.availableTickets.map(t => t.price));
  }

  ngOnInit() {
    this.svc.getAll().subscribe({ next: d => { this.events.set(d); this.loading.set(false); }, error: () => this.loading.set(false) });
  }

  delete(id: number) {
    if (!confirm('Delete this event?')) return;
    this.svc.delete(id).subscribe(() => this.events.update(list => list.filter(e => e.id !== id)));
  }

  canManage(event: EventDto): boolean {
    const user = this.auth.currentUser();
    if (this.auth.isAdmin()) return true;
    return event.userId === user?.id;
    }

}
