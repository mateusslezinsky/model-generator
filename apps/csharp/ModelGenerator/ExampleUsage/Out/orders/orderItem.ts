import { Order } from './order';
import { Product } from '../products/product';

export interface OrderItem {
  orderId: number;
  order: Order;
  productId: number;
  product: Product;
  quantity: number;
  unitPrice: number;
}
