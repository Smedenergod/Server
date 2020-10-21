using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class ServerProgram
    {
        static void Main()
        {
            var server = new TcpListener(IPAddress.Loopback, 5000);
            server.Start();
            Console.WriteLine("Server started!");
            CreateStartCategories(1, "Beverages");
            CreateStartCategories(2, "Condiments");
            CreateStartCategories(3, "Confections");

            while (true)
            {
                var client = server.AcceptTcpClient();
                Console.WriteLine("Accepted client!");

                var stream = client.GetStream();
                stream.ReadTimeout = 150;
                var msg = Read(client, stream);
                var request = new Request();
                if (!msg.Equals(""))
                {
                    request = JsonSerializer.Deserialize<Request>(msg);
                    request.Stream = stream;
                    Action action = new Action(request.run);
                    Task task = new Task(action);
                    task.Start();
                }
                else
                    Console.WriteLine("No Response expected");
            }
        }

        private static void CreateStartCategories(int cid, string name)
        {
            var category = new Categories();
            category.cid = cid;
            category.name = name;
            Helper.categoryList.Add(category);
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
