using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static UTF8Encoding encoding = new UTF8Encoding();

        static void Main(string[] args)
        {
            Connect("ws://localhost:2587/test/").Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static async Task Connect(string uri)
        {
            Thread.Sleep(1000); //wait for a sec, so server starts and ready to accept connection..

            ClientWebSocket webSocket = null;
            try
            {
                webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                await Task.WhenAll(Receive(webSocket), Send(webSocket));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }
            finally
            {
                if (webSocket != null)
                    webSocket.Dispose();
                Console.WriteLine();
                Console.WriteLine("WebSocket closed.");
            }
        }

        private static async Task Send(ClientWebSocket webSocket)
        {
            
            while (webSocket.State == WebSocketState.Open)
            {
                Console.WriteLine("Write some to send over to server..");
                string stringtoSend = Console.ReadLine();
                byte[] buffer = encoding.GetBytes(stringtoSend);

                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, false, CancellationToken.None);
                Console.WriteLine("Sent:     " + stringtoSend);

                await Task.Delay(1000);
            }
        }

        private static async Task Receive(ClientWebSocket webSocket)
        {
            byte[] buffer = new byte[1024];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    Console.WriteLine("Receive:  " + Encoding.UTF8.GetString(buffer).TrimEnd('\0'));
                }
            }
        }
    }
}
