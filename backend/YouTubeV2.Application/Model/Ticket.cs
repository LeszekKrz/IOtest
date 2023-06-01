using YouTubeV2.Application.Enums;

namespace YouTubeV2.Application.Model
{
    public class Ticket
    {
        public Guid Id { get; init; }

        public DateTimeOffset CreateDate { get; init; }
        
        public Guid TargetId { get; init; }

        public TicketTargetType TargetType { get; init; }
        
        public string Reason { get; init; }
        
        public TicketStatus Status { get; set; }

        public string? Response { get; set; }

        public virtual User Submitter { get; init; }

        public Ticket() { }

        public Ticket(DateTimeOffset now, Guid targetId, TicketTargetType targetType, string reason, User submitter)
        {            
            CreateDate = now;
            TargetId = targetId;
            TargetType = targetType;
            Reason = reason;
            Status = TicketStatus.Submitted;
            Submitter = submitter;
        }
    }
}
