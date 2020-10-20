using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Server
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            var server = new TcpListener(IPAddress.Loopback, 5000);
            server.Start();
            Console.WriteLine("Server started!");

            Categories category1 = new Categories();
            category1.cid = 1;
            category1.name = "Beverages";
            Categories category2 = new Categories();
            category2.cid = 2;
            category2.name = "Condiments";
            Categories category3 = new Categories();
            category3.cid = 3;
            category3.name = "Confections";

            Helper.categoryList.Add(category1);
            Helper.categoryList.Add(category2);
            Helper.categoryList.Add(category3);


            while (true)
            {
                var client = server.AcceptTcpClient();
                Console.WriteLine("Accepted client!");
                var stream = client.GetStream();
                var msg = Read(client, stream);
                var request = new Request();
                if (!msg.Equals(""))
                {
                    request = JsonSerializer.Deserialize<Request>(msg);
                    request.setStream(stream);
                    request.run();
                }
                else
                    Console.WriteLine("OY MATE");
            }
        }
        private static string Read(TcpClient client, NetworkStream stream)
        {
            byte[] data = new byte[client.ReceiveBufferSize];

            try
            {
                var cnt = stream.Read(data);
                var msg = Encoding.UTF8.GetString(data, 0, cnt);
                return msg;
            }

            catch (Exception E)
            {
                return "";
            }
        }
    }
    }
