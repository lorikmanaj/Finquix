import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  constructor(private http: HttpClient) { }

  get<HttpResponseModel>(path: string, options?: { headers?: HttpHeaders, params?: HttpParams }): Observable<HttpResponseModel> {
    const url = `${environment.apiUrl}/api/${path}`;
    return this.http.get<HttpResponseModel>(url, options);
  }

  getText(path: string, options?: { headers?: HttpHeaders, params?: HttpParams }): Observable<string> {
    const url = `${environment.apiUrl}/api/${path}`;
    return this.http.get(url, { ...options, responseType: 'text' }) as Observable<string>;
  }

  getBlob(path: string, options?: { headers?: HttpHeaders, params?: HttpParams }): Observable<Blob> {
    const url = `${environment.apiUrl}/api/${path}`;
    return this.http.get(url, { ...options, responseType: 'blob' });
  }

  getById<HttpResponseModel>(path: string, id: number | string, options?: { headers?: HttpHeaders, params?: HttpParams }): Observable<HttpResponseModel> {
    const url = `${environment.apiUrl}/api/${path}/${id}`;
    return this.http.get<HttpResponseModel>(url, options);
  }

  put<HttpResponseModel, RequestModel>(
    path: string,
    data: RequestModel,
    options?: { headers?: HttpHeaders, params?: HttpParams }
  ): Observable<HttpResponseModel> {
    const url = `${environment.apiUrl}/api/${path}`;
    return this.http.put<HttpResponseModel>(url, data, options);
  }

  post<HttpResponseModel, RequestModel>(
    path: string,
    data: RequestModel,
    options?: { headers?: HttpHeaders, params?: HttpParams }
  ): Observable<HttpResponseModel> {
    const url = `${environment.apiUrl}/api/${path}`;
    return this.http.post<HttpResponseModel>(url, data, options);
  }

  delete<HttpResponseModel>(
    path: string,
    options?: { headers?: HttpHeaders, params?: HttpParams }
  ): Observable<HttpResponseModel> {
    const url = `${environment.apiUrl}/api/${path}`;
    return this.http.delete<HttpResponseModel>(url, options);
  }
}