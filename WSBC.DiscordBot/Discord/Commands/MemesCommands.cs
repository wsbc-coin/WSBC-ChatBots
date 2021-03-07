using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WSBC.DiscordBot.Memes;

namespace WSBC.DiscordBot.Discord.Commands
{
    [Name("Memes")]
    public class MemesCommands : ModuleBase<SocketCommandContext>
    {
        private readonly MemesOptions _memesOptions;
        private readonly IRandomFilePicker _randomFile;
        private readonly ILogger _log;

        public MemesCommands(IRandomFilePicker randomFilePicker, IOptionsSnapshot<MemesOptions> memesOptions, ILogger<CoinCheckCommands> log)
        {
            this._randomFile = randomFilePicker;
            this._memesOptions = memesOptions.Value;
            this._log = log;
        }

        [Command("lambo")]
        [Summary("Shows the Lambo you'll buy with WSBC! \uD83D\uDE0E")]
        public Task CmdLamboAsync()
            => SendRandomFileAsync(_memesOptions.LamboPath,
                $"<:emoji_51:808859069779279883> Lambo for you, {MentionUtils.MentionUser(base.Context.User.Id)}! \uD83D\uDE0E");

        private async Task SendRandomFileAsync(string path, string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    throw new ArgumentNullException(nameof(path));

                string file = _randomFile.Pick(path);

                await base.Context.Channel.SendFileAsync(file, text).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.LogAsError(this._log, "Exception when picking a random meme from path '{Path}'", path))
            {
                await base.ReplyAsync("\u274C Failed sending meme, please contact the developers").ConfigureAwait(false);
                return;
            }
        }
    }
}
