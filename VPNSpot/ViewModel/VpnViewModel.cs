using VPNModel;
using VPNSpot.LoadOnDemand;
using VPNSpot.ViewModel.DataModel;

namespace VPNSpot.ViewModel
{
    public class VpnViewModel : TreeViewItemViewModel
    {
        readonly Vpn _vpn;

        public VpnViewModel(Vpn vpn, LocationViewModel parentLocation)
            : base(parentLocation, false)
        {
            _vpn = vpn;
        }

        public VPNobject VpnObj
        {
            get { return _vpn.VpnObj; }
        }
    }
}
