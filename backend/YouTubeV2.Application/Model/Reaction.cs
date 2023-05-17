using System.ComponentModel.DataAnnotations;
using YouTubeV2.Application.Enums;

namespace YouTubeV2.Application.Model
{
    public class Reaction
    {
        public Guid Id { get; init; }

        [Required]
        public ReactionType ReactionType { get; set; }

        public virtual User User { get; init; }

        public virtual Video Video { get; init; }

        public Reaction(ReactionType reactionType, User user, Video video)
        {
            ReactionType = reactionType;
            User = user;
            Video = video;
        }

        public Reaction() { }
    }
}
