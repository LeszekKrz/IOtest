using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using YouTubeV2.Api.Enums;
using YouTubeV2.Api.Tests.Providers;
using YouTubeV2.Application;
using YouTubeV2.Application.DTO.SearchDTOS;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.BlobServices;

namespace YouTubeV2.Api.Tests.SearchTests
{
    [TestClass]
    public class SearchAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;
        private readonly Mock<IBlobImageService> _blobImageServiceMock = new();

        private readonly User _user = new()
        {
            Name = "name",
            Surname = "sur",
            UserName = "username",
            Email = "doda@doda.com"
        };

        private const string _pictureDomain = "http://www.profile-pictures/";

        [TestInitialize]
        public async Task Initialize()
        {
            _blobImageServiceMock.Setup(x => x.GetVideoThumbnailUrl(It.IsAny<string>()))
                .Returns<string>(fileName => new Uri(_pictureDomain));
            _blobImageServiceMock.Setup(x => x.GetProfilePictureUrl(It.IsAny<string>()))
                .Returns<string>(fileName => new Uri(_pictureDomain));
            _webApplicationFactory = Setup.GetWebApplicationFactory(_blobImageServiceMock.Object);
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);
        }

        [TestMethod]
        public async Task SearchAsync_Alphabetical_ShouldReturnUsers()
        {
            // ARRANGE
            string userId = null!;
            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_user);
                  userId = await userManager.GetUserIdAsync(_user);
              });

            var httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Simple, userId)).CreateClient();

            var registerDtos = new[]
            {
                new RegisterDto("mail@mail.com", "Alaa", "Alaa", "Tomasz", "asdf1243@#$GJH", Role.Creator, ""),
                new RegisterDto("list@list.com", "Alab", "Alab", "Tomek", "asdf1243@#$GJH", Role.Simple, ""),
                new RegisterDto("wiad@wiad.com", "Alac", "Alac", "Tomus", "asdf1243@#$GJH", Role.Creator, ""),
                new RegisterDto("polska@polska.com", "Maikołaj", "Maikołaj", "Tomaszewski", "asdf1243@#$GJH", Role.Creator, "")
            };

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
                async userManager =>
                {
                    foreach (var registerDto in registerDtos)
                    {
                        var user = new User(registerDto);
                        await userManager.CreateAsync(user, registerDto.password);
                        await userManager.AddToRoleAsync(user, registerDto.userType);
                    }
                });

            // ACT
            var querys = new Dictionary<string, string?>()
            {
                { "query", "ala" },
                { "sortingCriterion", SortingTypes.Alphabetical.ToString() },
                { "sortingType", SortingDirections.Descending.ToString() }
            };
            var path = QueryHelpers.AddQueryString("search", querys);
            HttpResponseMessage response = await httpClient.GetAsync(path);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().NotBeNull();

            string responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().NotBeNull().Should().NotBe(string.Empty);
            var searchDTO = JsonConvert.DeserializeObject<SearchResultsDto>(responseString);
            var foundUsers = searchDTO!.users.ToList();

            foundUsers.Count.Should().BeGreaterThanOrEqualTo(2);
            foundUsers.FindIndex(x => x.nickname.Equals("Alab")).Should().Be(-1);
            foundUsers.FindIndex(x => x.nickname.Equals("Maikołaj")).Should().Be(-1);
            int userSmallerID = foundUsers.FindIndex(x => x.nickname.Equals("Alaa"));
            int userLargerID = foundUsers.FindIndex(x => x.nickname.Equals("Alac"));

            userSmallerID.Should().NotBe(-1);
            userLargerID.Should().NotBe(-1);
            userLargerID.Should().BeLessThan(userSmallerID);
        }

        [TestMethod]
        public async Task SearchAsync_PublishDate_ShouldReturnVideos()
        {
            //ARRANGE
            User creator = new()
            {
                Name = "Jan",
                Surname = "Janosik",
                UserName = "skkf",
                Email = "skkf@skkf.com"
            };

            string userId = null!;
            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(_user);
                  await userManager.CreateAsync(creator);
                  userId = await userManager.GetUserIdAsync(_user);
              });

            var httpClient = _webApplicationFactory.WithAuthentication(ClaimsProvider.WithRoleAccessAndUserId(Role.Simple, userId)).CreateClient();

            var videos = new Video[]
            {
                new ()
                {
                    Title = "TitleA",
                    Description = "generic description A",
                    Visibility = Visibility.Public,
                    UploadDate = DateTimeOffset.UtcNow.AddDays(1),
                    EditDate = DateTimeOffset.UtcNow.AddDays(1),
                    Author = creator,
                    ProcessingProgress = ProcessingProgress.Ready
                },
                new ()
                {
                    Title = "TitleB",
                    Description = "generic description B",
                    Visibility = Visibility.Private,
                    UploadDate = DateTimeOffset.UtcNow,
                    EditDate = DateTimeOffset.UtcNow,
                    Author = creator,
                    ProcessingProgress = ProcessingProgress.Ready
                },
                new ()
                {
                    Title = "TitleC",
                    Description = "generic description C",
                    Visibility = Visibility.Public,
                    UploadDate = DateTimeOffset.UtcNow,
                    EditDate = DateTimeOffset.UtcNow,
                    Author = creator,
                    ProcessingProgress = ProcessingProgress.Uploading
                },
                new ()
                {
                    Title = "TitleD",
                    Description = "generic description D",
                    Visibility = Visibility.Public,
                    UploadDate = DateTimeOffset.UtcNow.AddDays(3),
                    EditDate = DateTimeOffset.UtcNow.AddDays(3),
                    Author = creator,
                    ProcessingProgress = ProcessingProgress.Ready
                },
                new ()
                {
                    Title = "TitleE",
                    Description = "generic description E",
                    Visibility = Visibility.Public,
                    UploadDate = DateTimeOffset.UtcNow.AddDays(4),
                    EditDate = DateTimeOffset.UtcNow.AddDays(4),
                    Author = creator,
                    ProcessingProgress = ProcessingProgress.Ready
                },
            };

            await _webApplicationFactory.DoWithinScope<YTContext>(
                async context =>
                {
                    context.Users.Attach(creator);
                    await context.Videos.AddRangeAsync(videos);
                    await context.SaveChangesAsync();
                });

            // ACT
            var querys = new Dictionary<string, string?>()
            {
                { "query", "Title" },
                { "sortingCriterion", SortingTypes.PublishDate.ToString() },
                { "sortingType", SortingDirections.Ascending.ToString() },
                { "beginDate", DateTimeOffset.UtcNow.AddDays(2).ToString(CultureInfo.InvariantCulture) },
                { "endDate", DateTimeOffset.UtcNow.AddDays(7).ToString(CultureInfo.InvariantCulture) }
            };
            var path = QueryHelpers.AddQueryString("search", querys);
            HttpResponseMessage response = await httpClient.GetAsync(path);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().NotBeNull();

            string responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().NotBeNull().Should().NotBe(string.Empty);
            var searchDTO = JsonConvert.DeserializeObject<SearchResultsDto>(responseString);
            var foundVideos = searchDTO!.videos.ToList();

            foundVideos.Count.Should().BeGreaterThanOrEqualTo(2);
            foundVideos.FindIndex(x => x.title.Equals("TitleA")).Should().Be(-1);
            foundVideos.FindIndex(x => x.title.Equals("TitleB")).Should().Be(-1);
            foundVideos.FindIndex(x => x.title.Equals("TitleC")).Should().Be(-1);

            int videoIDSmaller = foundVideos.FindIndex(x => x.title.Equals("TitleD"));
            int videoIDBigger = foundVideos.FindIndex(x => x.title.Equals("TitleE"));

            videoIDBigger.Should().BeGreaterThan(videoIDSmaller);
        }
    }
}