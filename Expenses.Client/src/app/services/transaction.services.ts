import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Transaction} from '../models/transaction';

@Injectable({
  providedIn: 'root'
})
export class TransactionServices {

  private apiUrl = 'http://localhost:5175/api/Transactions/';
  constructor(private http: HttpClient) { }
  getTransactions() {
    return this.http.get<Transaction[]>(this.apiUrl+'All');
  }
  getById(id: string) {
    return this.http.get<Transaction>(this.apiUrl + id);
  }
  create(transaction: Transaction) {
    return this.http.post<Transaction>(this.apiUrl, transaction);
  }
  update(transaction: Transaction) {
    return this.http.put<Transaction>(this.apiUrl + "Update/"+ transaction.id, transaction);
  }
  delete(id: string) {
    return this.http.delete(this.apiUrl + id);
  }
}
