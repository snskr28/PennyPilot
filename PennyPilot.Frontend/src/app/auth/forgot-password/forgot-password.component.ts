import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MATERIAL_IMPORTS } from '../../shared/material';

@Component({
  selector: 'app-forgot-password',
  imports: [FormsModule, CommonModule, RouterModule, ReactiveFormsModule, ...MATERIAL_IMPORTS],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss'
})
export class ForgotPasswordComponent {
  sent = false;
  error: string | null = null;
  loading = false;
  forgotForm:FormGroup;

  constructor(private fb: FormBuilder, private auth: AuthService) {
    this.forgotForm = this.fb.group({
      emailOrUsername: ['', [Validators.required]]
    });
  }

  onSubmit() {
    if (this.forgotForm.invalid) return;
    this.error = null;
    this.loading = true;
    this.auth.forgotPassword(this.forgotForm.value.emailOrUsername!).subscribe({
      next: () => {
        this.sent = true;
        this.loading = false;
      },
      error: err => {
        this.error = err.error?.message || 'Username/Email not found.';
        this.loading = false;
      }
    });
  }
}