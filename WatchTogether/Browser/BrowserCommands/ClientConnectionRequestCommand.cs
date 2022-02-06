using Newtonsoft.Json;
using System;
using System.Linq;
using WatchTogether.Chatting;

namespace WatchTogether.Browser.BrowserCommands
{
    struct ClientConnectionRequestCommand : IBrowserCommand
    {
        private string serverPassword;
        private string userName;
        private string userIconData;

        public void Initialize(params object[] parameters)
        {
            serverPassword = Convert.ToString(parameters[0]);
            userName = Convert.ToString(parameters[1]);
            userIconData = Convert.ToString(parameters[2]);
        }

        public object Execute(SimpleTCP.Message message)
        {
            var server = ChatManagerWT.Instance.Server;
            ResponceStatus responceStatus;
            int userId;
            string connectedClientsData;

            if (server.ValidatePassword(serverPassword) == true)
            {
                // The password is valid
                responceStatus = ResponceStatus.Ok;
                userId = GenerateUserId();

                // Accept client connection
                server.AcceptClient(new ClientData(userId, userName, userIconData), message.TcpClient);

                // Serialize the dictionary of clients which are connected to a server
                connectedClientsData = JsonConvert.SerializeObject(server.ConnectedClients);
            }
            else
            {
                // The password is invalid
                responceStatus = ResponceStatus.Bad;
                userId = -1;
                connectedClientsData = null;
            }

            // Get a response (command) string that will be processed on a client's side
            var responceText = BrowserCommandSerializer.Serialize(
                typeof(ClientConnectionResponceCommand),
                responceStatus,
                userId,
                connectedClientsData);

            return responceText;
        }

        private int GenerateUserId()
        {
            const int maxAttempts = 1000;

            int currentAttempt = 0;
            int userId = Guid.NewGuid().GetHashCode();
            var occupiedUserIDs = ChatManagerWT.Instance.Server.GetOccupiedUserIDs();

            while (occupiedUserIDs.Contains(userId) == true
                && currentAttempt < maxAttempts
                && userId == ChatServer.ServerID)
            {
                userId = Guid.NewGuid().GetHashCode();
                currentAttempt++;
            }

            // If we used all the available attempts we have to assign a -1 value to userID variable.
            // When a client will receive this -1 response it will send a request again
            if (currentAttempt >= maxAttempts) userId = -1;
            //if (currentAttempt >= maxAttempts) throw new Exception("Could not generate a free userID!");

            return userId;
        }
    }
}

