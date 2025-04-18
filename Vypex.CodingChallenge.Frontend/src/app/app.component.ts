import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { EmployeeApiService } from './api';
import { ServerStates } from './api/models/serrver-states';
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NzLayoutModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  private readonly employeeApiService = inject(EmployeeApiService);
  public serverState$ = this.employeeApiService.serverState$;
}
