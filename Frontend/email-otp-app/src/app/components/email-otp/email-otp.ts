import { Component, inject, OnInit, signal, ChangeDetectionStrategy } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { OtpService } from '../../services/otp-service';

interface GenerateOtpForm {
  email: FormControl<string | null>;
}

interface VerifyOtpForm {
  email: FormControl<string | null>;
  otp: FormControl<string | null>;
}

@Component({
  selector: 'app-email-otp',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './email-otp.html',
  styleUrl: './email-otp.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EmailOtp implements OnInit {
  private fb = inject(FormBuilder);
  private otpService = inject(OtpService);
  
  generateOtpForm!: FormGroup<GenerateOtpForm>;
  verifyOtpForm!: FormGroup<VerifyOtpForm>;

  isOtpSent = signal(false);
  isLoading = signal(false);
  isError = signal(false);
  feedbackMessage = signal<string | null>(null);
  userEmail = signal('');

  ngOnInit(): void {
    this.generateOtpForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });

    this.verifyOtpForm = this.fb.group({
      email: [{ value: '', disabled: true }], 
      otp: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
    });
  }

  onGenerateOtpSubmit(): void {
    if (this.generateOtpForm.invalid) {
      return;
    }

    this.isLoading.set(true);
    this.feedbackMessage.set(null);

    const emailValue = this.generateOtpForm.getRawValue().email!;
    this.userEmail.set(emailValue);

    this.otpService.generateOtp({ email: this.userEmail() }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.isOtpSent.set(true);
        this.isError.set(false);
        this.feedbackMessage.set('An OTP has been sent to your email.');
        this.verifyOtpForm.patchValue({ email: this.userEmail() });
      },
      error: (err) => {
        this.isLoading.set(false);
        this.isError.set(true);
        this.feedbackMessage.set(err.error.errorMessage || 'An unexpected error occurred.');
      }
    });
  }

  onVerifyOtpSubmit(): void {
    if (this.verifyOtpForm.invalid) {
      return;
    }
    this.isLoading.set(true);
    this.feedbackMessage.set(null);
    
    const otpValue = this.verifyOtpForm.getRawValue().otp!;

    this.otpService.verifyOtp({ email: this.userEmail(), otp: otpValue }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.isError.set(false);
        this.feedbackMessage.set('Verification successful!');
      },
      error: (err) => {
        this.isLoading.set(false);
        this.isError.set(true);
        this.feedbackMessage.set(err.error?.errorMessage || 'Verification failed.');
      }
    });
  }

  goBack(): void {
    this.isOtpSent.set(false);
    this.feedbackMessage.set(null);
    this.generateOtpForm.reset();
    this.verifyOtpForm.reset();
  }

}
