import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
  { path: 'login', loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent) },

  // Events (public GET, protected POST/PUT/DELETE enforced on API)
  { path: 'events', loadComponent: () => import('./features/events/event-list/event-list.component').then(m => m.EventListComponent) },
  { path: 'events/new', canActivate: [authGuard], loadComponent: () => import('./features/events/event-form/event-form.component').then(m => m.EventFormComponent) },
  { path: 'events/:id', loadComponent: () => import('./features/events/event-detail/event-detail.component').then(m => m.EventDetailComponent) },
  { path: 'events/:id/edit', canActivate: [authGuard], loadComponent: () => import('./features/events/event-form/event-form.component').then(m => m.EventFormComponent) },

  // Venues
  { path: 'venues', loadComponent: () => import('./features/venues/venue-list/venue-list.component').then(m => m.VenueListComponent) },
  { path: 'venues/new', canActivate: [authGuard, adminGuard], loadComponent: () => import('./features/venues/venue-form/venue-form.component').then(m => m.VenueFormComponent) },
  { path: 'venues/:id/edit', canActivate: [authGuard, adminGuard], loadComponent: () => import('./features/venues/venue-form/venue-form.component').then(m => m.VenueFormComponent) },

  // Categories (Admin only for mutations)
  { path: 'categories', canActivate: [authGuard], loadComponent: () => import('./features/categories/category-list/category-list.component').then(m => m.CategoryListComponent) },

  // Registrations
  { path: 'registrations', canActivate: [authGuard], loadComponent: () => import('./features/registrations/registration-list/registration-list.component').then(m => m.RegistrationListComponent) },

  // Users (Admin only)
  { path: 'users', canActivate: [authGuard, adminGuard], loadComponent: () => import('./features/users/user-list/user-list.component').then(m => m.UserListComponent) },
  { path: 'users/new', canActivate: [authGuard, adminGuard], loadComponent: () => import('./features/users/user-form/user-form.component').then(m => m.UserFormComponent) },
  { path: 'users/:id/edit', canActivate: [authGuard, adminGuard], loadComponent: () => import('./features/users/user-form/user-form.component').then(m => m.UserFormComponent) },

  { path: '**', redirectTo: '' }
];
