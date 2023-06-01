import { Component, Input } from '@angular/core';
import { TicketService } from '../../services/ticket.service';
import { SubmitTicketDto } from '../../models/tickets/submit-ticket-dto';


@Component({
  selector: 'app-report-button',
  templateUrl: './report-button.component.html',
  styleUrls: ['./report-button.component.scss']
})
export class ReportButtonComponent {
  @Input() targetId: string | undefined;  // the guid of the element to report
  showDialog: boolean = false;
  reason: string = '';

  constructor(private ticketService: TicketService) {}

  report() {
    this.showDialog = false;  // close the dialog
    
    if(this.targetId && this.reason) {
      const dto: SubmitTicketDto = {
        targetId: this.targetId,
        reason: this.reason
      };
      this.ticketService.submitTicket(dto).subscribe(
        response => console.log(response),  // replace with actual response handling
        error => console.error(error)  // replace with actual error handling
      );
    }
    this.reason = '';  // reset the reason
  }
}
