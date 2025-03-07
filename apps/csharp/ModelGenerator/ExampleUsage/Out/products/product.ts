import { Category } from '../categories/category';
import { Supplier } from '../other/supplier';

export interface Product {
  name: string;
  description: string;
  price: number;
  categoryId: number;
  category: Category;
  supplierId: number;
  supplier: Supplier;
}
