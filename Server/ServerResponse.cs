using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class ServerResponse
    {
        public string status { get; set; }
        public string body {get; set;}

        public void printing()
        {
            Console.WriteLine(status);
            Console.WriteLine(body);
        }
 
    }
}
