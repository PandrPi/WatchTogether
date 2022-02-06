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
        private SettingsModelManager settingsManager;

        // TODO: Link for the WebView2 Runtime - https://go.microsoft.com/fwlink/p/?LinkId=2124703

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();

            // Initialize settingsManager instance
            settingsManager = new SettingsModelManager();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize chatManager instance
            chatManager = new ChatManagerWT(this);

            // Initialize browserManager instance
            browserManager = new BrowserManagerWT(this);
            await browserManager.InitializeAsync();

            // Set MinWidth property of the BrowserColumn
            BrowserColumn.MinWidth = SystemParameters.WorkArea.Width * 0.3;
            // Set MaxWidth property of the ChatColumn
            ChatColumn.MaxWidth = SystemParameters.WorkArea.Width * 0.3;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            chatManager.Dispose();
            browserManager.Dispose();
        }

        private void ProfileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Initialize a HostServerWindow instance
            var profileWindow = new ProfileWindow();
            profileWindow.Owner = this;

            if (profileWindow.ShowDialog() is true)
            {
                var iconData = profileWindow.UserIconData;
                var userName = profileWindow.UserName;

                var clientData = chatManager.Client.ClientData;
                var userID = clientData is null ? -1 : clientData.UserID;

                chatManager.Client.ClientData = null;
                chatManager.Client.ClientData = new ClientData(userID, userName, iconData);

                // Save the received iconData and userName values to the settings
                var settings = SettingsModelManager.CurrentSettings;
                settings.UserIconData = iconData;
                settings.UserName = userName;
                SettingsModelManager.CurrentSettings = settings;
            }

            // Close the window
            profileWindow.Close();
        }
    }
}
