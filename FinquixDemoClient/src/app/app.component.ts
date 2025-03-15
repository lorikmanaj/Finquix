import { Component } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { FinancialSignalsComponent } from './components/financial-signals/financial-signals.component';
import { HttpClientModule } from '@angular/common/http';
import { CryptoMarketComponent } from './components/crypto-market/crypto-market.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterModule,
    RouterOutlet,
    HttpClientModule,
    // FinancialSignalsComponent,
    // CryptoMarketComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'FinquixDemoClient';

  constructor(private router: Router) { }

  goToOnboarding() {
    this.router.navigate(['/onboarding']);
  }
}
