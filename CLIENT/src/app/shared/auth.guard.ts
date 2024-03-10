import { CanActivateFn, Router } from '@angular/router';
import { User } from '../models/user';
import { inject } from '@angular/core';
import { AccountService } from '../services/account.service';
import { jwtDecode } from 'jwt-decode';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const accountService = inject(AccountService);

  const user: User = JSON.parse(localStorage.getItem('user')!);

  if (!!user) {
    const token = user.token;
    let isValidToken: boolean = IsValidToken(token!);

    if (isValidToken) {
      return true;
    } else {
      accountService.removeCurrentUser();
      router.navigate(['login']);
      return false;
    }
  }

  router.navigate(['login']);
  return false;
};

export function IsValidToken(token: string) {
  let decoded = jwtDecode<any>(token!);
  if (!(!!decoded)) return false;

  let expiryDate = new Date(decoded.exp * 1000);
  if (expiryDate.getTime() <= new Date().getTime()) return false

  return true;
}

