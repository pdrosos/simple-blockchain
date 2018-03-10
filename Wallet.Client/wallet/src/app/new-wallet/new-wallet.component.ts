import { Component, OnInit } from '@angular/core';

import { WalletService } from '../core/services/wallet.service';

import { Wallet } from '../core/models/wallet';

@Component({
  selector: 'app-new-wallet',
  templateUrl: './new-wallet.component.html',
  styleUrls: ['./new-wallet.component.css']
})
export class NewWalletComponent implements OnInit {

  private generatedPrivateKey: string;

  private generatedPublicKey: string;

  private generatedAddress: string;

  private walletGenerated = false;

  constructor(private walletService: WalletService) { }

  ngOnInit() {
  }

  public generateWallet(): void {
    this.walletService.generateWallet().subscribe((wallet: Wallet) => {
      if (wallet) {
        this.generatedPrivateKey = wallet.privateKey;

        this.generatedPublicKey = wallet.publicKey;

        this.generatedAddress = wallet.address;

        this.walletGenerated = true;
      } else {
        this.walletGenerated = false;
      }
    });
  }
}
