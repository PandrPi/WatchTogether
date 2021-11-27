using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SimpleTCP;
using WatchTogether.Chatting.Messages;
using WatchTogether.Browser.BrowserCommands;

namespace WatchTogether.Chatting
{
	class ChatServer
	{
		private static readonly Encoding MessageEncoder = Encoding.UTF8;
		private static readonly int ServerID = 0;

		private SimpleTcpServer server;

		/// <summary>
		/// Initializes the instance of the server
		/// </summary>
		public ChatServer()
		{
			// Initialize instance
			server = new SimpleTcpServer();
			server.StringEncoder = MessageEncoder;

			// Initialize Events
			server.ClientConnected += Server_ClientConnected;
			server.ClientDisconnected += Server_ClientDisconnected;
			server.DataReceived += Server_DataReceived;
		}

		/// <summary>
		/// Starts the server at the specified IP address and port
		/// </summary>
		/// <param name="ip">The server IP address</param>
		/// <param name="port">The port of the server</param>
		public void StartServer(IPAddress ip, int port)
		{
			// Ensure that the server is stopped before starting a new server
			StopServer();

			try
			{
				server.Start(port);
			}
			catch (Exception e)
			{
				Console.WriteLine($"{e.Message}, {e.StackTrace}");
			}
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
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"{e.Message}, {e.StackTrace}");
			}
		}

		/// <summary>
		/// Handles all incoming user messages and broadcasts every message to all users
		/// </summary>
		private void Server_DataReceived(object sender, Message e)
		{
			// Get the MessageWT object from the received string
			var message = MessageWT.FromString(e.MessageString);
			// Set ReceivingDateTime of the message as DateTime.Now because it is the time when the message
			// have been actually received
			message.SetReceivingDateTime(DateTime.Now);

			if (message.MessageType == MessageTypeWT.User)
			{
				// Send the message to all the connected users
				server.Broadcast(message.ToString());
			}
			else
			{
				Console.WriteLine("Error: Server_DataReceived method has not implemented system message type branch!");
			}
		}

		/// <summary>
		/// Handles event when one of the connected clients disconnects from the server
		/// </summary>
		private void Server_ClientDisconnected(object sender, TcpClient e)
		{
			const string format = "{0} was disconnected!";
			var ipStr = ((IPEndPoint)e.Client.RemoteEndPoint).Address.ToString();
			var messageText = string.Format(format, ipStr);

			var message = new MessageWT(messageText, DateTime.Now, MessageTypeWT.User, ServerID);

			server.Broadcast(message.ToString());
		}

		/// <summary>
		/// Handles event when a new client connects to the server
		/// </summary>
		private void Server_ClientConnected(object sender, TcpClient e)
		{
			const string format = "{0} was connected!";
			var ipStr = ((IPEndPoint)e.Client.RemoteEndPoint).Address.ToString();
			var messageText = string.Format(format, ipStr);

			var message = new MessageWT(messageText, DateTime.Now, MessageTypeWT.User, ServerID);

			server.Broadcast(message.ToString());
		}
	}
}