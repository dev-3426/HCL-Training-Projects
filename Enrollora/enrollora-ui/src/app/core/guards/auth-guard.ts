import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../auth/token.service';

export const authGuard: CanActivateFn = () => {

  const tokenService = inject(TokenService);
  const router = inject(Router);

  if (tokenService.hasToken() && !tokenService.isTokenExpired()) {
    return true;
  }

  tokenService.removeToken();
  return router.createUrlTree(['/login']);
};