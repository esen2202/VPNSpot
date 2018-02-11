using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VPNModel;

namespace VPNSpot
{
    /// <summary>
    /// Interaction logic for AddConnection.xaml
    /// </summary>
    public partial class AddConnection : Window
    {

        public AddConnection()
        {
            InitializeComponent();
        }

        DB db = new DB("VPNSpotDB");
        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            db.CheckDB();
            if (txt_VpnName.Text != "" && txt_ServerAddress.Text != "" && txt_UserName.Text != "" && txt_Password.Text != "")
            {
                if (!db.IsThereRecord(txt_VpnName.Text))
                {
                    VPNobject data = new VPNobject
                    {
                        VpnName = txt_VpnName.Text,
                        ServerAddress = txt_ServerAddress.Text,
                        UserName = txt_UserName.Text,
                        Password = txt_Password.Text,
                        Location = txt_Location.Text != "" ? txt_Location.Text : "Undefined",
                        Company = txt_Company.Text != "" ? txt_Company.Text : "Undefined"
                    };
                    db.AddRecord(ref data);
                }
                else
                {
                    MessageBox.Show("VPN Name Zaten Var");
                }
            }
            else
            {
                MessageBox.Show("Boş Alan Bırakma");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lb_Locations.Items.Clear();
            var list = db.GetLocationList();
            foreach (string item in list)
            {
                lb_Locations.Items.Add(item);
            }

            lb_Companies.Items.Clear();
            list = db.GetCompanyList();
            foreach (string item in list)
            {
                lb_Companies.Items.Add(item);
            }
        }

        private void lb_Companies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lb_Companies.SelectedItem != null)
            {
                List<VPNobject> list = new List<VPNobject>();
                list = db.GetVPNsByCompany(lb_Companies.SelectedValue.ToString());
                lb_VPNs.ItemsSource = list;
            }
        }

        private void lb_Locations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb_Locations.SelectedItem != null)
            {
                List<VPNobject> list = new List<VPNobject>();
                list = db.GetVPNsByLocation(lb_Locations.SelectedValue.ToString());
                lb_VPNs.ItemsSource = list;
            }
        }

        private void lb_VPNs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb_VPNs.SelectedItem != null)
            {
                VPNobject item = (VPNobject)lb_VPNs.SelectedItem;

                txt_VpnName.Text = item.VpnName;
                txt_ServerAddress.Text = item.ServerAddress;
                txt_UserName.Text = item.UserName;
                txt_Password.Text = item.Password;
                txt_Company.Text = item.Company;
                txt_Location.Text = item.Location;
            }
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if(lb_VPNs.SelectedItem != null)
            {
                VPNobject item = (VPNobject)lb_VPNs.SelectedItem;
                if (db.DeleteRecord(item.Id) == 1)
                {
                    List<VPNobject> items = (List<VPNobject>)lb_VPNs.ItemsSource;
                    items.Remove(item);
                    lb_VPNs.ItemsSource = items;
                }
            }
         
        }
    }
}
