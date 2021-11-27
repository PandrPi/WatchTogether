using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace WatchTogether.Chatting.Messages
{
	struct MessageWT
	{
		/// <summary>
		/// The text of the message
		/// </summary>
		public string Text { get; private set; }

		/// <summary>
		/// The DateTime object that represents when a message was received by a server
		/// </summary>
		public DateTime ReceivingDateTime { get; private set; }

		/// <summary>
		/// The MessageTypeWT enum that represents the actual type of a message (User or System)
		/// </summary>
		public MessageTypeWT MessageType { get; private set; }

		/// <summary>
		/// The ID of the user from whom the message was received
		/// </summary>
		public int UserID { get; private set; }

		/// <summary>
		/// Initializes a new MessageWT instance
		/// </summary>
		/// <param name="text">The text of the message</param>
		/// <param name="receivingDateTime">The DateTime object that represents the time when the message was received</param>
		/// <param name="messageType">Determines whether the message belongs to internal messages of the app</param>
		public MessageWT(string text, DateTime receivingDateTime, MessageTypeWT messageType, int userID)
		{
			Text = text ?? throw new ArgumentNullException(nameof(text));
			ReceivingDateTime = receivingDateTime;
			MessageType = messageType;
			UserID = userID;
		}

		/// <summary>
		/// Converts the value of the current ReceivingDateTime object to local time
		/// </summary>
		public void ConvertReceivingDateTimeToLocalTime()
		{
			SetReceivingDateTime(ReceivingDateTime.ToLocalTime());
		}

		/// <summary>
		/// Sets a new value for ReceivingDateTime property
		/// </summary>
		/// <param name="dateTime">The DateTime object to be set</param>
		public void SetReceivingDateTime(DateTime dateTime)
		{
			ReceivingDateTime = dateTime;
		}

		/// <summary>
		/// Deserializes the specified message string (in JSON format) to a new MessageWT instance
		/// </summary>
		/// <param name="message">The message json string</param>
		/// <returns>The new MessageWT instance</returns>
		public static MessageWT FromString(string messageString)
		{
			return JsonConvert.DeserializeObject<MessageWT>(messageString);
		}

		/// <summary>
		/// Serializes this MessageWT instance to a Json string
		/// </summary>
		/// <returns>The Json string that represents this MessageWT instance</returns>
		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
