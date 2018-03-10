import { Injectable } from '@angular/core';

import { HttpClient, HttpErrorResponse } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { ErrorObservable } from 'rxjs/observable/ErrorObservable';

import { catchError } from 'rxjs/operators';

import 'rxjs/add/observable/of';

import { Wallet } from '../models/wallet';

import { AddressBalance } from '../models/address-balance';

import { CryptographyService } from './cryptography.service';

@Injectable()
export class WalletService {

  private _wallet: Wallet;

  get wallet(): Wallet {
    return this._wallet;
  }

  constructor(private cryptographyService: CryptographyService, private http: HttpClient) {
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
        catchError(this.handleError)
      );
  }

  private handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${error.error}`);
    }
    // return an ErrorObservable with a user-facing error message
    return new ErrorObservable(
      'Something bad happened; please try again later.');
  };
}
