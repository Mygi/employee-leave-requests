import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Employee } from '../models/employee';

@Injectable({ providedIn: 'root' })
export class EmployeeApiService {
  private readonly httpClient = inject(HttpClient);

  private readonly baseUrl = 'https://localhost:7189';

  public getEmployees(): Observable<Array<Employee>> {
    return this.httpClient.get<Array<Employee>>(`${this.baseUrl}/api/employees`);
  }
}
