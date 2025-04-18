import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { catchError, map, Observable, throwError } from 'rxjs';
import { Employee } from '../models/employee';
import { ServerState, ServerStates } from '../models/serrver-states';


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

  // Developer Only
  // public selectedEmployees$: linkedSignal<Employee[], Employee>( {

  // });
  public EmployeeApiService() {
    
  }
  private handleError(error: HttpErrorResponse) {
    if (error.status === 0) {
      // A client-side or network error occurred. Handle it accordingly.
      this.serverState$.set({state: ServerStates.CONFLICT, "message": "", hasData: false});
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      this.serverState$.set({state: ServerStates.ERROR, "message": "", hasData: false});
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
                                return true;
                              }),
                          catchError(this.handleError));
  }
}
