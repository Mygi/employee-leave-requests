// import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzTableModule, NzTableSortFn } from 'ng-zorro-antd/table';
import { EmployeeApiService } from '../api/services/employee-api.service';
import { AsyncPipe } from '@angular/common';
import { Employee } from '../api/models/employee';
import { NzModalService } from 'ng-zorro-antd/modal';
import { EmployeeLeaveListComponent } from './employee-leave-list/employee-leave-list.component';

@Component({
  selector: 'app-employees',
  imports: [
    NzTableModule,
    NzButtonModule,
    AsyncPipe
  ],
  templateUrl: './employees.component.html',
  styleUrl: './employees.component.scss',
  providers: [NzModalService]
})
export class EmployeesComponent {
  private readonly employeeApiService = inject(EmployeeApiService);
  private readonly modalService = inject(NzModalService)
  public readonly employeesData$ = this.employeeApiService.getEmployees();
  public readonly employees$ = this.employeeApiService.employeess$;
  public sortFn: NzTableSortFn<Employee> = (a: Employee, b: Employee) => a.name.localeCompare(b.name);

  public viewLeave(employee: Employee) {
    console.log("employee", employee);
    this.employeeApiService.selectedEmployee$.set(employee);
    this.modalService.create({
      nzTitle: 'Viewing Leave for ' + employee.name,
      nzContent: EmployeeLeaveListComponent
    });
  }
}
