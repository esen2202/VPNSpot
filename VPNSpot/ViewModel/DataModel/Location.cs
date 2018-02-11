using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPNSpot.ViewModel.DataModel
{
    public class Location
    {
        public Location(string locationStr)
        {
            this.LocationStr = locationStr;
        }
        public string LocationStr { get; private set; }

        readonly List<Vpn> _vpns = new List<Vpn>();
        public List<Vpn> Vpns
        {
            get { return _vpns; }
        }


    }
}
