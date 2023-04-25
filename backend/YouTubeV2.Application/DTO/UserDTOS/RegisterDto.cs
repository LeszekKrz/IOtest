namespace YouTubeV2.Application.DTO.UserDTOS
{
    public record RegisterDto(string email, string nickname, string name, string surname, string password, string userType, string? avatarImage);
}
