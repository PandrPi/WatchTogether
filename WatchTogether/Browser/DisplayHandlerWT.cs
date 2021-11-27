using CefSharp;
using CefSharp.Structs;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace WatchTogether.Browser
{
	public class DisplayHandlerWT : IDisplayHandler
	{
		private Grid defaultBrowserParent;
		private FullscreenWindow fullscreenWindow;

		/// <summary>
		/// Initializes the DisplayHandlerWT object
		/// </summary>
		/// <param name="mainWindow">The instance of MainWindow class</param>
		public DisplayHandlerWT(MainWindow mainWindow)
		{
			fullscreenWindow = new FullscreenWindow();
			fullscreenWindow.Hide();

			defaultBrowserParent = mainWindow.MainGrid;

			mainWindow.Closing += (sender, e) => fullscreenWindow.Close();
		}

		public bool OnAutoResize(IWebBrowser browserControl, IBrowser browser, Size newSize) => false;

		public bool OnConsoleMessage(IWebBrowser browserControl, ConsoleMessageEventArgs consoleMessageArgs) => false;

		public bool OnTooltipChanged(IWebBrowser browserControl, ref string text) => false;

		public void OnFullscreenModeChange(IWebBrowser browserControl, IBrowser browser, bool fullscreen)
		{
			var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;

			chromiumWebBrowser.Dispatcher.Invoke(() => {
				if (fullscreen == true)
				{
					// Switch to fullscreen mode
					defaultBrowserParent.Children.Remove(chromiumWebBrowser);
					fullscreenWindow.FullscreenGrid.Children.Add(chromiumWebBrowser);

					fullscreenWindow.Show();
					fullscreenWindow.Activate();
					chromiumWebBrowser.Focus();
				}
				else
				{
					// Switch to non fullscreen mode
					fullscreenWindow.FullscreenGrid.Children.Remove(chromiumWebBrowser);
					defaultBrowserParent.Children.Add(chromiumWebBrowser);

					fullscreenWindow.Hide();
					chromiumWebBrowser.Focus();
				}
			});
		}

		public void OnAddressChanged(IWebBrowser browserControl, AddressChangedEventArgs addressChangedArgs)
		{
			
		}

		public void OnFaviconUrlChange(IWebBrowser browserControl, IBrowser browser, IList<string> urls)
		{
			
		}

		public void OnStatusMessage(IWebBrowser browserControl, StatusMessageEventArgs statusMessageArgs)
		{
			
		}

		public void OnTitleChanged(IWebBrowser browserControl, TitleChangedEventArgs titleChangedArgs)
		{
			
		}

		public void OnLoadingProgressChange(IWebBrowser chromiumWebBrowser, IBrowser browser, double progress)
		{
			
		}
	}
}
