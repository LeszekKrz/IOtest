using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Enums;

namespace YouTubeV2.Application.Model
{
    public class Ticket
    {
        public Guid Id { get; init; }
        public Guid TargetId { get; init; }
        public string Reason { get; init; }
        public TicketStatus Status { get; set; } = TicketStatus.Submitted;
        public string Response { get; set; } = "";

        public User User { get; init; }

        public Ticket(Guid targetId, string reason, User user)
        {
            TargetId = targetId;
            Reason = reason;
            User = user;
        }

        public Ticket(SubmitTicketDTO submitTicketDTO, User user)
        {
            TargetId = submitTicketDTO.targetId; 
            Reason = submitTicketDTO.reason;
            User = user;
        }
    }
}
