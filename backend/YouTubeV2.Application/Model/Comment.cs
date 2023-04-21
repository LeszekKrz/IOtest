using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using YouTubeV2.Application.Constants;

namespace YouTubeV2.Application.Model
{
    public class Comment
    {
        public Guid Id { get; init; }

        [MinLength(1)]
        [MaxLength(CommentConstants.commentMaxLength)]
        public string Content { get; init; } = null!;

        [Required]
        public DateTimeOffset CreateDate { get; init; }

        public virtual User Author { get; init; }
        public virtual Video Video { get; init; }
        public virtual IReadOnlyCollection<CommentResponse> Responses { get; init; }

        public Comment() { }

        public Comment(string content, User author, Video video, DateTimeOffset now)
        {
            Content = content;
            Author = author;
            Video = video;
            CreateDate = now;
            Responses = ImmutableList.Create<CommentResponse>();
        }
    }
}
