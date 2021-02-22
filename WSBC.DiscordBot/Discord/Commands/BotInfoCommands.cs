using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;

namespace WSBC.DiscordBot.Discord.Commands
{
    [Name("Bot Info")]
    public class BotInfoCommands : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;
        private readonly WsbcOptions _coinOptions;
        private readonly DiscordOptions _discordOptions;

        public BotInfoCommands(CommandService commandService,
            IOptionsSnapshot<WsbcOptions> coinOptions, IOptionsSnapshot<DiscordOptions> discordOptions)
        {
            this._commandService = commandService;
            this._coinOptions = coinOptions.Value;
            this._discordOptions = discordOptions.Value;
        }

        [Command("help")]
        [Summary("Shows this help message")]
        public async Task HelpAsync()
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.ThumbnailUrl = this._coinOptions.CoinIconURL;
            embed.Title = this._coinOptions.CoinName;
            embed.Url = this._coinOptions.CoinURL;
            embed.Description = $"A bot for checking data about {this._coinOptions.CoinName} and its network!";
            embed.WithAuthor(base.Context.User);
            embed.WithFooter($"{this._coinOptions.CoinCode} Bot v{GetBotVersion()}", this._coinOptions.CoinIconURL);

            // build fields for all commands
            StringBuilder builder = new StringBuilder();
            IEnumerable<IGrouping<ModuleInfo, CommandInfo>> commands = this._commandService.Commands
                .OrderBy(c => c.Module.Name)
                .ThenBy(c => c.Priority).ThenBy(c => c.Name)
                .GroupBy(c => c.Module);
            foreach (IGrouping<ModuleInfo, CommandInfo> module in commands)
            {
                builder.Clear();
                // display module summary
                if (!string.IsNullOrWhiteSpace(module.Key.Summary))
                    builder.AppendLine(module.Key.Summary);

                // display each command in module
                foreach (CommandInfo cmd in module)
                {
                    // start command text
                    builder.AppendFormat("***{0}{1}", this._discordOptions.Prefix, cmd.Name);
                    // add params, if any
                    foreach (ParameterInfo param in cmd.Parameters)
                        builder.AppendFormat(" {1}{0}{2}", param.Name,
                            param.IsOptional ? '[' : '<',
                            param.IsOptional ? ']' : '>');
                    // close command text
                    builder.Append("***");

                    // add command summary
                    if (!string.IsNullOrWhiteSpace(cmd.Summary))
                        builder.AppendFormat(": {0}\n", cmd.Summary);
                }

                // add all text to field
                embed.AddField(module.Key.Name, builder.ToString(), inline: false);
            }

            // send response
            await base.ReplyAsync(null, false, embed.Build()).ConfigureAwait(false);
        }

        private static string GetBotVersion()
        {
            FileVersionInfo version = FileVersionInfo.GetVersionInfo(typeof(BotInfoCommands).Assembly.Location);
            return version.ProductVersion;
        }
    }
}
