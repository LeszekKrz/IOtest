using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace YouTubeV2.Application.Utils
{
    public static class StringExtensions
    {
        public static bool IsValidBase64Image(this string input)
        {
            var regex = new Regex(@"^(data:image\/(png|jpeg);base64,([a-zA-Z0-9+/]*={0,3}))$");
            return regex.IsMatch(input);
        }

        public static bool IsValidBase64ImageOrNullOrEmpty(this string? input) => input.IsNullOrEmpty() || IsValidBase64Image(input);

        public static string GetImageFormat(this string base64Input)
        {
            const char prefix = ':', suffix = ';';
            int startIndex = base64Input.IndexOf(prefix) + 1, endIndex = base64Input.IndexOf(suffix);
            return base64Input[startIndex .. endIndex];
        }

        public static string GetImageData(this string base64Input)
        {
            const char prefix = ',';
            int startIndex = base64Input.IndexOf(prefix) + 1;
            return base64Input[startIndex ..];
        }
    }
}
