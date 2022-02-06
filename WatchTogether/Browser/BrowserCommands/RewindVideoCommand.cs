using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchTogether.Chatting;

namespace WatchTogether.Browser.BrowserCommands
{
    struct RewindVideoCommand : IBrowserCommand
    {
        private string currentTime;
        private string isPaused;
        private int clientID;

        private const string RewindVideoScriptKey = "RewindVideo";

        public object Execute(Message message)
        {
            var client = ChatManagerWT.Instance.Client;

            if (client.ClientData.UserID == clientID) return null;

            var scriptVariablesStr = $"var currentTime = {currentTime};\nvar paused = {isPaused};\n";
            var scriptCode = JavaScriptsHolder.GetScriptCodeByKey(RewindVideoScriptKey);
            scriptCode = scriptVariablesStr + scriptCode;

            BrowserManagerWT.Instance.ExecuteJavaScript(scriptCode);

            return null;
        }

        public void Initialize(params object[] parameters)
        {
            currentTime = Convert.ToString(parameters[0]).Replace(',', '.');
            isPaused = Convert.ToString(parameters[1]).ToLower();
            clientID = Convert.ToInt32(parameters[2]);
        }
    }
}
