using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace YouTubeV2.Api.InputFormaters
{
    public class TextPlainInputFormatter : TextInputFormatter
    {
        public TextPlainInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding effectiveEncoding)
        {
            HttpRequest request = context.HttpContext.Request;
            using var reader = new StreamReader(request.Body, effectiveEncoding);
            try
            {
                string content = await reader.ReadToEndAsync();
                return await InputFormatterResult.SuccessAsync(content);
            }
            catch
            {
                return await InputFormatterResult.FailureAsync();
            }
        }
    }
}
