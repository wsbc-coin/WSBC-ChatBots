# WSBC Discord Bot
Main Discord Bot for WallStreetBets Coin.

### Building
1. Install [.NET 5 Runtime](https://dotnet.microsoft.com/download/dotnet/5.0).
2. Build the solution.
3. Publish the "WSBC.DiscordBot" project - [instructions](https://docs.microsoft.com/en-gb/dotnet/core/tutorials/publishing-with-visual-studio).

### Configuration
To run, bot requires that you create `appsecrets.json` file with bot secrets. See [appsecrets-example.json](WSBC.DiscordBot/appsecrets-example.json) for examples.  
If running from VS, you should ensure that file has Build Action of `Content` with Copying to Output Directory enabled. Otherwise simply add that file to your built bot directory.

`Discord:BotToken` is mandatory. You can create it on [Discord Developer Portal](https://discord.com/developers/applications/).

`Logging:DataDog:ApiKey` is optional - remove it to disable DataDog logs. To get DataDog API Key, visit [DataDog Integrations Settings](https://app.datadoghq.com/account/settings#api).  
Other logging configuration, such as file path, can be configured in [appsettings.json](WSBC.DiscordBot/appsettings.json) with `Logging` section. Refer to [Serilog.Settings.Configuration README](https://github.com/serilog/serilog-settings-configuration#serilogsettingsconfiguration--).

#### Other Configuration
Other configuration (like command prefix etc) can also be specified in [appsettings.json](WSBC.DiscordBot/appsettings.json).

Note that variables currently specified in settings file are just the most common ones. You can add other variables as long as they match variables in "Options"-suffixed classes, and sections match section names in `ConfigureServices` of [Program.cs](WSBC.DiscordBot/Program.cs). 

### Running
Run bot using `dotnet WsbcDiscordBot.dll` command. Alternatively if published to .exe, simply run the executable.

## License
Copyright (c) 2021 TehGM and WallStreetBetsCoin Developers

Licensed under [GNU Affero General Public License v3.0](LICENSE) (GNU AGPL-3.0).