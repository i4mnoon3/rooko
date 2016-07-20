nuget pack Rooko.Core.nuspec
nuget setApiKey 558b8342-f274-48cd-b41e-5db35171df31 -Source https://www.nuget.org/api/v2/package
nuget push %1 -Source https://www.nuget.org/api/v2/package