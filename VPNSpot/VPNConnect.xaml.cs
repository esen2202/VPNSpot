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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VPNModel;
using VPNSpot.Settings;
using VPNSpot.Test;

namespace VPNSpot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class VPNConnect : Window
    {
        //! Dependency Properties
        #region [Dependency Properties]
        public string StatusText { get { return (string)GetValue(StatusTextProperty); } set { SetValue(StatusTextProperty, value); } }
        public static readonly DependencyProperty StatusTextProperty = DependencyProperty.Register("StatusText", typeof(string), typeof(VPNConnect), new UIPropertyMetadata("Status"));

        #endregion

        private string VPNName { get; set; }
        private string VPNServerName { get; set; }
        private string UserName { get; set; }
        private string Password { get; set; }
        private VPNCore.VPNConnector vpnConnector;

        public VPNConnect()
        {
            LastSession.Default.VPNName = "VPNSpot";
            LastSession.Default.Save();

            InitializeComponent();
        }

        private void SaveLastSession()
        {
            LastSession.Default.VPNName = "VPNSpot";
            LastSession.Default.VPNServerName = txt_ServerAddress.Text;
            LastSession.Default.UserName = txt_UserName.Text;
            LastSession.Default.Password = txt_Password.Text;
            LastSession.Default.Save();
        }

        private void GetLastSession()
        {
            VPNName = LastSession.Default.VPNName.ToString();
            VPNServerName = LastSession.Default.VPNServerName.ToString();
            UserName = LastSession.Default.UserName.ToString();
            Password = LastSession.Default.Password.ToString();

            txt_ServerAddress.Text = VPNServerName;
            txt_UserName.Text = UserName;
            txt_Password.Text = Password;
        }

        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (txt_ServerAddress.Text != null && txt_ServerAddress.Text != "")
            {
                vpnConnector = new VPNCore.VPNConnector(txt_ServerAddress.Text, VPNName, txt_UserName.Text, txt_Password.Text);
                vpnConnector.CreateOrUpdate();

                vpnConnector.TryConnect();

                if (vpnConnector.IsActive)
                {
                    StatusText = "Connected";
                    SaveLastSession();
                }
                else
                {
                    StatusText = "Not Connected!!!";
                }
            }
            else
            {
                StatusText = "Server Name field must be not null";
            }

        }

        private void btn_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            if (vpnConnector != null)
            {
                if (vpnConnector.IsActive)
                {
                    vpnConnector.TryDisconnect();
                    if (!vpnConnector.IsActive)
                    {
                        StatusText = "Connection Closed";
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetLastSession();
        }


       
        private void Btn_AddConnection_Click(object sender, RoutedEventArgs e)
        {

            AddConnection addConnWin = new AddConnection();
            addConnWin.Show();

        }

        private void Btn_Test_Click(object sender, RoutedEventArgs e)
        {
            StartApp test = new StartApp();
            test.Show();
        }

        private void Btn_Test2_Click(object sender, RoutedEventArgs e)
        {
            VPNManagement test2 = new VPNManagement();
            test2.Show();
        }
    }
}
