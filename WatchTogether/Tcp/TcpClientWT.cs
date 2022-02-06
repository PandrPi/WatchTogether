using NLog;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatchTogether.Tcp
{
    internal class TcpClientWT : ITcpClientWT
    {
        // TODO: Implement disconnection timer feature
        private readonly Timer disconnectTimer;
        private readonly IMessageProcessor messageProcessor;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly byte[] buffer = new byte[1024];

        private Task ListenerLoopTask;
        private CancellationTokenSource listenerCancellationTokenSource;

        public event EventHandler<MessageEventArgs> OnMessageReceived;

        public TcpClient Client { get; private set; }
        public bool IsConnected => Client.Connected;

        public TcpClientWT(byte messageDelimiter, Encoding messageEncoding)
        {
            messageProcessor = new MessageProcessor(messageDelimiter, messageEncoding);
        }

        public async Task ConnectAsync(string hostNameOrIpAddress, int port)
        {
            if (string.IsNullOrEmpty(hostNameOrIpAddress))
            {
                var message = $"{nameof(ConnectAsync)}: {nameof(hostNameOrIpAddress)} is null or empty!";
                logger.Trace(message);
                throw new ArgumentNullException(message);
            }

            Client = new TcpClient();
            await Client.ConnectAsync(hostNameOrIpAddress, port);
            ListenerLoopTask = StartListenerLoop();
        }

        private Task StartListenerLoop(TaskScheduler scheduler = null, CancellationToken token = default)
        {
            TaskScheduler taskScheduler = scheduler ?? TaskScheduler.Default;
            listenerCancellationTokenSource = new CancellationTokenSource();

            var task = Task.Factory.StartNew(
                async () =>
            {
                await ListenerLoopAsync(listenerCancellationTokenSource.Token).ConfigureAwait(false);
            }, token, TaskCreationOptions.LongRunning, taskScheduler);

            return task;
        }

        private async Task ListenerLoopAsync(CancellationToken token)
        {
            logger.Trace("The client's listener loop started");

            try
            {
                while (token.IsCancellationRequested == false)
                {
                    token.ThrowIfCancellationRequested();

                    await ReadMessage(token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException cancelledException)
            {
                logger.Debug(cancelledException,
                    "The client's listener loop exited because of cancellation token expiration");
            }
            catch (Exception exc)
            {
                logger.Error(exc, $"The client's listener loop exited unexpectedly: {exc.Message}");
            }
            finally
            {
                logger.Trace("Listener loop finished");
                await Task.Run(Disconnect).ConfigureAwait(false);
            }
        }

        private async Task ReadMessage(CancellationToken token)
        {
            byte[] receivedBytes = await GetMessageBytesAsync(token).ConfigureAwait(false);

            await ProcessReceivedBytes(receivedBytes, token);
        }

        private async Task ProcessReceivedBytes(byte[] bytes, CancellationToken token)
        {
            try
            {
                await messageProcessor.ProcessReceivedBytes(bytes, token);
            }
            catch (Exception exception)
            {
                logger.Error(exception, $"Received bytes processing failed due to: {exception.Message}");
            }
        }

        private async Task<byte[]> GetMessageBytesAsync(CancellationToken token)
        {
            int readBytes = await Client.GetStream().ReadAsync(buffer, 0, buffer.Length, token)
                .ConfigureAwait(false);

            if (readBytes > 0)
            {
                var receivedBytes = new byte[readBytes];

                Array.Copy(buffer, receivedBytes, readBytes);

                return receivedBytes;
            }

            return null;
        }

        public Task WriteAsync(byte[] data)
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(string data)
        {
            throw new NotImplementedException();
        }

        public Task WriteLineAsync(string data)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            logger.Trace(nameof(Disconnect));

            if (IsConnected == false)
            {
                logger.Trace($"{nameof(Disconnect)} - already disconnected.");
                return;
            }

            if (Client is null)
            {
                logger.Trace($"{nameof(Disconnect)} - the client is null.");
                return;
            }

            CancelListenerLoop();

            Client.Close();
            Client.Dispose();
            Client = null;

            logger.Trace($"{nameof(Disconnect)} - disconnected.");
        }

        private void CancelListenerLoop()
        {
            logger.Trace(nameof(CancelListenerLoop));
            listenerCancellationTokenSource.Cancel();
        }
    }
}
