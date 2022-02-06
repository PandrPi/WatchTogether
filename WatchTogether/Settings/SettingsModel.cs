using System.Collections.Generic;

namespace WatchTogether.Settings
{
    internal class SettingsModel
    {
        public string UserName { get; set; }
        public string UserIconData { get; set; }
        public string LastHostServerAddress { get; set; }
        public string LastJoinServerAddress { get; set; }
        public List<string> AdsToBlock { get; set; }
    }
}
