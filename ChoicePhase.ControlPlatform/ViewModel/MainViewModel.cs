using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using ChoicePhase.PlatformModel;
using ChoicePhase.PlatformModel.DataItemSet;

namespace ChoicePhase.ControlPlatform.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private PlatformModelServer modelServer;

        
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            showUri = "view/CommunicationView.xaml";
            LoadDataCommand = new RelayCommand(ExecuteLoadDataCommand);
            TreeSelectedItemCommand = new RelayCommand<string>(ExecuteTreeSelectedItemCommand);
            ClosingCommand = new RelayCommand(ExecuteClosingCommand);
            ClearText = new RelayCommand<string>(ExecuteClearText);
            ExecuteLoadDataCommand();
        }


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
                modelServer.CommServer.PropertyChanged += ServerInformation_PropertyChanged;
                modelServer.MonitorData.PropertyChanged += MonitorData_PropertyChanged;
            }            
        }

        void MonitorData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            Messenger.Default.Send<string>("txt" + e.PropertyName, "MessengerSrcollToEnd");
        }

        private void ServerInformation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            Messenger.Default.Send<string>("txt" + e.PropertyName, "MessengerSrcollToEnd");
        }
        #endregion

        #region 退出处理
        /// <summary>
        /// 加载数据
        /// </summary>
        public RelayCommand ClosingCommand { get; private set; }

        //加载用户数据
        void ExecuteClosingCommand()
        {
            if (modelServer != null)
            {
                modelServer.Close();
            }
        }
        #endregion

        public LoopTest TestParameter
        {
            get
            {
                return modelServer.LogicalUI.TestParameter;
            }
            set
            {
                modelServer.LogicalUI.TestParameter = value;
                RaisePropertyChanged("TestParameter");
            }
        }

        private string showUri;

        /// <summary>
        /// 显示Uri
        /// </summary>
        public string ShowUri
        {
            get
            {
                return showUri;
            }
            set
            {
                showUri = value;
                RaisePropertyChanged("ShowUri");
            }
        }
        /// <summary>
        /// 绑定树状控件命令
        /// </summary>
        public RelayCommand<string> TreeSelectedItemCommand { get; private set; }

        
        /// <summary>
        /// 执行选择菜单命令
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteTreeSelectedItemCommand(string obj)
        {
            try
            {
                switch (obj)
                {
                    case "SerialPortConfig":
                        {
                            ShowUri = "view/CommunicationView.xaml";
                            break;
                        }
                    case "MonitorParameter":
                        {
                            ShowUri = "view/MonitorView.xaml";
                            break;
                        }
                    case "SwitchList":
                        {
                            ShowUri = "view/SwitchListView.xaml";
                            break;
                        }
                    case "FaultTest":
                        {
                            ShowUri = "view/FaultTestView.xaml";
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
                Messenger.Default.Send<Exception>(ex);
            }
        }

        public string LinkMessage
        {
            get
            {
                return modelServer.CommServer.LinkMessage;
            }
            set
            {
                modelServer.CommServer.LinkMessage = value;
                RaisePropertyChanged("LinkMessage");
            }
        }

        public string StatusMessage
        {
            get
            {
                return modelServer.MonitorData.StatusMessage;
            }
            set
            {
                modelServer.MonitorData.StatusMessage = value;
                RaisePropertyChanged("StatusMessage");  
            }
        }
        public string ExceptionMessage
        {
            get
            {
                return modelServer.MonitorData.ExceptionMessage;
            }
            set
            {
                modelServer.MonitorData.ExceptionMessage = value;
                RaisePropertyChanged("ExceptionMessage");
            }
        }

        public string Tips
        {
            get
            {
                return modelServer.LogicalUI.TestParameter.Tips;
            }
            set
            {
                modelServer.LogicalUI.TestParameter.Tips = value;
                RaisePropertyChanged("Tips");
            }
        }

        ///<summary>
        ///连接信息
        /// </summary>
        public string RawSendMessage
        {
            get
            {
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
                return modelServer.CommServer.RawReciveMessage;
            }
            set
            {
                modelServer.CommServer.RawReciveMessage = value;
                RaisePropertyChanged("RawReciveMessage");

            }
        }
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
                            RawSendMessage = "";
                            break;
                        }
                    case "RawReciveMessage": //原始接收信息
                        {
                            RawReciveMessage = "";
                            break;
                        }
                    case "LinkMessage": //链路信息
                        {
                            LinkMessage = "";
                            break;
                        }
                    case "StatusMessage": //状态信息
                        {
                            StatusMessage = "";
                            break;
                        }
                    case "ExceptionMessage":
                        {
                            ExceptionMessage = "";
                            break;
                        }
                    case "Tips":
                        {
                            Tips = "";
                          //  modelServer.LogicalUI.TestParameter.Tips = "";
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

        
        public StatusBarMessage StatusBar
        {
            get
            {
                return modelServer.LogicalUI.StatusBar;
            }
            set
            {
                modelServer.LogicalUI.StatusBar = value;
                RaisePropertyChanged("StatusBar");
            }
        }
    }
}