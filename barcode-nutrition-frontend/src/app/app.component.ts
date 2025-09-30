import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ZXingScannerModule } from '@zxing/ngx-scanner';
import { BarcodeFormat } from '@zxing/library';
import { ProductService } from '../services/product.service';
import { Product } from '../models/product';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, HttpClientModule, ZXingScannerModule],
  templateUrl: './app.component.html',
  styleUrls: []
})
export class AppComponent {
  product: Product | null = null;
  loading = false;
  error = '';

  // Permitir apenas alguns formatos comuns
  allowedFormats = [
    BarcodeFormat.CODE_128,
    BarcodeFormat.EAN_13,
    BarcodeFormat.EAN_8,
    BarcodeFormat.UPC_A,
    BarcodeFormat.UPC_E
  ];

  constructor(private productService: ProductService) {}

  onScanSuccess(barcode: string) {
    if (!barcode) return;

    console.log('Código lido:', barcode);

    this.loading = true;
    this.error = '';
    this.product = null;

    this.productService.getProduct(barcode).subscribe({
      next: (p: any) => {
        this.product = p;
        this.loading = false;
      },
      error: () => {
        this.error = 'Produto não encontrado ou erro na API';
        this.loading = false;
      }
    });
  }
}
