import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, AbstractControl, FormsModule, ReactiveFormsModule, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { CommonModule } from '@angular/common';
import { MATERIAL_IMPORTS } from '../../shared/material';

@Component({
  selector: 'app-reset-password',
  imports: [FormsModule, CommonModule, RouterModule, ReactiveFormsModule, ...MATERIAL_IMPORTS],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.scss'
})
export class ResetPasswordComponent implements OnInit {
  token: string | null = null;
  error: string | null = null;
  success = false;
  loading = false;
  hidePassword = true;
  hideConfirmPassword = true;
  resetForm: FormGroup;
  
  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private auth: AuthService,
    private router: Router
  ) {
    this.resetForm = this.fb.group({
        password: ['', [
        Validators.required,
        Validators.minLength(8),
        this.passwordStrengthValidator
        ]],
        confirmPassword: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.token = this.route.snapshot.queryParamMap.get('token');
    if (!this.token) {
      this.error = 'Invalid or expired token.';
    }
  }

  passwordStrengthValidator(control: AbstractControl) {
    const value = control.value;
    if (!value) return null;
    const hasLetter = /[a-zA-Z]/.test(value);
    const hasNumber = /[0-9]/.test(value);
    const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(value);
    return hasLetter && hasNumber && hasSpecialChar ? null : { passwordStrength: true };
  }

  passwordMatchValidator(group: AbstractControl) {
    const pass = group.get('password')?.value;
    const confirm = group.get('confirmPassword')?.value;
    return pass === confirm ? null : { passwordMismatch: true };
  }

  onSubmit() {
    if (this.resetForm.invalid || !this.token) return;
    this.error = null;
    this.loading = true;
    this.auth.resetPassword(this.token, this.resetForm.value.password!).subscribe({
      next: () => {
        this.success = true;
        this.loading = false;
      },
      error: err => {
        this.error = err.error || err.error?.message || 'Invalid or expired token.';
        this.loading = false;
      }
    });
  }

  togglePassword() { this.hidePassword = !this.hidePassword; }
  toggleConfirmPassword() { this.hideConfirmPassword = !this.hideConfirmPassword; }
}