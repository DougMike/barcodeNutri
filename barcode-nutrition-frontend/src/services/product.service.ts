import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Product } from '../models/product';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = 'http://localhost:5000'; // API .NET 9

  constructor(private http: HttpClient) {}

  getProduct(barcode: string): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/products/${barcode}`);
  }
}
