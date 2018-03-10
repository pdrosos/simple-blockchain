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

  private transactionSigned: boolean;

  private errorMessage: string;

  private model: any;

  constructor(private walletService: WalletService) { }

  ngOnInit() {
    this.transactionPostModel = new Transaction();

    let wallet = this.walletService.wallet;

    if (wallet) {
      this.transactionPostModel.from = wallet.address;
    }

    this.transactionSigned = false;
  }

  public onTransactionSign(): void {
    this.walletService.signTransaction(this.transactionPostModel);
    this.signedTransactionJSON = JSON.stringify(this.transactionPostModel, null, 2);
    if (this.signedTransactionJSON) {
      this.transactionSigned = true;
    }
  }

  public onTransactionSend(): void {
    this.walletService.sendTransaction(this.blockchainNodeUrl, this.transactionPostModel).subscribe(
      transactionSubmissionResponse => { 
        this.transactionSubmissionResponse = { ...transactionSubmissionResponse } 
      },
      error => this.errorMessage = error
    );
  }
}
