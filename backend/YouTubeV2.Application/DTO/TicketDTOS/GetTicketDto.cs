using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.DTO.TicketDTOS
{
    public record GetTicketDto(Guid TicketId, Guid SubmitterId, Guid TargetId, TicketTargetType TargetType, string Reason, GetTicketStatusDto Status, string Response)
    {
        public GetTicketDto(Ticket ticket, Guid submitterId) : this(ticket.Id, submitterId, ticket.TargetId, ticket.TargetType, ticket.Reason, new GetTicketStatusDto(ticket.Status), ticket.Response) { }
    }
}

