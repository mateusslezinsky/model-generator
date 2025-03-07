import { Product } from '../products/product';

export interface Supplier {
  companyName: string;
  contactName: string;
  phone: string;
  products: Product[];
  additionalInfo: Dictionary;
}
