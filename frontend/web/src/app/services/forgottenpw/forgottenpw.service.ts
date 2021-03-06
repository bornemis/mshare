import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ForgottenpwService {

  constructor(private http: HttpClient){

  }

  sendForgottenPwMail(email: string) {

    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post<any>(`${environment.API_URL}/profile/password/forgot`, {
      'email': email}, httpOptions)
      .pipe(map(email => {
        return email;
      }));
  }

}
