Push-Location -Path $PSScriptRoot\..\backend\YouTubeV2.Application

dotnet ef database update --startup-project ..\YouTubeV2
dotnet ef database update --connection "Server=tcp:wetubedbserver.database.windows.net,1433;Initial Catalog=WeTubeDB;Persist Security Info=False;User ID=WeTubeAdmin;Password=Password1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" --startup-project ..\YouTubeV2

Pop-Location