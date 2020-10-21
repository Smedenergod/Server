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
                WriteResponse("missing method");
            }

            if (string.IsNullOrEmpty(date))
            {
                WriteResponse("missing date");
            }

            try
            {
                var testing = int.Parse(date);
            }
            catch (Exception E)
            {
                WriteResponse("illegal date");
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
                    WriteResponse("illegal method", true);
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
                WriteResponse("4 bad request", true);
                return;
            }

            var categories = JsonSerializer.Deserialize<Categories>(body);
            Helper.categoryList.Add(categories);
            categories.cid = Helper.categoryList.Count;

            WriteResponse("2 created", JsonSerializer.Serialize(categories), true);
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
                WriteResponse("illegal body", true);
                return;
            }

            if (path.EndsWith("categories"))
            {
                WriteResponse("4 Bad Request", true);
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
                WriteResponse("3 updated", JsonSerializer.Serialize(category), true);
            }
            else
            {
                WriteResponse("5 not found", true);
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
                WriteResponse("1 Ok", JsonSerializer.Serialize(Helper.categoryList), true);
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
                WriteResponse("1 Ok", JsonSerializer.Serialize(categories), true);
            }
            else
            {
                WriteResponse("5 not found", true);
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
                WriteResponse("4 Bad Request", true);
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
                WriteResponse("1 Ok", true);
            }
            else
            {
                WriteResponse("5 Not Found", true);
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
            WriteResponse("missing body", true);
        }

        public void MissingPath()
        {
            WriteResponse("missing resource", true);
        }

        public bool IsInt(string pathid)
        {
            for (var i = 0; i < pathid.Length; i++)
            {
                var isInt = char.IsDigit(pathid[i]);
                if (!isInt)
                {
                    WriteResponse("4 Bad Request", true);
                    return false;
                }
            }
            return true;
        }

        public void WriteResponse(string status, string body, bool shouldWrite = false)
        {
            if (!String.IsNullOrEmpty(response.status))
                response.status = String.Concat(response.status, status);
            else
                response.status = status;
            response.body = body;

            if(shouldWrite)
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
            return;
        }

        public void WriteResponse(string status, bool shouldWrite = false)
        {
            if (!String.IsNullOrEmpty(response.status))
                response.status = String.Concat(response.status, status);
            else
                response.status = status;
            if(shouldWrite)
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(response));
            return;
        }
    }
}