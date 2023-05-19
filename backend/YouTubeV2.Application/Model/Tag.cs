using System.ComponentModel.DataAnnotations;
using YouTubeV2.Application.Constants;

namespace YouTubeV2.Application.Model
{
    public class Tag
    {
        public Guid Id { get; init; }

        [MinLength(1)]
        [MaxLength(VideoConstants.tagMaxLength)]
        public string Value { get; init; } = null!;
        public virtual Video Video { get; init; } = null!;

        public Tag(string value)
        {
            Id = Guid.NewGuid();
            Value = value;
        }

        public Tag(string value, Video video): this(value)
        {
            Video = video;
        }
    }
}
