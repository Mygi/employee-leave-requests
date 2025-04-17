import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzTableModule } from 'ng-zorro-antd/table';
import { EmployeeApiService } from '../api/services/employee-api.service';

@Component({
  selector: 'app-employees',
  imports: [
    NzTableModule,
    NzButtonModule,
    AsyncPipe
  ],
  templateUrl: './employees.component.html',
  styleUrl: './employees.component.scss'
})
export class EmployeesComponent {
  private readonly employeeApiService = inject(EmployeeApiService);

  public readonly employees$ = this.employeeApiService.getEmployees();
}
