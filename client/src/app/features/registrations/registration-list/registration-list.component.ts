import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RegistrationService } from '../../../core/services/registration.service';
import { AuthService } from '../../../core/services/auth.service'; // Adăugat importul
import { RegistrationDto } from '../../../core/models';

@Component({
  selector: 'app-registration-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="main-content page-enter">
      <div class="page-header"><div><h1>Registrations</h1><p>All event bookings</p></div></div>
      <div class="loading-container" *ngIf="loading()"><div class="spinner"></div></div>
      <div class="table-wrapper" *ngIf="!loading()">
        <table>
          <thead><tr><th>#</th><th>Participant</th><th>Event</th><th>Ticket</th><th>Price</th><th>Date</th></tr></thead>
          <tbody>
            <tr *ngFor="let r of regs()">
              <td class="text-muted">{{ r.id }}</td>
              <td style="color:var(--text-primary)">{{ r.participantName }}</td>
              <td>{{ r.eventTitle }}</td>
              <td>{{ r.ticketName }}</td>
              <td class="text-accent">{{ r.ticketPrice | currency:'RON':'symbol':'1.2-2' }}</td>
              <td class="text-muted text-xs">{{ r.registrationDate | date:'short' }}</td>
            </tr>
            <tr *ngIf="!regs().length"><td colspan="7" style="text-align:center;color:var(--text-muted);padding:2rem">No registrations.</td></tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class RegistrationListComponent implements OnInit {
  private svc = inject(RegistrationService);
  private auth = inject(AuthService);
  
  regs = signal<RegistrationDto[]>([]);
  loading = signal(true);

  ngOnInit() {
    const userId = this.auth.currentUser()?.id;
    
    if (userId) {
      this.svc.getHistory(userId).subscribe({
        next: r => { 
          this.regs.set(r); 
          this.loading.set(false); 
        },
        error: () => this.loading.set(false)
      });
    } else {
      this.loading.set(false);
    }
  }

}