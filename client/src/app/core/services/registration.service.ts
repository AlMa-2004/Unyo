import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { RegistrationDto, CreateRegistrationDto } from '../models';

@Injectable({ providedIn: 'root' })
export class RegistrationService {
  private url = `${environment.apiUrl}/EventRegistrations`;
  constructor(private http: HttpClient) {}

  getHistory(userId: string): Observable<RegistrationDto[]> {
    return this.http.get<RegistrationDto[]>(`${this.url}/history/${userId}`);
  }

  create(dto: CreateRegistrationDto): Observable<RegistrationDto> { 
    return this.http.post<RegistrationDto>(this.url, dto); 
  }
}