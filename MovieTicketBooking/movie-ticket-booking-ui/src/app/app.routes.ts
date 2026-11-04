import { Routes } from '@angular/router';
import { HomeComponent } from './features/home.component';
import { LoginComponent } from './features/login.component';
import { RegisterComponent } from './features/register.component';
import { MovieDetailsComponent } from './features/movie-details.component';
import { BookingComponent } from './features/booking.component';
import { MyBookingsComponent } from './features/my-bookings.component';
import { AdminComponent } from './features/admin.component';
import { customerGuard, adminGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'movies/:id', component: MovieDetailsComponent },
  { path: 'book/:showId', component: BookingComponent, canActivate: [customerGuard] },
  { path: 'my-bookings', component: MyBookingsComponent, canActivate: [customerGuard] },
  { path: 'admin', component: AdminComponent, canActivate: [adminGuard] },
  { path: '**', redirectTo: '' }
];
