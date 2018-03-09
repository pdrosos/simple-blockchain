import { Injectable } from '@angular/core';

import { CryptographyService } from './cryptography.service';

@Injectable()
export class WalletService {

  private _privateKey: string;

  private _publicKey: string;

  private _address: string;

  get privateKey(): string {
    return this._privateKey;
  }

  get publicKey(): string {
    return this._publicKey;
  }

  get address(): string {
    return this._address;
  }

  constructor(private cryptographyService: CryptographyService) {
  }
}
