Push-Location -Path $PSScriptRoot\..\backend\YouTubeV2.Application

dotnet ef database update --startup-project ..\YouTubeV2
# dotnet ef database update --connection "Server=localhost;Integrated Security = true;Database=BankingMasterTest"

Pop-Location