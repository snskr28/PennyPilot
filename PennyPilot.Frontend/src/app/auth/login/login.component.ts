import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MATERIAL_IMPORTS } from '../../shared/material';

@Component({
  selector: 'app-login',
  imports: [CommonModule, RouterModule, ReactiveFormsModule, FormsModule, ...MATERIAL_IMPORTS],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  hidePassword = true;

  constructor(private fb: FormBuilder) {
    this.loginForm = this.fb.group({
      emailOrUsername: ['', [Validators.required, this.emailOrUsernameValidator]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit() {}

  // Custom validator for email or username
  emailOrUsernameValidator(control: AbstractControl) {
    const value = control.value;
    if (!value) return null;
    
    // If it contains @, validate as email
    if (value.includes('@')) {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      return emailRegex.test(value) ? null : { email: true };
    }
    
    return null;
  }

  onSubmit() {
    if (this.loginForm.valid) {
      console.log('Login submitted:', this.loginForm.value);
      // Handle login logic here
    }
  }

  togglePassword() {
    this.hidePassword = !this.hidePassword;
  }
}