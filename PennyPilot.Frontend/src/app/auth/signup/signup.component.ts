import { Component, OnInit, signal } from '@angular/core';
import { MATERIAL_IMPORTS } from '../../shared/material';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { confirmPasswordValidator, passwordMatchValidator } from '../custom-validator';

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
  signupForm!: FormGroup;
  password = signal(true);
  cpassword = signal(true);

  constructor(private fb: FormBuilder) {}
  ngOnInit(): void {
    this.signUpForm();
  }
  
  public signUpForm(): void {
    this.signupForm = this.fb.group({
      username: ['', [Validators.required]],
      firstname: ['', [Validators.required]],
      middlename: [''],
      lastname: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      dob: ['', [Validators.required]],
      password: [
        '', 
        [
          Validators.required,
          Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/)
        ]
      ],
        cpassword: ['', [Validators.required, confirmPasswordValidator('password')]],
    });

    // Subscribe to password field to trigger confirm password validation
    this.signupForm.get('password')?.valueChanges.subscribe(() => {
      this.signupForm.get('cpassword')?.updateValueAndValidity();
    });
  }

  onSubmit() {
    if (this.signupForm.valid) {
      console.log(this.signupForm.value);
    }
  }
  
  hidePassword(event: MouseEvent) {
    this.password.set(!this.password());
    event.stopPropagation();
  }
  hideCPassword(event: MouseEvent) {
    this.cpassword.set(!this.cpassword());
    event.stopPropagation();
  }
}
