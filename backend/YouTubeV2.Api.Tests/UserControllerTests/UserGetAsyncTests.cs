using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Net;
using YouTubeV2.Api.Tests.Providers;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Api.Tests.UserControllerTests
{
    [TestClass]
    public class UserGetAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;

        [TestInitialize]
        public async Task Initialize()
        {
            _webApplicationFactory = Setup.GetWebApplicationFactory();
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config!.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection!);
        }

        [TestMethod]
        public async Task GetUser_BeingSimple_ChoosingSbElse_ShouldReturnUserFromDB_DataHidden()
        {
            // Arrange
            User userSimpleMe = new User("user1@user1.com", "user1", "user1", "user1");
            User userSimpleHim = new User("user2@user2.com", "user2", "user2", "user2");

            string myUserID = null!;
            string hisUserID = null!;

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(userSimpleMe);
                  await userManager.CreateAsync(userSimpleHim);

                  await userManager.AddToRoleAsync(userSimpleMe, Role.Simple);
                  await userManager.AddToRoleAsync(userSimpleHim, Role.Simple);

                  myUserID = await userManager.GetUserIdAsync(userSimpleMe);
                  hisUserID = await userManager.GetUserIdAsync(userSimpleHim);
              });

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(
                ClaimsProvider.WithRoleAccessAndUserId(Role.Simple, myUserID)).CreateClient();

            // Act
            var query = new Dictionary<string, string?>()
            {
                { "id", hisUserID }
            };

            var path = QueryHelpers.AddQueryString("/user", query);
            HttpResponseMessage response = await httpClient.GetAsync(path);

            var content = await response.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<UserDto>(content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseDto.Should().NotBeNull();
            responseDto!.id.Should().Be(hisUserID);
            responseDto!.name.Should().Be(userSimpleHim.Name);
            responseDto!.surname.Should().Be(userSimpleHim.Surname);
            responseDto!.email.Should().Be(userSimpleHim.Email);
            responseDto!.nickname.Should().Be(userSimpleHim.UserName);
            responseDto!.accountBalance.Should().BeNull();
            responseDto!.subscriptionsCount.Should().BeNull();
        }

        [TestMethod]
        public async Task GetUser_BeingAdmin_ChoosingSbElse_ShouldReturnUserFromDB_DataFull()
        {
            // Arrange
            User userAdminMe = new User("user3@user3.com", "user3", "user3", "user3");
            User userSimpleHim = new User("user4@user4.com", "user4", "user4", "user4");

            string myUserID = null!;
            string hisUserID = null!;

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(userAdminMe);
                  await userManager.CreateAsync(userSimpleHim);

                  await userManager.AddToRoleAsync(userAdminMe, Role.Administrator);
                  await userManager.AddToRoleAsync(userSimpleHim, Role.Simple);

                  myUserID = await userManager.GetUserIdAsync(userAdminMe);
                  hisUserID = await userManager.GetUserIdAsync(userSimpleHim);
              });

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(
                ClaimsProvider.WithRoleAccessAndUserId(Role.Administrator, myUserID)).CreateClient();

            // Act
            var query = new Dictionary<string, string?>()
            {
                { "id", hisUserID }
            };

            var path = QueryHelpers.AddQueryString("/user", query);
            HttpResponseMessage response = await httpClient.GetAsync(path);

            var content = await response.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<UserDto>(content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseDto.Should().NotBeNull();
            responseDto!.id.Should().Be(hisUserID);
            responseDto!.name.Should().Be(userSimpleHim.Name);
            responseDto!.surname.Should().Be(userSimpleHim.Surname);
            responseDto!.email.Should().Be(userSimpleHim.Email);
            responseDto!.nickname.Should().Be(userSimpleHim.UserName);
            responseDto!.accountBalance.Should().Be(decimal.Zero);
        }


        [TestMethod]
        public async Task GetUser_ChoosingSelf_NoID_ShouldReturnFromDB_DataFull()
        {
            // Arrange
            User user = new User("user5@user5.com", "user5", "user5", "user5");

            string userID = null!;

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(user);

                  await userManager.AddToRoleAsync(user, Role.Simple);

                  userID = await userManager.GetUserIdAsync(user);
              });

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(
                ClaimsProvider.WithRoleAccessAndUserId(Role.Simple, userID)).CreateClient();

            // Act
            var path = "/user";
            HttpResponseMessage response = await httpClient.GetAsync(path);

            var content = await response.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<UserDto>(content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseDto.Should().NotBeNull();
            responseDto!.id.Should().Be(userID);
            responseDto!.name.Should().Be(user.Name);
            responseDto!.surname.Should().Be(user.Surname);
            responseDto!.email.Should().Be(user.Email);
            responseDto!.nickname.Should().Be(user.UserName);
            responseDto!.accountBalance.Should().Be(decimal.Zero);
        }

        [TestMethod]
        public async Task GetUser_ChoosingNonExistant_ShouldReturnNotFound()
        {
            // Arrange
            User user = new User("user6@user6.com", "user6", "user6", "user6");

            string userID = null!;

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(user);

                  await userManager.AddToRoleAsync(user, Role.Simple);

                  userID = await userManager.GetUserIdAsync(user);
              });

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(
                ClaimsProvider.WithRoleAccessAndUserId(Role.Simple, userID)).CreateClient();

            // Act
            var query = new Dictionary<string, string?>()
            {
                { "id", Guid.NewGuid().ToString() }
            };

            var path = QueryHelpers.AddQueryString("/user", query);
            HttpResponseMessage response = await httpClient.GetAsync(path);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}