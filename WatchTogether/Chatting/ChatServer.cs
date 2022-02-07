using NLog;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using WatchTogether.Browser.BrowserCommands;
using WatchTogether.Chatting.Messages;

namespace WatchTogether.Chatting
{
    sealed class ChatServer
    {
        public const int ServerID = 0;
        public const string ServerUserName = "Server";

        private ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<int, ClientData> acceptedClients;
        private readonly Dictionary<int, TcpClient> acceptedTcpClients;
        private SimpleTcpServer server;
        private string serverPassword;

        /// <summary>
        /// Initializes the instance of the server
        /// </summary>
        public ChatServer()
        {
            server = new SimpleTcpServer
            {
                StringEncoder = ChatManagerWT.MessageEncoder,
                Delimiter = ChatManagerWT.MessageDelimiter
            };

            server.DelimiterDataReceived += Server_DelimiterDataReceived;

            acceptedClients = new Dictionary<int, ClientData>();
            acceptedTcpClients = new Dictionary<int, TcpClient>();
        }

        /// <summary>
        /// Returns a KeyCollection object that contains all user IDs that are currently in use
        /// </summary>
        public Dictionary<int, ClientData>.KeyCollection GetOccupiedUserIDs() => acceptedClients.Keys;

        /// <summary>
        /// Returns a copy of the acceptedClients dictionary
        /// </summary>
        public Dictionary<int, ClientData> ConnectedClients
        {
            get { return new Dictionary<int, ClientData>(acceptedClients); }
        }

        /// <summary>
        /// Checks whether the specified password equals to the password of the server
        /// </summary>
        /// <param name="password">The password that need to be validated</param>
        /// <returns>True if the specified password is valid, otherwise False</returns>
        public bool ValidatePassword(string password)
        {
            return serverPassword.Equals(password);
        }

        /// <summary>
        /// Receives a new client connection and sends the newly connected client data
        /// to the other connected clients
        /// </summary>
        /// <param name="clientData">The data of a new client</param>
        public void AcceptClient(ClientData clientData, TcpClient tcpClient)
        {
            acceptedClients.Add(clientData.UserID, clientData);

            acceptedTcpClients.Add(clientData.UserID, tcpClient);

            UpdateClientConnectionStatus(newClientDataJson: clientData.ToString());

            var messageText = string.Format("{0} has joined the server", clientData.UserName);
            BroadcastMessage(messageText);
        }

        /// <summary>
        /// Disconnects the connected client whose ID is equal to the specified client ID
        /// </summary>
        /// <param name="clientID"></param>
        public void DisconnectClient(int clientID)
        {
            UpdateClientConnectionStatus(clientIDToRemove: clientID);

            if (acceptedClients.ContainsKey(clientID) == true)
            {
                acceptedClients.Remove(clientID);
                acceptedTcpClients.Remove(clientID);
            }
        }

        /// <summary>
        /// Sends a new ClientCommand to all connected clients about a new client data
        /// they have to add and the ID of the client to be removed
        /// </summary>
        /// <param name="newClientDataJson">The serialized ClientData object to add</param>
        /// <param name="clientIDToRemove">The ID of the client to remove</param>
        private void UpdateClientConnectionStatus(string newClientDataJson = null,
                                                  int clientIDToRemove = ServerID)
        {
            var responceText = BrowserCommandSerializer.Serialize(
                typeof(UpdateConnectedClientsCommand), newClientDataJson, clientIDToRemove);

            BroadcastMessage(responceText, MessageTypeWT.ClientCommand);
        }

        /// <summary>
        /// Sends the specified messageText to all the connected users
        /// </summary>
        /// <param name="messageText">The message text to send</param>
        /// <param name="messageType">The type of the message</param>
        public void BroadcastMessage(string messageText, MessageTypeWT messageType = MessageTypeWT.UserMessage)
        {
            var message = new MessageWT(messageText, DateTime.Now, messageType, ServerID, ServerUserName);
            BroadcastLine(message.ToString());
        }

        /// <summary>
        /// Sends the specified messageText to all the connected users
        /// </summary>
        /// <param name="message">The message to broadcast</param>
        private void BroadcastLine(string message)
        {
            try
            {
                server.BroadcastLine(message);
            }
            catch (Exception e)
            {
                Logger.Trace(e, $"{nameof(BroadcastLine)} failed due to {e.Message}");
            }
        }

        /// <summary>
        /// Starts the server at the specified port number
        /// </summary>
        /// <param name="ip">The ip address of the server</param>
        /// <param name="port">The port of the server</param>
        /// <param name="password">The server password for connection</param>
        public async Task StartServerAsync(string ip, int port, string password)
        {
            // Ensure that the server is stopped before starting a new server
            StopServer();

            await Task.Run(() =>
            {
                try
                {
                    
                    server.Start(IPAddress.Parse(ip), port);
                    
                    serverPassword = password;
                }
                catch (Exception e)
                {
                    Logger.Trace(e, $"{nameof(StartServerAsync)} failed due to {e.Message}");
                    MessageBox.Show(e.StackTrace, e.Message);
                }
            });
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        public void StopServer()
        {
            var methodName = nameof(StopServer);
            
            try
            {
                if (server.IsStarted == false)
                {
                    Logger.Trace($"{methodName} already stopped");
                    return;
                }
                
                    server.Stop();
                    ResedServerData();

                    ChatManagerWT.Instance.ClearMessageHistory();
                    Logger.Trace($"{methodName} stopped successfully");
                
            }
            catch (Exception e)
            {
                Logger.Trace(e, $"{methodName} failed due to {e.Message}");
                MessageBox.Show(e.StackTrace, e.Message);
            }
        }

        private void ResedServerData()
        {
            serverPassword = null;
            acceptedClients.Clear();
            acceptedTcpClients.Clear();
        }

        /// <summary>
        /// Handles all incoming user messages and broadcasts every message to all users
        /// </summary>
        private void Server_DelimiterDataReceived(object sender, Message e)
        {
            
            var message = MessageWT.FromString(e.MessageString);
            
            message.SetReceivingDateTime(DateTime.Now);

            if (message.MessageType == MessageTypeWT.UserMessage || message.MessageType == MessageTypeWT.ClientCommand)
            {
                BroadcastLine(message.ToString());
            }
            else if (message.MessageType == MessageTypeWT.ServerRequest)
            {
                ProcessServerRequest(e, message);
            }
        }

        private static void ProcessServerRequest(Message e, MessageWT message)
        {
            var request = BrowserCommandSerializer.Deserialize(message.Text);

            var requestResult = (string)request.Execute(e);

            var responseMessage = new MessageWT(requestResult, DateTime.Now,
                MessageTypeWT.ClientCommand, ServerID, ServerUserName);

            e.ReplyLine(responseMessage.ToString());
        }
    }
}