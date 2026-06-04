import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CategoryService } from '../../../core/services/category.service';
import { AuthService } from '../../../core/services/auth.service';
import { CategoryDto } from '../../../core/models';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="main-content page-enter">
      <div class="page-header">
        <div><h1>Categories</h1><p>Event types and genres</p></div>
      </div>
      <div class="card" style="max-width:480px" *ngIf="auth.isAdmin()">
        <h3 style="margin-bottom:1rem">Add Category</h3>
        <div class="flex gap-1">
          <input class="form-control" [(ngModel)]="newName" placeholder="Category name" (keyup.enter)="add()">
          <button class="btn btn-primary" (click)="add()" [disabled]="!newName.trim()">Add</button>
        </div>
      </div>
      <div class="table-wrapper mt-2">
        <table>
          <thead><tr><th>#</th><th>Name</th><th *ngIf="auth.isAdmin()">Actions</th></tr></thead>
          <tbody>
            <tr *ngFor="let c of cats()">
              <td class="text-muted">{{ c.id }}</td>
              <td style="color:var(--text-primary)">{{ c.name }}</td>
              <td *ngIf="auth.isAdmin()"><button class="btn btn-danger btn-sm" (click)="del(c.id)">Delete</button></td>
            </tr>
            <tr *ngIf="!cats().length"><td colspan="3" class="empty-state" style="padding:2rem;text-align:center;color:var(--text-muted)">No categories yet.</td></tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class CategoryListComponent implements OnInit {
  private svc = inject(CategoryService);
  auth = inject(AuthService);
  cats = signal<CategoryDto[]>([]);
  newName = '';
  ngOnInit() { this.svc.getAll().subscribe(c => this.cats.set(c)); }
  add() { if (!this.newName.trim()) return; this.svc.create({ name: this.newName.trim() }).subscribe(c => { this.cats.update(cs => [...cs, c]); this.newName = ''; }); }
  del(id: number) { if (!confirm('Delete?')) return; this.svc.delete(id).subscribe(() => this.cats.update(cs => cs.filter(c => c.id !== id))); }
}
