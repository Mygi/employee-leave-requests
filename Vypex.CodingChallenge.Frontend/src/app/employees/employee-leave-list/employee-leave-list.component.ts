import { Component, inject, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EmployeeApiService, Leave, LeaveResponse } from '../../api';
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
  public leaveCheck$: WritableSignal<LeaveResponse | undefined> = signal(undefined);
  public cannotCommit$:WritableSignal<boolean> = signal(false);

  public noChanges = true;
  public alterState = this.datastateEnum.INIT;
  public showLeaveForm = false;
  public date:Date[] =[];
  public previewUpdates = false;
  public showLoader = false;
  
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
    this.alterState = ServerStates.DELETED;
    this.employeeApiService.checkDeletedLeaveRequest(leave, this.selectedEmployee$())
                           .then( result => {
                            this.leaveCheck$.set(result);
                            this.employeeApiService.resetServerState();
                            this.noChanges = true;
                            this.cannotCommit$.set(false);
  });
  }

  public saveChanges() {
    this.employeeApiService.resetServerState();
    this.employeeApiService.checkLeaveRequest({
      startDate: this.currentLeave.startDate,
      endDate: this.currentLeave.endDate,
      employeeId: this.selectedEmployee$().id,
      id: this.currentLeave.id == "" ? undefined : this.currentLeave.id
    }).then( result => {
      this.employeeApiService.resetServerState();
      this.leaveCheck$.set(result);
      this.noChanges = true;
      this.cannotCommit$.set(!result.allowed);
  });;
  }
  cancelChanges() {
    this.leaveCheck$.set(undefined);
    this.noChanges = true;
  }

  public handleCommit() {
    // this could be a signal instead. Would be cooler to handle together and respond with signle signal but time is upon us
    this.showLoader = true;
    this.cannotCommit$.set(true);
    if(this.alterState === ServerStates.DELETED) {
      this.employeeApiService.deleteEmployeeLeave(this.currentLeave.id, this.selectedEmployee$().id).then(complete => this.showLoader = false);
    }
    if(this.alterState === ServerStates.CREATED) {
      this.employeeApiService.insertEmployeeLeave(this.selectedEmployee$().id, this.currentLeave).then(complete => this.showLoader = false);
    }
    if(this.alterState == ServerStates.UPDATED) {
      this.employeeApiService.updateEmployeeLeave(this.selectedEmployee$().id, this.currentLeave).then(complete => this.showLoader = false);
    }
    this.noChanges = true;
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
