namespace YouTubeV2.Application.DTO
{
    public record GetTicketDTO(Guid submitterId, Guid targetId, string reason, GetTicketStatusDTO status, string response);
}
