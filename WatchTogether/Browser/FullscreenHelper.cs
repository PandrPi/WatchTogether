using Microsoft.Web.WebView2.Wpf;
using System.Windows.Controls;
using WatchTogether.UI.Windows;

namespace WatchTogether.Browser
{
    internal class FullscreenHelper
    {
        public bool FullScreen
        {
            set
            {
                if (value == true)
                {
                    // Switch to fullscreen mode
                    defaultBrowserParent.Child = null;
                    fullscreenWindow.FullscreenGrid.Children.Add(webBrowser);

                    fullscreenWindow.Show();
                    fullscreenWindow.Activate();
                    webBrowser.Focus();
                }
                else
                {
                    // Switch back to usual mode
                    fullscreenWindow.FullscreenGrid.Children.Remove(webBrowser);
                    defaultBrowserParent.Child = webBrowser;

                    fullscreenWindow.Hide();
                    webBrowser.Focus();
                }
            }
        }

        private FullscreenWindow fullscreenWindow;
        private Border defaultBrowserParent;
        private WebView2 webBrowser;

        public FullscreenHelper(MainWindow window)
        {
            fullscreenWindow = new FullscreenWindow();
            fullscreenWindow.Hide();

            defaultBrowserParent = window.BrowserBorder;
            webBrowser = window.Browser;

            window.Closing += (sender, e) => fullscreenWindow.Close();
        }
    }
}
