using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchTogether.Chatting.Messages
{
	class BrowserCommandParser
	{
		Dictionary<string, Action> commands = new Dictionary<string, Action>
		{
			{"play", () => { }},
			{"pause", () => { }},
			{"rewind", () => { }},
			{"setUserID", () => { }},
		};

		
	}
}
