using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Api.Tests
{
    [TestClass]
    public class UserTests
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
        public async Task RegisterShouldAddToDB()
        {
            // Arrange
            var httpClient = _webApplicationFactory.CreateClient();

            var registerDto = new RegisterDto("mail@mail.com", "Senior", "Generator", "Frajdy", "asdf1243@#$GJH", Role.Simple, "");

            var stringContent = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8, "application/json");

            // Act
            HttpResponseMessage response = await httpClient.PostAsync("register", stringContent);

            // Assert
            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
                async userManager =>
                {
                    User? userResult = await userManager.FindByEmailAsync(registerDto.email);

                    userResult.Should().NotBeNull();
                    userResult!.Name.Should().Be(registerDto.name);
                    userResult!.Surname.Should().Be(registerDto.surname);
                    userResult!.UserName.Should().Be(registerDto.nickname);
                    userResult!.NormalizedUserName.Should().Be(registerDto.nickname.ToUpper());
                    userResult!.Email.Should().Be(registerDto.email);
                    userResult!.NormalizedUserName.Should().Be(registerDto.nickname.ToUpper());
                    userManager.PasswordHasher.VerifyHashedPassword(userResult, userResult!.PasswordHash!, registerDto.password);

                    var roles = await userManager.GetRolesAsync(userResult);
                    roles.Should().Contain(Role.Simple);
                });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [TestMethod]
        public async Task LoginUserShouldReturnOk()
        {
            // ARRANGE
            var httpClient = _webApplicationFactory.CreateClient();

            var registerDto = new RegisterDto("mail@mail.com", "Senior", "Generator", "Frajdy", "asdf1243@#$GJH", Role.Simple, "");

            await _webApplicationFactory.DoWithinScope<UserManager<User>>(
              async userManager =>
              {
                  User user = new(registerDto);
                  await userManager.CreateAsync(user);
                  await userManager.AddPasswordAsync(user, "asdf1243@#$GJH");
              });
            var loginDto = new LoginDto("mail@mail.com", "asdf1243@#$GJH");

            // Act

            var response = await httpClient.PostAsync("login", new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json"));

            var content = await response.Content.ReadAsStringAsync();
            LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(content);
            // ASSERT
            loginResponseDto.token.Should().NotBeNullOrEmpty();
            response.Content.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
