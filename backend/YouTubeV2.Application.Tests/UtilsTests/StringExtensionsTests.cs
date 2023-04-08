using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YouTubeV2.Application.Utils;

namespace YouTubeV2.Application.Tests.UtilsTests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void IsValidBase64ImageOrEmpty_ValidPng_ReturnsTrue()
        {
            // ARRANGE
            string input = "data:image/png;base64,iVBORw0KGg==";

            // ACT
            bool result = input.IsValidBase64ImageOrEmpty();

            // ASSERT
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsValidBase64ImageOrEmpty_ValidJpeg_ReturnsTrue()
        {
            // ARRANGE
            string input = "data:image/jpeg;base64,/9j/4AA==";

            // ACT
            bool result = input.IsValidBase64ImageOrEmpty();

            // ASSERT
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsValidBase64ImageOrEmpty_InvalidFormat_ReturnsFalse()
        {
            // ARRANGE
            string input = "data:image/gif;base64,R0lGODlhAQABAAAAACw=";

            // ACT
            bool result = input.IsValidBase64ImageOrEmpty();

            // ASSERT
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsValidBase64ImageOrEmpty_MissingDataPrefix_ReturnsFalse()
        {
            // ARRANGE
            string input = "image/png;base64,iVBORw0KGg==";

            // ACT
            bool result = input.IsValidBase64ImageOrEmpty();

            // ASSERT
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsValidBase64ImageOrEmpty_IncorrectBase64_ReturnsFalse()
        {
            // ARRANGE
            string input = "data:image/png;base64,iVBORw0K###";

            // ACT
            bool result = input.IsValidBase64ImageOrEmpty();

            // ASSERT
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsValidBase64ImageOrEmpty_EmptyString_ReturnsTrue()
        {
            // ARRANGE
            string input = string.Empty;

            // ACT
            bool result = input.IsValidBase64ImageOrEmpty();

            // ASSERT
            result.Should().BeTrue();
        }

        [TestMethod]
        public void GetImageFormat_ShouldReturnImagePng_WhenGivenValidPngBase64String()
        {
            // ARRANGE
            string base64String = "data:image/png;base64,iVBORw0KGg==";

            // ACT
            string result = base64String.GetImageFormat();

            // ASSERT
            result.Should().Be("image/png");
        }

        [TestMethod]
        public void GetImageFormat_ShouldReturnImageJpeg_WhenGivenValidJpegBase64String()
        {
            // ARRANGE
            string base64String = "data:image/jpeg;base64,/9j/4AA==";

            // ACT
            string result = base64String.GetImageFormat();

            // ASSERT
            result.Should().Be("image/jpeg");
        }

        [TestMethod]
        public void GetImageData_ShouldReturnImageData_WhenGivenValidBase64String()
        {
            // ARRANGE
            string base64String = "data:image/png;base64,iVBORw0KGg==";
            string expectedOutput = "iVBORw0KGg==";

            // ACT
            string result = base64String.GetImageData();

            // ASSERT
            result.Should().Be(expectedOutput);
        }
    }
}
