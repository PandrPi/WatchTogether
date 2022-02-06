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

        private SimpleTcpServer server;
        private string serverPassword;
        private readonly Dictionary<int, ClientData> acceptedClients;
        private readonly Dictionary<int, TcpClient> acceptedTcpClients;

        /// <summary>
        /// Initializes the instance of the server
        /// </summary>
        public ChatServer()
        {
            // Initialize instance
            server = new SimpleTcpServer
            {
                StringEncoder = ChatManagerWT.MessageEncoder,
                Delimiter = ChatManagerWT.MessageDelimiter
            };

            // Initialize Events
            server.DelimiterDataReceived += Server_DelimiterDataReceived;

            // Initialize clients dictionary
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
        /// <returns>True if the specified password equals to the server's password, otherwise False</returns>
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
            // Add the ClientData object to the acceptedClients dictionary
            acceptedClients.Add(clientData.UserID, clientData);

            // Add the TCPClient object to the acceptedTcpClients dictionary
            acceptedTcpClients.Add(clientData.UserID, tcpClient);

            // Send a ClientCommand to all connected clients to have them add a new client
            UpdateClientConnectionStatus(newClientDataJson: clientData.ToString());

            // Send a message that a new user has joined the server
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

            // Remove client data from the acceptedClient dictionary if present
            if (acceptedClients.ContainsKey(clientID) == true)
            {
                acceptedClients.Remove(clientID);
                acceptedTcpClients.Remove(clientID);
            }
        }

        /// <summary>
        /// Sends a new ClientCommand to all connected clients about the serialized ClientData
        /// json object to add and the client ID to remove from the ConnectedClients collection.
        /// </summary>
        /// <param name="newClientDataJson">The serialized ClientData object to add</param>
        /// <param name="clientIDToRemove">The ID of the client to remove</param>
        private void UpdateClientConnectionStatus(
            string newClientDataJson = null,
            int clientIDToRemove = ServerID)
        {
            // Get a response (command) string that will be processed on a client's side
            var responceText = BrowserCommandSerializer.Serialize(
                typeof(UpdateConnectedClientsCommand), newClientDataJson, clientIDToRemove);

            // Send the responceText to all the connected clients
            BroadcastMessage(responceText, MessageTypeWT.ClientCommand);
        }

        /// <summary>
        /// Sends the specified messageText to all the connected users
        /// </summary>
        /// <param name="messageText">The message text to send</param>
        /// <param name="messageType">The type of the message</param>
        public void BroadcastMessage(string messageText, MessageTypeWT messageType = MessageTypeWT.UserMessage)
        {
            // Create a message instance
            var message = new MessageWT(messageText, DateTime.Now, messageType, ServerID, ServerUserName);
            // Send the created message instance to all the connected clients as string
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
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}, {ex.StackTrace}");
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
                    // Start the server at the specified port
                    server.Start(IPAddress.Parse(ip), port);
                    // Initialize a SecurePassword instance from the specified password string
                    serverPassword = password;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message}, {e.StackTrace}");
                    MessageBox.Show(e.StackTrace, e.Message);
                }
            });
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        public void StopServer()
        {
            try
            {
                // We should stop the server only if it was started before
                if (server.IsStarted == true)
                {
                    server.Stop();
                    serverPassword = null;
                    acceptedClients.Clear();
                    acceptedTcpClients.Clear();

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
        /// Handles all incoming user messages and broadcasts every message to all users
        /// </summary>
        private void Server_DelimiterDataReceived(object sender, Message e)
        {
            // Get the MessageWT object from the received string
            var message = MessageWT.FromString(e.MessageString);
            // Set ReceivingDateTime of the message as DateTime.Now because it is the time when the message
            // have been actually received
            message.SetReceivingDateTime(DateTime.Now);

            if (message.MessageType == MessageTypeWT.UserMessage || message.MessageType == MessageTypeWT.ClientCommand)
            {
                // Send the message to all the connected users. Note that massages of type UserMessage or ClientCommand
                // will be sent to every connected client
                BroadcastLine(message.ToString());
            }
            else if (message.MessageType == MessageTypeWT.ServerRequest)
            {
                // If a client sends a request to the server this message request will be processed here
                // Note that "request" term equals to "command" term inside this branch. A "request"
                // term can better represent what is going on inside this branch than a "command" term

                // Parse the message.Text to get an actual IBrowserCommand instance
                var request = BrowserCommandSerializer.Deserialize(message.Text);
                // Execute the request and write the result to the requestResult variable
                var requestResult = (string)request.Execute(e);
                // Create a MessageWT instance for the response
                var responseMessage = new MessageWT(requestResult, DateTime.Now,
                    MessageTypeWT.ClientCommand, ServerID, ServerUserName);

                // Send a response message to the client which have sent the request
                e.ReplyLine(responseMessage.ToString());
            }
        }
    }
}