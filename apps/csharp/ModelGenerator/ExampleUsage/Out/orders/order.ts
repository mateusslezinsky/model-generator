import { Customer } from '../customers/customer';
import { OrderStatus } from './orderStatus';
import { OrderItem } from './orderItem';
import { Payment } from '../payments/payment';
import { ShippingDetail } from '../other/shippingDetail';

export interface Order {
  customerId: number;
  customer: Customer;
  orderDate: Date;
  status: OrderStatus;
  orderItems: OrderItem[];
  payment: Payment;
  shippingDetail: ShippingDetail;
  totalAmount: number;
}
