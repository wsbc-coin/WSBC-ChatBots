using System.IO;

namespace WSBC.ChatBots.Memes
{
    public interface IRandomFilePicker
    {
        string Pick(string path, string searchPattern, SearchOption searchOption);
    }
}
