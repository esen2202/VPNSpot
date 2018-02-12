using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VPNSpot.Settings;
using VPNSpot.Control;
using System.Windows.Threading;
using System.Threading;
using System.Net.NetworkInformation;
using VPNCore;

namespace VPNSpot
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class StartApp : Window
    {
        double screenWidth = SystemParameters.MaximizedPrimaryScreenWidth;
        double screenHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        VPNManagement vpnManagement;

        double lastPosLeft;
        double lastPosTop;

        private void GetLocationValues()
        {
            lastPosLeft = AppSettings.Default.LocationLeft;
            lastPosTop = AppSettings.Default.LocationTop;

            if (lastPosLeft > screenWidth - 150)
            {
                lastPosLeft = screenWidth - 150;
            }
            if (lastPosLeft < 0)
            {
                lastPosLeft = 0;
            }
            if (lastPosTop > screenHeight - 250)
            {
                lastPosTop = screenHeight - 250;
            }
            SaveLocationValues();
        }

        private void SaveLocationValues()
        {
            AppSettings.Default.LocationLeft = lastPosLeft;
            AppSettings.Default.LocationTop = lastPosTop;
            AppSettings.Default.Save();
        }

        public StartApp()
        {
            InitializeComponent();
            GetLocationValues();

            this.Left = lastPosLeft;
            this.Top = lastPosTop;

            VPNConnector.OnConnectionStatusChanged += VPNConnector_OnConnectionStatusChanged;
            VPNConnector.OnStartAuthentication += VPNConnector_OnStartAuthentication;
            VPNConnector.OnConnected += VPNConnector_OnConnected;
            VPNConnector.OnDisconnected += VPNConnector_OnDisconnected;
        }

        private void VPNConnector_OnDisconnected()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                Tb_Status.Text = "Disconnected";
            });
        }

        private void VPNConnector_OnConnected()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                Tb_Status.Text = "Connected";
            });
        }

        private void VPNConnector_OnStartAuthentication()
        {
            
        }

        private void VPNConnector_OnConnectionStatusChanged()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                Tb_VpnName.Text = VPNManagement.ConnectVPNInfo.VpnName;
                Tb_ServerName.Text = VPNManagement.ConnectVPNInfo.ServerAddress;
                Tb_Status.Foreground = new SolidColorBrush(Colors.Red);
                Tb_Status.Text = VPNConnector.status != ""? VPNConnector.status : "-";
            });
        }
 
        private void Flipper_IsFlippedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {

        }

        private void PopupBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Btn_Connection_Click(object sender, RoutedEventArgs e)
        {
            if (vpnManagement == null)
            {
                vpnManagement = new VPNManagement();
                vpnManagement.Closed += delegate (System.Object o, System.EventArgs ea)
                {
                    vpnManagement = null;
                };
                vpnManagement.Show();
            }
        }

 

        private void Btn_MinimizedApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Btn_ExitApp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // close all windows 
            var windows = Application.Current.Windows;
            foreach (var item in windows)
            {
                if ((item as Window).Title.ToLower() == "mainwindow") continue;
                (item as Window).Close();
            }
            Environment.Exit(0);
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.Left > screenWidth - 150)
            {
                this.Left = screenWidth - 150;
            }
            if (this.Left < 0)
            {
                this.Left = 0;
            }
            if (this.Top > screenHeight - 250)
            {
                this.Top = screenHeight - 250;
            }

            lastPosLeft = this.Left;
            lastPosTop = this.Top;
            SaveLocationValues();
        }

        private void Btn_Settings_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface Interface in interfaces)
                {
                    if (Interface.Description == VPNManagement.VPNAdapterName && Interface.OperationalStatus == OperationalStatus.Up)
                    {
                        if ((Interface.NetworkInterfaceType == NetworkInterfaceType.Ppp) && (Interface.NetworkInterfaceType != NetworkInterfaceType.Loopback))
                        {
                            var test = Interface;
                            
                            IPv4InterfaceStatistics statistics = Interface.GetIPv4Statistics();
                            MessageBox.Show(Interface.Name + " " + Interface.NetworkInterfaceType.ToString() + " " + Interface.Description);
                        }
                        else
                        {
                            MessageBox.Show("VPN Connection is lost!");
                        }
                    }
                }
            }
        }

        private void Btn_Link_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/esen2202");
        }
    }
}
