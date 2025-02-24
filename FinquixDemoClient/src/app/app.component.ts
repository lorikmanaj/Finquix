import { Component } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { FinancialSignalsComponent } from './components/financial-signals/financial-signals.component';
import { HttpClientModule } from '@angular/common/http';
import { CryptoComponent } from './components/crypto/crypto.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterModule,
    RouterOutlet,
    HttpClientModule,
    FinancialSignalsComponent,
    CryptoComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'FinquixDemoClient';

  // In your component or constructor
  constructor(private router: Router) { }

  // Call this method to navigate
  goToOnboarding() {
    this.router.navigate(['/onboarding']);
  }

}
