import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

// Form-level validator (keep this as backup)
export const passwordMatchValidator: ValidatorFn = (form: AbstractControl): ValidationErrors | null => {
  const password = form.get('password')?.value;
  const confirmPassword = form.get('cpassword')?.value;
  if (!password || !confirmPassword) return null;
  return password === confirmPassword ? null : { passwordMismatch: true };
};

// Field-level validator for confirm password field
export const confirmPasswordValidator = (passwordControlName: string): ValidatorFn => {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.parent) return null;
    
    const passwordControl = control.parent.get(passwordControlName);
    if (!passwordControl || !control.value || !passwordControl.value) return null;
    
    return passwordControl.value === control.value ? null : { passwordMismatch: true };
  };
};