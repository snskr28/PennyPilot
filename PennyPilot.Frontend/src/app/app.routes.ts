import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { AuthContainerComponent } from './auth/auth-container.component';

export const routes: Routes = [
  {
    path: '',
    component: AuthContainerComponent,
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'signup', component: SignupComponent },
      { path: '', redirectTo: 'login', pathMatch: 'full' }
    ]
  }
];
