using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WatchTogether.Browser;
using WatchTogether.Chatting;

namespace WatchTogether
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ChatClient chatClient;
		private ChatServer chatServer;

		public MainWindow()
		{
			// Initialize CefSettings and CommandLine arguments for the browser
			var browserSettings = new Browser.BrowserSettings();
			browserSettings.Initialize();

			InitializeComponent();

			loginButton.IsEnabled = true; // кнопка входа
			logoutButton.IsEnabled = false; // кнопка выхода
			sendButton.IsEnabled = false; // кнопка отправки
			chatTextBox.IsReadOnly = true; // поле для сообщений

			// Initialize chatServer and chatClient objects
			chatServer = new ChatServer();
			chatClient = new ChatClient();
			chatClient.SetMessageReceivedUICallback((message) =>
			{
				Dispatcher.Invoke(() => chatTextBox.AppendText(message.Text + '\n'));
			});

			// Initialize browser handlers
			webBrowser.RequestHandler = new RequestHandlerWT();
			webBrowser.DisplayHandler = new DisplayHandlerWT(this);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//webBrowser.Load("https://beggins.allohastream.com/?token_movie=bfe943f4d76128cb55ba974d048b1d&token=54e4d6aaabcbf28610ee8c39e2eca4");
			webBrowser.Load("https://watchtogether.ucoz.net/news/?title=deadpool");
			//webBrowser.Load("https://rezka.ag/series/fantasy/74-dnevniki-vampira-2009.html#t:1-s:4-e:14");
		}

		private void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			var userName = userNameTextBox.Text;
			userNameTextBox.IsReadOnly = true;

			try
			{
				chatClient.SetUserName(userName);

				loginButton.IsEnabled = false;
				logoutButton.IsEnabled = true;
				sendButton.IsEnabled = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{ex.Message}, {ex.StackTrace}");
			}
		}

		private void SendButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string message = messageTextBox.Text;
				chatClient.SendMessage(message);

				messageTextBox.Clear();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{ex.Message}, {ex.StackTrace}");
			}
		}

		private void LogoutButton_Click(object sender, RoutedEventArgs e)
		{
			ExitChat();
		}

		private void ExitChat()
		{
			chatClient.Disconnect();
			chatServer.StopServer();

			loginButton.IsEnabled = true;
			logoutButton.IsEnabled = false;
			sendButton.IsEnabled = false;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			ExitChat();
			chatClient.Dispose();
			webBrowser.Dispose();
		}

		private void WebBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
		{
			const string exitFullscreenWithEscapeButtonKey = "ExitFullscreenWithEscapeButton";

			Dispatcher.Invoke(() =>
			{
				if (e.Frame.IsMain == true)
				{
					// The main frame has finished loading and we can execute some code

					// Execute the script that gives the user an opportunity to exit 
					// fullscreen mode by pressing Escape button
					var exitFullscreenWithEscapeButtonCode = JavaScriptsHolder.GetScriptCodeByKey(exitFullscreenWithEscapeButtonKey);
					e.Frame.ExecuteJavaScriptAsync(exitFullscreenWithEscapeButtonCode);
				}
			});
		}

		private void StartServerBtn_Click(object sender, RoutedEventArgs e)
		{
			string socketAddress = sockedAddressTb.Text;
			if (ChatHelper.TryParseSocketAddress(socketAddress, out IPAddress ip, out int port) == true)
			{
				chatServer.StartServer(ip, port);
				chatClient.ConnectAsync(ip.ToString(), port);
			}
		}

		private void ConnectToServerBtn_Click(object sender, RoutedEventArgs e)
		{
			string socketAddress = sockedAddressTb.Text;
			if (ChatHelper.TryParseSocketAddress(socketAddress, out IPAddress ip, out int port) == true)
			{
				chatClient.ConnectAsync(ip.ToString(), port);
			}
		}

		private void SettingsMenuButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
