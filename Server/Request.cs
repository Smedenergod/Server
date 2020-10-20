using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.VisualBasic;

namespace Server
{
    class Request
    {
        public string method { get; set; }
        public string path { get; set; }
        public string date { get; set; }
        public string body { get; set; }

        private Stream stream { get; set; }
        private readonly Random _random = new Random();
        private static List<Categories> categorieses = new List<Categories>() { new Categories() { cid = 1, name = "Beverages" }, new Categories() { cid = 2, name = "Condiments" }, new Categories() { cid = 3, name = "Confections" } };

        //public List<Categories> categoryList = new List<Categories>();

        public void setStream(Stream stream)
        {
            this.stream = stream;
        }

        public void run()
        {
            ServerResponse response = new ServerResponse();
            var shouldContinue = simpleValidate(/*response*/);
            if (shouldContinue)
                RequestPath(response);
        }

        public bool simpleValidate(/*ServerResponse response*/)
        {
            ServerResponse response = new ServerResponse();
            int paramId;

            if (method == "echo")
            {
                return true;
            }

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

            if (String.IsNullOrEmpty(body) && (method == "create" || method == "update" ||  method == "echo"))
            {
                if (String.IsNullOrEmpty(response.status))
                    response.status = "missing body";
                else
                    response.status = String.Concat(response.status, "+ missing body");

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
            else if (date.Contains('/'))
            {
                if (String.IsNullOrEmpty(response.status))
                    response.status = "illegal date";
                else
                    response.status = String.Concat(response.status, "+ illegal date");
            }

            if (!String.IsNullOrEmpty(method) && (!method.Contains("update") && !method.Contains("delete") && !method.Contains("read") &&
                                                   !method.Contains("create")))
            {
                if (String.IsNullOrEmpty(response.status))
                    response.status = "illegal method";
                else
                    response.status = String.Concat(response.status, "+ illegal method");
            }

            if (string.IsNullOrEmpty(path) || !path.Contains("categories") || (path.Contains("categories/") && !int.TryParse(path.Split("categories/")[1], out paramId)))
            {
                if (string.IsNullOrEmpty(path))
                {
                    if (String.IsNullOrEmpty(response.status))
                        response.status = "missing resource";
                    else
                        response.status = String.Concat(response.status, "+ missing resource");
                }
                //ServerResponse response = new ServerResponse();
                if (String.IsNullOrEmpty(response.status))
                    response.status = "4 Bad Request";
                else
                    response.status = String.Concat(response.status, "+ 4 bad request");
                //stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
            }

            if (!String.IsNullOrEmpty(response.status))
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
            return true;
        }

        public void RequestPath(ServerResponse response)
        {
            switch (method)
            {
                case "create":
                    create(response);
                    break;
                case "delete":
                    delete(response);
                    break;
                case "read":
                    read(response);
                    break;
                case "update":
                    update(response);
                    break;
                case "echo":
                    echo(response);
                    break;
                default:
                    break;
            }
        }

        public void create(ServerResponse response)
        {
            if (path == null)
            {
                response.status = "missing resource";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            if (body == null)
            {
                response.status = "missing body";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

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
            categories.cid = _random.Next(4, 100);
            categorieses.Add(categories);
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

        public void read(ServerResponse response)
        {
            if (path == null)
            {
                response.status = "missing resource";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            int paramId;
            if (path.EndsWith("categories"))
            {
                if (String.IsNullOrEmpty(response.status))
                    response.status = "1 Ok";
                else
                    response.status = String.Concat(response.status, "+ 1 Ok");

                response.body = JsonSerializer.Serialize(categorieses);
            }
            else if (path.Contains("categories/"))
            {
                if (int.TryParse(path.Split("categories/")[1], out paramId))
                {
                    if (categorieses.Exists(x => x.cid == paramId))
                    {
                        if (String.IsNullOrEmpty(response.status))
                            response.status = "1 Ok";
                        else
                            response.status = String.Concat(response.status, "+ 1 Ok");

                        response.body = JsonSerializer.Serialize(categorieses.Find(x => x.cid == paramId));
                    }
                    else
                    {
                        response.status = "5 Not Found";
                    }
                }
            }

            stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
        }

        public void delete(ServerResponse response)
        {
            if (path == null)
            {
                response.status = "missing resource";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            if (path.EndsWith("categories"))
            {
                if (String.IsNullOrEmpty(response.status))
                    response.status = "4 Bad Request";
                else
                    response.status = String.Concat(response.status, "+ 4 Bad Request");

                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            int paramId;
            if (int.TryParse(path.Split("categories/")[1], out paramId))
            {
                if (categorieses.Exists(x => x.cid == paramId))
                {
                    categorieses.Remove(categorieses.Find(x => x.cid == paramId));

                    response.status = "1 Ok";
                }
                else
                {
                    response.status = "5 Not Found";
                }

            }
            else
            {
                response.status = "5 Not Found";
            }

            stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
        }

        public void update(ServerResponse response)
        {
            if (path == null)
            {
                response.status = "missing resource";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            if (body == null)
            {
                response.status = "missing body";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            if (path.EndsWith("categories"))
            {
                if (String.IsNullOrEmpty(response.status))
                    response.status = "4 Bad Request";
                else
                    response.status = String.Concat(response.status, "+ 4 Bad Request");

                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            int paramId;
            if (path.Contains("categories/"))
            {
                if (int.TryParse(path.Split("categories/")[1], out paramId))
                {
                    if (categorieses.Exists(x => x.cid == paramId))
                    {
                        var cateogryToUpdate = categorieses.Find(x => x.cid == paramId);
                        categorieses.Remove(cateogryToUpdate);
                        try
                        {
                            cateogryToUpdate.name = JsonSerializer.Deserialize<Categories>(body).name;
                        }
                        catch (Exception e)
                        {
                            response.status = "illegal body";
                            stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                            return;
                        }

                        categorieses.Add(cateogryToUpdate);

                        response.status = "3 Updated";
                    }
                    else
                    {
                        response.status = "5 Not Found";
                    }

                }
            }
            else

            {
                response.status = "5 Not Found";
            }

            stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
        }

        public void echo(ServerResponse response)
        {
            if (body == null)
            {
                response.status = "missing body";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            response.body = body;

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
