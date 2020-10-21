using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace Server
{
    internal class Request
    {
        public string method { get; set; }
        public string path { get; set; }
        public string date { get; set; }
        public string body { get; set; }
        public Stream Stream { get; set; }
        private ServerResponse Response;

        public void run()
        {
            Response = new ServerResponse();
            simpleValidate();
            requestPath();
        }
        private void simpleValidate()
        {
            if (string.IsNullOrEmpty(method))
                writeResponse("missing method");
            if (string.IsNullOrEmpty(date))
                writeResponse("missing date");
            else
                 if (!isInt(date))
                    writeResponse("illegal date");
            if (!string.IsNullOrEmpty(Response.status))
                Stream.Write(JsonSerializer.SerializeToUtf8Bytes(Response));
        }
        private void requestPath()
        {
            switch (method)
            {
                case "create":
                    create();
                    break;
                case "delete":
                    delete();
                    break;
                case "read":
                    read();
                    break;
                case "update":
                    update();
                    break;
                case "echo":
                    echo();
                    break;
                default:
                    writeResponse("illegal method", true);
                    break;
            }
        }
        private void create()
        {
            if (isPathEmpty())
            {
                missingPath();
                return;
            }
            if (isBodyEmpty())
            {
                missingBody();
                return;
            }
            if (!path.EndsWith("categories"))
            {
                writeResponse("4 bad request", true);
                return;
            }
            var categories = JsonSerializer.Deserialize<Categories>(body);
            categories.cid = Helper.categoryList.Count;
            Helper.categoryList.Add(categories);
            writeResponse("2 created", JsonSerializer.Serialize(categories), true);
        }

        private void update()
        {
            if (isPathEmpty())
            {
                missingPath();
                return;
            }
            if (isBodyEmpty())
            {
                missingBody();
                return;
            }
            if (!body.StartsWith("{"))
            {
                writeResponse("illegal body", true);
                return;
            }
            if (path.EndsWith("categories"))
            {
                writeResponse("4 Bad Request", true);
                return;
            }
            var lastElement = getLastElement();
            if (!isInt(lastElement))
            {
                writeResponse("4 Bad Request", true);
                return;
            }
            var pathId = int.Parse(lastElement);
            if (pathId <= Helper.categoryList.Count)
            {
                var category = Helper.categoryList[pathId - 1];
                var newCategory = JsonSerializer.Deserialize<Categories>(body);
                category.name = newCategory.name;
                writeResponse("3 updated", JsonSerializer.Serialize(category), true);
            }
            else
                writeResponse("5 not found", true);
        }

        private void read()
        {
            if (isPathEmpty())
            {
                missingPath();
                return;
            }
            if (path.Equals("/api/categories"))
            {
                writeResponse("1 Ok", JsonSerializer.Serialize(Helper.categoryList), true);
                return;
            }
            var lastElement = getLastElement();
            if (!isInt(lastElement))
            {
                writeResponse("4 Bad Request", true);
                return;
            }
            var pathId = int.Parse(lastElement);
            if (pathId <= Helper.categoryList.Count)
            {
                var categories = Helper.categoryList[pathId - 1];
                writeResponse("1 Ok", JsonSerializer.Serialize(categories), true);
            }
            else
                writeResponse("5 not found", true);
        }

        private void delete()
        {
            if (isPathEmpty())
            {
                missingPath();
                return;
            }
            if (path.EndsWith("categories"))
            {
                writeResponse("4 Bad Request", true);
                return;
            }
            var lastElement = getLastElement();
            if (!isInt(lastElement))
            {
                writeResponse("4 Bad Request", true);
                return;
            }
            var pathId = int.Parse(lastElement);
            if (pathId <= Helper.categoryList.Count)
            {
                Helper.categoryList.RemoveAt(pathId);
                writeResponse("1 Ok", true);
            }
            else
                writeResponse("5 Not Found", true);
        }

        private void echo()
        {
            if (isBodyEmpty())
                missingBody();
            else
            {
                Response.body = body;
                Stream.Write(JsonSerializer.SerializeToUtf8Bytes(Response));
            }
        }

        private bool isPathEmpty()
        {
            return string.IsNullOrEmpty(path);
        }

        private bool isBodyEmpty()
        {
            return string.IsNullOrEmpty(body);
        }

        private void missingBody()
        {
            writeResponse("missing body", true);
        }

        private void missingPath()
        {
            writeResponse("missing resource", true);
        }

        private bool isInt(string pathid)
        {
            foreach (var t in pathid)
            {
                var isInt = char.IsDigit(t);
                if (!isInt)
                {
                    return false;
                }
            }
            return true;
        }

        private void writeResponse(string status, string body, bool shouldWrite = false)
        {
            Response.status = !String.IsNullOrEmpty(Response.status) ? String.Concat(Response.status, status) : status;
            Response.body = body;
            if(shouldWrite)
                Stream.Write(JsonSerializer.SerializeToUtf8Bytes(Response));
        }

        private string getLastElement()
        {
            var elements = path.Split("/");
            return elements[^1];
        }

        private void writeResponse(string status, bool shouldWrite = false)
        {
            Response.status = !String.IsNullOrEmpty(Response.status) ? String.Concat(Response.status, status) : status;
            if(shouldWrite)
                Stream.Write(JsonSerializer.SerializeToUtf8Bytes(Response));
        }
    }
}