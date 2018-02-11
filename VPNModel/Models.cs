using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPNModel
{
    public class VPNobject
    {
        public long Id { get; set; }
        public string VpnName { get; set; }
        public string ServerAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }

        public override string ToString()
        {
            return "[" + VpnName + "] " + ServerAddress + "."  + Location + "." + Company;
        }
    }
}
