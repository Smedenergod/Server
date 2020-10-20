using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Server
{
    class Request
    {
        public string method { get; set; }
        public string path { get; set; }
        public string date { get; set; }
        public string body { get; set; }

        private Stream stream { get; set; }

        public void setStream(Stream stream)
        {
            this.stream = stream;
        }

        public void simpleValidate()
        {
            ServerResponse response = new ServerResponse();

            if (String.IsNullOrEmpty(method))
            {
                if (String.IsNullOrEmpty(response.status))
                    response.status = "missing method";
                else
                    response.status = String.Concat(response.status, "+ missing method");

                //response.body = "missing method";
                //response.printing();
                //var msg = JsonSerializer.Serialize<ServerResponse>(response);
                //var data = JsonSerializer.SerializeToUtf8Bytes(response);
                //Console.WriteLine($"Replying with message: {msg}");
                //var data = Encoding.UTF8.GetBytes(msg);
                //stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
            }

            if (String.IsNullOrEmpty(date))
            {
                //ServerResponse response = new ServerResponse();
                if (String.IsNullOrEmpty(response.status))
                    response.status = "missing date";
                else
                    response.status = String.Concat(response.status, "+ missing date");
                //stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
            }

            if ( !String.IsNullOrEmpty(method) &&  (!method.Contains("update") || !method.Contains("delete") || !method.Contains("read") ||
                                                   !method.Contains("create")) )
            {
                if (String.IsNullOrEmpty(response.status))
                    response.status = "illegal method";
                else
                    response.status = String.Concat(response.status, "+ illegal method");
            }

            stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
        }

        /*public Request(string method, string path, string date, string body)
        {
            this.method = method;
            this.path = path;
            this.date = date;
            this.body = body;
        }*/
    }
}
