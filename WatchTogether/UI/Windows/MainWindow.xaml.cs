using NLog;
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

        private readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private ChatManagerWT chatManager;
        private BrowserManagerWT browserManager;

        // TODO: Link for the WebView2 Runtime - https://go.microsoft.com/fwlink/p/?LinkId=2124703

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();

            UpdateApp();

            new SettingsModelManager();

            Logger.Trace("Initialized successfully");
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Logger.Trace("Started loading");

            chatManager = new ChatManagerWT(this);

            browserManager = new BrowserManagerWT(this);
            await browserManager.InitializeAsync();

            BrowserColumn.MinWidth = SystemParameters.WorkArea.Width * 0.3;
            ChatColumn.MaxWidth = SystemParameters.WorkArea.Width * 0.5;

            Logger.Trace("Loaded successfully");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            chatManager.Dispose();
            browserManager.Dispose();
            Logger.Trace("Closed successfully");
        }

        private async void UpdateApp()
        {
            const string repoUrl = "https://github.com/PandrPi/WatchTogether";

            if (System.Diagnostics.Debugger.IsAttached) return;

            try
            {
                using (var manager = await UpdateManager.GitHubUpdateManager(repoUrl))
                {
                    await manager.UpdateApp();
                    Logger.Trace("The app has been updated successfully");
                }
            }
            catch (System.Exception exception)
            {
                Logger.Trace(exception, $"The app update failed due to {exception.Message}");
            }
        }

        private void ProfileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProfileMenu.ProfileMenuItem_Click(sender, e);
        }
    }
}
