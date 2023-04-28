using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Net;
using YouTubeV2.Api.Tests.Providers;
using YouTubeV2.Application.DTO.SearchDTOS;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Api.Tests.SearchTests
{
    [TestClass]
    public class SearchAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;
        private readonly User _user = new()
        {
            Name = "name",
            Surname = "sur",
            UserName = "username",
            Email = "doda@doda.com"
        };

        [TestInitialize]
        public async Task Initialize()
        {
            _webApplicationFactory = Setup.GetWebApplicationFactory();
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);
        }

        [TestMethod]
        public async Task SearchShouldReturnUsersFromDB()
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

    }
}