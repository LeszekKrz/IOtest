Push-Location -Path $PSScriptRoot\..\backend\YouTubeV2.Application

dotnet ef database update --startup-project ..\YouTubeV2
dotnet ef database update --connection "Server=localhost;Database=YouTubeV2Test;User Id=sa;Password=dbatools.IO" --startup-project ..\YouTubeV2

Pop-Location