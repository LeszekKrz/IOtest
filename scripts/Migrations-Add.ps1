param (
    [Parameter(Mandatory=$true)][string]$name
)

Push-Location -Path $PSScriptRoot\..\backend\YouTubeV2.Application

dotnet ef migrations add $name --startup-project ..\YouTubeV2

Pop-Location