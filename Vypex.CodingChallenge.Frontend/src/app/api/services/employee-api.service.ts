import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { catchError, map, Observable, of, switchMap, throwError } from 'rxjs';
import { Employee } from '../models/employee';
import { ServerState, ServerStates } from '../models/serrver-states';
import { Leave, LeaveRequest, LeaveResponse } from '../models';


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
  public serverState$: WritableSignal<ServerState> = signal( {state: ServerStates.INIT, message: "", hasData: false}); 

  private handleError(error: HttpErrorResponse, serverState$:  WritableSignal<ServerState>) {
    if (error.status === 409) {
      // A client-side or network error occurred. Handle it accordingly.
      serverState$.set({state: ServerStates.CONFLICT, "message": error.error.message, hasData: false});
      // return of([]);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      this.serverState$.set({state: ServerStates.ERROR, "message": error.error.message, hasData: false});
      // return throwError(() => new Error('Something bad happened; please try again later.'));
    }
    // Return an observable with a user-facing error message.
    return throwError(() => new Error('Something bad happened; please try again later.'));
  }

  public getEmployees(): Observable<boolean> {
    
      this.serverState$.set({state: ServerStates.LOADING, "message": "", hasData: false})
      return this.httpClient.get<Array<Employee>>(`${this.baseUrl}/api/employees`)
                            .pipe(
                              map( result => {
                                this.employeess$.set(result);
                                this.serverState$.set({state: ServerStates.INIT, "message": "", hasData: true})
                                return true;
                              }),
                              catchError(error => this.handleError(error, this.serverState$)))
  }


  
  public deleteEmployeeLeave(leaveId: string, employeeId: string) {
      this.serverState$.set({state: ServerStates.LOADING, message: "", hasData: false})
      return this.httpClient.delete<number>(`${this.baseUrl}/api/Employees/leave/${leaveId}`)
                            .pipe(
                              switchMap( result => {
                                if(result >= 0) {
                                  this.serverState$.set({state: ServerStates.LOADING, "message": `Leave Deleted: ${result} total days taken off`, hasData: false})
                                }
                                return this.getSingleEmployeeAndUpdateState(employeeId);
                              }),
                                catchError(error => this.handleError(error, this.serverState$)))
                              .subscribe(
                                complete => this.serverState$.set({state: ServerStates.DELETED, "message": `Leave Deleted: ${complete.accumulatedLeaveDays} total days taken off`, hasData: true})
                              );
                            
    }


    public getSingleEmployeeAndUpdateState(employeeId: string): Observable<Employee> {
      return this.httpClient.get<Employee>(`${this.baseUrl}/api/Employees/${employeeId}`)
                    .pipe(map( result => {
                        this.selectedEmployee$.set(result);
                        const changed_index = this.employeess$().findIndex(x => this.selectedEmployee$().id == x.id);
                        if(changed_index != -1) {
                            console.log("I found", changed_index);
                            this.employeess$.update(employees => 
                              employees.map( employee => employee.id == employeeId ? result :employee) 
                            );                          
                        }                         
                        return result;
                      }),
                      catchError(error => this.handleError(error, this.serverState$)))
    }


    public insertEmployeeLeave(employeeId: string, leave: Leave) {
      console.log(this.serverState$());
      this.serverState$.set({state: ServerStates.LOADING, message: "", hasData: false})
      const request: LeaveRequest = {
        id: undefined,
        startDate: leave.startDate,
        endDate: leave.endDate,
        employeeId: employeeId
      }
      return this.httpClient.put<LeaveResponse>(`${this.baseUrl}/api/Employees/UpsertLeave`,request)
                            .pipe(
                            switchMap( result => {
                            console.log(this.serverState$());
      
                                if( result.leaveDaysTaken > 0) {
                                    this.serverState$.set({state: ServerStates.LOADING, "message": `Leave Inserted: ${result.leaveDaysTaken} leave days taken`, hasData: false})  
                                }
                                  return this.getSingleEmployeeAndUpdateState(employeeId);
                              }),
                              catchError(error => this.handleError(error, this.serverState$)))
                              .subscribe(
                                complete => this.serverState$.set({state: ServerStates.CREATED, message: `Leave Inserted: ${complete.accumulatedLeaveDays} total days taken off`, hasData: true})
                              );
    }


    public updateEmployeeLeave(employeeId: string, leave: Leave) {
      console.log("clicked");
      this.serverState$.set({state: ServerStates.LOADING, message: "", hasData: false})
      const request: LeaveRequest = {
        id: leave.id,
        startDate: leave.startDate,
        endDate: leave.endDate,
        employeeId: employeeId
      }
      return this.httpClient.put<LeaveResponse>(`${this.baseUrl}/api/Employees/UpsertLeave`,request)
                            .pipe(
                            switchMap( result => {
                                if( result.leaveDaysTaken > 0) {
                                    this.serverState$.set({state: ServerStates.LOADING, message: `Leave Updated: ${result.leaveDaysTaken} leave days taken`, hasData: false})  
                                }
                                  return this.getSingleEmployeeAndUpdateState(employeeId);
                              }),
                              catchError(error => this.handleError(error, this.serverState$)))
                              .subscribe(
                                complete => this.serverState$.set({state: ServerStates.UPDATED, message: `Employee Updated: ${complete.accumulatedLeaveDays} total days taken off`, hasData: true})
                              );
    }


    public resetServerState() {
      this.serverState$.set({state: ServerStates.OK, message: "", hasData: false})
    }
}
