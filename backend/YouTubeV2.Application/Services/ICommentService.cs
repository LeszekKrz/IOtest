using System.Linq.Expressions;
using YouTubeV2.Application.DTO.CommentsDTO;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Services
{
    public interface ICommentService
    {
        Task AddCommentAsync(string commentContent, User author, Video video, CancellationToken cancellationToken = default);

        Task<CommentsDTO> GetAllCommentsAsync(Guid videoId, CancellationToken cancellationToken = default);

        Task<Comment?> GetCommentByIdAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<Comment, object>>[] includes);

        Task AddCommentResponseAsync(string responseContent, User author, Comment comment, CancellationToken cancellationToken = default);

        Task RemoveCommentAsync(Comment comment, CancellationToken cancellationToken = default);

        Task<CommentsDTO> GetAllCommentResponsesAsync(Guid commentId, CancellationToken cancellationToken = default);

        Task<CommentResponse?> GetCommentResponseByIdAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<CommentResponse, object>>[] includes);

        Task RemoveCommentResponseAsync(CommentResponse commentResponse, CancellationToken cancellationToken = default);
    }
}
