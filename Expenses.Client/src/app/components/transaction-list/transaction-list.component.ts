import {Component, OnInit} from '@angular/core';
import {Transaction} from '../../models/transaction';
import {CommonModule} from '@angular/common';
import {TransactionServices} from '../../services/transaction.services';
import {Router} from '@angular/router';

@Component({
  selector: 'app-transaction-list',
  imports : [CommonModule],
  templateUrl: './transaction-list.component.html',
  styleUrl: './transaction-list.component.css'
})
export class TransactionListComponent implements OnInit {


  transactions: Transaction[] = [];
  constructor(private  transactionService: TransactionServices, private router: Router) {
    // This is where you would typically fetch transactions from a service
    // this.transactionService.getTransactions().subscribe(data => {
    //   this.transactions = data;
    // });
  }

  ngOnInit() {
    this.loadTransactions();
  }

  loadTransactions() {
    this.transactionService.getTransactions()
      .subscribe(data => {
        this.transactions = data;
      });
  }

  getTotalIncome(): number {
    return this.transactions
      .filter(transaction => transaction.type === 'Income')
      .reduce((total, transaction) => total + transaction.amount, 0);
  }

  getTotalExpense(): number {
    return this.transactions
      .filter(transaction => transaction.type === 'Expense')
      .reduce((total, transaction) => total + transaction.amount, 0);
  }

  getNetBalance(): number {
    return this.getTotalIncome() - this.getTotalExpense();
  }

  editTransaction(transaction: Transaction) {
    if (transaction.id){
      // Navigate to the edit form with the transaction ID
      this.router.navigate(['/edit', transaction.id]);
      console.log(`Editing transaction with ID: ${transaction.id}`);
    }
  }

  deleteTransaction(id: string | undefined) {
    if (id) {
      if (confirm('Are you sure you want to delete this transaction?'))
      {
        this.transactionService.delete(id)
          .subscribe({
            next: () => {
              console.log(`Transaction with ID: ${id} deleted`);
              this.loadTransactions(); // Reload transactions after deletion
            },
            error: (error) => {
              console.error('Error deleting transaction:', error);
            }
          });
      }
    }
  }
}

function generateGUID(): string {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    const r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
    return v.toString(16);
  });
}
