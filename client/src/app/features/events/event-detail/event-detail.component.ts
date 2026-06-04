import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { EventService } from '../../../core/services/event.service';
import { RegistrationService } from '../../../core/services/registration.service';
import { AuthService } from '../../../core/services/auth.service';
import { EventDto } from '../../../core/models';

@Component({
  selector: 'app-event-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="main-content page-enter" *ngIf="event()">
      <a routerLink="/events" class="btn btn-ghost btn-sm mb-2">← Back to Events</a>
      <div class="detail-hero">
        <div>
          <div class="event-date-chip">{{ event()!.date | date:'EEEE, MMMM d, y · HH:mm' }}</div>
          <h1>{{ event()!.title }}</h1>
          <p class="venue-line">📍 {{ event()!.venueName }}</p>
          <div class="cats-row" *ngIf="event()!.categoryNames.length">
            <span class="badge badge-accent" *ngFor="let c of event()!.categoryNames">{{ c }}</span>
          </div>
        </div>
        <div class="detail-actions" *ngIf="auth.isVendor()">
          <a [routerLink]="['/events', event()!.id, 'edit']" class="btn btn-secondary">Edit</a>
        </div>
      </div>
      <div class="divider"></div>
      <div class="detail-body">
        <div class="description-section">
          <h2>About this event</h2>
          <p style="white-space:pre-line;color:var(--text-secondary);">{{ event()!.description }}</p>
        </div>
        <div class="tickets-section">
          <h2>Tickets</h2>
          <div *ngIf="!event()!.availableTickets.length" class="text-muted text-sm">No tickets configured.</div>
          <div class="ticket-card" *ngFor="let t of event()!.availableTickets">
            <div>
              <div class="ticket-name">{{ t.name }}</div>
              <div class="ticket-price">{{ t.price | currency:'RON':'symbol':'1.2-2' }}</div>
            </div>
            <div *ngIf="auth.isLoggedIn()">
              <div class="form-group" style="margin:0;min-width:160px">
                <input class="form-control form-control-sm" [(ngModel)]="participantNames[t.id]" placeholder="Your name">
              </div>
              <button class="btn btn-primary btn-sm mt-1" (click)="register(t.id)" [disabled]="registering()">
                {{ registering() ? '...' : 'Register' }}
              </button>
            </div>
          </div>
          <div class="alert alert-success mt-2" *ngIf="regSuccess()">{{ regSuccess() }}</div>
          <div class="alert alert-error mt-2" *ngIf="regError()">{{ regError() }}</div>
        </div>
      </div>
    </div>
    <div class="loading-container" *ngIf="loading()"><div class="spinner"></div></div>
  `,
  styles: [`
    .event-date-chip { font-size: 0.8rem; color: var(--accent); font-weight: 600; text-transform: uppercase; letter-spacing: 0.06em; margin-bottom: 0.5rem; }
    .venue-line { color: var(--text-muted); margin: 0.5rem 0; }
    .detail-hero { display: flex; align-items: flex-start; justify-content: space-between; gap: 1rem; flex-wrap: wrap; }
    .cats-row { display: flex; gap: 0.4rem; flex-wrap: wrap; margin-top: 0.75rem; }
    .detail-body { display: grid; grid-template-columns: 1fr 340px; gap: 2rem; }
    .description-section h2, .tickets-section h2 { margin-bottom: 1rem; }
    .ticket-card { display: flex; align-items: center; justify-content: space-between; gap: 1rem; background: var(--bg-surface); border: 1px solid var(--border); border-radius: var(--radius-sm); padding: 1rem 1.25rem; margin-bottom: 0.75rem; flex-wrap: wrap; }
    .ticket-name { font-weight: 600; color: var(--text-primary); }
    .ticket-price { color: var(--accent); font-family: var(--font-display); font-size: 1.3rem; }
    .form-control-sm { padding: 0.4rem 0.7rem; font-size: 0.85rem; }
    @media (max-width: 768px) { .detail-body { grid-template-columns: 1fr; } }
  `]
})
export class EventDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private svc = inject(EventService);
  private regSvc = inject(RegistrationService);
  auth = inject(AuthService);
  event = signal<EventDto | null>(null);
  loading = signal(true);
  registering = signal(false);
  regSuccess = signal('');
  regError = signal('');
  participantNames: Record<number, string> = {};

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.svc.getById(id).subscribe({ next: e => { this.event.set(e); this.loading.set(false); }, error: () => this.loading.set(false) });
  }

  register(ticketId: number) {
    const name = this.participantNames[ticketId]?.trim();
    if (!name) { this.regError.set('Please enter your name.'); return; }
    this.registering.set(true);
    this.regSvc.create({ ticketId, participantName: name }).subscribe({
      next: () => { this.regSuccess.set('Registered successfully!'); this.regError.set(''); this.registering.set(false); },
      error: (e) => { this.regError.set(e?.error?.message ?? 'Registration failed.'); this.regSuccess.set(''); this.registering.set(false); }
    });
  }
}
