
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../auth/token.service';

export const roleGuard: CanActivateFn = (route) => {

  const tokenService = inject(TokenService);
  const router = inject(Router);
  const requiredRole = route.data['role'];
  const currentRole = tokenService.getRole();

  if (currentRole === requiredRole) {
    return true;
  }

  if(currentRole === 'Administrator') {
    return router.createUrlTree(['/admin/dashboard']);
  } else if(currentRole === 'Student') {
    return router.createUrlTree(['/student/dashboard']);
  }

  return router.createUrlTree(['/login']);
};
