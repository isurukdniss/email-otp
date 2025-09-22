import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface GenerateOtpRequest {
  email: string;
}

export interface VerifyOtpRequest {
  email: string;
  otp: string;
}

@Injectable({
  providedIn: 'root'
})
export class OtpService {
  private readonly baseUrl = environment.apiUrl;

  private http = inject(HttpClient);

  generateOtp(request: GenerateOtpRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/generate`, request);
  }

  verifyOtp(request: VerifyOtpRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/verify`, request);
  }
}
