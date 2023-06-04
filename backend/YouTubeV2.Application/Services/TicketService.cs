using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeV2.Application.DTO.TicketDTOS;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Exceptions;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Providers;

namespace YouTubeV2.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly YTContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUserService _userService;

        public TicketService(YTContext context, IDateTimeProvider dateTimeProvider, IUserService userService)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _userService = userService;
        }

        public async Task<SubmitTicketResponseDto> SubmitTicketAsync(SubmitTicketDto dto, User submitter, CancellationToken cancellationToken = default)
        {
            var targetType = await GetTargetType(dto.TargetId, cancellationToken);

            var ticket = new Ticket(_dateTimeProvider.UtcNow, dto.TargetId, targetType, dto.Reason, submitter);

            _context.Attach(submitter);
            await _context.Tickets.AddAsync(ticket, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new SubmitTicketResponseDto(ticket.Id);
        }

        public async Task<GetTicketDto> GetTicketAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var ticket = await _context.Tickets.Include(x => x.Submitter).SingleAsync(x => x.Id == id, cancellationToken);

            return new GetTicketDto(ticket, new Guid(ticket.Submitter.Id));
        }

        public async Task<SubmitTicketResponseDto> RespondToTicketAsync(Guid id, RespondToTicketDto dto, CancellationToken cancellationToken = default)
        {
            var ticket = await _context.Tickets.SingleAsync(x => x.Id == id, cancellationToken);
            ticket.Status = TicketStatus.Resolved;
            ticket.Response = dto.Response;
            await _context.SaveChangesAsync(cancellationToken);

            return new SubmitTicketResponseDto(id);
        }

        public async Task<GetTicketStatusDto> GetTicketStatusAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var ticket = await _context.Tickets.SingleAsync(x => x.Id == id, cancellationToken);

            return new GetTicketStatusDto(ticket.Status);
        }


        public async Task<IEnumerable<GetTicketDto>> GetTicketListAsync(CancellationToken cancellationToken = default)
        {
            var tickets = await _context.Tickets
                .Include(x => x.Submitter)
                .Where(x => x.Status != TicketStatus.Resolved)
                .OrderBy(x => x.CreateDate)
                .Select(x => new GetTicketDto(x, new Guid(x.Submitter.Id)))
                .ToListAsync(cancellationToken);

            return tickets;
        }

        private async Task<TicketTargetType> GetTargetType(Guid id, CancellationToken cancellationToken)
        {
            var playlist = await _context.Playlists.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (playlist != null) return TicketTargetType.Playlist;

            var video = await _context.Videos.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (video != null) return TicketTargetType.Video;

            var comment = await _context.Comments.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (comment != null) return TicketTargetType.Comment;

            var commentResponse = await _context.CommentResponses.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (commentResponse != null) return TicketTargetType.CommentResponse;

            var user = await _userService.GetByIdAsync(id.ToString());
            if (user != null) return TicketTargetType.User;

            throw new NotFoundException("Wrong target id");
        }
    }
}
