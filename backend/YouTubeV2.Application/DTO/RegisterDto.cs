namespace YouTubeV2.Application.DTO
{
    public record RegisterDto(string email, string nickname, string name, string surname, string password, string userType, string avatarImage);
}
