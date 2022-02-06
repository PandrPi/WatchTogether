using System.Collections.Generic;
using System.Text.RegularExpressions;
using WatchTogether.Settings;

namespace WatchTogether.Browser
{
    internal class AdBlocker
    {
        private readonly List<string> BlockedWords;
        private readonly string BlockedWordsJoined;
        private readonly Regex BlockerRegex;

        private static readonly List<string> DefaultAdSources = new List<string>()
        {
            "franecki.net", "track.adpod.in", "franeski.net", "cdn4.life"
        };

        public AdBlocker()
        {
            BlockedWords = SettingsModelManager.CurrentSettings.AdsToBlock ?? DefaultAdSources;
            BlockedWordsJoined = string.Format("(?:{0})", string.Join("|", BlockedWords));
            BlockerRegex = new Regex(BlockedWordsJoined, RegexOptions.Compiled);
        }

        /// <summary>
        /// Checks whether the specified url string can be processed by a browser
        /// </summary>
        /// <param name="url">The url address which to check</param>
        /// <returns>True if the url should be blocked and False otherwise</returns>
        public bool IsUrlAdvertisement(string url)
        {
            return BlockerRegex.IsMatch(url);
        }
    }
}
