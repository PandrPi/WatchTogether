using System;
using System.Text;
using System.Threading.Tasks;
using SimpleTCP;
using WatchTogether.Chatting.Messages;

namespace WatchTogether.Chatting
{
	class ChatClient : IDisposable
	{
		private static readonly Encoding MessageEncoder = Encoding.UTF8;

		private Action<MessageWT> messageReceivedUICallback;
		private SimpleTcpClient client;
		private string userName;
		private int userID;

		/// <summary>
		/// Initializes the client instance and its events
		/// </summary>
		public ChatClient()
		{
			// Initialize instance
			client = new SimpleTcpClient();
			client.StringEncoder = MessageEncoder;

			// Initialize events
			client.DataReceived += Client_DataReceived;
		}

		/// <summary>
		/// Creates a new SimpleTcpClient object and connects it to the specified IP address and port
		/// </summary>
		/// <param name="ip">The server IP address</param>
		/// <param name="port">The port of the server</param>
		public async void ConnectAsync(string ip, int port)
		{
			// Ensure that the client is not connected to any other server
			Disconnect();

			await Task.Run(() =>
			{
				try
				{
					client.Connect(ip, port);
				}
				catch (Exception e)
				{
					Console.WriteLine($"{e.Message}, {e.StackTrace}");
					Disconnect();
				}
			});
		}

		/// <summary>
		/// Disconnects the client from a server
		/// </summary>
		public void Disconnect()
		{
			try
			{
				// We should disconnect the client only if it is not null and it is connected to a server
				if (client.TcpClient is null == false && client.TcpClient.Connected == true)
				{
					client.Disconnect();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"{e.Message}, {e.StackTrace}");
			}
		}

		/// <summary>
		/// Releases the resources of the client
		/// </summary>
		public void Dispose()
		{
			try
			{
				client.Dispose();
				client = null;
			}
			catch (Exception e)
			{
				Console.WriteLine($"{e.Message}, {e.StackTrace}");
			}
		}

		/// <summary>
		/// Sets a new user name for the client
		/// </summary>
		/// <param name="userName"></param>
		public void SetUserName(string userName)
		{
			this.userName = userName;
			// TODO: Remove userID field initialization from this method
			userID = userName.GetHashCode();
		}

		/// <summary>
		/// Sets a callback action which will add all incoming messages to UI
		/// </summary>
		/// <param name="callback">The UI callback action for DataReceived event</param>
		public void SetMessageReceivedUICallback(Action<MessageWT> callback)
		{
			messageReceivedUICallback = callback;
		}

		/// <summary>
		/// Sets a new value for the userID field
		/// </summary>
		/// <param name="userID">The new userID value</param>
		private void SetUserID(int userID)
		{
			this.userID = userID;
		}

		/// <summary>
		/// Sends a request to the server to get a free userID
		/// </summary>
		private void SendUserIDRequest()
		{
			const string messageText = ""
		}

		/// <summary>
		/// Sends message to the server
		/// </summary>
		/// <param name="message">The message that will be sent to the server</param>
		public void SendMessage(string messageText, MessageTypeWT messageType = MessageTypeWT.User)
		{
			var message = new MessageWT(messageText, default, messageType, userID);
			client.Write(message.ToString());
		}

		/// <summary>
		/// DataReceived event handler, receives all the incoming messages
		/// </summary>
		private void Client_DataReceived(object sender, Message e)
		{
			MessageWT message = MessageWT.FromString(e.MessageString);

			if (message.MessageType == MessageTypeWT.User)
			{
				message.ConvertReceivingDateTimeToLocalTime();
				messageReceivedUICallback(message);
			}
			else
			{
				Console.WriteLine("Error: Client_DataReceived method has not implemented system message type branch!");
			}
		}
	}
}