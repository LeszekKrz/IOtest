using Microsoft.Extensions.Configuration;

namespace YouTubeV2.Application.Tests
{
    [TestClass]
    public class UserTests
    {
        private YTContext Context;
        private IConfiguration Configuration;

        [TestInitialize]
        public async Task Setup()
        {
            Configuration = Common.GetConfiguration();
            string connection = Configuration.GetConnectionString("Db");
            Context = Common.SetUpContext(connection);
            await Common.ResetDatabaseAsync(connection);
        }

        [TestMethod]
        public void RegisterSHouldAddToDB()
        {
            // Arrange

            // Act

            // Assert
            
        }
    }
}