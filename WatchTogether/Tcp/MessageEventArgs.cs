using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchTogether.Tcp
{
    internal class MessageEventArgs : EventArgs
    {
        private readonly Encoding encoding;
        private string message;
        private byte[] messageBytes;

        public string Message => GetMessageString();

        public byte[] MessageBytes => messageBytes;

        public MessageEventArgs(byte[] messageBytes, Encoding encoding)
        {
            this.messageBytes = messageBytes;
            this.encoding = encoding;
        }

        private string GetMessageString()
        {
            if (message is null == false) return message;

            message = encoding.GetString(messageBytes);
            return message;
        }
    }
}
