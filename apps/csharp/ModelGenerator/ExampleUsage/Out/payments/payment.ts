import { Order } from '../orders/order';
import { PaymentType } from './paymentType';
import { PaymentStatus } from './paymentStatus';

export interface Payment {
  orderId: number;
  order: Order;
  paymentMethod: PaymentType;
  status: PaymentStatus;
  paymentDate: Date;
  amount: number;
}
