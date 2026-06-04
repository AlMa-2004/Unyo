import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { VenueDto, CreateVenueDto } from '../models';

@Injectable({ providedIn: 'root' })
export class VenueService {
  private url = `${environment.apiUrl}/venues`;
  constructor(private http: HttpClient) {}
  getAll(): Observable<VenueDto[]> { return this.http.get<VenueDto[]>(this.url); }
  getById(id: number): Observable<VenueDto> { return this.http.get<VenueDto>(`${this.url}/${id}`); }
  create(dto: CreateVenueDto): Observable<VenueDto> { return this.http.post<VenueDto>(this.url, dto); }
  update(id: number, dto: CreateVenueDto): Observable<void> { return this.http.put<void>(`${this.url}/${id}`, dto); }
  delete(id: number): Observable<void> { return this.http.delete<void>(`${this.url}/${id}`); }
}
