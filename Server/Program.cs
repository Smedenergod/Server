using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            var server = new TcpListener(IPAddress.Loopback, 5000);
            server.Start();
            Console.WriteLine("Server started!");
            while (true)
            {
                var client = server.AcceptTcpClient();
                Console.WriteLine("Accepted client!");
                var stream = client.GetStream();

                var msg = Read(client, stream);
                
                /*var validate = Validate(msg);

                ServerResponse serverResponse = new ServerResponse(6, validate);
                var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(serverResponse));
                */

                if (/*validate.Equals("Fine")*/true)
                {
                    Console.WriteLine(msg);
                    var request = new Request();
                    request = JsonSerializer.Deserialize<Request>(msg);
                    //request = JsonSerializer.Deserialize(msg);
                    request.setStream(stream);
                    request.run();

                    /*var newResponse = Path(request);
                    var data = Encoding.UTF8.GetBytes(newResponse);
                    stream.Write(data);*/
                }
                //stream.Write(data);

                //Console.WriteLine($"Message from client {msg}");
            }
        }
        private static string Read(TcpClient client, NetworkStream stream)
        {
            byte[] data = new byte[client.ReceiveBufferSize];
            var cnt = stream.Read(data);
            var msg = Encoding.UTF8.GetString(data, 0, cnt);
            return msg;
        }

       /* private static string Validate(string msg)
        {
            if(!msg.ToLower().Contains("method")) return "missing method";
            if (!msg.ToLower().Contains("path")) return "missing path";
            if (!msg.ToLower().Contains("date")) return "missing date";
            return "Fine";
        }*/
       /*
        private static string Path(Request request)
        {
            string method = request.getMethod();

            switch (method)
            {
                case "create":
                    Create(request);
                    return "Creating Object";
                case "update":
                    Update(request);
                    return "Updating Object";
                case "delete":
                    Delete(request);
                    return "Deleting Object";
                case "retrieve":
                    Retrieve(request);
                    return "Retrieving Object";
                default:
                    ServerResponse serverResponse = new ServerResponse("6", "illegal method");
                    return JsonSerializer.Serialize(serverResponse);
            }
        }*/


        private static void Create(Request request)
        {
        }

        private static void Update(Request request)
        {
        }

        private static void Delete(Request request)
        {
        }

        private static void Retrieve(Request request)
        {
        }


    }
    }
