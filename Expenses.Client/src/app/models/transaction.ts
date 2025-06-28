export interface Transaction {
  id?: string;
  type: string;
  category?: string;
  amount: number;
  transactionDate: Date;
}
