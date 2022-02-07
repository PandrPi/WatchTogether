using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;
using WatchTogether.Chatting;

namespace WatchTogether.Browser.BrowserCommands
{
    struct ClientConnectionResponceCommand : IBrowserCommand
    {
        private int UserID;
        private ResponceStatus responceStatus;
        private Dictionary<int, ClientData> connectedClients;
        private ILogger Logger = LogManager.GetCurrentClassLogger();

        private const string MessageBoxText = "Check whether the password is correct and try again.";
        private const string MessageBoxTitle = "A host has declined the connection.";

        public void Initialize(params object[] parameters)
        {
            responceStatus = (ResponceStatus)Convert.ToInt32(parameters[0]);
            UserID = Convert.ToInt32(parameters[1]);

            connectedClients = JsonConvert.DeserializeObject<Dictionary<int, ClientData>>
                (Convert.ToString(parameters[2]));

            // We must re-initialize the connected clients only if the dictionary is not null
            if (connectedClients is null == false)
            {
                InitializeIconData();
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

                client.ConnectedClients = connectedClients;

                client.IsConnected = true;

                client.ShowSuccessfullConnectionMessage();
            }
            else if (responceStatus == ResponceStatus.Bad || UserID == -1)
            {
                Logger.Trace($"Failed due to responce status {responceStatus} and/or UserID {UserID}");
                client.Disconnect();
                MessageBox.Show(MessageBoxText, MessageBoxTitle);
            }

            return null;
        }

        private void InitializeIconData()
        {
            foreach (var item in connectedClients)
            {
                var iconData = item.Value.UserIconData;
                connectedClients[item.Key].UserIconData = iconData;
            }
        }
    }
}


