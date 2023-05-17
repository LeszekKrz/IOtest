namespace YouTubeV2.Application.DTO.CommentsDTO
{
    public record class CommentsDTO(IReadOnlyCollection<CommentsDTO.CommentDTO> comments)
    {
        public record class CommentDTO(Guid id, string authorId, string content, Uri avatarImage, string nickname, bool hasResponses);
    }
}
