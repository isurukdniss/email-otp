import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { EmailOtp } from "./components/email-otp/email-otp";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, EmailOtp],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('email-otp-app');
}
