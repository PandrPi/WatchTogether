using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchTogether.Chatting;

namespace WatchTogether.Browser.BrowserCommands
{
    struct DisconnectClientCommand : IBrowserCommand
    {
        public object Execute(Message message)
        {
            var client = ChatManagerWT.Instance.Client;

            client.Disconnect();

            return null;
        }

        public void Initialize(params object[] parameters)
        {
            
        }
    }
}
