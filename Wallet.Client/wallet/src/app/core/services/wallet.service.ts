import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { ErrorObservable } from 'rxjs/observable/ErrorObservable';

import { catchError } from 'rxjs/operators';

import 'rxjs/add/observable/of';

import { Wallet } from '../models/wallet';

import { AddressBalance } from '../models/address-balance';

import { Transaction } from '../models/transaction';

import { TransactionSubmissionResponse } from '../models/transaction-submission-response';

import { CryptographyService } from './cryptography.service';

import { ErrorHandlerService } from './error-handler.service';

import { TransactionFee } from '../../constants/constants';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type':  'application/json'
  })
};

@Injectable()
export class WalletService {

  private _wallet: Wallet;

  get wallet(): Wallet {
    return this._wallet;
  }

  constructor(
    private cryptographyService: CryptographyService,
    private http: HttpClient,
    private errorHandlerService: ErrorHandlerService) {
  }

  public generateWallet(): Observable<Wallet> {
    let privateKey = this.cryptographyService.generatePrivateKey();

    let publicKey = this.cryptographyService.privateKeyToPublicKey(privateKey);

    let address = this.cryptographyService.publicKeyToAddress(publicKey);

    let wallet = new Wallet(privateKey, publicKey, address);

    return Observable.of(wallet);
  }

  public getWallet(walletPrivateKey: string): Observable<Wallet> {

    let publicKey = this.cryptographyService.privateKeyToPublicKey(walletPrivateKey);

    let address = this.cryptographyService.publicKeyToAddress(publicKey);

    let wallet = new Wallet(walletPrivateKey, publicKey, address);

    if (wallet.privateKey && wallet.publicKey && wallet.address) {
      this._wallet = wallet;
    }

    return Observable.of(wallet);
  }

  public getBalance(blockchainNodeUrl: string): Observable<AddressBalance> {
    return this.http.get<AddressBalance>(blockchainNodeUrl + `/address/${this.wallet.address}/balance`)
      .pipe(
        catchError(this.errorHandlerService.handleError)
      );
  }

  public signTransaction(transactionPostModel: Transaction): void {
    transactionPostModel.senderPubKey = this.wallet.publicKey;

    transactionPostModel.fee = TransactionFee;

    transactionPostModel.dateCreated = new Date();

    let transaction = {
      from: transactionPostModel.from,
      to: transactionPostModel.to,
      senderPubKey: transactionPostModel.senderPubKey,
      value: transactionPostModel.value,
      fee: transactionPostModel.fee,
      dateCreated: transactionPostModel.dateCreated
    }

    let transactionJSON = JSON.stringify(transaction);

    console.log('transactionJSON:');
    console.log(transactionJSON);

    let transactionHash = this.cryptographyService.sha256(transactionJSON);

    transactionPostModel.senderSignature = this.cryptographyService.signData(transactionHash, this.wallet.privateKey);
  }

  public sendTransaction(blockchainNodeUrl: string, transaction: Transaction): Observable<TransactionSubmissionResponse> {

    let requestUrl = `${blockchainNodeUrl}/transactions/send`;

    return this.http.post<TransactionSubmissionResponse>(requestUrl, transaction, httpOptions)
      .pipe(
        catchError(this.errorHandlerService.handleError)
      );
  }
}
