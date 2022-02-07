using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WatchTogether.Chatting;
using WatchTogether.Settings;
using WatchTogether.UI.Windows;

namespace WatchTogether
{
    internal class ProfileMenu
    {
        public static void ProfileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow();
            profileWindow.Owner = MainWindow.Instance;

            if (profileWindow.ShowDialog() is true)
            {
                var iconData = profileWindow.UserIconData;
                var userName = profileWindow.UserName;

                var clientData = ChatManagerWT.Instance.Client.ClientData;
                var userID = clientData is null ? -1 : clientData.UserID;

                UpdateClientData(iconData, userName, userID);

                SaveUserDataToSettings(iconData, userName);
            }

            profileWindow.Close();
        }

        private static void UpdateClientData(string iconData, string userName, int userID)
        {
            ChatManagerWT.Instance.Client.ClientData = null;
            ChatManagerWT.Instance.Client.ClientData = new ClientData(userID, userName, iconData);
        }

        private static void SaveUserDataToSettings(string iconData, string userName)
        {
            var settings = SettingsModelManager.CurrentSettings;
            settings.UserIconData = iconData;
            settings.UserName = userName;
            SettingsModelManager.CurrentSettings = settings;
        }
    }
}
