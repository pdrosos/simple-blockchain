import { Injectable } from '@angular/core';

import { Observable } from 'rxjs/Observable';

import 'rxjs/add/observable/of';

import { Wallet } from '../models/wallet';

import { CryptographyService } from './cryptography.service';

@Injectable()
export class WalletService {

  private _wallet: Wallet;

  get wallet(): Wallet {
    return this._wallet;
  }

  constructor(private cryptographyService: CryptographyService) {
  }

  public generateWallet(): Observable<Wallet> {
    let privateKey = this.cryptographyService.generatePrivateKey();

    let publicKey = this.cryptographyService.privateKeyToPublicKey(privateKey);

    let address = this.cryptographyService.publicKeyToAddress(publicKey);

    this._wallet = new Wallet(privateKey, publicKey, address);

    return Observable.of(this._wallet);
  }
}
