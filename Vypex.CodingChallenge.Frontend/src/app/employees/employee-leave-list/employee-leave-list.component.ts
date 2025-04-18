import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EmployeeApiService, Leave } from '../../api';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { DatePipe } from '@angular/common';
import { NzModalModule, NzModalRef } from 'ng-zorro-antd/modal';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { ServerStates } from '../../api/models/serrver-states';

@Component({
  selector: 'app-employee-leave-list',
  imports: [NzTableModule,
    NzDatePickerModule,
    FormsModule,
    DatePipe,
    NzModalModule,
    NzButtonModule,
    NzIconModule
  ],
  templateUrl: './employee-leave-list.component.html',
  styleUrl: './employee-leave-list.component.scss',
  providers: [
    DatePipe
  ]
})
export class EmployeeLeaveListComponent {
  private readonly employeeApiService = inject(EmployeeApiService);  
  private readonly modal = inject(NzModalRef);
  protected readonly datastateEnum = ServerStates;
  protected serverState$ = this.employeeApiService.serverState$;  
  protected readonly selectedEmployee$ = this.employeeApiService.selectedEmployee$;
  
  public noChanges = true;
  public alterState = this.datastateEnum.INIT;
  public showLeaveForm = false;
  public date:Date[] =[];
  public previewUpdates = false;
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
    if(this.currentLeave.startDate !== startDate.toISOString()){
      this.currentLeave.startDate = startDate.toISOString();
      this.noChanges = false;
    }
    if(this.currentLeave.endDate !== endDate.toISOString()){
      this.currentLeave.endDate = endDate.toISOString();
      this.noChanges = false;
    }
    
  }

  public editLeave(leave: Leave) {
    this.currentLeave = leave;
    this.date = [new Date(Date.parse(this.currentLeave.startDate)), new Date(Date.parse(this.currentLeave.endDate))];
    this.showLeaveForm = true;
    this.alterState = ServerStates.UPDATED;
  }

  public deleteLeave(leave: Leave) {
    this.currentLeave = leave;
    this.showLeaveForm = false;
    this.previewUpdates = true;
    this.alterState = ServerStates.DELETED;
  }

  public saveChanges() {
    this.employeeApiService.resetServerState();
    this.previewUpdates = true;
  }

  public handleCommit() {
    if(this.alterState === ServerStates.DELETED) {
      this.employeeApiService.deleteEmployeeLeave(this.currentLeave.id, this.selectedEmployee$().id);
    }
    if(this.alterState === ServerStates.CREATED) {
      this.employeeApiService.insertEmployeeLeave(this.selectedEmployee$().id, this.currentLeave);
    }
    if(this.alterState == ServerStates.UPDATED) {
      this.employeeApiService.updateEmployeeLeave(this.selectedEmployee$().id, this.currentLeave);
    }
    this.noChanges = true;
  }
  public canCommit() {
    return !(this.serverState$().state in [ServerStates.CREATED,ServerStates.DELETED,ServerStates.UPDATED])
  }
  public addLeave() {
    this.date = [];
    this.currentLeave = {
      id: "",
      startDate: new Date(Date.now()).toISOString(),
      endDate: new Date(Date.now() + 640000).toISOString(),
      leaveDaysAllocated: 0
    }
    this.showLeaveForm = true;
    this.noChanges = true;
    this.previewUpdates = false;
    this.alterState = ServerStates.CREATED;
  }
  public closeModal(){
    this.employeeApiService.resetServerState();
    this.modal.destroy();
  }
}
