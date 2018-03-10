export class Transaction {
    public from: string;
    
    public to: string;

    public value: number;

    public fee: number;

    public dateCreated: Date;

    public senderPubKey: string;

    public senderSignature: string[];

    constructor() {
    }
}