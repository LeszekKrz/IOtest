using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YouTubeV2.Api.Enums;
using YouTubeV2.Application.DTO.SearchDTOS;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.DTO.VideoMetadataDTOS;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Exceptions;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.BlobServices;
using YouTubeV2.Application.Services.VideoServices;

namespace YouTubeV2.Application.Services
{
    public class SearchService : ISearchService
    {
        private readonly YTContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IBlobImageService _blobImageService;

        public SearchService(YTContext context, UserManager<User> userManager, 
            IUserService userService, ISubscriptionService subscriptionService,
            IBlobImageService blobImageService) 
        {
            _context = context;
            _userManager = userManager;
            _userService = userService;
            _subscriptionService = subscriptionService;
            _blobImageService = blobImageService;
        }

        public async Task<SearchResultsDto> SearchAsync(string query, SortingDirections sortingDirection,
            SortingTypes sortingType, DateTimeOffset? dateBegin, DateTimeOffset? dateEnd, CancellationToken cancellationToken)
        {
            var videos = await SearchForVideosAsync(query, sortingDirection, sortingType, dateBegin, dateEnd, cancellationToken);
            var users = await SearchForUsersAsync(query, sortingDirection, sortingType, dateBegin, dateEnd, cancellationToken);

            return new SearchResultsDto(videos, users);
        }

        private async Task<IReadOnlyList<UserDto>> SearchForUsersAsync(string query, SortingDirections sortingDirection,
            SortingTypes sortingType, DateTimeOffset? dateBegin, DateTimeOffset? dateEnd, CancellationToken cancellationToken)
        {
            var searchableUsers = await _userManager.GetUsersInRoleAsync(Role.Creator);
            var matchingUsers = searchableUsers.Where(user => user.UserName!
                .Contains(query, StringComparison.InvariantCultureIgnoreCase)).AsQueryable();

            ClipUsersBasedOnDate(ref matchingUsers, dateBegin, dateEnd);
            SortUsers(ref matchingUsers, sortingDirection, sortingType, cancellationToken);

            var sortedUsers = matchingUsers.ToList();

            var userDtos = new List<UserDto>();
            foreach (var user in sortedUsers)
            {
                var userDto = await _userService.GetDTOForUser(user, false, cancellationToken);
                userDtos.Add(userDto);
            }

            return userDtos;
        }

        private void ClipUsersBasedOnDate(ref IQueryable<User> users, DateTimeOffset? dateBegin, DateTimeOffset? dateEnd)
        {
            if (dateBegin > dateEnd)
                throw new BadRequestException("Begin date cannot be bigger than end date");

            if (dateBegin != null)
                users = users.Where(user => user.CreationDate >= dateBegin);
            if (dateBegin != null)
                users = users.Where(user => user.CreationDate <= dateEnd);
        }

        private void SortUsers(ref IQueryable<User> users, SortingDirections sortingDirection, SortingTypes sortingType, 
            CancellationToken cancellationToken = default)
        {
            switch (sortingType)
            {
                case SortingTypes.Alphabetical:
                    SortUsersAlphabetical(ref users, sortingDirection);
                    break;
                case SortingTypes.PublishDate:
                    SortUsersPublish(ref users, sortingDirection);
                    break;
                case SortingTypes.Popularity:
                    SortUsersPopularity(ref users, sortingDirection, cancellationToken);
                    break;
            }
        }

        private void SortUsersAlphabetical(ref IQueryable<User> users, SortingDirections sortingDirection)
        {
            if (sortingDirection == SortingDirections.Ascending)
                users = users.OrderBy(x => x.UserName);
            else
                users = users.OrderByDescending(x => x.UserName);
        }

        private void SortUsersPublish(ref IQueryable<User> users, SortingDirections sortingDirection)
        {
            if (sortingDirection == SortingDirections.Ascending)
                users = users.OrderBy(x => x.CreationDate);
            else
                users = users.OrderByDescending(x => x.CreationDate);
        }

        private void SortUsersPopularity(ref IQueryable<User> users, SortingDirections sortingDirection, 
            CancellationToken cancellationToken = default)
        {
            if (sortingDirection == SortingDirections.Ascending)
                users = users.OrderBy(x =>
                    _subscriptionService.GetSubscriptionCountAsync(x.Id, cancellationToken).GetAwaiter().GetResult());
            else
                users = users.OrderByDescending(x => 
                    _subscriptionService.GetSubscriptionCountAsync(x.Id, cancellationToken).GetAwaiter().GetResult());
        }

        private async Task<IReadOnlyList<VideoMetadataDto>> SearchForVideosAsync(string query, SortingDirections sortingDirection,
           SortingTypes sortingType, DateTimeOffset? dateBegin, DateTimeOffset? dateEnd, CancellationToken cancellationToken)
        {
            // for some reason Contains() with StringComparison doesn't work here
            var matchingVideos = _context.Videos
                .Include(video => video.Author)
                .Include(video => video.Tags)
                .Where(video => video.Visibility == Visibility.Public
                     && video.ProcessingProgress == ProcessingProgress.Ready
                     && video.Title.ToLower().Contains(query.ToLower()));

            ClipVideosBasedOnDate(ref matchingVideos, dateBegin, dateEnd);
            SortVideos(ref matchingVideos, sortingDirection, sortingType);


            return await matchingVideos.ToVideoMetadataDto(_blobImageService).ToListAsync(cancellationToken);
        }

        private void ClipVideosBasedOnDate(ref IQueryable<Video> videos, DateTimeOffset? dateBegin, DateTimeOffset? dateEnd)
        {
            if (dateBegin > dateEnd)
                throw new BadRequestException("Begin date cannot be bigger than end date");

            if (dateBegin != null)
                videos = videos.Where(video => video.UploadDate >= dateBegin);
            if (dateBegin != null)
                videos = videos.Where(video => video.UploadDate <= dateEnd);
        }

        private void SortVideos(ref IQueryable<Video> videos, SortingDirections sortingDirection, SortingTypes sortingType)
        {
            switch (sortingType)
            {
                case SortingTypes.Alphabetical:
                    SortVideosAlphabetical(ref videos, sortingDirection);
                    break;
                case SortingTypes.PublishDate:
                    SortVideosPublish(ref videos, sortingDirection);
                    break;
                case SortingTypes.Popularity:
                    SortVideosPopularity(ref videos, sortingDirection);
                    break;
            }
        }

        private void SortVideosAlphabetical(ref IQueryable<Video> videos, SortingDirections sortingDirection)
        {
            if (sortingDirection == SortingDirections.Ascending)
                videos = videos.OrderBy(x => x.Title);
            else
                videos = videos.OrderByDescending(x => x.Title);
        }

        private void SortVideosPublish(ref IQueryable<Video> videos, SortingDirections sortingDirection)
        {
            if (sortingDirection == SortingDirections.Ascending)
                videos = videos.OrderBy(x => x.UploadDate);
            else
                videos = videos.OrderByDescending(x => x.UploadDate);
        }

        private void SortVideosPopularity(ref IQueryable<Video> videos, SortingDirections sortingDirection)
        {
            if (sortingDirection == SortingDirections.Ascending)
                videos = videos.OrderBy(x => x.ViewCount);
            else
                videos = videos.OrderByDescending(x => x.ViewCount);
        }
    }
}
