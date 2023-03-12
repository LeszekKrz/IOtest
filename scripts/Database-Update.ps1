Push-Location -Path $PSScriptRoot\..\backend\YouTubeV2.Application

dotnet ef database update --startup-project ..\YouTubeV2
dotnet ef database update --connection "Server=localhost;Integrated Security = true;Database=YouTubeV2Test;TrustServerCertificate=True" --startup-project ..\YouTubeV2

Pop-Location