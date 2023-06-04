import { TicketTargetType } from "../enums/target-type";
import { GetTicketStatusDto } from "./get-ticket-status-dto";

export interface GetTicketDto {
    ticketId: string;
    submitterId: string;
    targetId: string;
    targetType: TicketTargetType;
    reason: string;
    status: GetTicketStatusDto;
    response: string;
  }