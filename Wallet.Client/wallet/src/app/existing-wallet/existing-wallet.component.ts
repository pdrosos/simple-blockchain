import { Component, OnInit } from '@angular/core';

import { WalletService } from '../core/services/wallet.service';

import { Wallet } from '../core/models/wallet';

@Component({
  selector: 'app-existing-wallet',
  templateUrl: './existing-wallet.component.html',
  styleUrls: ['./existing-wallet.component.css']
})
export class ExistingWalletComponent implements OnInit {

  private walletForDisplay: Wallet;

  private errorMessage: string;

  constructor(private walletService: WalletService) {
  }

  ngOnInit() {
  }

  public openWallet(walletPrivateKey: string): void {
    
    this.walletForDisplay = null;
    this.errorMessage = null;

    if (walletPrivateKey == null || walletPrivateKey.length !== 64) {
      this.errorMessage = 'Private key should be 64 characters long';
      return;
    }
    
    this.walletService.getWallet(walletPrivateKey).subscribe((wallet: Wallet) => {
      if (wallet) {
        this.walletForDisplay = wallet;
      } else {
        this.errorMessage = 'Wallet with this private key does not exist';
      }
    });
  }
}
