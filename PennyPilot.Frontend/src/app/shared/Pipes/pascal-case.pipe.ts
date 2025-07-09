import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'pascalCase',
})
export class PascalCasePipe implements PipeTransform {
  transform(value: string): string {
    if (!value || typeof value !== 'string') return value;

    // 🚫 Skip ISO-like date strings (e.g., '2025-07-09', '2024/12/25')
    if (
      /^\d{1}[-/]\d{1}[-/]\d{4}/.test(value) ||
      /^\d{2}[-/]\d{1}[-/]\d{4}/.test(value) ||
      /^\d{1}[-/]\d{2}[-/]\d{4}/.test(value) ||
      /^\d{2}[-/]\d{2}[-/]\d{4}/.test(value)
    ) {
      return value;
    }
    return value
      .replace(/[^a-zA-Z0-9 ]/g, '') // remove special characters
      .split(' ')
      .map((word) => word.charAt(0).toUpperCase() + word.slice(1).toLowerCase())
      .join('');
  }
}
