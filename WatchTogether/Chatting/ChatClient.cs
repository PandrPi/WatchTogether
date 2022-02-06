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
        private event EventHandler<MessageWT> OnMessageReceived;
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

            InitializeEvents();

            InitializeClientData(out string userName, out string userIconData);

            SaveUserDataToSettings(userName, userIconData);
        }

        private static void SaveUserDataToSettings(string userName, string userIconData)
        {
            var setting = SettingsModelManager.CurrentSettings;
            setting.UserName = userName;
            setting.UserIconData = userIconData;
            SettingsModelManager.CurrentSettings = setting;
        }

        private void InitializeClientData(out string userName, out string userIconData)
        {
            userName = ChatHelper.GetUserName();
            ChatHelper.GetUserIconBrush(out userIconData);
            ClientData = new ClientData(-1, userName, userIconData);
        }

        private void InitializeEvents()
        {
            client.DelimiterDataReceived += Client_DelimiterDataReceived;
            OnMessageReceived += ChatManagerWT.Instance.ChatManagerWT_MessageReceived;
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
                if (client.TcpClient is null) return;

                if (client.TcpClient.Connected == false) return;

                client.Disconnect();

                ResetUserID();

                ResetConnectedClients();

                IsConnected = false;

                ChatManagerWT.Instance.ClearMessageHistory();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}, {e.StackTrace}");
                MessageBox.Show(e.StackTrace, e.Message);
            }
        }

        private void ResetConnectedClients()
        {
            ConnectedClients.Clear();
            ConnectedClients = null;
        }

        private void ResetUserID()
        {
            var clientData = ClientData;
            clientData.UserID = -1;
            ClientData = clientData;
        }

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
            OnMessageReceived?.Invoke(this, message);
        }

        /// <summary>
        /// Sends message to the server
        /// </summary>
        /// <param name="messageText">The message that will be sent to the server</param>
        public void SendMessage(string messageText, MessageTypeWT messageType = MessageTypeWT.UserMessage)
        {
            if (client.TcpClient is null) return;
            if (client.TcpClient.Connected == false) return;

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

            if (message.MessageType == MessageTypeWT.UserMessage)
            {
                if (IsConnected == false) return;

                message.ConvertReceivingDateTimeToLocalTime(); 
                OnMessageReceived?.Invoke(this, message);
            }
            else if (message.MessageType == MessageTypeWT.ClientCommand)
            {
                var request = BrowserCommandSerializer.Deserialize(message.Text);
                request.Execute(e);
            }
        }
    }
}