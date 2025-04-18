import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { catchError, map, Observable, of, throwError, EMPTY, lastValueFrom } from 'rxjs';
import { Employee } from '../models/employee';
import { ServerState, ServerStates } from '../models/serrver-states';
import { Leave, LeaveRequest, LeaveResponse } from '../models';
import { toSignal } from '@angular/core/rxjs-interop';


// Should inject this
const defaultEmployee: Employee = {
  id: "",
  name: "",
  leaveTaken: [],
  accumulatedLeaveDays: 0
}
@Injectable({ providedIn: 'root' })
export class EmployeeApiService {
  private readonly httpClient = inject(HttpClient);

  private readonly baseUrl = 'https://localhost:7189';
  public employeess$: WritableSignal<Employee[]> = signal([]);
  public selectedEmployee$: WritableSignal<Employee> = signal(defaultEmployee);
  public serverState$: WritableSignal<ServerState> = signal({ state: ServerStates.INIT, message: "", hasData: false });

  private handleError<T>(error: HttpErrorResponse, serverState$: WritableSignal<ServerState>) {
    if (error.status === 409) {
      // A client-side or network error occurred. Handle it accordingly.
      serverState$.set({ state: ServerStates.CONFLICT, "message": error.error.message, hasData: false });
      // return of([]);
      return EMPTY;
    }
    if (error.status == 404) {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      this.serverState$.set({ state: ServerStates.NOT_FOUND, "message": error.error.message, hasData: false });
      return EMPTY;
    }
    // Return an observable with a user-facing error message.
    return throwError(() => new Error('Something bad happened; please try again later.'));
  }

  public getEmployees(): Observable<boolean> {

    this.serverState$.set({ state: ServerStates.LOADING, "message": "", hasData: false })
    return this.httpClient.get<Array<Employee>>(`${this.baseUrl}/api/employees`)
      .pipe(
        map(result => {
          this.employeess$.set(result);
          this.serverState$.set({ state: ServerStates.INIT, "message": "", hasData: true })
          return true;
        }),
        catchError(error => this.handleError(error, this.serverState$)))
  }



  public async deleteEmployeeLeave(leaveId: string, employeeId: string): Promise<boolean> {
    this.serverState$.set({ state: ServerStates.LOADING, message: "", hasData: false })
    return await lastValueFrom(this.httpClient.delete<Employee>(`${this.baseUrl}/api/Employees/leave/${leaveId}`)
      .pipe(
        map(result => {
          if (result !== undefined) {
            this.UpdateSingleEmployeeState(result);
            this.serverState$.set({ state: ServerStates.DELETED, "message": `Leave Deleted: ${result.accumulatedLeaveDays} total days taken off`, hasData: true })
          }
          return true;
        }),
        catchError(error => this.handleError(error, this.serverState$)))
    );

  }


  public UpdateSingleEmployeeState(employeeResult: Employee): void {
    this.selectedEmployee$.set(employeeResult);
    const changed_index = this.employeess$().findIndex(x => this.selectedEmployee$().id == x.id);
    if (changed_index != -1) {
      this.employeess$.update(employees =>
        employees.map(employee => employee.id == employeeResult.id ? employeeResult : employee)
      );
    }
  }


  public async insertEmployeeLeave(employeeId: string, leave: Leave): Promise<boolean> {
    this.serverState$.set({ state: ServerStates.LOADING, message: "", hasData: false })
    const request: LeaveRequest = {
      id: undefined,
      startDate: leave.startDate,
      endDate: leave.endDate,
      employeeId: employeeId
    }
    return await lastValueFrom(this.httpClient.put<Employee>(`${this.baseUrl}/api/Employees/UpsertLeave`, request)
      .pipe(
        map(result => {
          this.UpdateSingleEmployeeState(result);
          this.serverState$.set({ state: ServerStates.CREATED, message: `Leave Inserted: ${result.accumulatedLeaveDays} total days taken off`, hasData: true })
          return true;
        }),
        catchError(error => this.handleError(error, this.serverState$)))
    );
  }


  public async updateEmployeeLeave(employeeId: string, leave: Leave): Promise<boolean> {
    this.serverState$.set({ state: ServerStates.LOADING, message: "", hasData: false })
    const request: LeaveRequest = {
      id: leave.id,
      startDate: leave.startDate,
      endDate: leave.endDate,
      employeeId: employeeId
    }
    return await lastValueFrom(this.httpClient.put<Employee>(`${this.baseUrl}/api/Employees/UpsertLeave`, request)
      .pipe(
        map(result => {
          this.UpdateSingleEmployeeState(result);
          this.serverState$.set({ state: ServerStates.UPDATED, message: `Leave Updated: ${result.accumulatedLeaveDays} total days taken off`, hasData: true })
          return true;
        }),
        catchError(error => this.handleError(error, this.serverState$)))
    );
  }

  public checkLeaveRequest(request: LeaveRequest): Promise<LeaveResponse> {
    this.serverState$.set({ state: ServerStates.LOADING, message: "", hasData: false })
    return lastValueFrom(this.httpClient.post<LeaveResponse>(`${this.baseUrl}/api/Employees/CheckConstraints`, request));
  }

  public checkDeletedLeaveRequest(leave:Leave, employee: Employee): Promise<LeaveResponse> {
    return lastValueFrom(of({
      allowed: true,
      leaveDaysTaken: employee.accumulatedLeaveDays - leave.leaveDaysAllocated,
      reason: ""  
    }));
  }

  public resetServerState() {
    this.serverState$.set({ state: ServerStates.OK, message: "", hasData: false })
  }
}
