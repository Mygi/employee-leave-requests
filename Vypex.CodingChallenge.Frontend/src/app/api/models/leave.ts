export interface Leave {
    id: string;
    startDate: string;
    endDate: string;
    leaveDaysAllocated: number;
}

export interface LeaveRequest {
    id: string;
    startDate: string;
    endDate: string;
    employeeId: string;
}

export interface LeaveResponse {
    id: string;
    startDate: string;
    endDate: string;
    employeeId: string;
    leaveDaysTaken: number;
    updatedEmployeAnnualLeave: number
}