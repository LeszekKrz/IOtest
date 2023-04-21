using YouTubeV2.Application.DTO;

namespace YouTubeV2.Application.Exceptions
{
    public class BadRequestException : Exception
    {
        public IEnumerable<ErrorResponseDTO> Errors { get; } = null!;

        public BadRequestException() { }

        public BadRequestException(string messaege) : base(messaege) { }

        public BadRequestException(string messaege, Exception innerException) : base(messaege, innerException) { }

        public BadRequestException(string messaege, IEnumerable<ErrorResponseDTO> errors) : base(messaege)
        {
            Errors = errors;
        }

        public BadRequestException(string messaege, Exception innerException, IEnumerable<ErrorResponseDTO> errors) : base(messaege, innerException)
        {
            Errors = errors;
        }
        public BadRequestException(IEnumerable<ErrorResponseDTO> errors)
        {
            Errors = errors;
        }
    }
}
