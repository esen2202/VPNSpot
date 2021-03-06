﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using VPNCore;
using VPNModel;
using VPNSpot.ViewModel;
using VPNSpot.ViewModel.DataModel;

namespace VPNSpot.Control
{

    /// <summary>
    /// Interaction logic for TestWindow2.xaml
    /// </summary>
    public partial class VPNManagement : Window
    {
        #region Definations
        public static string VPNAdapterName { get { return "EopyVPN"; } }
        public static VPNobject ConnectVPNInfo;
        public static string Status = "-";   

        public static VPNobject SelectedVPN = new VPNobject { Id = -1, VpnName = "Null" };

        private DB db = new DB("VPNSpotDB");
        private Company[] vpnlist;
        private List<string> companylist;
        private VpnCollectionViewModel viewModel;

        public static Snackbar Snackbar;

        Thread connectThread;
        public static VPNCore.VPNConnector vpnConnector;
    
        #endregion

        public VPNManagement()
        {
            InitializeComponent();
            db.CheckDB();

            TreeViewDataSourceUpdate();

            Snackbar = this.MainSnackbar;

            VPNConnector.OnConnected += VPNConnector_OnConnected;
            VPNConnector.OnDisconnected += VPNConnector_OnDisconnected;
          
            VPNConnector.OnConnectionStatusChanged += VPNConnector_OnConnectionStatusChanged;
        }



        private void TreeViewDataSourceUpdate()
        {
            companylist = db.GetCompanyList();
            vpnlist = new Company[companylist.Count];
            int i = 0;

            foreach (string item in companylist)
            {
                vpnlist[i] = new Company(item);
                i++;
            }

            viewModel = new VpnCollectionViewModel(vpnlist);
            base.DataContext = viewModel;
        }

        private void Tv_Vpn_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Tv_Vpn.SelectedItem != null && Tv_Vpn.SelectedItem is VpnViewModel)
            {
                var obj = (VpnViewModel)Tv_Vpn.SelectedValue;
                SelectedVPN = obj.VpnObj;
                lbl_Card_VpnName.Content = obj.VpnObj.VpnName;
                lbl_Card_Company.Content = obj.VpnObj.Company;
                lbl_Card_Location.Content = obj.VpnObj.Location;
                lbl_Card_ServerAddress.Content = obj.VpnObj.ServerAddress;
                lbl_Card_User.Content = obj.VpnObj.UserName;
                lbl_Card_Password.Content =
                    new Func<string>(() =>
                    {
                        string result = "";
                        foreach (var item in obj.VpnObj.Password)
                        {
                            result = result + "*";
                        }
                        return result;
                    })(); 

                LoadTextBoxFromLabels();
            }
            else
            {

            }

        }

        private void LoadTextBoxFromLabels()
        {
            txt_VpnName.Text = lbl_Card_VpnName.Content.ToString();
            txt_Company.Text = lbl_Card_Company.Content.ToString();
            txt_Location.Text = lbl_Card_Location.Content.ToString();
            txt_ServerAddress.Text = lbl_Card_ServerAddress.Content.ToString();
            txt_UserName.Text = lbl_Card_User.Content.ToString();
            txt_Password.Text = SelectedVPN.Password;
        }

        private void LoadTextBoxFromObject()
        {
            if (SelectedVPN.Id != -1) {
                txt_VpnName.Text = SelectedVPN.VpnName;
                txt_Company.Text = SelectedVPN.Company != "" ? SelectedVPN.Company : "Undefined";
                txt_Location.Text = SelectedVPN.Location != "" ? SelectedVPN.Location : "Undefined";
                txt_ServerAddress.Text = SelectedVPN.ServerAddress;
                txt_UserName.Text = SelectedVPN.UserName;
                txt_Password.Text = SelectedVPN.Password;
            }
            else
            {
                ClearTextBoxes();
            }
           
        }

        private void LoadObjectFromTextBox()
        {
            SelectedVPN.VpnName = txt_VpnName.Text;
            SelectedVPN.ServerAddress = txt_ServerAddress.Text;
            SelectedVPN.Company = txt_Company.Text;
            SelectedVPN.Location = txt_Location.Text;
            SelectedVPN.UserName = txt_UserName.Text;
            SelectedVPN.Password = txt_Password.Text;
        }

        private void LoadLabelsFromObject()
        {
            lbl_Card_VpnName.Content = SelectedVPN.VpnName;
            lbl_Card_Company.Content = SelectedVPN.Company != "" ? SelectedVPN.Company : "Undefined";
            lbl_Card_Location.Content = SelectedVPN.Location != "" ? SelectedVPN.Location : "Undefined";
            lbl_Card_ServerAddress.Content = SelectedVPN.ServerAddress;
            lbl_Card_User.Content = SelectedVPN.UserName;
            lbl_Card_Password.Content = new Func<string>(() =>
            {
                string result = "";
                foreach (var item in SelectedVPN.Password)
                {
                    result = result + "*";
                }
                return result;
            })();
        }

        #region TreeView Operations

        private void cmdExpandAll_Click(object sender, RoutedEventArgs e)
        {
            SetTreeViewItems(Tv_Vpn, true);
        }

        private void cmdCollapse_Click(object sender, RoutedEventArgs e)
        {
            SetTreeViewItems(Tv_Vpn, false);
        }

        void SetTreeViewItems(object obj, bool expand)
        {
            if (obj is TreeViewItem)
            {
                ((TreeViewItem)obj).IsExpanded = expand;
                foreach (object obj2 in ((TreeViewItem)obj).Items)
                    SetTreeViewItems(obj2, expand);
            }
            else if (obj is ItemsControl)
            {
                foreach (object obj2 in ((ItemsControl)obj).Items)
                {
                    if (obj2 != null)
                    {
                        SetTreeViewItems(((ItemsControl)obj).ItemContainerGenerator.ContainerFromItem(obj2), expand);

                        TreeViewItem item = obj2 as TreeViewItem;
                        if (item != null)
                            item.IsExpanded = expand;
                    }
                }
            }
        }

        void SelectTreeViewItem(VPNobject item)
        {
            throw new Exception();
        }

        #endregion


        #region VPNCard Operations

        private void ClearTextBoxes()
        {
            txt_VpnName.Text = "";
            txt_Company.Text = "";
            txt_Location.Text = "";
            txt_ServerAddress.Text = "";
            txt_UserName.Text = "";
            txt_Password.Text = "";
        }

        private void ClearLabels()
        {
            lbl_Card_VpnName.Content = "...";
            lbl_Card_Company.Content = "-";
            lbl_Card_Location.Content = "-";
            lbl_Card_ServerAddress.Content = "-";
            lbl_Card_User.Content = "-";
            lbl_Card_Password.Content = "-";
        }

        private bool AddNewRecord()
        {
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
                    Snackbar.MessageQueue.Enqueue("New VPN added with successful!");
                    return true;
                }
                else
                {
                    Snackbar.MessageQueue.Enqueue("the VPN name is already exist!");
                }
            }
            else
            {
                Snackbar.MessageQueue.Enqueue("VPN Name - ServerAddress - User Name - Password - Required");
            }
            return false;
        }

        private bool UpdateRecord()
        {

            if (txt_VpnName.Text != "" && txt_ServerAddress.Text != "" && txt_UserName.Text != "" && txt_Password.Text != "")
            {
                if (!db.IsThereRecord(txt_VpnName.Text, SelectedVPN.Id))
                {
                    VPNobject data = new VPNobject
                    {
                        Id = SelectedVPN.Id,
                        VpnName = txt_VpnName.Text,
                        ServerAddress = txt_ServerAddress.Text,
                        UserName = txt_UserName.Text,
                        Password = txt_Password.Text,
                        Location = txt_Location.Text != "" ? txt_Location.Text : "Undefined",
                        Company = txt_Company.Text != "" ? txt_Company.Text : "Undefined"
                    };
                    if (db.UpdateRecord(data) != -1)
                    {
                        LoadObjectFromTextBox();
                        LoadLabelsFromObject();
                        Snackbar.MessageQueue.Enqueue("The VPN updated with successful!");
                        return true;
                    }
                    else
                    {
                        Snackbar.MessageQueue.Enqueue("Unsuccesful!");
                        return false;
                    }
                }
                else
                {
                    Snackbar.MessageQueue.Enqueue("the VPN name is already exist!");
                }
            }
            else
            {
                Snackbar.MessageQueue.Enqueue("VPN Name - ServerAddress - User Name - Password - Required");
            }
            return false;

        }

        private void Btn_NewVPN_Click(object sender, RoutedEventArgs e)
        {
            FlipperCard.IsFlipped = true;
            ClearTextBoxes();
            lbl_EditNewTitle.Content = "New VPN";
            Tv_Vpn.IsEnabled = false;
        }

        private void Btn_EditVPN_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedVPN.Id != -1)
            {
                FlipperCard.IsFlipped = true;
                lbl_EditNewTitle.Content = "Edit VPN";
                LoadTextBoxFromLabels();
                Tv_Vpn.IsEnabled = false;
            }
        }

        private void Btn_DeleteVPN_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedVPN.Id != -1)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Selected VPN Delete?", SelectedVPN.VpnName, MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    if (db.DeleteRecord(SelectedVPN.Id) == 1)
                    {
                        Snackbar.MessageQueue.Enqueue("The VPN deleted with successfull!");
                        SelectedVPN.Id = -1;
                        ClearLabels();
                        TreeViewDataSourceUpdate();
                    }
                    else
                    {
                        Snackbar.MessageQueue.Enqueue("Not Delete!");
                    }
                }

            }
        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            switch (lbl_EditNewTitle.Content)
            {
                case "New VPN":
                    if (AddNewRecord())
                    {
                        FlipperCard.IsFlipped = false;
                        Tv_Vpn.IsEnabled = true;
                        TreeViewDataSourceUpdate();
                    }

                    break;
                case "Edit VPN":
                    if (UpdateRecord())
                    {
                        FlipperCard.IsFlipped = false;
                        Tv_Vpn.IsEnabled = true;
                        TreeViewDataSourceUpdate();
                    }

                    break;
                default:
                    break;
            }

        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            FlipperCard.IsFlipped = false;
            Snackbar.MessageQueue.Enqueue("Not Saved!");
            Tv_Vpn.IsEnabled = true;
            LoadTextBoxFromObject();

        }

        private void Btn_CardBackward_Click(object sender, RoutedEventArgs e)
        {
            FlipperCard.IsFlipped = false;
            Snackbar.MessageQueue.Enqueue("Not Saved!");
            Tv_Vpn.IsEnabled = true;
            LoadTextBoxFromObject();
        }

        #endregion


        #region Windows Control Buttons

        private void bd_WindowMainTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btn_help_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_min_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_max_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        #endregion


        #region VPN Connection

        private void Btn_ConnectVPN_Click(object sender, RoutedEventArgs e)
        {
            if (vpnConnector != null)
            {
                if (vpnConnector.IsActive)
                {
                    vpnConnector.DisconnectDotras();
                }
                else
                {
                    ConnectVPN();
                }
            }
            else
            {
                ConnectVPN();
            }


        }

        void ConnectVPN()
        {
            if (txt_ServerAddress.Text != "" && txt_UserName.Text != "" && txt_Password.Text != "")
            {
                string serverAddress = txt_ServerAddress.Text;
                string userName = txt_UserName.Text;
                string password = txt_Password.Text;
                ConnectVPNInfo = new VPNobject { ServerAddress = serverAddress, UserName = userName, Password = password, VpnName = txt_VpnName.Text == "" ? "-" : txt_VpnName.Text };
                
                ThreadStart connect = delegate { ConnectThread(serverAddress, VPNAdapterName, userName, password); };

                connectThread = new Thread(connect)
                {
                    IsBackground = true
                };
 
                connectThread.Start();

          
            }
            else
            {
                Snackbar.MessageQueue.Enqueue("ServerAddress - User Name - Password - Required");
            }
        }


        private static void ConnectThread(string ServerAddress, string VpnName, string UserName, string Password)
        {
            vpnConnector = new VPNCore.VPNConnector(ServerAddress, VpnName, UserName, Password);
            //if (vpnConnector.IsActive)
            //{
            //    vpnConnector.TryDisconnect();

            //}
            vpnConnector.CreateOrUpdate();

            vpnConnector.ConnectDotras();

        }

        #region Dotras

        private void VPNConnector_OnDisconnected()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                Status = "Not Connected!";
                Snackbar.MessageQueue.Enqueue(Status);
                Btn_ConnectVPN.Content = "Connect";
                Btn_ConnectVPN.IsEnabled = true;
            });
        }

        private void VPNConnector_OnConnected()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                Status = "Connected :)";
                Snackbar.MessageQueue.Enqueue(Status);
                Btn_ConnectVPN.Content = "Disconnect";
                Btn_ConnectVPN.IsEnabled = true;
            });
        }

        private void VPNConnector_OnConnectionStatusChanged()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                Btn_ConnectVPN.IsEnabled = false;
                Btn_ConnectVPN.Content = VPNConnector.status;
            });
        }
        #endregion
        #endregion
    }
}
