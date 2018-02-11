using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPNModel;
using VPNSpot.LoadOnDemand;
using VPNSpot.ViewModel.DataModel;

namespace VPNSpot.ViewModel
{
    public class LocationViewModel : TreeViewItemViewModel
    {
        readonly Location _location;
        private string _parentCompanyStr;
        public LocationViewModel(Location location, CompanyViewModel parentCompany)
            : base(parentCompany, true)
        {
            _location = location;
            _parentCompanyStr = parentCompany.CompanyStr;
        }

        public string LocationStr
        {
            get { return _location.LocationStr; }
        }

        protected override void LoadChildren()
        {
              foreach (var vpnObj in DBStatic.db.ListVpnRecords(_parentCompanyStr, _location.LocationStr))
                base.Children.Add(new VpnViewModel(new Vpn(vpnObj), this));
        }
    }
}
