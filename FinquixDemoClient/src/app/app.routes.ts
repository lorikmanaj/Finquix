// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { OnboardingComponent } from './components/onboarding/onboarding.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { FinancialSignalsComponent } from './components/financial-signals/financial-signals.component';
import { CryptoMarketComponent } from './components/crypto-market/crypto-market.component';

export const routes: Routes = [
    { path: '', component: LoginComponent },
    { path: 'dashboard', component: DashboardComponent },
    { path: 'onboarding', component: OnboardingComponent },
    { path: 'crypto-market', component: CryptoMarketComponent },
    { path: 'financial-signals', component: FinancialSignalsComponent }
];
