import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environments/environment.development';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../models/user';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl: string = environment.apiUrl;

  private currentUser = new BehaviorSubject<any>(null);
  currentUser$ = this.currentUser.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router) { }

  logIn(model: any, isRememberUser: boolean = true) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        if (response) {
          response.isRememberUser = isRememberUser;
          this.setCurrentUser(response)
        }
      })
    );
  }

  registerUser(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((response: User) => {
        if (response) {
          this.setCurrentUser(response)
        }
      })
    );
  }

  logout() {
    this.removeCurrentUser();
  }

  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.next(user);
  }

  removeCurrentUser() {
    if (!!localStorage.getItem('user')) localStorage.removeItem('user');
    this.currentUser.next(null);
  }
}
