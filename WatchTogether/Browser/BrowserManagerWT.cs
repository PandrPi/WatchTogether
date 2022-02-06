using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Windows;
using WatchTogether.Browser.BrowserCommands;
using WatchTogether.Chatting;
using WatchTogether.Chatting.Messages;
using WatchTogether.UI.Windows;

namespace WatchTogether.Browser
{
    class BrowserManagerWT : IDisposable
    {
        private readonly MainWindow window;
        private readonly WebView2 webBrowser;
        private readonly AdBlocker adBlocker;
        private CoreWebView2 coreWebView;
        private CoreWebView2Environment browserEnvironment;
        private FullscreenHelper fullscreenHelper;

        public static BrowserManagerWT Instance { get; private set; }

        public BrowserManagerWT(MainWindow window)
        {
            Instance = this;
            this.window = window;
            webBrowser = window.Browser;
            adBlocker = new AdBlocker();
        }

        /// <summary>
        /// Asynchronously initializes the browser
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            try
            {
                // Ensure that CoreWebView2 initialization was completed
                await webBrowser.EnsureCoreWebView2Async(null);

                // Get the browser environment
                browserEnvironment = webBrowser.CoreWebView2.Environment;
                coreWebView = webBrowser.CoreWebView2;
                coreWebView.AddWebResourceRequestedFilter(null, CoreWebView2WebResourceContext.All);

                // Initialize the FullscreenHelper object
                fullscreenHelper = new FullscreenHelper(window);

                // Initialize JavaScriptsHolder
                JavaScriptsHolder.Initialize();

                

                InitializeEvents();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}, {ex.StackTrace}");
                MessageBox.Show(ex.StackTrace, ex.Message);
            }
        }

        private void InitializeEvents()
        {
            coreWebView.WebMessageReceived += CoreWebView2_WebMessageReceived;
            coreWebView.WebResourceRequested += CoreWebView2_WebResourceRequested;
            coreWebView.NewWindowRequested += CoreWebView2_NewWindowRequested;
            coreWebView.ContainsFullScreenElementChanged += CoreWebView2_ContainsFullScreenElementChanged;
            coreWebView.NavigationCompleted += CoreWebView_NavigationCompleted;
        }

        public async void ExecuteJavaScript(string script)
        {
            await webBrowser.Dispatcher.BeginInvoke(new Action(async () =>
            {
                await coreWebView.ExecuteScriptAsync(script);
            }));
        }

        private async void CoreWebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess == true)
            {
                var script = JavaScriptsHolder.GetScriptCodeByKey("NavigationCompleted");
                await coreWebView.ExecuteScriptAsync(script);
            }
        }

        private void CoreWebView2_ContainsFullScreenElementChanged(object sender, object e)
        {
            fullscreenHelper.FullScreen = coreWebView.ContainsFullScreenElement;
        }

        /// <summary>
        /// This method does not allow the browser to open links in a new window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Blocks ads of the videoPlayer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            if (adBlocker.IsUrlAdvertisement(e.Request.Uri))
            {
                e.Response = browserEnvironment.CreateWebResourceResponse(null, 404, "Not Found", null);
            }
        }

        /// <summary>
        /// Handels all messages which were sent by JavaScript window.chrome.webview.postMessage method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            const string fullTypeCore = "WatchTogether.Browser.BrowserCommands.";

            var client = ChatManagerWT.Instance.Client;

            var receivedMessage = e.TryGetWebMessageAsString();
            var data = JsonConvert.DeserializeObject<BrowserCommandSerializer.CommandEntity>(receivedMessage);
            data.FullTypeName = fullTypeCore + data.FullTypeName;
            data.Parameters[2] = client.ClientData.UserID;

            client.SendMessage(JsonConvert.SerializeObject(data), MessageTypeWT.ClientCommand);
        }

        public void Dispose()
        {
            webBrowser?.Dispose();
        }
    }
}
