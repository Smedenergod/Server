using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace Server
{
    internal class Request
    {
        public string method { get; set; }
        public string path { get; set; }
        public string date { get; set; }
        public string body { get; set; }

        public Stream stream { get; set; }

        private ServerResponse response;

        public void run()
        {
            response = new ServerResponse();
            simpleValidate();
            RequestPath();
        }

        public void simpleValidate()
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

            if (!string.IsNullOrEmpty(response.status))
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
        }

        public void RequestPath()
        {
            switch (method)
            {
                case "create":
                    Create();
                    break;
                case "delete":
                    Delete();
                    break;
                case "read":
                    Read();
                    break;
                case "update":
                    Update();
                    break;
                case "echo":
                    Echo();
                    break;
                default:
                    response.status = "illegal method";
                    stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                    break;
            }
        }

        public void Create()
        {
            if (IsPathEmpty())
            {
                MissingPath();
                return;
            }

            if (IsBodyEmpty())
            {
                MissingBody();
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

        public void Update()
        {
            if (IsPathEmpty())
            {
                MissingPath();
                return;
            }

            if (IsBodyEmpty())
            {
                MissingBody();
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

            if (!IsInt(result))
                return;

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

        public void Read()
        {
            if (IsPathEmpty())
            {
                MissingPath();
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

            if (!IsInt(result))
                return;

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

        public void Delete()
        {
            if (IsPathEmpty())
            {
                MissingPath();
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

            if (!IsInt(result))
                return;

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

        public void Echo()
        {
            if (IsBodyEmpty())
            {
                MissingBody();
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

        public void MissingBody()
        {
            response.status = "missing body";
            stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
        }

        public void MissingPath()
        {
            response.status = "missing resource";
            stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
        }

        public bool IsInt(string pathid)
        {
            for (var i = 0; i < pathid.Length; i++)
            {
                var isInt = char.IsDigit(pathid[i]);
                if (!isInt)
                {
                    response.status = "4 Bad Request";
                    stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
                    return false;
                }
            }
            return true;
        }
    }
}