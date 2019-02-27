using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPNModel;

namespace VPNTest
{
    class Program
    {
        static DB db;

        static void Main(string[] args)
        {
            db = new DB("VPNSpotDB");
            db.CheckDB();
            VPNobject data = new VPNobject { VpnName = "Deneme", Company="eopy", UserName ="isim", Password="pass" , Location="izmir",ServerAddress="serverAddd"};
            db.AddRecord(ref data);
        }
    }
}
