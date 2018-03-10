import { Component, OnInit } from '@angular/core';

import { Subscription } from 'rxjs/Subscription';

import { WalletService } from '../core/services/wallet.service';
import { Wallet } from '../core/models/wallet';
import { AddressBalance } from '../core/models/address-balance';

@Component({
  selector: 'app-account-balance',
  templateUrl: './account-balance.component.html',
  styleUrls: ['./account-balance.component.css']
})
export class AccountBalanceComponent implements OnInit {

  private walletAddress: string;

  private blockchainNodeUrl: string;

  private balanceForAddress: AddressBalance;

  constructor(private walletService: WalletService) { }

  ngOnInit() {
    const wallet = this.walletService.wallet;
    if (wallet) {
      this.walletAddress = this.walletService.wallet.address;
    }
  }

  public displayBalance(): void {
    this.walletService.getBalance(this.blockchainNodeUrl).subscribe((addressBalance: AddressBalance) => {
      this.balanceForAddress = addressBalance;
    });
  }
}
