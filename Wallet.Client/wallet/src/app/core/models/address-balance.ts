import { Balance } from './balance';

export class AddressBalance {
    
    public address: string;

    public confirmedBalance: Balance;

    public lastMinedBalance: Balance;

    public pendingBalance: Balance;
}