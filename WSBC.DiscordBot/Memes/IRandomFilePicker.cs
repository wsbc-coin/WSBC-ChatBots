using System.IO;

namespace WSBC.DiscordBot.Memes
{
    public interface IRandomFilePicker
    {
        string Pick(string path, string searchPattern, SearchOption searchOption);
    }
}
