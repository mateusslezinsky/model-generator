import { Order } from '../orders/order';
import { ShippingStatus } from './shippingStatus';

export interface ShippingDetail {
  orderId: number;
  order: Order;
  shippingAddress: string;
  estimatedDelivery: Date;
  status: ShippingStatus;
}
