import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from '../home/home.component';
import { NewWalletComponent } from '../new-wallet/new-wallet.component';
import { ExistingWalletComponent } from '../existing-wallet/existing-wallet.component';
import { AccountBalanceComponent } from '../account-balance/account-balance.component';
import { TransactionComponent } from '../transaction/transaction.component';
import { LogoutComponent } from '../logout/logout.component';

const appRoutes: Routes = [
  {
    path: 'new-wallet',
    component: NewWalletComponent
  },
  {
    path: 'existing-wallet',
    component: ExistingWalletComponent
  },
  {
    path: 'account-balance',
    component: AccountBalanceComponent
  },
  {
    path: 'send-transaction',
    component: TransactionComponent
  },
  {
    path: 'logout',
    component: LogoutComponent
  },
  {
    path: '',
    component: HomeComponent
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(appRoutes)
  ],
  exports: [
    RouterModule
  ],
  providers: []
})
export class AppRoutingModule { }
