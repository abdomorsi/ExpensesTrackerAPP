import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {CommonModule} from '@angular/common';
import {ActivatedRoute, Router} from '@angular/router';
import {TransactionServices} from '../../services/transaction.services';

@Component({
  selector: 'app-transaction-form',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './transaction-form.component.html',
  styleUrl: './transaction-form.component.css'
})
export class TransactionFormComponent implements OnInit {
  transactionForm : FormGroup;
  incomeCategories = ['Salary', 'Freelance', 'Investment', 'Other'];
  expenseCategories = ['Food', 'Transport', 'Bills', 'Entertainment', 'Other'];
  availableCategories : string[] = [];

  editMode: boolean = false;
  transactionId: string | null = null;
  constructor(private formBuilder: FormBuilder,
              private activatedRoute: ActivatedRoute,
              private router: Router,
              private transactionService: TransactionServices) {
    this.transactionForm = this.formBuilder.group({
      type: ['Expense', Validators.required],
      category: ['', Validators.required],
      amount: ['', [Validators.required, Validators.min(0)]],
      transactionDate: [new Date(), Validators.required],
    })
  }

  ngOnInit(): void {
    const type = this.transactionForm.get('type')?.value;
    this.updateAvailableCategories(type);
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (id) {
      this.enterEditMode(id);
    } else {
      this.editMode = false;
      this.transactionId = null;
    }
  }
  private enterEditMode(id: string) {
    this.editMode = true;
    this.transactionId = id;
    this.loadTransaction(id);
  }

  loadTransaction(id: string) {
    this.transactionService.getById(id).subscribe({
      next: (transaction) => {
        console.log(transaction);
        this.updateAvailableCategories(transaction.type);
        this.transactionForm.patchValue({
          type: transaction.type,
          category: transaction.category,
          amount: transaction.amount,
          transactionDate: new Date(transaction.transactionDate).toISOString().substring(0, 10),
        });
      },
      error: (err) => {
        console.error('Error fetching transaction:', err);
      }
    });
  }

  onTypeChange() {
    const type = this.transactionForm.get('type')?.value;
    this.updateAvailableCategories(type);
  }

  updateAvailableCategories(type?: string) {
    this.availableCategories = type === 'Expense' ? this.expenseCategories : this.incomeCategories;
    this.transactionForm.patchValue({category:''});
  }

  cancel() {
    this.router.navigate(['/transaction']);
  }

  onSubmit() {
    if (this.transactionForm.valid) {
      const transactionData = this.transactionForm.value;

      if (this.editMode && this.transactionId) {
        // Update existing transaction
        transactionData.id = this.transactionId;
        this.transactionService.update(transactionData).subscribe({
          next: () => {
            this.router.navigate(['/transactions']);
          },
          error: (err) => {
            console.error('Error updating transaction:', err);
          }
        });
      }
      else {
        // Create new transaction
        transactionData.id = undefined; // Ensure id is not set for new transactions
        this.transactionService.create(transactionData).subscribe({
          next: () => {
            this.router.navigate(['/transactions']);
          },
          error: (err) => {
            console.error('Error adding transaction:', err);
          }
        });

      }
    }
  }
}
