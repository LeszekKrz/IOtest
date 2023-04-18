using FluentValidation;
using Microsoft.EntityFrameworkCore;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Validator;

namespace YouTubeV2.Application.Services.TicketServices
{
    internal class TicketService
    {
        private YTContext _context;
        private SubmitTicketValidator _validator;

        public TicketService(YTContext context, SubmitTicketValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<Guid> SubmitTicketAsync(SubmitTicketDTO submitTicket, User user, CancellationToken cancellationToken = default)
        {
            await _validator.ValidateAndThrowAsync(submitTicket, cancellationToken);

            var ticket = new Ticket(submitTicket, user);
            _context.Users.Attach(user);
            await _context.Tickets.AddAsync(ticket, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return ticket.Id;
        }

        public async Task<GetTicketDTO> GetTicketAsync(Guid ticketId, CancellationToken cancellationToken = default)
        {
            var ticket = await _context.Tickets
                .Include(ticket => ticket.User)
                .SingleAsync(ticket => ticket.Id == ticketId, cancellationToken);

            return new GetTicketDTO(new Guid(ticket.User.Id), ticket.Id, ticket.Reason, new GetTicketStatusDTO(ticket.Status), ticket.Response);
        }

        public async Task RespondToTicketAsync(Guid ticketId, string response, CancellationToken cancellationToken = default)
        {
            var ticket = await _context.Tickets.SingleAsync(ticket => ticket.Id == ticketId, cancellationToken);
            ticket.Response = response;
            ticket.Status = TicketStatus.Resolved;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<GetTicketStatusDTO> GetTicketStatusAsync(Guid ticketId, CancellationToken cancellationToken = default)
        {
            var ticket = await _context.Tickets
                .SingleAsync(ticket => ticket.Id == ticketId, cancellationToken);

            return new GetTicketStatusDTO(ticket.Status);
        }

        public async Task<IEnumerable<GetTicketDTO>> GetTicketListAsync(string userId, CancellationToken cancellationToken = default)
        {
            var tickets = await _context.Tickets.Where(ticket => ticket.User.Id == userId).ToListAsync(cancellationToken);

            return tickets.Select(ticket => new GetTicketDTO(new Guid(userId), ticket.TargetId, ticket.Reason, new GetTicketStatusDTO(ticket.Status), ticket.Response));
        }
    }
}
