import { Leave } from "./leave";

export interface Employee {
  id: string;
  name: string;
  leaveTaken: Leave[];
  accumulatedLeaveDays: number;
}
