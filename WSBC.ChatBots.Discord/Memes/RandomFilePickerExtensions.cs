using System.IO;

namespace WSBC.ChatBots.Discord.Memes
{
    public static class RandomFilePickerExtensions
    {
        public static string Pick(this IRandomFilePicker picker, string path, bool recursive = true)
            => picker.Pick(path, "*", true);

        public static string Pick(this IRandomFilePicker picker, string path, string searchPattern, bool recursive = true)
            => picker.Pick(path, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
    }
}
