import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { NgModule } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';


export const routes: Routes = [
    {path:'login', component:LoginComponent},
    {path:'signup', component:SignupComponent},
    {path:'',redirectTo:'login',pathMatch:'full'}
];
