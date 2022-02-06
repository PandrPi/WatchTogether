using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;
using WatchTogether.Chatting;

namespace WatchTogether.Browser.BrowserCommands
{
    struct ClientConnectionResponceCommand : IBrowserCommand
    {
        private ResponceStatus responceStatus;
        private int UserID;
        private Dictionary<int, ClientData> connectedClients;

        private const string MessageBoxText = "Check whether the password is correct and try again.";
        private const string MessageBoxTitle = "A host has declined the connection.";

        public void Initialize(params object[] parameters)
        {
            // Get the status of the responce
            responceStatus = (ResponceStatus)Convert.ToInt32(parameters[0]);
            // Get the generated UserID
            UserID = Convert.ToInt32(parameters[1]);

            // Deserialize parameter value to get the connectedClientsData dictionary
            connectedClients = JsonConvert.DeserializeObject<Dictionary<int, ClientData>>
                (Convert.ToString(parameters[2]));

            // We must re-initialize the connected clients only if the dictionary is not null
            if (connectedClients is null == false)
            {
                // Re-initialize the every ClientData object of the dictionary
                foreach (var item in connectedClients)
                {
                    var iconData = item.Value.UserIconData;
                    connectedClients[item.Key].UserIconData = iconData;
                }
            }
        }

        public object Execute(SimpleTCP.Message message)
        {
            var client = ChatManagerWT.Instance.Client;

            if (responceStatus == ResponceStatus.Ok && UserID != -1)
            {
                // Update the client's userID with the received one
                var clientData = client.ClientData;
                clientData.UserID = UserID;
                client.ClientData = clientData;

                // Set the dictionary of the clients data which are connected to the server
                client.ConnectedClients = connectedClients;

                // Mark client as connected
                client.IsConnected = true;

                // Show message about the successfull connection on the UI
                client.ShowSuccessfullConnectionMessage();
            }
            else if (responceStatus == ResponceStatus.Bad || UserID == -1)
            {
                client.Disconnect();
                MessageBox.Show(MessageBoxText, MessageBoxTitle);
            }

            return null;
        }
    }
}


