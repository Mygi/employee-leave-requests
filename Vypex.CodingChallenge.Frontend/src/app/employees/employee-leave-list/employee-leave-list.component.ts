import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EmployeeApiService, Leave } from '../../api';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { DatePipe } from '@angular/common';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzButtonModule } from 'ng-zorro-antd/button';

@Component({
  selector: 'app-employee-leave-list',
  imports: [NzTableModule,
    NzDatePickerModule,
    FormsModule,
    DatePipe,
    NzModalModule,
    NzButtonModule
  ],
  templateUrl: './employee-leave-list.component.html',
  styleUrl: './employee-leave-list.component.scss',
  providers: [
    DatePipe
  ]
})
export class EmployeeLeaveListComponent {
  private readonly employeeApiService = inject(EmployeeApiService);
  public readonly selectedEmployee$ = this.employeeApiService.selectedEmployee$;
  public showLeaveForm = false;
  public date = null;
  public currentLeave: Leave = {
    id: "",
    startDate: new Date(Date.now()).toISOString(),
    endDate: new Date(Date.now() + 640000).toISOString(),
    leaveDaysAllocated: 0
  };
  
  public updateRange(event: Date[]) {
    if(event.length < 2) {
      return;
    }
    const startDate =event[0];
    const endDate = event[1];
    this.currentLeave.startDate = startDate.toISOString();
    this.currentLeave.endDate = endDate.toISOString()
  }

  public editLeave(leave: Leave) {
    console.log(leave);
    this.showLeaveForm = true;
  }

  public deleteLeave(leave: Leave) {
    console.log(leave);
  }
}
