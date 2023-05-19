using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using YouTubeV2.Api.Enums;
using YouTubeV2.Application.Constants;
using YouTubeV2.Application.DTO.VideoMetadataDTOS;
using YouTubeV2.Application.Enums;

namespace YouTubeV2.Application.Model
{
    public class Video
    {
        public Guid Id { get; init; }

        [MinLength(1)]
        [MaxLength(VideoConstants.titleMaxLength)]
        public string Title { get; set; } = null!;

        [MinLength(1)]
        [MaxLength(VideoConstants.descriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Required]
        public Visibility Visibility { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int ViewCount { get; set; } = 0;

        [Required]
        public ProcessingProgress ProcessingProgress { get; set; } = ProcessingProgress.MetadataRecordCreater;

        [Required]
        public DateTimeOffset UploadDate { get; init; }

        [Required]
        public DateTimeOffset EditDate { get; init; }

        [Required]
        public string Duration { get; set; } = "00:00";


        public virtual User Author { get; init; }

        public virtual IReadOnlyCollection<Tag> Tags { get; set; }

        public virtual IReadOnlyCollection<Comment> Comments { get; init; }

        public virtual ICollection<Playlist> Playlists { get; init; } = null!;

        public virtual IReadOnlyCollection<Reaction> Reactions { get; init; } = null!;

        public Video() { }

        public static Video FromDTO(VideoMetadataAddOrUpdateDto videoMetadata, User author, DateTimeOffset now) =>
            new (videoMetadata.title, videoMetadata.description, videoMetadata.visibility, videoMetadata.tags, author, now);

        public Video(string title, string description, Visibility visibility, IReadOnlyCollection<string> tags, User author, DateTimeOffset now)
        {
            Title = title;
            Description = description;
            Visibility = visibility;
            Tags = tags.Select(tag => new Tag(tag)).ToImmutableList();
            Author = author;
            UploadDate = EditDate = now;
            Comments = ImmutableList.Create<Comment>();
        }
    }
}
