using System;
using System.IO;
using System.Text.Json;

namespace Server
{
    internal class Request
    {
        public string method { get; set; }
        public string path { get; set; }
        public string date { get; set; }
        public string body { get; set; }

        //public DateTime time { get; set; } = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        private Stream stream { get; set; }

        //public static List<Categories> categoryList = new List<Categories>();
        public void setStream(Stream stream)
        {
            this.stream = stream;
        }

        public void run()
        {
            var response = new ServerResponse();
            simpleValidate(response);
            RequestPath(response);
        }

        public void simpleValidate(ServerResponse response)
        {
            if (string.IsNullOrEmpty(method))
            {
                if (string.IsNullOrEmpty(response.status))
                    response.status = "missing method";
                else
                    response.status = string.Concat(response.status, "+ missing method");
            }

            if (string.IsNullOrEmpty(date))
            {
                if (string.IsNullOrEmpty(response.status))
                    response.status = "missing date";
                else
                    response.status = string.Concat(response.status, "+ missing date");
            }

            try
            {
                var testing = int.Parse(date);
            }
            catch (Exception E)
            {
                if (string.IsNullOrEmpty(response.status))
                    response.status = "illegal date";
                else
                    response.status = string.Concat(response.status, "+ illegal date");
            }


            if (!string.IsNullOrEmpty(method) && !method.Contains("update") && !method.Contains("delete") &&
                !method.Contains("read") && !method.Contains("create") && !method.Contains("echo"))
            {
                if (string.IsNullOrEmpty(response.status))
                    response.status = "illegal method";
                else
                    response.status = string.Concat(response.status, "+ illegal method");
            }

            if (!string.IsNullOrEmpty(response.status))
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
        }

        public void RequestPath(ServerResponse response)
        {
            switch (method)
            {
                case "create":
                    Create(response);
                    break;
                case "delete":
                    Delete(response);
                    break;
                case "read":
                    Read(response);
                    break;
                case "update":
                    Update(response);
                    break;
                case "echo":
                    Echo(response);
                    break;
                default:
                    stream.Write(JsonSerializer.SerializeToUtf8Bytes("Defaulting"));
                    break;
            }
        }

        public void Create(ServerResponse response)
        {
            if (IsPathEmpty())
            {
                response.status = "missing resource";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            if (IsBodyEmpty())
            {
                MissingBody(response);
                return;
            }

            if (!path.EndsWith("categories"))
            {
                if (string.IsNullOrEmpty(response.status))
                    response.status = "4 bad request";
                else
                    response.status = string.Concat(response.status, "+ 4 bad request");
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            var categories = new Categories();

            categories = JsonSerializer.Deserialize<Categories>(body);
            Helper.categoryList.Add(categories);
            categories.cid = Helper.categoryList.Count;

            response.status = "2 created";

            response.body = JsonSerializer.Serialize(categories);
            stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
        }

        public void Update(ServerResponse response)
        {
            if (IsPathEmpty())
            {
                response.status = "missing resource";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            if (IsBodyEmpty())
            {
                MissingBody(response);
                return;
            }

            if (!body.StartsWith("{"))
            {
                response.status = "illegal body";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            if (path.EndsWith("categories"))
            {
                response.status = "4 Bad Request";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            var results = path.Split("/");
            var result = results[results.Length - 1];

            for (var i = 0; i < result.Length; i++)
            {
                var isInt = char.IsDigit(result[i]);
                if (!isInt)
                {
                    response.status = "4 Bad Request";
                    stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                    return;
                }
            }

            var intresult = int.Parse(result);

            Categories category;
            if (intresult <= Helper.categoryList.Count)
            {
                category = Helper.categoryList[intresult - 1];
                var newCategory = JsonSerializer.Deserialize<Categories>(body);
                category.name = newCategory.name;
                response.status = "3 updated";
                response.body = JsonSerializer.Serialize(category);
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
            }
            else
            {
                response.status = "5 not found";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
            }
        }

        public void Read(ServerResponse response)
        {
            if (IsPathEmpty())
            {
                response.status = "missing resource";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            if (path.Equals("/api/categories"))
            {
                response.status = "1 Ok";
                response.body = JsonSerializer.Serialize(Helper.categoryList);
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            var results = path.Split("/");
            var result = results[results.Length - 1];

            for (var i = 0; i < result.Length; i++)
            {
                var isInt = char.IsDigit(result[i]);
                if (!isInt)
                {
                    response.status = "4 Bad Request";
                    stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                    return;
                }
            }

            var intresult = int.Parse(result);

            if (intresult <= Helper.categoryList.Count)
            {
                var categories = Helper.categoryList[intresult - 1];
                response.status = "1 Ok";
                response.body = JsonSerializer.Serialize(categories);
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
            }
            else
            {
                response.status = "5 not found";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
            }
        }

        public void Delete(ServerResponse response)
        {
            if (IsPathEmpty())
            {
                response.status = "missing resource";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }


            if (path.EndsWith("categories"))
            {
                response.status = "4 Bad Request";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                return;
            }

            var results = path.Split("/");
            var result = results[results.Length - 1];

            for (var i = 0; i < result.Length; i++)
            {
                var isInt = char.IsDigit(result[i]);

                if (!isInt)
                {
                    response.status = "4 Bad Request";
                    stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                    return;
                }
            }

            var intresult = int.Parse(result);

            if (intresult <= Helper.categoryList.Count)
            {
                Helper.categoryList.RemoveAt(intresult - 1);
                response.status = "1 Ok";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
            }
            else
            {
                response.status = "5 Not Found";
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
            }
        }

        public void Echo(ServerResponse response)
        {
            if (IsBodyEmpty())
            {
                MissingBody(response);
            }

            else
            {
                response.body = body;
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
            }
        }

        public bool IsPathEmpty()
        {
            return string.IsNullOrEmpty(path);
        }

        public bool IsBodyEmpty()
        {
            return string.IsNullOrEmpty(body);
        }

        public void MissingBody(ServerResponse response)
        {
            response.status = "missing body";
            stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
        }
    }
}