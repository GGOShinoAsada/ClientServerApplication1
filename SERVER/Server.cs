using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Server.SERVER
{
    class Server
    {
        public readonly Socket listen;
        public Server(int port)
        {
            IPEndPoint point = new IPEndPoint(IPAddress.Loopback, port);
            listen = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listen.Bind(point);
            listen.Listen(1);
            listen.BeginAccept(_Accept, null);
        }
        private async void _Accept(IAsyncResult result)
        {
            try
            {
                using (Socket client = listen.EndAccept(result))
                using (NetworkStream stream = new NetworkStream(client))
                using (StreamReader reader = new StreamReader(stream))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    Console.WriteLine("SERVER: accepted new clien"); 
                    string text;
                    while ((text = await reader.ReadLineAsync()) != null)
                    {
                        Console.WriteLine($"SERVER received: {text}");
                        writer.WriteLine(text);
                        writer.Flush();
                    }
                }
                listen.BeginAccept(_Accept, null);
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine("ERROR: server was closed: " + ex.Message);
            }
            catch (SocketException ex2)
            {
                Console.WriteLine("ERROR: other socket error" + ex2.Message);
            }
        }
        public void Stop()
        {
            listen.Close();
        }
    }
}
