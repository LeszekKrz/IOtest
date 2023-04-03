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
using YouTubeV2.Application.Model;

namespace YouTubeV2.Api.Tests
{
    [TestClass]
    public class SubscriptionsTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory;

        [TestInitialize]
        public async Task Initialize()
        {
            _webApplicationFactory = Setup.GetWebApplicationFactory();
            var config = _webApplicationFactory.Services.GetService<IConfiguration>();
            var connection = config.GetConnectionString("Db");
            await Setup.ResetDatabaseAsync(connection);
        }

    }
}
