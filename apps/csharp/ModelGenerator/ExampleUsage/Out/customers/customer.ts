import { Order } from '../orders/order';
import { Address } from '../addresses/address';

export interface Customer {
  firstName: string;
  lastName: string;
  email: string;
  orders: Order[];
  addresses: Address[];
  fullName: string;
}
