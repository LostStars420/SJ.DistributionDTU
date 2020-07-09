using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;

using System;

using System.Threading;
using System.Net;
using Monitor.DASModel.GetViewData;
using ChoicePhase.PlatformModel;
using ChoicePhase.PlatformModel.DataItemSet;
using System.Text;
using System.Collections.Generic;
using TransportProtocol.Comself;
using System.IO.Ports;
using System.Windows.Media;
using ChoicePhase.PlatformModel.GetViewData;
using System.Net.Sockets;
using System.Linq;
using System.Windows;

namespace ChoicePhase.ControlPlatform.ViewModel
{
    public class CommunicationViewModel : ViewModelBase
    {
        private PlatformModelServer modelServer;

        /// <summary>
        /// Initializes a new instance of the DataGridPageViewModel class.
        /// </summary>
        public CommunicationViewModel()
        {
            modelServer = PlatformModelServer.GetServer();
            LoadDataCommand = new RelayCommand(ExecuteLoadDataCommand);            
         
            ClearText = new RelayCommand<string>(ExecuteClearText);
            ToEnd = new RelayCommand<string>(ExecuteToEnd);

            SerialCommand = new RelayCommand<string>(ExecuteSerialCommand);

            SendFrameCommand = new RelayCommand<string>(ExecuteSendFrameCommand);

            ExecuteLoadDataCommand();
            _commParameterSet = new SerialPortParameterSet();
            _netParameterSet = new NetPortParameterSet();

            //新加的
            NetCommand = new RelayCommand<string>(ExecuteNetCommand);
            openNetEnable = true;
        }

        private IPAddress localIp;

        public IPAddress LocalIp
        {
            get
            {
                return localIp;
            }
            set
            {
                localIp = _netParameterSet.ComBoIPListItem;
                RaisePropertyChanged("LocalIp");
            }
        }

        private NetPortParameterSet _netParameterSet;

        /// <summary>
        /// 网口参数
        /// </summary>
        public NetPortParameterSet NetParameterSet
        {
            get
            {
                return _netParameterSet;
            }
            set
            {
                _netParameterSet = value;
                RaisePropertyChanged("NetParameterSet");
            }
        }

        private SerialPortParameterSet _commParameterSet;

        /// <summary>
        /// 串口参数集合
        /// </summary>
        public SerialPortParameterSet CommParameterSet
        {
            get
            {
                return _commParameterSet;
            }
            set
            {
                _commParameterSet = value;
                RaisePropertyChanged("CommParameterSet");
            }
        }

        /// <summary>
        /// 服务信息到来
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ServerInformation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #region 加载数据命令：LoadDataCommand
        /// <summary>
        /// 加载数据
        /// </summary>
        public RelayCommand LoadDataCommand { get; private set; }

        //加载用户数据
        void ExecuteLoadDataCommand()
        {
            if (modelServer == null) //防止重复初始化
            {
                modelServer = PlatformModelServer.GetServer(); 
                modelServer.CommServer.PropertyChanged += ServerInformation_PropertyChanged;

            }
        }
        #endregion
     
        #region 串口数据处理

        public bool OpenEnable
        {
            get
            {
                if (modelServer.CommServer != null)
                {
                    return !modelServer.CommServer.CommonServer.CommState;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool CloseEnable
        {
            get
            {
                if (modelServer.CommServer != null)
                {
                    return modelServer.CommServer.CommonServer.CommState;
                }
                else
                {
                    return false;
                }

            }
        }
        

        public RelayCommand<string> SerialCommand { get; private set; }

        public void ExecuteSerialCommand(string arg)
        {
            try
            {
                switch (arg)
                {
                    case "OpeanSerial":
                        {   
                            var select = CommParameterSet.GetSelectedPortAttribute();
                            modelServer.CommServer.CommonServer.Open(select.CommonPort, select.Baud, select.DataBit,
                                select.ParityBit, select.StopBit);
                            RaisePropertyChanged("OpenEnable");
                            RaisePropertyChanged("CloseEnable");
                            modelServer.LogicalUI.StatusBar.SetCom(true);
                            modelServer.ControlNetServer.RestartLinkServer();
                            CommParameterSet.SaveLastPortRecod();
                            break;
                        }
                    case "CloseSerial":
                        {
                            modelServer.CommServer.CommonServer.Close();
                            RaisePropertyChanged("OpenEnable");
                            RaisePropertyChanged("CloseEnable");
                            modelServer.LogicalUI.StatusBar.SetCom(false);
                            modelServer.ControlNetServer.StopLinkServer();
                            modelServer.LogicalUI.StatusBar.CloseAll();//关闭所有状态显示
                            break;
                        }
                    default:
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


        #region 网口数据处理

        private bool openNetEnable;

        public bool OpenNetEnable
        {
            get
            {
                return openNetEnable;
            }
            set
            {
                openNetEnable = value;
                RaisePropertyChanged(() => OpenNetEnable);
            }
        }

        private bool closeNetEnable;

        public bool CloseNetEnable
        {
            get
            {
                return closeNetEnable;
            }
            set
            {
                closeNetEnable = !openNetEnable;
                RaisePropertyChanged(() => CloseNetEnable);
            }
        }


        public RelayCommand<string> NetCommand { get; private set; }

        public void ExecuteNetCommand(string arg)
        {
            try
            {
                switch (arg)
                {
                    case "OpeanNetPort":
                        {
                            localIp = _netParameterSet.ComBoIPListItem;
                            if (localIp == null)
                            {
                                MessageBox.Show("请选择网口");
                                return;
                            }
                            OpenNetEnable = false;
                            CloseNetEnable = true;
                            
                            modelServer.LocalAddr = (ushort)(localIp.GetAddressBytes()[2] << 8 | localIp.GetAddressBytes()[3]);
                            modelServer.RTUConnect(localIp, modelServer.LocalAddr);
                            if(modelServer.CommServer.UdpServer.IsRun)
                            {
                                MessageBox.Show("网络已连接");
                            }

                            break;
                        }
                    case "CloseNetPort":
                        {
                            if (modelServer.RtuServer != null)
                            {                                                                                                                                                             
                                modelServer.RtuServer.Close();
                                modelServer.RTUClose();
                            }
                            if(!modelServer.CommServer.UdpServer.IsRun)
                            {
                                MessageBox.Show("网络已断开");
                            }
                            OpenNetEnable = true;
                            CloseNetEnable = false;
                            break;
                        }

                    default:
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



        #region 发送帧命令

        private byte sendAddr = 0xA1;
        public string SendAddr
        {
            get
            {
                return sendAddr.ToString("X2");
            }
            set
            {
                byte.TryParse(value, System.Globalization.NumberStyles.HexNumber, null,out sendAddr);
                RaisePropertyChanged("SendAddr");
            }
        }

        private byte sendFunction = 0x01;
        public string SendFunction
        {
            get
            {
                return sendFunction.ToString("X2");
            }
            set
            {
                byte.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out sendFunction);                

                RaisePropertyChanged("SendFunction");
            }
        }

        private byte sendDataLen = 5; //发送数据长度
        public string SendDataLen
        {
            get
            {
                return sendDataLen.ToString("X2");
            }
            set
            {
                byte.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out sendDataLen);   
                RaisePropertyChanged("SendDataLen");
            }
        }

        private byte[] sendData = new byte[3]{10 , 0, 2};

        public string SendDataString
        {
            get
            {
                StringBuilder strBuilder = new StringBuilder(30);
                foreach (var m in sendData)
                {
                    strBuilder.AppendFormat("{0:X2} ", m);
                }
                return strBuilder.ToString();
            }
            set
            {
               sendData  = GetByteData(value);
               RaisePropertyChanged("SendDataString");
            }
        }

        /// <summary>
        /// 由字符串数组提取数据
        /// </summary>
        /// <param name="str">待提取的字符</param>
        /// <returns>提取的字节数组</returns>
        private byte[] GetByteData(string str)
        {
            str = str.Trim();
            var strSplite = str.Split(' ');
            byte[] array = new byte[strSplite.Length];
            int i = 0;
            foreach (var m in strSplite)
            {
                try
                {
                    var data = Convert.ToByte(m, 16);
                    array[i++] = data;
                }
                catch (ArgumentException ex)
                {
                    Common.LogTrace.CLog.LogError(ex.Message + "" + ex.StackTrace);
                }
                catch (FormatException ex)
                {
                    Common.LogTrace.CLog.LogError(ex.Message + "" + ex.StackTrace);
                }
                catch (OverflowException ex)
                {
                    Common.LogTrace.CLog.LogError(ex.Message + "" + ex.StackTrace);
                }

            }
            byte[] result = new byte[i];
            Array.Copy(array, 0, result, 0, i);
            return result;
        }

          public RelayCommand<string> SendFrameCommand { get; private set; }

        //加载用户数据
          private void ExecuteSendFrameCommand(string param)
          {
              try
              {
                  switch(param)
                  {
                      case "SendPure"://单纯的发送
                          {
                              if (sendData.Length < sendDataLen)
                              {
                                  throw new ArgumentException("数据长度小于设定的发送长度");
                              }
                              var frame = new RTUFrame(sendAddr, sendFunction, sendData, sendDataLen);
                              modelServer.RtuServer.SendFrame(frame);
                              break;
                          }
                      case "CheckCommunicationDevice":
                          {
                              byte[] cmd = new byte[] {0xF1, 0, 0x11 };

                              var frame = new RTUFrame(0xA1, 0x55, cmd, 3);
                              modelServer.RtuServer.SendFrame(frame);
                              break;
                          }
                  }
              }
              catch(Exception ex)
              {
                  Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
              }
          }
    
        #endregion


        /// <summary>
        /// 连接信息
        /// </summary>
        public string RawSendMessage
        {
            get
            {
                InitserverData();
                return modelServer.CommServer.RawSendMessage;
            }
            set
            {
                modelServer.CommServer.RawSendMessage = value;
                RaisePropertyChanged("RawSendMessage");

            }
        }
        public string RawReciveMessage
        {
            get
            {
                InitserverData();
                return modelServer.CommServer.RawReciveMessage;
            }
            set
            {
                modelServer.CommServer.RawReciveMessage = value;
                RaisePropertyChanged("RawReciveMessage");

            }
        }

        private void InitserverData()
        {
            
        }

        #region ClearText
        public RelayCommand<string> ClearText { get; private set; }

        void ExecuteClearText(string name)
        {
            try
            {
                switch(name)
                {
                    case "Send":
                        {
                            RawSendMessage = "";
                            break;
                        }
                    case "Receive":
                        {
                            RawReciveMessage = "";
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
        
        #region ToEnd
        public RelayCommand<string> ToEnd { get; private set; }

        void ExecuteToEnd(string name)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
            }
        }
        #endregion
       
       
    }
}