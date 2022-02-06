using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatchTogether.Tcp
{
    /// <summary>
    /// Processes message bytes from the server
    /// </summary>
    internal interface IMessageProcessor
    {
        /// <summary>
        /// This event is raised when a new message is received ftom the server
        /// </summary>
        event EventHandler<MessageEventArgs> OnMessageReceived;

        /// <summary>
        /// Asyncronously processes received message bytes
        /// </summary>
        /// <param name="bytes">The bytes to process</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task ProcessReceivedBytes(byte[] bytes, CancellationToken token);
    }
}
