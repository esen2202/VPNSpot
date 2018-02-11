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
using VPNSpot.Settings;
using VPNSpot.Test;

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

            OnConnecting
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

        private void Btn_EopyLink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.eopy.com.tr/en");
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
    }
}
