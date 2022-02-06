using System.Linq;
using System.Windows.Media;

namespace WatchTogether.Chatting.Messages
{
    struct MessageData
    {
        /// <summary>
		/// The text of the message
		/// </summary>
        public string MessageText { get; set; }

        /// <summary>
        /// The user name of a message sender
        /// </summary>
        public string SenderUserName { get; set; }

        /// <summary>
        /// The icon brush of a message sender
        /// </summary>
        public Brush SenderIconBrush { get; set; }

        /// <summary>
        /// The short user name of a message sender
        /// </summary>
        public string ShortSenderUserName { get; set; }

        /// <summary>
        /// The DateTime object that represents when a message was received by a server
        /// </summary>
        public string ReceivingTime { get; set; }

        /// <summary>
        /// Determines whether the message belongs to the user who received the message
        /// </summary>
        public bool IsMessageMine { get; set; }

        public MessageData(string messageText, string senderUserName, string receivingTime, bool isMessageMine) : this(messageText, senderUserName, UserIconColorBrushManager.GetRandomColorBrush(), GetShortUserName(senderUserName), receivingTime, isMessageMine) { }

        public MessageData(string messageText, string senderUserName, Brush senderIconBrush, string receivingTime, bool isMessageMine) : this(messageText, senderUserName, senderIconBrush, GetShortUserName(senderUserName), receivingTime, isMessageMine) { }

        private MessageData(string messageText, string senderUserName, Brush senderIconBrush, string shortSenderUserName,
                           string receivingTime, bool isMessageMine)
        {
            MessageText = messageText;
            SenderUserName = senderUserName;
            SenderIconBrush = senderIconBrush;
            ShortSenderUserName = shortSenderUserName;
            ReceivingTime = receivingTime;
            IsMessageMine = isMessageMine;
        }

        /// <summary>
        /// Converts the specified usual user name to a short user name
        /// </summary>
        /// <param name="userName">The user name value</param>
        /// <returns>Short user name value</returns>
        public static string GetShortUserName(string userName)
        {
            // Max length of the shortUserName string
            const int maxLength = 2;

            // Construct a string from the first letter of every word in the userName
            var shortUserName = string.Join(string.Empty, userName.Split(' ').Select(s => s.First()));
            // As the specified userName can contain multiple words we have to limit the length
            // of the shortUserName string value to only two characters
            return shortUserName.Length <= maxLength ? shortUserName : shortUserName.Substring(0, maxLength);
        }
    }
}
