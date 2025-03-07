import { Order } from '../orders/order';

export interface Employee {
  firstName: string;
  lastName: string;
  email: string;
  managerId: number | null;
  manager: Employee;
  subordinates: Employee[];
  ordersHandled: Order[];
  fullName: string;
}
