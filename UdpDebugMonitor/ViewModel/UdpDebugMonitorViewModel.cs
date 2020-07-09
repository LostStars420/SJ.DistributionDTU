using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using UdpDebugMonitor.Model;
using Common;
using Net.UDP;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace UdpDebugMonitor.ViewModel
{
    public class UdpDebugMonitorViewModel : ViewModelBase
    {
        private PlatformModelServer modelServer;
        ReciveIp _locaReciveAll;
        Thread _currentThread;
        Dispatcher _showDispatcher;
        public UdpDebugMonitorViewModel()
        {
            Messenger.Default.Register<Dispatcher>(this, "Dispatcher", GetDispatcher);
            Messenger.Default.Send<Dispatcher>(null, "GetDispatcher");

            LoadDataCommand = new RelayCommand(ExecuteLoadDataCommand);
            UnLoadedLoadDataCommand = new RelayCommand(ExecuteUnLoadedLoadDataCommand);
            ClearText = new RelayCommand<string>(ExecuteClearText);
            CmdOperate = new RelayCommand<string>(ExecuteCmdOperate);
            Messenger.Default.Register<Tuple<string, int>>(this, "UpdateIPPort", UpdateIPPort);

            var ipArray = Dns.GetHostAddresses(Dns.GetHostName());
            //var localIp = ipArray.First(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            var lip= ipArray.First(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            CombboxList = new List<IPAddress>();
            for (int i = 0; i < ipArray.Length; i++)
            {
                if(ipArray[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    CombboxList.Add(ipArray[i]);
                }
            }

           // _localIPEndPoint = new IPEndPoint(localIp, 5533);
            _remoteIPEndPoint = new IPEndPoint(lip, 5533);
            UserData = new ObservableCollection<ReciveIp>();
            //添加作为本地节点即
            _locaReciveAll = new ReciveIp(new IPEndPoint(lip, 5533));
            UserData.Add(_locaReciveAll);

            keyData = new StringBuilder();

        }


        private void GetDispatcher(Dispatcher obj)
        {
            _showDispatcher = obj;
        }

        void ExceptionAction(Exception ex)
        {
            Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
        }


        #region 下拉框相关
        private IPAddress combboxItem;
        /// <summary>
        /// 下拉框选中信息
        /// </summary>
        public IPAddress CombboxItem
        {
            get { return combboxItem; }
            set { combboxItem = value; RaisePropertyChanged(() => CombboxItem); }
        }

        private List<IPAddress> combboxList;
        /// <summary>
        /// 下拉框列表
        /// </summary>
        public List<IPAddress> CombboxList
        {
            get { return combboxList; }
            set { combboxList = value; RaisePropertyChanged(() => CombboxList); }
        }
        #endregion



        #region 加载数据命令：LoadDataCommand
        /// <summary>
        /// 加载数据
        /// </summary>
        public RelayCommand LoadDataCommand { get; private set; }

        //加载用户数据
        void ExecuteLoadDataCommand()
        {

            if (modelServer == null)
            {
                modelServer = PlatformModelServer.GetServer();
                ConnectEnable = true;

                SelectedIndex = 0;
                _currentThread = Thread.CurrentThread;
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public RelayCommand UnLoadedLoadDataCommand { get; private set; }

        //加载用户数据
        void ExecuteUnLoadedLoadDataCommand()
        {


        }
        #endregion


        private ObservableCollection<ReciveIp> _userData;
        /// <summary>
        /// 用户信息数据
        /// </summary>
        public ObservableCollection<ReciveIp> UserData
        {
            get { return _userData; }
            set
            {
                _userData = value;
                RaisePropertyChanged("UserData");
            }
        }

        private int _selectedIndex;
        /// <summary>
        /// 选择索引
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value< 0 ? 0:value;
                RaisePropertyChanged("SelectedIndex");
                RaisePropertyChanged("Message");
                RaisePropertyChanged("SelectedIP");
                RaisePropertyChanged("SelectedPort");
            }
        }


        /// <summary>
        /// 显示信息
        /// </summary>
        public string Message
        {
            get
            {
                return UserData[_selectedIndex].ReicveCharacter;
            }
            set
            {
                RaisePropertyChanged("Message");
            }
        }


        private String sendMessage;

        public String SendMessage
        {
            get
            {
                return sendMessage;
            }
            set
            {
                sendMessage = value;
                RaisePropertyChanged(() => SendMessage);
            }
        }

        #region udpparamter


        IPEndPoint _localIPEndPoint;

        private IPAddress localIp;
        /// <summary>
        /// 显示信息
        /// </summary>
        public IPAddress LocalIP
        {
            get
            {
                return localIp;//_localIPEndPoint.Address.ToString();
            }
            set
            {
                var ip = new byte[6];
                bool state = ChoicePhase.PlatformModel.Helper.TypeConvert.IpTryToInt(value.ToString(), out ip);
                if (state)
                {
                    localIp = combboxItem;
                   // _localIPEndPoint.Address = new IPAddress(ip);
                    RaisePropertyChanged("LocalIP");
                }
            }
        }

        private int localPort;
        public int LocalPort
        {
            get
            {
                return localPort;
                //return _localIPEndPoint.Port;
            }
            set
            {
                localPort = value;
                //_localIPEndPoint.Port = value;
                RaisePropertyChanged("LocalPort");
            }
        }


        IPEndPoint _remoteIPEndPoint;

        
        /// <summary>
        /// 显示信息
        /// </summary>
        public string RemoteIP//string
        {
            get
            {
                return _remoteIPEndPoint.Address.ToString();
            }
            set
            {
                var ip = new byte[6];
                bool state = ChoicePhase.PlatformModel.Helper.TypeConvert.IpTryToInt(value, out ip);
                if (state)
                {
                    _remoteIPEndPoint.Address = new IPAddress(ip);
                    RaisePropertyChanged("RemoteIP");
                }
            }
        }

        public int RemotePort
        {
            get
            {
                return _remoteIPEndPoint.Port;
            }
            set
            {
                _remoteIPEndPoint.Port = value;
                RaisePropertyChanged("RemotePort");
            }
        }

        #region 10.23日新加的
        private string selectedIP;
        /// <summary>
        /// 选择的IP地址
        /// </summary>
        public string SelectedIP
        {
            get
            {
                return selectedIP;//_userData[_selectedIndex].IP.ToString();
            }
            set
            {
                selectedIP = value;//_userData[_selectedIndex].IP.ToString();
                RaisePropertyChanged(() => SelectedIP);
            }
        }

        private int selectedPort;
        /// <summary>
        /// 选择的端口
        /// </summary>
        public int SelectedPort
        {
            get
            {
                return UserData[_selectedIndex].Port;
            }
            set
            {
                selectedPort = value;
                RaisePropertyChanged(() => SelectedPort);
            }
        }

      

        private void UpdateIPPort(Tuple<string, int> obj)
        {
            selectedIP = obj.Item1;
            selectedPort = obj.Item2;
            RaisePropertyChanged("SelectedIP");
            RaisePropertyChanged("SelectedPort");
        }


   

        #endregion

        private bool _connectEnable;
        public bool ConnectEnable
        {
            get
            {
                return _connectEnable;
            }
            set
            {
                _connectEnable = value;
                RaisePropertyChanged("ConnectEnable");
                RaisePropertyChanged("DisconnectEnable");
            }
        }

        public bool DisconnectEnable
        {
            get
            {
                return !_connectEnable;
            }
            set
            {
                _connectEnable = !value;
                RaisePropertyChanged("ConnectEnable");
                RaisePropertyChanged("DisconnectEnable");
            }
        }
        #endregion


        public RelayCommand<string> CmdOperate { get; private set; }

        void ExecuteCmdOperate(string str)
        {
            try
            {
                switch (str)
                {
                    case "Connect":
                        {
                            if (CombboxItem == null)
                            {
                                MessageBox.Show("请选择IP地址");
                                return;
                            }
                            _localIPEndPoint = new IPEndPoint(CombboxItem, 5533);
                            
                            modelServer.CommServer.OpenUdpMonitor(_localIPEndPoint, ExceptionAction);
                            modelServer.CommServer.UdpMonitorServer.DataArrived += _udpServer_DataArrived;
                            ConnectEnable = false;
                            break;
                        }
                    case "Disconnect":
                        {
                            modelServer.CommServer.Close();
                            DisconnectEnable = false;
                            break;
                        }
                    case "Send":
                        {
                            var ip = IPAddress.Parse(selectedIP);//_userData[_selectedIndex].CurrentEndpoint.Address;
                            var port = selectedPort;//_userData[_selectedIndex].CurrentEndpoint.Port;
                            byte[] data = System.Text.Encoding.Default.GetBytes(sendMessage);
                            modelServer.CommServer.UdpMonitorServer.Send(ip, port, data);
                            break;
                        }
                    case "SaveAll":
                        {
                            SaveData(_userData);
                            break;
                        }
                    case "ClearAll":
                        {
                            ClearAll(_userData);
                            RaisePropertyChanged("Message");
                            break;
                        }
                    default:
                        {
                            throw new Exception("未识别的命令");
                        }
                }
            }
            catch (Exception ex)
            {
                ExceptionAction(ex);
            }
        }

        #region 按键发送

        private StringBuilder keyData;
        public StringBuilder KeyData
        {
            get;
            set;
        }

        private bool isReadOnly;
        public bool IsReadOnly
        {
            get
            { 
                return isReadOnly;
            }
            set
            {
                if (keyData.Length > 0)
                {
                    isReadOnly = false;
                }
                else
                {
                    isReadOnly = true;
                }
                RaisePropertyChanged(() => IsReadOnly);
            }
        }

        public void UserNameKeyDown(Object sender, KeyEventArgs e)
        {
            Regex regexStr = new Regex("^[a-zA-Z0-9]$");
            Regex regexNum = new Regex("^D[0-9]$");
            var key = e.Key.ToString();
            if (regexStr.IsMatch(key))
            {
                    keyData.Append(key);
                    SendKey(key);
            }
            if (regexNum.IsMatch(key))
            {
                key = key.Remove(0,1);
                keyData.Append(key);
                SendKey(key);
            }
            if (key == Key.Enter.ToString())
            {
                SendKey("\n");
            }
            if (key == Key.Back.ToString())//
            {
                if (keyData.Length > 0)
                {
                    SendKey("\b");
                    keyData.Remove(keyData.Length - 1, 1);
                    RaisePropertyChanged("IsReadOnly");
                }
            }
            RaisePropertyChanged("Message");
        }


        

        public void SendKey(String key)
        {
            var ip = _userData[_selectedIndex].CurrentEndpoint.Address;
            var port = _userData[_selectedIndex].CurrentEndpoint.Port;
            byte[] data = System.Text.Encoding.ASCII.GetBytes(key);
            if (modelServer.CommServer.UdpMonitorServer == null)
            {
                MessageBox.Show("请先打开连接");
                return;
            }
            modelServer.CommServer.UdpMonitorServer.Send(ip, port, data);
        }

        #endregion

        #region 消息接收
        private void _udpServer_DataArrived(object sender, UdpDataEventArgs e)
        {
            RaisePropertyChanged("Message");
            try
            {
                Action acton = () => { UpdateUserData(e); };
                _showDispatcher.Invoke(acton);
            }
            catch (Exception ex)
            {
                ExceptionAction(ex);
            }

        }

        /// <summary>
        /// 检查ID和端口号，更新数据
        /// </summary>
        private void UpdateUserData(UdpDataEventArgs arg)
        {
            _remoteIPEndPoint = new IPEndPoint(arg.RemoteIpEndPoint.Address, arg.RemoteIpEndPoint.Port);
            RaisePropertyChanged("RemoteIP");
            RaisePropertyChanged("RemotePort");
            //先更新本地数据
            _locaReciveAll.UpdateReicveRawData(arg.DataArray);
            RaisePropertyChanged("Message");
            //分类更新
            foreach (var m in UserData)
            {
                if ((m.CurrentEndpoint.Address.Equals(arg.RemoteIpEndPoint.Address))
                     && (m.CurrentEndpoint.Port == arg.RemoteIpEndPoint.Port))
                {
                    m.UpdateReicveRawData(arg.DataArray);
                    return;
                }
            }
            //没有接到则添加
            var newRecive = new ReciveIp(arg.RemoteIpEndPoint);
            newRecive.UpdateReicveRawData(arg.DataArray);
            UserData.Add(newRecive);
        }


        #endregion

        #region ClearText
        public RelayCommand<string> ClearText { get; private set; }

        void ExecuteClearText(string name)
        {
            try
            {
                switch (name)
                {
                    case "RawSendMessage": //原始发送信息
                        {
                            break;
                        }
                    case "RawReciveMessage": //原始接收信息
                        {
                            break;
                        }
                    case "LinkMessage": //链路信息
                        {
                            break;
                        }
                    case "StatusMessage": //状态信息
                        {
                            break;
                        }
                    case "ExceptionMessage":
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
            }
        }
        #endregion


        #region SaveData
        /// <summary>
        /// 使用时间和IP端口生成名字
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        private string MakeName(IPEndPoint end)
        {
            var ip = end.Address.GetAddressBytes();
            var str = string.Format("{0:###}-{1:###}-{2:###}-{3:###}-({4:####})", ip[0], ip[1], ip[2], ip[3], end.Port);
            var time = string.Format("{0:##}-{1:##}-{2:##}-{3:##}-{4:##}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour,
                DateTime.Now.Minute, DateTime.Now.Second);
            var name = str + "@" + time + ".txt";
            return name;
        }

        private void SaveData(ObservableCollection<ReciveIp> collect)
        {
            try
            {
                foreach (var m in collect)
                {
                    var name = MakeName(m.CurrentEndpoint);
                    FileOperate.WriteTextFile(@"./record/" + name, m.ReicveCharacter);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ClearAll(ObservableCollection<ReciveIp> collect)
        {                                                             
            try
            {
                foreach (var m in collect)
                {
                    m.ReicveRawData.Clear();
                    m.ReceiveCharacter.Clear();
                    m.ReicveCharacter = "";

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
