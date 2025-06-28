import { Component } from '@angular/core';
import {Router, RouterLink} from '@angular/router';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {CommonModule} from '@angular/common';
import {AuthService} from '../../services/auth.service';

@Component({
  selector: 'app-signup',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css'
})

export class SignupComponent {
  signupForm: FormGroup;
  errorMessage: string | null = null;
  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router ) {
    this.signupForm = this.fb.group(
      {
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', [Validators.required, Validators.minLength(6)]]
      },
      {
        validators: this.passwordMatchValidator
      }
    );
  }

  hasError(controlName: string, errorName: string): boolean {
    const control = this.signupForm.get(controlName);
    return control?.hasError(errorName) && (control?.touched || control?.dirty) || false;
  }

  private passwordMatchValidator(fg: FormGroup) {
    return fg.get('password')?.value === fg.get('confirmPassword')?.value
      ? null : { passwordMismatch: true };

  }

  onSubmit() {
    this.errorMessage = null; // Reset error message on new submission

    if (this.signupForm.valid){
      const signupData = this.signupForm.value;
      this.authService.register(signupData).subscribe({
        next: (response) => {
          console.log('Signup successful', response);
          this.router.navigate(['/transactions']);
        },
        error: (error) => {
          console.error('Signup failed', error);
          this.errorMessage = error.error?.message || 'An error occurred during signup. Please try again.';
        }
      });
    }
  }
}
