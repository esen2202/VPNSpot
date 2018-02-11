using VPNModel;

namespace VPNSpot.ViewModel.DataModel
{
    public class Vpn
    {
        public Vpn(VPNobject vpnObj)
        {
            this.VpnObj = vpnObj;
        }

        public VPNobject VpnObj { get; private set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }

}


