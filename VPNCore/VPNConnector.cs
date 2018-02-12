using DotRas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VPNCore
{
    public delegate void ConnectionStateChangedHandler();

    public class VPNConnector
    {
        public static event ConnectionStateChangedHandler OnConnectionStatusChanged;
        public static event ConnectionStateChangedHandler OnStartAuthentication;
        public static event ConnectionStateChangedHandler OnConnected;
        public static event ConnectionStateChangedHandler OnDisconnected;


        public VPNConnector(string serverAddress, string userName, string passWord)
           : this(serverAddress, serverAddress, userName, passWord, Protocol.SSTP)
        {
            CreateDotrasEvents();
        }
        public VPNConnector(string serverAddress, string connectionName, string userName, string passWord)
            : this(serverAddress, connectionName, userName, passWord, Protocol.SSTP)
        {
            CreateDotrasEvents();
        }

        public VPNConnector(string serverAddress, string connectionName, string userName, string passWord, Protocol protocol)
        {
            this.serverAddress = serverAddress;
            this.connectionName = connectionName;
            this.userName = userName;
            this.passWord = passWord;
            this.protocol = protocol;
            this.rasDialFileName = Path.Combine(WinDir, "rasdial.exe");

            CreateDotrasEvents();
        }

        private static string WinDir = Environment.GetFolderPath(Environment.SpecialFolder.System);
        private string rasDialFileName;
        private static RasDialer dialer = new RasDialer();
        RasConnectionWatcher watcher = new RasConnectionWatcher();

        public static string status;

        public string RasDialFileName
        {
            get { return rasDialFileName; }
            set
            {
                if (File.Exists(value))
                {
                    rasDialFileName = value;
                }

                throw new FileNotFoundException();
            }
        }

        private readonly string serverAddress;
        private readonly string connectionName;
        private readonly string userName;
        private readonly string passWord;
        private readonly Protocol protocol;
        private readonly static string allUserPhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
        public bool IsActive
        {
            get
            {
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.StartInfo.UseShellExecute = false;
                    myProcess.StartInfo.RedirectStandardInput = true;
                    myProcess.StartInfo.RedirectStandardOutput = true;
                    myProcess.StartInfo.FileName = "cmd.exe";
                    myProcess.Start();
                    myProcess.StandardInput.WriteLine("ipconfig");
                    myProcess.StandardInput.WriteLine("exit");
                    myProcess.WaitForExit();

                    string content = myProcess.StandardOutput.ReadToEnd();
                    if (content.Contains("0.0.0.0"))
                    {
                        return true;
                    }

                    return false;
                }

            }
        }

        public RasDevice RasDevice
        {
            get
            {
                var name = Enum.GetName(typeof(Protocol), this.protocol);
                var rasDevice = RasDevice.GetDevices().FirstOrDefault(c => c.Name.Contains(name));
                if (rasDevice == null)
                {
                    throw new Exception("No device found.");
                }
                return rasDevice;
            }
        }

        public RasVpnStrategy RasVpnStrategy
        {
            get
            {
                if (protocol == Protocol.SSTP)
                {
                    return RasVpnStrategy.SstpFirst;
                }
                else
                {
                    return RasVpnStrategy.IkeV2First;
                }
            }
        }

        public bool WaitUntilActive(int timeOut = 10)
        {
            for (int i = 0; i < timeOut; i++)
            {
                if (!this.IsActive)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public bool WaitUntilInActive(int timeOut = 10)
        {
            for (int i = 0; i < timeOut; i++)
            {
                if (this.IsActive)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    return true;
                }
            }

            return false;
        }


        public bool TryConnect()
        {
            try
            {
                string args = $"{connectionName} {userName} {passWord}";
                ProcessStartInfo myProcess = new ProcessStartInfo(rasDialFileName, args);
                myProcess.CreateNoWindow = true;
                myProcess.UseShellExecute = false;
                var proc = Process.Start(myProcess);
                //proc.WaitForExit();              
                //string result;
                //switch (proc.ExitCode)
                //{
                //    case 0: //connection succeeded
                //        result = "Connected";
                //        break;
                //    case 691: //wrong credentials
                //        result = "wrong credentials";
                //        break;
                //    case 623: // The VPN doesn't excist
                //        result = "The VPN doesn't excist";
                //        break;
                //    case 868: //the IP or domainname can't be found
                //        result = "the IP or domainname can't be found";
                //        break;
                //    default: //other faults
                //        result = "other faults : " + proc.ExitCode.ToString();
                //        break;
                //}
            }
            catch (Exception Ex)
            {
                Debug.Assert(false, Ex.ToString());
            }

            WaitUntilActive();
            if (IsActive)
            {
                return true;
            }

            return false;
        }

        public bool TryDisconnect()
        {
            try
            {
                string args = $@"{connectionName} /d";
                ProcessStartInfo myProcess = new ProcessStartInfo(rasDialFileName, args);
                myProcess.CreateNoWindow = true;
                myProcess.UseShellExecute = false;
                Process.Start(myProcess);
            }
            catch (Exception Ex)
            {
                Debug.Assert(false, Ex.ToString());
            }

            WaitUntilInActive();
            if (!IsActive)
            {
                return true;
            }

            return false;
        }

        public void CreateOrUpdate()
        {

            using (var allUsersPhoneBook = new RasPhoneBook())
            {
                allUsersPhoneBook.Open(true);
                if (allUsersPhoneBook.Entries.Contains(connectionName))
                {
                    allUsersPhoneBook.Entries[connectionName].PhoneNumber = serverAddress;
                    allUsersPhoneBook.Entries[connectionName].VpnStrategy = RasVpnStrategy;
                    allUsersPhoneBook.Entries[connectionName].Device = RasDevice;
                    allUsersPhoneBook.Entries[connectionName].Update();
                }
                else
                {
                    RasEntry entry = RasEntry.CreateVpnEntry(connectionName, serverAddress, RasVpnStrategy, RasDevice);
                    allUsersPhoneBook.Entries.Add(entry);
                    dialer.EntryName = connectionName;
                    dialer.PhoneBookPath = allUserPhoneBookPath;
                }
            }
        }

        public void TryDelete()
        {
            using (var dialer = new RasDialer())
            using (var allUsersPhoneBook = new RasPhoneBook())
            {
                allUsersPhoneBook.Open(true);
                if (allUsersPhoneBook.Entries.Contains(connectionName))
                {
                    TryDisconnect();
                    WaitUntilInActive();
                    allUsersPhoneBook.Entries.Remove(connectionName);
                }
            }
        }

        #region Dotras

        private void CreateDotrasEvents()
        {
            dialer.StateChanged += Dialer_StateChanged;
            dialer.DialCompleted += Dialer_DialCompleted;
            watcher.Connected += new EventHandler<RasConnectionEventArgs>(this.watcher_Connected);
            watcher.Disconnected += new EventHandler<RasConnectionEventArgs>(this.watcher_Disconnected);
            watcher.EnableRaisingEvents = true;
        }

        private void watcher_Connected(object sender, RasConnectionEventArgs e)
        {
            OnConnected?.Invoke();
        }
        private void watcher_Disconnected(object sender, RasConnectionEventArgs e)
        {
            OnDisconnected?.Invoke();
        }

        private void Dialer_DialCompleted(object sender, DialCompletedEventArgs e)
        {
            string message = "";


            if (e.Cancelled)
            {
                message = "Cancelled";
            }
            else if (e.TimedOut)
            {
                message = "Timeout";
            }
            else if (e.Connected)
            {
                message = "Connection successful";
                OnConnected?.Invoke();
            }
            else if (e.Error != null)
            {
                message = e.Error.ToString();
            }

            if (!e.Connected)
            {
                OnDisconnected?.Invoke();
                // Not Connected
            }
        }

        private void Dialer_StateChanged(object sender, StateChangedEventArgs e)
        {
            status = e.State.ToString();
            OnConnectionStatusChanged?.Invoke();
        }

        private RasHandle handle = null;
        public bool ConnectDotras()
        {
            using (var allUsersPhoneBook = new RasPhoneBook())
            {
                allUsersPhoneBook.Open(true);
                dialer.PhoneBookPath = allUsersPhoneBook.Path;
            }
            dialer.EntryName = connectionName;


            try
            {
                dialer.Credentials = new NetworkCredential(userName, passWord);
                this.handle = dialer.DialAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool DisconnectDotras()
        {
            if (dialer.IsBusy)
            {
                dialer.DialAsyncCancel();
            }
            else
            {
                RasConnection connection = RasConnection.GetActiveConnectionByHandle(handle);
                if (connection != null)
                {
                    connection.HangUp();
                }
            }

            return true;
        }

        #endregion
    }
}
