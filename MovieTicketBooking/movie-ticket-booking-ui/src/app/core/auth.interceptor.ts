import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const raw = localStorage.getItem('cinebook-auth');
  if (!raw) return next(request);
  try {
    const auth = JSON.parse(raw) as { token?: string; expiresAt?: string };
    if (auth.token && (!auth.expiresAt || new Date(auth.expiresAt) > new Date())) {
      request = request.clone({ setHeaders: { Authorization: `Bearer ${auth.token}` } });
    }
  } catch { /* Ignore invalid local storage and continue as anonymous. */ }
  return next(request);
};
