import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { EventDto, CreateEventDto } from '../models';

@Injectable({ providedIn: 'root' })
export class EventService {
  private url = `${environment.apiUrl}/events`;
  constructor(private http: HttpClient) {}
  getAll(): Observable<EventDto[]> { return this.http.get<EventDto[]>(this.url); }
  getById(id: number): Observable<EventDto> { return this.http.get<EventDto>(`${this.url}/${id}`); }
  create(dto: CreateEventDto): Observable<EventDto> { return this.http.post<EventDto>(this.url, dto); }
  update(id: number, dto: CreateEventDto): Observable<void> { return this.http.put<void>(`${this.url}/${id}`, dto); }
  delete(id: number): Observable<void> { return this.http.delete<void>(`${this.url}/${id}`); }
}
