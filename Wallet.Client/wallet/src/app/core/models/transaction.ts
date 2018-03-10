export class Transaction {
    public from: string;
    
    public to: string;

    public senderPubKey: string;

    public value: number;

    public fee: number;

    public dateCreated: Date;

    public senderSignature: string[];

    constructor() {
    }
}