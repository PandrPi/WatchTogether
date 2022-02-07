using Squirrel;
using System.Windows;
using WatchTogether.Browser;
using WatchTogether.Chatting;
using WatchTogether.Settings;

namespace WatchTogether.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        private ChatManagerWT chatManager;
        private BrowserManagerWT browserManager;

        // TODO: Link for the WebView2 Runtime - https://go.microsoft.com/fwlink/p/?LinkId=2124703

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();

            UpdateApp();

            new SettingsModelManager();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            chatManager = new ChatManagerWT(this);

            browserManager = new BrowserManagerWT(this);
            await browserManager.InitializeAsync();

            BrowserColumn.MinWidth = SystemParameters.WorkArea.Width * 0.3;
            ChatColumn.MaxWidth = SystemParameters.WorkArea.Width * 0.3;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            chatManager.Dispose();
            browserManager.Dispose();
        }

        private void ProfileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow();
            profileWindow.Owner = this;

            if (profileWindow.ShowDialog() is true)
            {
                var iconData = profileWindow.UserIconData;
                var userName = profileWindow.UserName;

                var clientData = chatManager.Client.ClientData;
                var userID = clientData is null ? -1 : clientData.UserID;

                UpdateClientData(iconData, userName, userID);

                SaveUserDataToSettings(iconData, userName);
            }

            profileWindow.Close();
        }

        private void UpdateClientData(string iconData, string userName, int userID)
        {
            chatManager.Client.ClientData = null;
            chatManager.Client.ClientData = new ClientData(userID, userName, iconData);
        }

        private static void SaveUserDataToSettings(string iconData, string userName)
        {
            var settings = SettingsModelManager.CurrentSettings;
            settings.UserIconData = iconData;
            settings.UserName = userName;
            SettingsModelManager.CurrentSettings = settings;
        }

        private async void UpdateApp()
        {
            const string repoUrl = "https://github.com/PandrPi/WatchTogether";

            if (System.Diagnostics.Debugger.IsAttached) return;

            using (var manager = await UpdateManager.GitHubUpdateManager(repoUrl))
            {
                await manager.UpdateApp();
            }
        }
    }
}
