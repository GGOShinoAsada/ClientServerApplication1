using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Server
{
    class Program
    {
        public static int port = 8080;
        
        public  static void Main(string[] args)
        {
            SERVER.Server server = new SERVER.Server(port);
            Socket remote = new Socket(SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint point = new IPEndPoint(IPAddress.Loopback, port);
            remote.Connect(point);
            using (NetworkStream stream = new NetworkStream(remote))
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                Task recievetask = _Receive(reader);
                string text;
                Console.WriteLine("CLIENT: connected.Enter text to send...");
                while ((text = Console.ReadLine()) != "")
                {
                    writer.WriteLine(text);
                    writer.Flush();
                }

                remote.Shutdown(SocketShutdown.Send);
                recievetask.Wait();
            }
            server.Stop();
        }
        private static async Task _Receive(StreamReader reader)
        {
            string receiveText;

            while ((receiveText = await reader.ReadLineAsync()) != null)
            {
                Console.WriteLine("CLIENT: received \"" + receiveText + "\"");
            }

            Console.WriteLine("CLIENT: end-of-stream");
        }
    }
}
