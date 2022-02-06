using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using WatchTogether.Browser.BrowserCommands;
using WatchTogether.Chatting.Messages;
using WatchTogether.Settings;

namespace WatchTogether.Chatting
{
    sealed class ChatClient : IDisposable
    {
        private event EventHandler<MessageWT> MessageReceived;
        private SimpleTcpClient client;

        public Dictionary<int, ClientData> ConnectedClients { get; set; }
        public ClientData ClientData { get; set; }
        public bool IsConnected { get; set; }

        /// <summary>
        /// Initializes the client instance and its events
        /// </summary>
        public ChatClient()
        {
            // Initialize instance
            client = new SimpleTcpClient
            {
                StringEncoder = ChatManagerWT.MessageEncoder,
                Delimiter = ChatManagerWT.MessageDelimiter
            };

            // Initialize events
            client.DelimiterDataReceived += Client_DelimiterDataReceived;
            MessageReceived += ChatManagerWT.Instance.ChatManagerWT_MessageReceived;

            // Initialize a ClientData property
            var userName = ChatHelper.GetUserName();
            ChatHelper.GetUserIconBrush(out string userIconData);
            ClientData = new ClientData(-1, userName, userIconData);

            // Save the UserName and UserIconData to the settings
            var setting = SettingsModelManager.CurrentSettings;
            setting.UserName = userName;
            setting.UserIconData = userIconData;
            SettingsModelManager.CurrentSettings = setting;
        }

        /// <summary>
        /// Creates a new SimpleTcpClient object and connects it to the specified IP address and port
        /// </summary>
        /// <param name="ip">The ip address of the server</param>
        /// <param name="port">The port of the server</param>
        /// <param name="password">The server password for connection</param>
        public async Task ConnectAsync(string ip, int port, string password)
        {
            // Ensure that the client is not connected to any other server
            Disconnect();

            await Task.Run(() =>
            {
                try
                {
                    client.Connect(ip, port);
                    ConnectedClients = new Dictionary<int, ClientData>();

                    SendConnectionRequest(password);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message}, {e.StackTrace}");
                    MessageBox.Show(e.StackTrace, e.Message);
                    Disconnect();
                }
            });
        }

        /// <summary>
        /// Disconnects the client from a server
        /// </summary>
        public void Disconnect()
        {
            try
            {
                // We should disconnect the client only if it is not null and it is connected to a server
                if (client.TcpClient is null == false && client.TcpClient.Connected == true)
                {
                    // Disconnect client
                    client.Disconnect();
                    // Reset the UserID
                    var clientData = ClientData;
                    clientData.UserID = -1;
                    ClientData = clientData;
                    // Reset the ConnectedClients dictionary
                    ConnectedClients.Clear();
                    ConnectedClients = null;
                    // Mark client as disconnected
                    IsConnected = false;

                    // Clear the Chat history
                    ChatManagerWT.Instance.ClearMessageHistory();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}, {e.StackTrace}");
                MessageBox.Show(e.StackTrace, e.Message);
            }
        }

        /// <summary>
        /// Releases the resources of the client
        /// </summary>
        public void Dispose()
        {
            try
            {
                Disconnect();
                client.Dispose();
                client = null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}, {e.StackTrace}");
                MessageBox.Show(e.StackTrace, e.Message);
            }
        }

        /// <summary>
        /// Sends a connection request to a server
        /// </summary>
        /// <param name="serverPassword">The password of a server</param>
        private void SendConnectionRequest(string serverPassword)
        {
            var commandText = BrowserCommandSerializer.Serialize(
                typeof(ClientConnectionRequestCommand),
                serverPassword,
                ClientData.UserName,
                ClientData.UserIconData);
            SendMessage(commandText, MessageTypeWT.ServerRequest);
        }

        /// <summary>
        /// Shows a message about the successfull connection on the UI. This method exists
        /// because the client cannot receive any client messages until the client has received
        /// a response to the connection request. The successful connection message is sent by
        /// the server to all connected clients when the connection request is processed by the
        /// server, but before the server sends a response to this request.
        /// </summary>
        public void ShowSuccessfullConnectionMessage()
        {
            var messageText = string.Format("{0} has joined the server", ClientData.UserName);
            var message = new MessageWT(messageText, DateTime.Now,
                MessageTypeWT.UserMessage, ChatServer.ServerID, ChatServer.ServerUserName);
            MessageReceived(this, message);
        }

        /// <summary>
        /// Sends message to the server
        /// </summary>
        /// <param name="message">The message that will be sent to the server</param>
        public void SendMessage(string messageText, MessageTypeWT messageType = MessageTypeWT.UserMessage)
        {
            if (client.TcpClient is null == true) return;
            if (client.TcpClient.Connected == false) return;

            // Create a message object
            var message = new MessageWT(messageText, default, messageType,
                ClientData.UserID, ClientData.UserName);

            // Send the serialized message string to the server
            client.WriteLine(message.ToString());
        }

        /// <summary>
        /// DataReceived event handler, receives all the incoming messages
        /// </summary>
        private void Client_DelimiterDataReceived(object sender, Message e)
        {
            MessageWT message = MessageWT.FromString(e.MessageString);

            if (message.MessageType == MessageTypeWT.UserMessage && IsConnected)
            {
                // Add the received user message to UI
                message.ConvertReceivingDateTimeToLocalTime();
                MessageReceived(this, message);
            }
            else if (message.MessageType == MessageTypeWT.ClientCommand)
            {
                // Parse the message.Text to get an actual IBrowserCommand instance
                var request = BrowserCommandSerializer.Deserialize(message.Text);
                // Execute the command
                request.Execute(e);
            }
        }
    }
}