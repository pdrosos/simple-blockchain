export class Wallet {
    private _privateKey: string;

    private _publicKey: string;

    private _address: string;

    public constructor(privateKey: string, publicKey: string, address: string) {
        this._privateKey = privateKey;
        this._publicKey = publicKey;
        this._address = address;
    }
    
    get privateKey(): string {
        return this._privateKey;
    }

    get publicKey(): string {
        return this._publicKey;
    }

    get address(): string {
        return this._address;
    }
}