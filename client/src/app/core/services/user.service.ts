import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserDto, CreateUserDto, UpdateUserDto } from '../models';

@Injectable({ providedIn: 'root' })
export class UserService {
  private url = `${environment.apiUrl}/users`;
  constructor(private http: HttpClient) {}
  getAll(): Observable<UserDto[]> { return this.http.get<UserDto[]>(this.url); }
  getById(id: string): Observable<UserDto> { return this.http.get<UserDto>(`${this.url}/${id}`); }
  create(dto: CreateUserDto): Observable<UserDto> { return this.http.post<UserDto>(this.url, dto); }
  update(id: string, dto: UpdateUserDto): Observable<void> { return this.http.put<void>(`${this.url}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.url}/${id}`); }
}
