using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPNSpot.ViewModel.DataModel
{
    public class Company
    {
        public Company(string companyStr)
        {
            this.CompanyStr = companyStr;
        }

        public string CompanyStr { get; private set; }

        readonly List<Location> _locations = new List<Location>();
        public List<Location> Locations
        {
            get { return _locations; }
        }
    }
}
