import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MATERIAL_IMPORTS } from '../../shared/material';
import { TransactionsService } from '../services/transactions.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-add-transaction-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ...MATERIAL_IMPORTS],
  templateUrl: './add-transaction-dialog.component.html',
  styleUrls: ['./add-transaction-dialog.component.scss'],
})
export class AddTransactionDialogComponent {
  form: FormGroup;
  get entries() {
    return this.form.get('entries') as FormArray;
  }

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddTransactionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { type: 'income' | 'expense' },
    private transactionsService: TransactionsService,
    private snackBar: MatSnackBar
  ) {
    this.form = this.fb.group({
      entries: this.fb.array([this.createEntry()]),
    });
  }

  createEntry(): FormGroup {
    if (this.data.type === 'income') {
      return this.fb.group({
        title: ['', Validators.required],
        category: ['', Validators.required],
        source: ['', Validators.required],
        description: [''],
        amount: [0, [Validators.required, Validators.min(0.01)]],
        date: ['', Validators.required],
      });
    } else {
      return this.fb.group({
        title: ['', Validators.required],
        category: ['', Validators.required],
        description: [''],
        amount: [0, [Validators.required, Validators.min(0.01)]],
        paymentMode: ['', Validators.required],
        paidBy: ['', Validators.required],
        date: ['', Validators.required],
      });
    }
  }

  addEntry() {
    this.entries.push(this.createEntry());
  }

  removeEntry(i: number) {
    if (this.entries.length > 1) this.entries.removeAt(i);
  }

  submit() {
    if (this.form.invalid) return;
    const entries = this.entries.value;

    if (this.data.type === 'expense') {
      this.transactionsService.addExpenses(entries).subscribe({
        next: (res) => {
          this.snackBar.open(res.message, 'Close', { duration: 3000 });
          this.dialogRef.close(true); // Pass true to indicate refresh needed
        },
        error: (err) => {
          console.error(err);
          this.snackBar.open('Failed to add expenses', 'Close', { duration: 3000 });
        }
      });
    } else {
      // ...existing code for income (if any)...
    }
  }

  cancel() {
    this.dialogRef.close();
  }
}
