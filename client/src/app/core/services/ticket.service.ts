import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TicketDto, CreateTicketDto } from '../models';

@Injectable({ providedIn: 'root' })
export class TicketService {
  private url = `${environment.apiUrl}/tickets`;
  constructor(private http: HttpClient) {}
  getAll(): Observable<TicketDto[]> { return this.http.get<TicketDto[]>(this.url); }
  create(dto: CreateTicketDto): Observable<TicketDto> { return this.http.post<TicketDto>(this.url, dto); }
  delete(id: number): Observable<void> { return this.http.delete<void>(`${this.url}/${id}`); }
}
