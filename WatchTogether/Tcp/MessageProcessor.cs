using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatchTogether.Tcp
{
    internal class MessageProcessor : IMessageProcessor
    {
        private readonly List<byte> currentBytes = new List<byte>();
        private readonly byte messageDelimiter;
        private readonly Encoding messageEncoding;

        public MessageProcessor(byte delimiter, Encoding encoding)
        {
            messageDelimiter = delimiter;
            messageEncoding = encoding;
        }

        /// <inheritdoc />
        public event EventHandler<MessageEventArgs> OnMessageReceived;

        /// <inheritdoc />
        public async Task ProcessReceivedBytes(byte[] bytes, CancellationToken token)
        {
            if (bytes is null) return;

            foreach (byte b in bytes)
            {
                currentBytes.Add(b);
                var lastIndex = currentBytes.Count - 1;

                if (currentBytes.Count == 1 && currentBytes[0] == messageDelimiter)
                {
                    currentBytes.Clear();

                    continue;
                }

                if (currentBytes[lastIndex] == messageDelimiter)
                {
                    await ProcessNewMessage(token);
                }
            }
        }

        private async Task ProcessNewMessage(CancellationToken token)
        {
            await Task.Run(() =>
            {
                OnMessageReceived?.Invoke(this, new MessageEventArgs(currentBytes.ToArray(), messageEncoding));
            }, token);
        }
    }
}
