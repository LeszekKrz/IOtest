using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using YouTubeV2.Application.DTO.CommentsDTO;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Providers;
using YouTubeV2.Application.Services.BlobServices;

namespace YouTubeV2.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly YTContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBlobImageService _blobImageService;

        public CommentService(YTContext context, IDateTimeProvider dateTimeProvider, IBlobImageService blobImageService)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _blobImageService = blobImageService;
        }

        public async Task AddCommentAsync(string commentContent, User author, Video video, CancellationToken cancellationToken = default)
        {
            Comment comment = new(commentContent, author, video, _dateTimeProvider.UtcNow);

            _context.Users.Attach(author);
            _context.Videos.Attach(video);
            await _context.Comments.AddAsync(comment, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<CommentsDTO> GetAllCommentsAsync(Guid videoId, CancellationToken cancellationToken = default)
        {
            var comments = await _context
                .Comments
                .Include(comment => comment.Author)
                .Where(comment => comment.Video.Id == videoId)
                .Select(comment => new CommentsDTO.CommentDTO(
                    comment.Id,
                    comment.Author.Id,
                    comment.Content,
                    _blobImageService.GetProfilePictureUrl(comment.Author.Id),
                    comment.Author.UserName!,
                    comment.Responses.Any()))
                .ToListAsync(cancellationToken);

            return new CommentsDTO(comments);
        }

        public async Task<Comment?> GetCommentByIdAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<Comment, object>>[] includes)
        {
            var query = _context.Comments.AsTracking();
            query = includes.Aggregate(query, (current, includeExpresion) => current.Include(includeExpresion));
            return await query.FirstOrDefaultAsync(comment => comment.Id == id, cancellationToken);
        }

        public async Task AddCommentResponseAsync(string responseContent, User author, Comment comment, CancellationToken cancellationToken = default)
        {
            CommentResponse commentResponse = new(responseContent, _dateTimeProvider.UtcNow, author, comment);

            _context.Users.Attach(author);
            _context.Comments.Attach(comment);
            await _context.CommentResponses.AddAsync(commentResponse, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveCommentAsync(Comment comment, CancellationToken cancellationToken = default)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<CommentsDTO> GetAllCommentResponsesAsync(Guid commentId, CancellationToken cancellationToken = default)
        {
            var commentResponses = await _context
                .CommentResponses
                .Include(response => response.Author)
                .Where(response => response.RespondOn.Id == commentId)
                .Select(response => new CommentsDTO.CommentDTO(
                    response.Id,
                    response.Author.Id,
                    response.Content,
                    _blobImageService.GetProfilePictureUrl(response.Author.Id),
                    response.Author.UserName!,
                    false))
                .ToArrayAsync(cancellationToken);

            return new CommentsDTO(commentResponses);
        }

        public async Task<CommentResponse?> GetCommentResponseByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default,
            params Expression<Func<CommentResponse, object>>[] includes)
        {
            var query = _context.CommentResponses.AsTracking();
            query = includes.Aggregate(query, (current, includeExpresion) => current.Include(includeExpresion));
            return await query.FirstOrDefaultAsync(commentResponse => commentResponse.Id == id, cancellationToken);
        }

        public async Task RemoveCommentResponseAsync(CommentResponse commentResponse, CancellationToken cancellationToken = default)
        {
            _context.CommentResponses.Remove(commentResponse);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
