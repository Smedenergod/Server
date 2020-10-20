using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        //public List<Categories> categoryList = new List<Categories>();

        public void setStream(Stream stream)
        {
            this.stream = stream;
        }

        public void run()
        {
            ServerResponse response = new ServerResponse();
            simpleValidate(/*response*/);
            RequestPath(response);
        }

        public void simpleValidate(/*ServerResponse response*/)
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

            if ( !String.IsNullOrEmpty(method) &&  (!method.Contains("update") && !method.Contains("delete") && !method.Contains("read") &&
                                                   !method.Contains("create")) )
            {
                if (String.IsNullOrEmpty(response.status))
                    response.status = "illegal method";
                else
                    response.status = String.Concat(response.status, "+ illegal method");
            }

            if(!String.IsNullOrEmpty(response.status)) 
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
        }

        public void RequestPath(ServerResponse response)
        {
            switch (method)
            {
                case "create":
                    create(response);
                    break;
                case "delete":
                    break;
                case "read":
                    break;
                case "update":
                    break;
                default:
                    break;
            }
        }

        public void create(ServerResponse response)
        {
            Console.WriteLine(path);
            if (!path.EndsWith("categories"))
            {
                Console.WriteLine("Testing");
                if (String.IsNullOrEmpty(response.status))
                    response.status = "4 bad request";
                else
                    response.status = String.Concat(response.status, "+ 4 bad request");
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }
            //if (!String.IsNullOrEmpty(response.status))
            //stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));


            //var test = JsonSerializer.Serialize(new {name = "Testing"});



            Categories categories = new Categories();



            //var test = JsonSerializer.Deserialize<Categories>(response.body);

            //Console.WriteLine(test);
            categories = JsonSerializer.Deserialize<Categories>(body);
            //categoryList.Add(categories);
            //categories.cid = categoryList.Count;

            
            //categories = JsonSerializer.Deserialize<Categories>(test);

            //Console.WriteLine(categories);

            response.status = "2 created";

            response.body = JsonSerializer.Serialize(categories);
            //Console.WriteLine(response.body);
            stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
            return;
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
