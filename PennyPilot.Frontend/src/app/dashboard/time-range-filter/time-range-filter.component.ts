import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { MATERIAL_IMPORTS } from '../../shared/material';

interface TimeRangeOption {
  label: string;
  value: string;
}

@Component({
  selector: 'app-time-range-filter',
  imports: [
    FormsModule,
    CommonModule,
    ReactiveFormsModule,
    ...MATERIAL_IMPORTS,
  ],
  templateUrl: './time-range-filter.component.html',
  styleUrl: './time-range-filter.component.scss',
})
export class TimeRangeFilterComponent {
  @Output() dateRangeChange = new EventEmitter<{
    start: Date | null;
    end: Date | null;
    granularity: string;
  }>();

  timeRanges: TimeRangeOption[] = [
    { label: 'All', value: 'all' },
    { label: 'Last 7 Days', value: 'last7' },
    { label: 'Last 30 Days', value: 'last30' },
    { label: 'This Month', value: 'thisMonth' },
    { label: 'Last Month', value: 'lastMonth' },
    { label: 'This Year', value: 'thisYear' },
    { label: 'Custom', value: 'custom' },
  ];

  granularities = [
    { label: 'Daily', value: 'daily' },
    { label: 'Monthly', value: 'monthly' },
    { label: 'Quarterly', value: 'quarterly' },
    { label: 'Yearly', value: 'yearly' },
  ];

  form: FormGroup;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      range: ['all'],
      start: [null],
      end: [null],
      granularity: ['monthly'],
    });

    this.form.valueChanges.subscribe((val) => {
      this.emitRange();
    });
  }

  emitRange() {
    const { range, start, end, granularity } = this.form.value;
    let startDate: Date | null = null;
    let endDate: Date | null = null;

    const today = new Date();
    switch (range) {
      case 'all':
        startDate = null;
        endDate = null;
        break;
      case 'last7':
        startDate = new Date(today);
        startDate.setDate(today.getDate() - 6);
        endDate = today;
        break;
      case 'last30':
        startDate = new Date(today);
        startDate.setDate(today.getDate() - 29);
        endDate = today;
        break;
      case 'thisMonth':
        startDate = new Date(today.getFullYear(), today.getMonth(), 1);
        endDate = today;
        break;
      case 'lastMonth':
        startDate = new Date(today.getFullYear(), today.getMonth() - 1, 1);
        endDate = new Date(today.getFullYear(), today.getMonth(), 0);
        break;
      case 'thisYear':
        startDate = new Date(today.getFullYear(), 0, 1);
        endDate = today;
        break;
      case 'custom':
        startDate = start;
        endDate = end;
        break;
    }
    this.dateRangeChange.emit({ start: startDate, end: endDate, granularity });
  }

  showCustom(): boolean {
    return this.form.get('range')?.value === 'custom';
  }

  clear() {
    this.form.patchValue({ range: 'all', start: null, end: null, granularity: 'monthly' });
  }
}
