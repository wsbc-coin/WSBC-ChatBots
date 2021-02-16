# WSBC Discord Bot
Main Discord Bot for WallStreetBets Coin.

### Building
1. Install [.NET 5 Runtime](https://dotnet.microsoft.com/download/dotnet/5.0).
2. Build the solution.

### Running
To run, bot requires that you create `appsecrets.json` file with bot secrets. See [appsecrets-example.json](appsecrets-example.json) for examples.  
If running from VS, you should ensure that file has Build Action of `Content` with Copying to Output Directory enabled. Otherwise simply add that file to your built bot directory.

Run bot using `dotnet WsbcDiscordBot.dll` command. Alternatively if published to .exe, simply run the executable.

## License
Copyright (c) 2021 TehGM and WallStreetBetsCoin Developers

Licensed under [GNU Affero General Public License v3.0](LICENSE) (GNU AGPL-3.0).