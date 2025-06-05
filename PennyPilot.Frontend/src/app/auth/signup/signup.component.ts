import { Component, inject, OnInit, signal } from '@angular/core';
import { MATERIAL_IMPORTS } from '../../shared/material';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-signup',
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    FormsModule,
    ...MATERIAL_IMPORTS,
  ],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss',
})
export class SignupComponent implements OnInit {
  signupForm: FormGroup;
  hidePassword = true;
  hideConfirmPassword = true;
  authService = inject(AuthService);
  signupError: string | null = null;
  loading = false;

  constructor(private fb: FormBuilder, private router: Router) {
    this.signupForm = this.fb.group(
      {
        username: [
          '',
          [
            Validators.required,
            this.usernameValidator, // Add the custom validator
          ],
        ],
        email: ['', [Validators.required, Validators.email]],
        firstName: ['', [Validators.required]],
        middleName: [null],
        lastName: ['', [Validators.required]],
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(8),
            this.passwordStrengthValidator,
          ],
        ],
        confirmPassword: ['', [Validators.required]],
      },
      { validators: this.passwordMatchValidator }
    );
  }

  ngOnInit() {}

  // Custom validator for password match
  passwordMatchValidator(form: AbstractControl) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');

    if (!password || !confirmPassword) return null;

    if (confirmPassword.value && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
    } else if (confirmPassword.hasError('passwordMismatch')) {
      delete confirmPassword.errors!['passwordMismatch'];
      if (Object.keys(confirmPassword.errors!).length === 0) {
        confirmPassword.setErrors(null);
      }
    }

    return null;
  }

  // Custom validator for username
  usernameValidator(control: AbstractControl) {
    const forbidden = /[!@#$%^&*()+\=\[\]{};':"\\|,.<>\/?]+/;
    return forbidden.test(control.value) ? { specialCharacters: true } : null;
  }

  // Update the passwordStrengthValidator function
  passwordStrengthValidator(control: AbstractControl) {
    const value = control.value;
    if (!value) return null;

    const hasLetter = /[a-zA-Z]+/.test(value); // at least one letter
    const hasNumber = /[0-9]+/.test(value); // at least one number
    const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]+/.test(value); // at least one special char

    const passwordValid = hasLetter && hasNumber && hasSpecialChar;

    return !passwordValid ? { passwordStrength: true } : null;
  }

  onSubmit() {
    if (this.signupForm.invalid) return;
    this.loading = true;
    this.authService.signup(this.signupForm.value).subscribe({
      next: () => {
        this.router.navigate(['/login']);
        this.loading = false;
      },
      error: (err) => {
        this.signupError =
          err.error?.message || 'Signup failed. Please try again.';
        this.loading = false;
      },
    });
  }

  togglePassword() {
    this.hidePassword = !this.hidePassword;
  }

  toggleConfirmPassword() {
    this.hideConfirmPassword = !this.hideConfirmPassword;
  }
}
