import { Customer } from '../customers/customer';

export interface Address {
  street: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
  customerId: number;
  customer: Customer;
}
