using System.ComponentModel.DataAnnotations;
using YouTubeV2.Application.Constants;

namespace YouTubeV2.Application.Model
{
    public class CommentResponse
    {
        public Guid Id { get; init; }

        [MinLength(1)]
        [MaxLength(CommentConstants.commentMaxLength)]
        public string Content { get; init; } = null!;

        [Required]
        public DateTimeOffset CreateDate { get; init; }

        public virtual User Author { get; init; } = null!;
        public virtual Comment RespondOn { get; init; } = null!;

        public CommentResponse() { }

        public CommentResponse(string content, DateTimeOffset createData, User author, Comment respondOn)
        {
            Content = content;
            CreateDate = createData;
            Author = author;
            RespondOn = respondOn;
        }
    }
}
