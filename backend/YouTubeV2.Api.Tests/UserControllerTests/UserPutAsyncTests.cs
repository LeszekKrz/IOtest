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
using System.Text;
using YouTubeV2.Api.Tests.Providers;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Api.Tests.UserControllerTests
{
    [TestClass]
    public class UserPutAsyncTests
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
        public async Task PutUser_ChoosingSelf_ShouldEditDB()
        {
            User userSimpleMe = new User("put1@put1.com", "put1", "put1", "put1");

            string myUserID = null!;

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(userSimpleMe);

                  await userManager.AddToRoleAsync(userSimpleMe, Role.Simple);

                  myUserID = await userManager.GetUserIdAsync(userSimpleMe);
              });

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(
                ClaimsProvider.WithRoleAccessAndUserId(Role.Simple, myUserID)).CreateClient();

            UpdateUserDto updateUserDto = new UpdateUserDto("robert", "robert", "robert", "Simple", "");
            var stringContent = new StringContent(JsonConvert.SerializeObject(updateUserDto), Encoding.UTF8, "application/json");

            // Act
            var query = new Dictionary<string, string?>()
            {
                { "id", myUserID }
            };

            var path = QueryHelpers.AddQueryString("/user", query);
            HttpResponseMessage response = await httpClient.PutAsync(path, stringContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  var user = await userManager.FindByIdAsync(userSimpleMe.Id);

                  user!.UserName.Should().Be(updateUserDto.nickname);
                  user!.Name.Should().Be(updateUserDto.name);
                  user!.Surname.Should().Be(updateUserDto.surname);
              });
        }

        [TestMethod]
        public async Task PutUser_ChoosingSelf_NoID_ShouldEditDB()
        {
            User userSimpleMe = new User("put2@put2.com", "put2", "put2", "put2");

            string myUserID = null!;

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  await userManager.CreateAsync(userSimpleMe);

                  await userManager.AddToRoleAsync(userSimpleMe, Role.Simple);

                  myUserID = await userManager.GetUserIdAsync(userSimpleMe);
              });

            using HttpClient httpClient = _webApplicationFactory.WithAuthentication(
                ClaimsProvider.WithRoleAccessAndUserId(Role.Simple, myUserID)).CreateClient();

            UpdateUserDto updateUserDto = new UpdateUserDto("roberto", "roberto", "roberto", "Simple", "");
            var stringContent = new StringContent(JsonConvert.SerializeObject(updateUserDto), Encoding.UTF8, "application/json");

            // Act
            var path = "/user";
            HttpResponseMessage response = await httpClient.PutAsync(path, stringContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  var user = await userManager.FindByIdAsync(myUserID);

                  user!.UserName.Should().Be(updateUserDto.nickname);
                  user!.Name.Should().Be(updateUserDto.name);
                  user!.Surname.Should().Be(updateUserDto.surname);
              });
        }

        [TestMethod]
        public async Task PutUser_BeingSimple_ChoosingSbElse_ShouldReturnForbidden()
        {
            // Arrange
            User userSimpleMe = new User("put3@put3.com", "put3", "put3", "put3");
            User userSimpleHim = new User("put4@put4.com", "put4", "put4", "put4");

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

            UpdateUserDto updateUserDto = new UpdateUserDto("robert", "robert", "robert", "Simple", "");
            var stringContent = new StringContent(JsonConvert.SerializeObject(updateUserDto), Encoding.UTF8, "application/json");

            // Act
            var query = new Dictionary<string, string?>()
            {
                { "id", hisUserID }
            };

            var path = QueryHelpers.AddQueryString("/user", query);
            HttpResponseMessage response = await httpClient.PutAsync(path, stringContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
