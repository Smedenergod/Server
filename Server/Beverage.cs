using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Server
{
    class Beverage
    {
        private int cid { get; set; }
        private string name { get; set; }

        public Beverage(int cid, string name)
        {
            this.cid = cid;
            this.name = name;
        }

    }
}
