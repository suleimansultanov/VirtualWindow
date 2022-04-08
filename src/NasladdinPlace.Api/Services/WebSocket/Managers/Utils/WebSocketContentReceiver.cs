using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.Utils
{
    public class WebSocketContentReceiver : IWebSocketContentReceiver
    {
        private const int BytesInKb = 1024;
        private const int FourKb = 4 * BytesInKb;

        public async Task ReceiveAsync(
            System.Net.WebSockets.WebSocket socket, Action<WebSocketReceiveResult, string> handleMessage)
        {
            try
            {
                await ReceiveAuxAsync(socket, handleMessage);
            }
            catch (Exception)
            {
                // do nothing
            }
        }
        
        private static async Task ReceiveAuxAsync(
            System.Net.WebSockets.WebSocket socket, Action<WebSocketReceiveResult, string> handleMessage)
        {
            while (socket.State == WebSocketState.Open)
            {
                var buffer = new ArraySegment<byte>(new byte[FourKb]);
                string serializedInvocationDescriptor;
                WebSocketReceiveResult result;

                MemoryStream ms = null;
                try
                {
                    ms = new MemoryStream();
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);

                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        ms = null;
                        serializedInvocationDescriptor = await reader.ReadToEndAsync().ConfigureAwait(false);
                    }
                }
                finally
                {
                    ms?.Dispose();
                }

                handleMessage(result, serializedInvocationDescriptor);
            }
        }
    }
}