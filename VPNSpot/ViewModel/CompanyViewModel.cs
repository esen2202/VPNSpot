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
    public class CompanyViewModel : TreeViewItemViewModel
    {
        readonly Company _company;

        public CompanyViewModel(Company company)
            : base(null, true)
        {
            _company = company;
        }

        public string CompanyStr
        {
            get { return _company.CompanyStr; }
        }

        protected override void LoadChildren()
        {

            foreach (var companyStr in DBStatic.db.GetLocationList(_company.CompanyStr))
                base.Children.Add(new LocationViewModel(new Location(companyStr), this));
        }
    }
}
