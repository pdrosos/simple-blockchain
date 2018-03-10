import { Injectable } from '@angular/core';

import { HttpClient, HttpErrorResponse } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { ErrorObservable } from 'rxjs/observable/ErrorObservable';

import { catchError } from 'rxjs/operators';

import 'rxjs/add/observable/of';

import { Wallet } from '../models/wallet';

import { AddressBalance } from '../models/address-balance';

import { CryptographyService } from './cryptography.service';

import { ErrorHandlerService } from './error-handler.service';

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

  public getBalance(blockchainNodeUrl): Observable<AddressBalance>{
    return this.http.get<AddressBalance>(blockchainNodeUrl + `/address/${this.wallet.address}/balance`)
      .pipe(
        catchError(this.errorHandlerService.handleError)
      );
  }
}
