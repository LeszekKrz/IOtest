namespace YouTubeV2.Application.DTO.UserDTOS
{
    public record UserDto(
        string id,
        string email,
        string nickname,
        string name,
        string surname,
        double accountBalance,
        string userType,
        string avatarImage,
        int subscriptionsCount);
}
