## Introduction

This explains how to push the Rooko library to Nuget.

## Pack

Packing the project is using the nuget pack command.

	> nuget pack Rooko.Core.csproj
	> nuget setApiKey %NUGET_API_KEY% -Source https://www.nuget.org/api/v2/package

Please take note that NUGET_API_KEY environment variable needs to be set. Please ask @iescarro for it, or let him do th packing and setting of API KEY.
	
## Push

Before pushing we need to change the nupkg package. Some details need to be changed.

	<version>MAJOR.MINOR</version>
	
We're not really expecting to do a major version anytime soon so we need to change the minor version. As of the time of writing major is still on 0 version and minor is 12 so it should be

	<version>0.12</version>
	
Also we need to change the copyright to the current year.

	<copyright>2019</copyright>
	
After the changes, we push it using the nuget push command

	> nuget push Rooko.Core.dll.0.12.nupkg -Source https://www.nuget.org/api/v2/package