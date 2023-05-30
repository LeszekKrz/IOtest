import { Component, IterableDiffers, NgZone } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { Observable, Subscription, finalize, of, switchMap } from 'rxjs';
import { GetTicketDto } from 'src/app/core/models/tickets/get-ticket-dto';
import { RespondToTicketDto } from 'src/app/core/models/tickets/respond-to-ticket-dto';
import { TicketService } from 'src/app/core/services/ticket.service';
import { ChangeDetectorRef } from '@angular/core';
import { CommentsService } from '../video/components/comments/services/comments.service';
import { CommentDTO } from '../video/components/comments/models/comment-dto';


@Component({
  selector: 'app-ticket',
  templateUrl: './ticket.component.html',
  styleUrls: ['./ticket.component.scss']
})
export class TicketComponent {
  tickets!: GetTicketDto[];
  subscriptions: Subscription[] = [];
  isProgressSpinnerVisible = false;
  showDialog = false;
  response = '';
  ticketId = '';
  showCommentDialog = false;;
  comment: CommentDTO | undefined;

  constructor(
    private ticketService: TicketService,
    private messageService: MessageService,
    private router: Router,
    private commentService: CommentsService) {
      this.getTickets();
    }

    public onResponseButtonClicked(ticketId: string) {
      this.showDialog = true;
      this.ticketId = ticketId;
    }  

    goToSubmitter(submitterId: string) {
      this.router.navigate(['creator/' + submitterId]);
    }
  
    goToTarget(ticket: GetTicketDto) {
      switch (ticket.targetType) {
        case 'Video':
          this.router.navigate(['video/' + ticket.targetId]);
          break;
        case 'User':
          this.router.navigate(['creator/' + ticket.targetId]);
          break;
        case 'Playlist':
          this.router.navigate(['playlist/' + ticket.targetId]);
          break;
        case 'Comment':
          this.getComment(ticket.targetId);
          break;
        case 'CommentResponse':
          this.getCommentResponse(ticket.targetId);
          break;
      }
    }
  
    respondToTicket() {
      const responseToTicket: RespondToTicketDto = {
        response: this.response
      };
      this.subscriptions.push(
        this.ticketService.respondToTicket(this.ticketId, responseToTicket).subscribe());
      
      this.tickets = [...this.tickets.filter(ticket => ticket.ticketId !== this.ticketId)];
      this.ticketId = '';
      this.response = '';
      this.showDialog = false;
    }

  getTickets() {
    this.subscriptions.push(
      this.ticketService.getTicketList().subscribe(tickets => {
        this.tickets = tickets;
      }));
  }
  
  doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }

  getComment(id: string) {
    this.subscriptions.push(
      this.commentService.getCommentByIdAsync(id).subscribe(comment => {
        this.comment = comment;
        this.showCommentDialog = true;
      }));
  }

  getCommentResponse(id: string) {
    this.subscriptions.push(
      this.commentService.getCommentResponseByIdAsync(id).subscribe(comment => {
        this.comment = comment;
        this.showCommentDialog = true;
      }));
  }
}
