export interface Leave {
    id: string;
    startDate: string;
    endDate: string;
    leaveDaysAllocated: number;
}

export interface LeaveRequest {
    id?: string;
    startDate: string;
    endDate: string;
    employeeId: string;
}

export interface LeaveResponse {
    allowed: boolean;
    leaveDaysTaken: number;
    reason: string;

}