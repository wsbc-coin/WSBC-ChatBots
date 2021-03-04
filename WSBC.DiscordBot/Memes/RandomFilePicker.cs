using System;
using System.IO;

namespace WSBC.DiscordBot.Memes.Services
{
    class RandomFilePicker : IRandomFilePicker
    {
        public string Pick(string path, string searchPattern, SearchOption searchOption)
        {
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            Random random = new Random();
            int index = random.Next(0, files.Length);
            return files[index];
        }
    }
}
