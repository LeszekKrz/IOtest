namespace YouTubeV2.Application.Services
{
    public interface IDonationService
    {
        Task SendDonationAsync(string senderId, string recipientId, decimal ammount);

        Task WithdrawMoneyAsync(string withdrawerId, decimal ammount);
    }
}
