import { Component, OnInit } from '@angular/core';

import { Transaction } from '../core/models/transaction';

import { TransactionSubmissionResponse } from '../core/models/transaction-submission-response';

import { WalletService } from '../core/services/wallet.service';

@Component({
  selector: 'app-transaction',
  templateUrl: './transaction.component.html',
  styleUrls: ['./transaction.component.css']
})
export class TransactionComponent implements OnInit {

  private transactionPostModel: Transaction;

  private transactionSubmissionResponse: TransactionSubmissionResponse;

  private blockchainNodeUrl: string;

  private signedTransactionJSON: string;

  private errorMessage: string;

  private model: any;

  constructor(private walletService: WalletService) { }

  ngOnInit() {
    this.transactionPostModel = new Transaction();

    this.model = {};

    let wallet = this.walletService.wallet;

    if (wallet) {
      this.transactionPostModel.from = wallet.address;
    }
  }

  public onTransactionSign(): void {
    this.walletService.signTransaction(this.transactionPostModel);
    this.signedTransactionJSON = JSON.stringify(this.transactionPostModel);
  }

  public onTransactionSend(): void {
    this.walletService.sendTransaction(this.blockchainNodeUrl, this.transactionPostModel).subscribe(
      transactionSubmissionResponse => this.transactionSubmissionResponse = { ...transactionSubmissionResponse },
      error => this.errorMessage = error
    );
  }
}
