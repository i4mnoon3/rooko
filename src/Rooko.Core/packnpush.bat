:: Example: packnpush Rooko.Core 0.10
nuget pack %1.csproj
nuget push %1.dll.%2.nupkg -Source https://www.nuget.org/api/v2/package