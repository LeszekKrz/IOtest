using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.BlobServices;
using YouTubeV2.Application.Services;

namespace YouTubeV2.Api.Tests.VideoControllerTests
{
    [TestClass]
    internal class GetVideoMetadataAsyncTests
    {
        private WebApplicationFactory<Program> _webApplicationFactory = null!;
        private readonly Mock<IBlobVideoService> _blobVideoService = new();
        private readonly Mock<IUserService> _userService = new();
        private readonly byte[] _wholeFileStreamContent = Encoding.UTF8.GetBytes("testStreamContent");

        [TestInitialize]
        public void Initialize()
        {
            _blobVideoService
                .Setup(x => x.GetVideoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream(_wholeFileStreamContent));
            _userService
                .Setup(x => x.ValidateToken(It.IsAny<string>()))
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(ClaimTypes.Role, Role.Simple) })));
            _webApplicationFactory = Setup.GetWebApplicationFactory(_blobVideoService.Object, _userService.Object);
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
