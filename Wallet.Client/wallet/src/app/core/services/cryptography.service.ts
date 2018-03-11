import { Injectable } from '@angular/core';

import * as CryptoJS from 'crypto-js';

@Injectable()
export class CryptographyService {

  private secp256k1 = new elliptic.ec('secp256k1');

  constructor() { }

  public generatePrivateKey(): string {
    let keyPair = this.secp256k1.genKeyPair();
    let privateKey = keyPair.getPrivate().toString(16);
    return privateKey;
  }

  // public privateKeyToPublicKey(privateKey): string {
  //   let keyPair = this.secp256k1.keyFromPrivate(privateKey);
  //   let publicKey = keyPair.getPublic().getX().toString(16) +
  //       (keyPair.getPublic().getY().isOdd() ? "1" : "0");
  //   return publicKey;
  // }

  public privateKeyToPublicKey(privateKey) {
    let keyPair = this.secp256k1.keyFromPrivate(privateKey);
    let publicKey = keyPair.getPublic();

    let prefix = publicKey.getY().isOdd() ? "03" : "02";
    let x = publicKey.getX().toString(16);

    return prefix + x;
  }

  public publicKeyToAddress(publicKey): string {
    let address = CryptoJS.RIPEMD160(publicKey).toString();
    return address;
  }

  public privateKeyToAddress(privateKey): string {
    let publicKey = this.privateKeyToPublicKey(privateKey);
    let address = this.publicKeyToAddress(publicKey);
    return address;
  }

  public signData(data, privateKey): string[] {
    let keyPair = this.secp256k1.keyFromPrivate(privateKey);
    let signature = keyPair.sign(data);
    return [signature.r.toString(16), signature.s.toString(16)];
  }

  public verifySignature(data, publicKey, signature): boolean {
    let publicKeyPoint = this.decompressPublicKey(publicKey);
    let keyPair = this.secp256k1.keyPair({pub: publicKeyPoint});
    let valid = keyPair.verify(data, {r: signature[0], s: signature[1]});
    return valid;
  }

  public decompressPublicKey(publicKeyCompressed): any {
    let publicKeyX = publicKeyCompressed.substring(0, 64);
    let publicKeyYOdd = parseInt(publicKeyCompressed.substring(64));
    let publicKeyPoint = this.secp256k1.curve.pointFromX(publicKeyX, publicKeyYOdd);
    return publicKeyPoint;
  }

  public sha256(data): string {
    return CryptoJS.SHA256(data).toString(CryptoJS.enc.Hex);
  }
}
