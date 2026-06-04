import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginDto, RegisterDto, AuthResponse, CurrentUser } from '../models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'unyo_token';
  private readonly apiUrl = environment.apiUrl;

  private _currentUser = signal<CurrentUser | null>(null);
  readonly currentUser = this._currentUser.asReadonly();
  readonly isLoggedIn = computed(() => !!this._currentUser());
  readonly isAdmin = computed(() => this._currentUser()?.roles.includes('Admin') ?? false);
  readonly isVendor = computed(() => this._currentUser()?.roles.some(r => r === 'Vendor' || r === 'Admin') ?? false);

  constructor(private http: HttpClient, private router: Router) {
    this.loadFromStorage();
  }

  login(dto: LoginDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/AuthApi/login`, dto).pipe(
      tap(res => this.handleToken(res.token))
    );
  }

  register(dto: RegisterDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/AuthApi/register`, dto);
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this._currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private handleToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    const user = this.parseToken(token);
    this._currentUser.set(user);
  }

  private loadFromStorage(): void {
    const token = localStorage.getItem(this.TOKEN_KEY);
    if (!token) return;
    const payload = this.decodeToken(token);
    if (!payload || payload.exp * 1000 < Date.now()) {
      localStorage.removeItem(this.TOKEN_KEY);
      return;
    }
    this._currentUser.set(this.parseToken(token));
  }

  private parseToken(token: string): CurrentUser | null {
    const payload = this.decodeToken(token);
    if (!payload) return null;
    const roleKey = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
    const roles = Array.isArray(payload[roleKey]) ? payload[roleKey] : (payload[roleKey] ? [payload[roleKey]] : []);
    return {
      id: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ?? '',
      email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ?? '',
      name: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ?? '',
      roles
    };
  }

  private decodeToken(token: string): any {
    try {
      const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
      return JSON.parse(atob(base64));
    } catch { return null; }
  }
}
