import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { UserService } from '../../../core/services/user.service';
import { UserDto } from '../../../core/models';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="main-content page-enter">
      <div class="page-header"><div><h1>Users</h1><p>Admin — manage accounts</p></div><a routerLink="/users/new" class="btn btn-primary">+ Add User</a></div>
      <div class="loading-container" *ngIf="loading()"><div class="spinner"></div></div>
      <div class="table-wrapper" *ngIf="!loading()">
        <table>
          <thead><tr><th>Name</th><th>Email</th><th>Username</th><th>Birth Date</th><th>Actions</th></tr></thead>
          <tbody>
            <tr *ngFor="let u of users()">
              <td style="color:var(--text-primary)">{{ u.firstName }} {{ u.lastName }}</td>
              <td>{{ u.email }}</td>
              <td class="text-muted">{{ u.userName }}</td>
              <td class="text-muted text-xs">{{ u.birthDate | date:'mediumDate' }}</td>
              <td><div class="flex gap-1"><a [routerLink]="['/users', u.id, 'edit']" class="btn btn-secondary btn-sm">Edit</a><button class="btn btn-danger btn-sm" (click)="del(u.id)">Delete</button></div></td>
            </tr>
            <tr *ngIf="!users().length"><td colspan="5" style="text-align:center;color:var(--text-muted);padding:2rem">No users.</td></tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class UserListComponent implements OnInit {
  private svc = inject(UserService);
  users = signal<UserDto[]>([]);
  loading = signal(true);
  ngOnInit() { this.svc.getAll().subscribe({ next: u => { this.users.set(u); this.loading.set(false); }, error: () => this.loading.set(false) }); }
  del(id: string) { if (!confirm('Delete user?')) return; this.svc.delete(id).subscribe(() => this.users.update(us => us.filter(u => u.id !== id))); }
}
