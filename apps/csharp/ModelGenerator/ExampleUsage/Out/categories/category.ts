import { Product } from '../products/product';

export interface Category {
  name: string;
  description: string;
  products: Product[];
}
