using System.IO;

namespace WSBC.ChatBots.Discord.Memes
{
    public interface IRandomFilePicker
    {
        string Pick(string path, string searchPattern, SearchOption searchOption);
    }
}
