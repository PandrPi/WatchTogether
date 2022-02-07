using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WatchTogether.Chatting.Messages;
using WatchTogether.UI.Windows;

namespace WatchTogether.Chatting
{

    /// <summary>
    /// Manages interaction between client/server and UI
    /// </summary>
    class ChatManagerWT : IDisposable
    {
        public static ChatManagerWT Instance { get; private set; }
        public static readonly System.Text.Encoding MessageEncoder = System.Text.Encoding.UTF8;
        public static readonly byte MessageDelimiter = (byte)'\r';
        public static Brush ServerIconBrush { get; private set; }

        private readonly MainWindow window;
        private ChatClient chatClient;
        private ChatServer chatServer;
        private ObservableCollection<MessageData> messages;

        public ChatManagerWT(MainWindow window)
        {
            Instance = this;
            this.window = window;

            // Initialize the ServerIconBrush
            var brushResource = window.FindResource("ServerIconBrush") as Freezable;
            ServerIconBrush = brushResource.GetAsFrozen() as Brush;

            // Initialize the chunk manager
            Initialize();
        }

        public ChatClient Client
        {
            get { return chatClient; }
        }

        public ChatServer Server
        {
            get { return chatServer; }
        }

        private void Initialize()
        {
            // Initialize chatServer and chatClient objects
            chatServer = new ChatServer();
            chatClient = new ChatClient();

            // Initialize chat ItemSource collection
            messages = new ObservableCollection<MessageData>();
            window.ChatLB.ItemsSource = messages;

            // Add event handlers
            window.HostServerMenuItem.Click += HostServerMenuItem_Click;
            window.JoinServerMenuItem.Click += JoinServerMenuItem_Click;
            window.DisconnectMenuItem.Click += DisconnectMenuItem_Click;
            window.SendMessageBtn.Click += SendMessageBtn_Click;
            window.MessageTextTb.KeyDown += MessageTextTb_KeyDown;
        }
        /// <summary>
        /// Handles the Click event of the DisconnectMenuItem object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisconnectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Disconnect client from the server
            chatClient.Disconnect();
            // Stop the server if it is started
            chatServer.StopServer();

            // Clear the messages collection 
            ClearMessageHistory();
        }

        /// <summary>
        /// Cleares the Chat from all the messages
        /// </summary>
        public void ClearMessageHistory()
        {
            window.Dispatcher.Invoke(() => messages.Clear());
        }

        /// <summary>
        /// Sends a message to the host server
        /// </summary>
        private void SendMessage()
        {
            // Trim the message text to get rid of space characters at the start and end of the message
            var messageText = window.MessageTextTb.Text.Trim();

            // We cannot send a message if it is null or empty
            // We cannot send a message if the client is not connected to server
            if (string.IsNullOrEmpty(messageText) == false && chatClient.IsConnected == true)
            {
                chatClient.SendMessage(messageText);
            }

            // Clear the message textbox
            window.MessageTextTb.Text = string.Empty;
        }

        /// <summary>
        /// Handles the KeyDown event of the MessageTextTb object. Sends message when Enter key is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageTextTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
                {
                    // User pressed Enter + Shift so we need to add a new line
                    var caretIndex = window.MessageTextTb.CaretIndex;
                    window.MessageTextTb.Text =
                        window.MessageTextTb.Text.Insert(caretIndex, Environment.NewLine);
                    window.MessageTextTb.CaretIndex = caretIndex + 1;
                }
                else
                {
                    // Just send a message
                    SendMessage();
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the SendMessageBtn object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendMessageBtn_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        /// <summary>
        /// Hamdles the MessageReceived event of the ChatManagerWT object
        /// </summary>
        /// <param name="sender">The ChatClient object which received the message</param>
        /// <param name="e">The received message</param>
        public void ChatManagerWT_MessageReceived(object sender, MessageWT e)
        {
            window.Dispatcher.Invoke(() =>
            {
                Brush userBrush = (e.UserID == ChatServer.ServerID) ? ServerIconBrush : chatClient.ConnectedClients[e.UserID].UserIconBrush;
                var messageData = new MessageData(e.Text,
                                                  e.UserName,
                                                  userBrush,
                                                  e.ReceivingDateTime.ToShortTimeString(),
                                                  e.UserID == chatClient.ClientData.UserID);
                messages.Add(messageData);
            });
        }

        /// <summary>
        /// Handles the Click event of the HostServerMenuItem object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HostServerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Initialize a HostServerWindow instance
            var hostServerWindow = new HostServerWindow();
            hostServerWindow.Owner = window;

            if (hostServerWindow.ShowDialog() is true)
            {
                // Get the data from the dialog window
                string ip = hostServerWindow.Ip;
                int port = hostServerWindow.Port;
                string password = hostServerWindow.Password;

                await chatServer.StartServerAsync(ip, port, password);
                await chatClient.ConnectAsync(ip, port, password);
            }

            // Close the window
            hostServerWindow.Close();
        }

        /// <summary>
        /// Handles the Click event of the JoinServerMenuItem object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void JoinServerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Initialize a JoinServerWindow instance
            var joinServerWindow = new JoinServerWindow();
            joinServerWindow.Owner = window;

            if (joinServerWindow.ShowDialog() is true)
            {
                // Get the data from the dialog window
                string ip = joinServerWindow.Ip;
                int port = joinServerWindow.Port;
                string password = joinServerWindow.Password;

                chatServer.StopServer();
                await chatClient.ConnectAsync(ip, port, password);
            }

            joinServerWindow.Close();
        }

        /// <summary>
        /// Removes all the in-code added event handlers
        /// </summary>
        private void RemoveEventHandlers()
        {
            window.HostServerMenuItem.Click -= HostServerMenuItem_Click;
            window.JoinServerMenuItem.Click -= JoinServerMenuItem_Click;
            window.DisconnectMenuItem.Click -= DisconnectMenuItem_Click;
            window.SendMessageBtn.Click -= SendMessageBtn_Click;
        }

        /// <summary>
        /// Releases all the resources of the ChatManagerWT instance
        /// </summary>
        public void Dispose()
        {
            // Remove the previously added event handlers
            RemoveEventHandlers();

            // Stop client and server
            chatServer.StopServer();
            chatClient.Disconnect();

            // Release client resourecs
            chatClient.Dispose();
        }
    }
}
