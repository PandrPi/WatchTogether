using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchTogether.Chatting;

namespace WatchTogether.Browser.BrowserCommands
{
    struct UpdateConnectedClientsCommand : IBrowserCommand
    {
        private ClientData clientToAdd;
        private int clientIDToRemove;

        public object Execute(Message message)
        {
            var client = ChatManagerWT.Instance.Client;

            // Check whether we have the data of the client to be added
            if (clientToAdd is null == false)
            {
                // Add a new ClientData instance to the ConnectedClients
                client.ConnectedClients[clientToAdd.UserID] = clientToAdd;
            }

            // Check whether we have the ID of the client to be removed
            if (clientIDToRemove != ChatServer.ServerID)
            {
                if (client.ClientData.UserID == clientIDToRemove)
                {
                    // We have to disconnect the client which received this command
                    client.Disconnect();
                }
                else
                {
                    // The client we have to disconnect isn't the current client
                    if (client.ConnectedClients.ContainsKey(clientIDToRemove))
                    {
                        client.ConnectedClients.Remove(clientIDToRemove);
                    }
                }
            }
            return null;
        }

        public void Initialize(params object[] parameters)
        {
            clientToAdd = ClientData.FromString(Convert.ToString(parameters[0]));
            clientIDToRemove = Convert.ToInt32(parameters[1]);
        }
    }
}
