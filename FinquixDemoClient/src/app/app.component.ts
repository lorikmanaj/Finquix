import { Component } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { FinancialSignalsComponent } from './components/financial-signals/financial-signals.component';
import {} from '@angular/common/http';
import { CryptoMarketComponent } from './components/crypto-market/crypto-market.component';
import { ChatComponent } from './components/chat/chat.component';

@Component({
    selector: 'app-root',
    imports: [
        RouterModule,
        RouterOutlet,
        // TODO: `HttpClientModule` should not be imported into a component directly.
        // Please refactor the code to add `provideHttpClient()` call to the provider list in the
        // application bootstrap logic and remove the `HttpClientModule` import from this component.
        HttpClientModule,
        ChatComponent
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
