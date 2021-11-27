using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WatchTogether.Chatting
{
	public class ChatHelper
	{
		/// <summary>
		/// Converts the specified socket address string to IpAddress object and port 32-bit integer number. A return
		/// value indicates whether the conversion succeeded.
		/// </summary>
		/// <param name="socketAddress">The socket address string to convert</param>
		/// <param name="ip">The out IPAddress object</param>
		/// <param name="port">The out port integer</param>
		/// <returns>True if conversion succeeded and False otherwise</returns>
		public static bool TryParseSocketAddress(string socketAddress, out IPAddress ip, out int port)
		{
			const char socketSeparator = ':';

			if (socketAddress.Contains(socketSeparator) == false)
			{
				ip = null;
				port = 0;
				return false;
			}

			var addressArray = socketAddress.Split(socketSeparator);
			string ipStr = addressArray[0];
			string portStr = addressArray[1];

			bool ipParseResult = IPAddress.TryParse(ipStr, out ip);
			bool portparseResult = int.TryParse(portStr, out port) && (port >= 0 && port <= 65536);
			//portparseResult &= port >= 0 && port <= 9999;

			return ipParseResult && portparseResult;
		}
	}
}
