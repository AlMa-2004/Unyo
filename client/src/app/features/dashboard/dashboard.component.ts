import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { EventService } from '../../core/services/event.service';
import { VenueService } from '../../core/services/venue.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page-enter">
      <div class="hero">
        <div class="hero-inner">
          <div class="hero-eyebrow">Event Platform</div>
          <h1 class="hero-title">Discover &amp;<br><em>experience</em> events</h1>
          <p class="hero-sub">Browse concerts, conferences, and cultural experiences — all in one place.</p>
          <div class="hero-actions" *ngIf="!auth.isLoggedIn()">
            <a routerLink="/register" class="btn btn-primary btn-lg">Get started</a>
            <a routerLink="/login" class="btn btn-secondary btn-lg">Sign in</a>
          </div>
          <div class="hero-actions" *ngIf="auth.isLoggedIn()">
            <a routerLink="/events" class="btn btn-primary btn-lg">Browse events</a>
          </div>
        </div>
        <div class="hero-deco">
          <div class="deco-ring deco-ring-1"></div>
          <div class="deco-ring deco-ring-2"></div>
          <div class="deco-dot deco-dot-1"></div>
          <div class="deco-dot deco-dot-2"></div>
        </div>
      </div>

      <div class="main-content">
        <div class="stats-row">
          <div class="stat-card">
            <div class="stat-num">{{ eventCount() }}</div>
            <div class="stat-label">Events</div>
          </div>
          <div class="stat-card">
            <div class="stat-num">{{ venueCount() }}</div>
            <div class="stat-label">Venues</div>
          </div>
          <div class="stat-card">
            <div class="stat-num" *ngIf="auth.isLoggedIn()">{{ auth.currentUser()?.roles?.join(', ') ?? '' }}</div>
            <div class="stat-num" *ngIf="!auth.isLoggedIn()">—</div>
            <div class="stat-label">Your Role</div>
          </div>
        </div>

        <div class="quick-links mt-4">
          <h2 style="margin-bottom:1.25rem">Quick access</h2>
          <div class="grid-3">
            <a routerLink="/events" class="qlink-card">
              <div class="qlink-icon">🎭</div>
              <div class="qlink-label">Events</div>
              <div class="qlink-desc">Browse all upcoming events</div>
            </a>
            <a routerLink="/venues" class="qlink-card">
              <div class="qlink-icon">🏛️</div>
              <div class="qlink-label">Venues</div>
              <div class="qlink-desc">Discover event locations</div>
            </a>
            <a routerLink="/registrations" class="qlink-card" *ngIf="auth.isLoggedIn()">
              <div class="qlink-icon">🎫</div>
              <div class="qlink-label">Registrations</div>
              <div class="qlink-desc">Manage your bookings</div>
            </a>
            <a routerLink="/categories" class="qlink-card" *ngIf="auth.isLoggedIn()">
              <div class="qlink-icon">🏷️</div>
              <div class="qlink-label">Categories</div>
              <div class="qlink-desc">Browse by type</div>
            </a>
            <a routerLink="/users" class="qlink-card" *ngIf="auth.isAdmin()">
              <div class="qlink-icon">👥</div>
              <div class="qlink-label">Users</div>
              <div class="qlink-desc">Admin: manage accounts</div>
            </a>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .hero {
      position: relative; overflow: hidden;
      padding: 5rem 2rem 4rem;
      background: linear-gradient(160deg, rgba(245,200,66,0.04) 0%, transparent 50%);
      border-bottom: 1px solid var(--border);
    }
    .hero-inner { max-width: 1200px; margin: 0 auto; position: relative; z-index: 1; }
    .hero-eyebrow { font-size: 0.75rem; font-weight: 600; letter-spacing: 0.15em; text-transform: uppercase; color: var(--accent); margin-bottom: 1rem; }
    .hero-title { font-size: clamp(3rem,7vw,5rem); line-height: 1.08; margin-bottom: 1.25rem; color: var(--text-primary); }
    .hero-title em { font-style: italic; color: var(--accent); }
    .hero-sub { max-width: 520px; font-size: 1.1rem; color: var(--text-secondary); margin-bottom: 2rem; }
    .hero-actions { display: flex; gap: 0.75rem; flex-wrap: wrap; }
    .hero-deco { position: absolute; right: -80px; top: 50%; transform: translateY(-50%); pointer-events: none; }
    .deco-ring { position: absolute; border-radius: 50%; border: 1px solid rgba(245,200,66,0.1); }
    .deco-ring-1 { width: 400px; height: 400px; top: -200px; right: 0; }
    .deco-ring-2 { width: 600px; height: 600px; top: -300px; right: -100px; border-color: rgba(245,200,66,0.05); }
    .deco-dot { position: absolute; border-radius: 50%; background: var(--accent); opacity: 0.4; }
    .deco-dot-1 { width: 8px; height: 8px; top: -80px; right: 120px; }
    .deco-dot-2 { width: 4px; height: 4px; top: 50px; right: 80px; opacity: 0.2; }
    .stats-row { display: grid; grid-template-columns: repeat(3, 1fr); gap: 1rem; }
    .stat-card { background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); padding: 1.5rem; text-align: center; }
    .stat-num { font-family: var(--font-display); font-size: 2.5rem; color: var(--accent); }
    .stat-label { font-size: 0.8rem; color: var(--text-muted); text-transform: uppercase; letter-spacing: 0.07em; margin-top: 0.25rem; }
    .qlink-card { display: flex; flex-direction: column; gap: 0.25rem; background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); padding: 1.5rem; text-decoration: none; transition: border-color 0.2s, transform 0.2s; }
    .qlink-card:hover { border-color: rgba(245,200,66,0.25); transform: translateY(-2px); }
    .qlink-icon { font-size: 1.75rem; margin-bottom: 0.25rem; }
    .qlink-label { font-size: 1rem; font-weight: 600; color: var(--text-primary); }
    .qlink-desc { font-size: 0.8rem; color: var(--text-muted); }
    @media (max-width: 768px) { .stats-row { grid-template-columns: 1fr; } }
  `]
})
export class DashboardComponent implements OnInit {
  auth = inject(AuthService);
  private eventSvc = inject(EventService);
  private venueSvc = inject(VenueService);
  eventCount = signal(0);
  venueCount = signal(0);
  ngOnInit() {
    this.eventSvc.getAll().subscribe(e => this.eventCount.set(e.length));
    this.venueSvc.getAll().subscribe(v => this.venueCount.set(v.length));
  }
}
