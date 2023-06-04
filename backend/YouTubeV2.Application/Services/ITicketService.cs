using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeV2.Application.DTO.TicketDTOS;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Services
{
    public interface ITicketService
    {
        Task<SubmitTicketResponseDto> SubmitTicketAsync(SubmitTicketDto dto, User submitter, CancellationToken cancellationToken = default);
        Task<GetTicketDto> GetTicketAsync(Guid id, CancellationToken cancellationToken = default);
        Task<SubmitTicketResponseDto> RespondToTicketAsync(Guid id, RespondToTicketDto dto, CancellationToken cancellationToken = default);
        Task<GetTicketStatusDto> GetTicketStatusAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<GetTicketDto>> GetTicketListAsync(CancellationToken cancellationToken = default);
    }
}
