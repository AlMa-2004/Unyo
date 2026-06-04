import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, CommonModule],
  template: `
    <header class="header">
      <div class="header-inner">
        <a routerLink="/" class="logo">
          <span class="logo-mark">U</span>
          <span class="logo-text">nyo</span>
        </a>
        <nav class="nav" *ngIf="auth.isLoggedIn()">
          <a routerLink="/events" routerLinkActive="active" class="nav-link">Events</a>
          <a routerLink="/venues" routerLinkActive="active" class="nav-link">Venues</a>
          <a routerLink="/categories" routerLinkActive="active" class="nav-link">Categories</a>
          <a routerLink="/registrations" routerLinkActive="active" class="nav-link">Registrations</a>
          <a *ngIf="auth.isAdmin()" routerLink="/users" routerLinkActive="active" class="nav-link">Users</a>
        </nav>
        <div class="header-actions">
          <ng-container *ngIf="auth.isLoggedIn(); else guestTpl">
            <span class="user-chip">
              <span class="user-dot"></span>
              {{ auth.currentUser()?.name }}
            </span>
            <button class="btn btn-ghost btn-sm" (click)="auth.logout()">Sign out</button>
          </ng-container>
          <ng-template #guestTpl>
            <a routerLink="/login" class="btn btn-ghost btn-sm">Sign in</a>
            <a routerLink="/register" class="btn btn-primary btn-sm">Register</a>
          </ng-template>
        </div>
      </div>
    </header>
  `,
  styles: [`
    .header {
      position: sticky; top: 0; z-index: 100;
      height: var(--header-h);
      background: rgba(13,13,15,0.88);
      backdrop-filter: blur(12px);
      border-bottom: 1px solid var(--border);
    }
    .header-inner {
      max-width: 1280px; margin: 0 auto; padding: 0 2rem;
      height: 100%; display: flex; align-items: center; gap: 2rem;
    }
    .logo {
      display: flex; align-items: center; gap: 2px;
      font-family: var(--font-display); font-size: 1.5rem;
      color: var(--text-primary); text-decoration: none;
      flex-shrink: 0;
    }
    .logo-mark {
      display: inline-flex; align-items: center; justify-content: center;
      width: 30px; height: 30px;
      background: var(--accent); color: #0d0d0f;
      border-radius: 6px; font-size: 1rem; font-weight: 700;
      font-family: var(--font-body);
    }
    .logo-text { color: var(--text-primary); }
    .nav { display: flex; align-items: center; gap: 0.25rem; flex: 1; }
    .nav-link {
      padding: 0.4rem 0.75rem;
      border-radius: var(--radius-sm);
      font-size: 0.875rem; font-weight: 500;
      color: var(--text-secondary); text-decoration: none;
      transition: color 0.15s, background 0.15s;
    }
    .nav-link:hover { color: var(--text-primary); background: var(--bg-surface); }
    .nav-link.active { color: var(--accent); background: var(--accent-dim); }
    .header-actions { display: flex; align-items: center; gap: 0.5rem; margin-left: auto; }
    .user-chip {
      display: flex; align-items: center; gap: 0.4rem;
      font-size: 0.8rem; color: var(--text-secondary);
      padding: 0.3rem 0.7rem;
      background: var(--bg-surface); border-radius: 999px;
      border: 1px solid var(--border);
    }
    .user-dot {
      width: 7px; height: 7px; border-radius: 50%;
      background: var(--success); flex-shrink: 0;
    }
  `]
})
export class HeaderComponent {
  auth = inject(AuthService);
}
