import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { getHttpOptionsWithAuthenticationHeader } from "../functions/get-http-options-with-authorization-header";
import { GetTicketDto } from '../models/tickets/get-ticket-dto';
import { GetTicketStatusDto } from '../models/tickets/get-ticket-status-dto';
import { RespondToTicketDto } from '../models/tickets/respond-to-ticket-dto';
import { SubmitTicketResponseDto } from '../models/tickets/submit-ticket-response-dto';
import { SubmitTicketDto } from '../models/tickets/submit-ticket-dto';
import { CommentsService } from 'src/app/pages/video/components/comments/services/comments.service';
import { getApiUrl } from '../functions/get-api-url';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  constructor(private httpClient: HttpClient,
    private commentService: CommentsService) { }

  submitTicket(dto: SubmitTicketDto): Observable<SubmitTicketResponseDto> {
    const url = `${getApiUrl()}/ticket`;
    return this.httpClient.post<SubmitTicketResponseDto>(url, dto, getHttpOptionsWithAuthenticationHeader());
  }

  getTicket(id: string): Observable<GetTicketDto> {
    const url = `${getApiUrl()}/ticket?id=${id}`;
    return this.httpClient.get<GetTicketDto>(url, getHttpOptionsWithAuthenticationHeader());
  }

  respondToTicket(id: string, dto: RespondToTicketDto): Observable<SubmitTicketResponseDto> {
    const url = `${getApiUrl()}/ticket?id=${id}`;
    return this.httpClient.put<SubmitTicketResponseDto>(url, dto, getHttpOptionsWithAuthenticationHeader());
  }

  getTicketStatus(id: string): Observable<GetTicketStatusDto> {
    const url = `${getApiUrl()}/ticket/status?id=${id}`;
    return this.httpClient.get<GetTicketStatusDto>(url, getHttpOptionsWithAuthenticationHeader());
  }

  getTicketList(): Observable<GetTicketDto[]> {
    const url = `${getApiUrl()}/ticket/list`;
    return this.httpClient.get<GetTicketDto[]>(url, getHttpOptionsWithAuthenticationHeader());
  }
}
