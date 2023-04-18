using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Services.TicketServices
{
    public interface ITicketService
    {
        Task<Guid> SubmitTicketAsync(SubmitTicketDTO submitTicket, User user, CancellationToken cancellationToken = default);

        Task<GetTicketDTO> GetTicketAsync(Guid ticketId, CancellationToken cancellationToken = default);

        Task RespondToTicketAsync(Guid ticketId, string response, CancellationToken cancellationToken = default);

        Task<GetTicketStatusDTO> GetTicketStatusAsync(Guid ticketId, CancellationToken cancellationToken = default);

        Task<IEnumerable<GetTicketDTO>> GetTicketListAsync(string userId, CancellationToken cancellationToken = default);
    }
}
