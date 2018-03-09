import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing/app-routing.module';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { NewWalletComponent } from './new-wallet/new-wallet.component';
import { ExistingWalletComponent } from './existing-wallet/existing-wallet.component';
import { AccountComponent } from './account/account.component';
import { BalanceComponent } from './balance/balance.component';
import { TransactionComponent } from './transaction/transaction.component';
import { LogoutComponent } from './logout/logout.component';
import { NavbarComponent } from './navbar/navbar.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    NewWalletComponent,
    ExistingWalletComponent,
    AccountComponent,
    BalanceComponent,
    TransactionComponent,
    LogoutComponent,
    NavbarComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
