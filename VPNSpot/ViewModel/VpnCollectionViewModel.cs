using System.Collections.ObjectModel;
using System.Linq;
using VPNSpot.ViewModel.DataModel;

namespace VPNSpot.ViewModel
{
    public class VpnCollectionViewModel
    {
        readonly ReadOnlyCollection<CompanyViewModel> _companies;

        public VpnCollectionViewModel(Company[] companies)
        {
            _companies = new ReadOnlyCollection<CompanyViewModel>(
                (from company in companies
                 select new CompanyViewModel(company))
                .ToList());
        }

        public ReadOnlyCollection<CompanyViewModel> Companies
        {
            get { return _companies; }
        }
    }
}
