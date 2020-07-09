using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;

using System;

using System.Collections.Generic;
using ChoicePhase.PlatformModel.DataItemSet;
using ChoicePhase.PlatformModel.GetViewData;
using ChoicePhase.PlatformModel;
using TransportProtocol.Comself;
using TransportProtocol.DistributionControl;
using ChoicePhase.PlatformModel.Helper;
using System.Windows.Media;
using System.Windows;

namespace ChoicePhase.ControlPlatform.ViewModel
{

    public class MonitorViewModel : ViewModelBase
    {

        private PlatformModelServer modelServer;
        private readonly byte _downAddress;


        private AttributeIndex _attributeIndex = AttributeIndex.YongciReadA;
        /// <summary>
        /// Initializes a new instance of the DataGridPageViewModel class.
        /// </summary>
        public MonitorViewModel()
        {
            
            LoadDataCommand = new RelayCommand(ExecuteLoadDataCommand);

            UpdateOperate = new RelayCommand<string>(ExecuteUpdateOperate);
            DataGridMenumSelected = new RelayCommand<string>(ExecuteDataGridMenumSelected);
            modelServer = PlatformModelServer.GetServer();
            _downAddress = modelServer.CommServer.DownAddress;
            _stationNameList = modelServer.MonitorData.StationNameList;

            UnloadedCommand = new RelayCommand<string>(ExecuteUnloadedCommand); 
        }


        public RelayCommand<string> UnloadedCommand { get; private set; }

        void ExecuteUnloadedCommand(string obj)
        {
            FixCheck = false;
        }

        /************** 属性 **************/
        private ObservableCollection<StationInformation> _userData;
        /// <summary>
        /// 用户信息数据
        /// </summary>
        public ObservableCollection<StationInformation> UserData
        {
            get { return _userData; }
            set
            {
                _userData = value;
                RaisePropertyChanged("UserData");
            }
        }

        #region 加载数据命令：LoadDataCommand
        /// <summary>
        /// 加载数据
        /// </summary>
        public RelayCommand LoadDataCommand { get; private set; }

        //加载用户数据
        void ExecuteLoadDataCommand()
        {
            UserData = modelServer.MonitorData.StationObservable;
            RaisePropertyChanged("MacAddress");
        }

        #endregion

        #region 值选择，下载,读取
        private byte _macAddress = 0x10;

        public string MacAddress
        {
            get
            {
                return _macAddress.ToString("X2");
            }
            set
            {
                byte.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out  _macAddress);
                RaisePropertyChanged("MacAddress");
            }
        }

        private ObservableCollection<string> _stationNameList;

        public ObservableCollection<string> StationNameList
        {
            get
            {
                return _stationNameList;
            }
        }
        private int _selectMacIndex;

        /// <summary>
        /// 选择的MACName
        /// </summary>
        public int SelectMacIndex
        {
            get
            {
                return _selectMacIndex;
            }
            set
            {
                _selectMacIndex = value;
                RaisePropertyChanged("SelectMacIndex");
            }
        }

        private byte _startAddress = 0x41;

        public string StartAddress
        {
            get
            {
                return _startAddress.ToString("X2");
            }
            set
            {
                byte.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out  _startAddress);
                RaisePropertyChanged("StartAddress");
            }
        }

        private byte _endAddress = 0x70;
        public string EndAddress
        {
            get
            {
                return _endAddress.ToString("X2");
            }
            set
            {
                byte.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out  _endAddress);
                RaisePropertyChanged("EndAddress");
            }
        }
        /// <summary>
        /// 定值功能
        /// </summary>
        public RelayCommand<string> UpdateOperate { get; private set; }


        public void ExecuteUpdateOperate(string str)
        {
            try
            {
                switch (str)
                {
                    case "UpdateAll":
                        {
                            if (!modelServer.CommServer.UdpServer.IsRun)
                            {
                                MessageBox.Show("请打开网络连接");
                                return;
                            }
                            var switchList = modelServer.MonitorData.ReadAttribute(SwitchPropertyIndex.SwitchList, false);
                            foreach (var m in switchList)
                            {
                                var data = new byte[] { (byte)NanopbType.NANOPB_GET_STATION };
                                var frame = new RTUFrame(modelServer.LocalAddr, modelServer.CommServer.DownAddress, (byte)ControlCode.NANOPB_TYPE, data, (ushort)data.Length);
                                uint address = (uint)(TypeConvert.IpToInt(m.IP));
                                modelServer.RtuServer.SendFrame(frame, address, (ushort)modelServer.UdpRemotePort);//固定为获取串口

                                System.Threading.Thread.Sleep(100);
                            }
                            break;
                        }
                    default:
                        {
                            throw new Exception("未识别的信息");
                           
                        }
                }                   
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex, "ExceptionMessage");
            }
        }

        #endregion

        #region 表格操作
        private bool fixCheck = false;
        /// <summary>
        /// 检测使能
        /// </summary>
        public bool FixCheck
        {
            get
            {
                return fixCheck;
            }
            set
            {
                fixCheck = value;
                RaisePropertyChanged("FixCheck");
                RaisePropertyChanged("ReadOnly");

            }
        }

        /// <summary>
        /// 检测使能
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                return !fixCheck;
            }

        }

        private int selectedIndex = 0;
        /// <summary>
        /// 选择索引
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = value;
                RaisePropertyChanged("SelectedIndex");
            }
        }


        #region 新加的
        private StationInformation selectedItem;

        public StationInformation SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = UserData[SelectMacIndex];
                RaisePropertyChanged("SelectedItem");
            }
        }

        private bool isCheck;
        public bool IsCheck
        {
            get
            {
                return isCheck;
            }
            set
            {
                isCheck = value;
                RaisePropertyChanged(() => IsCheck);
            }
        }

        private string closeColor = "red";

        public string CloseColor
        {
            get
            { 
                return closeColor;
            }
            set
            {
                closeColor = value;
                RaisePropertyChanged(() => CloseColor);
            }
        }
 
        SwitchListViewModel sw = new SwitchListViewModel();
        #endregion

        public RelayCommand<string> DataGridMenumSelected { get; private set; }

        private void ExecuteDataGridMenumSelected(string name)
        {
            if (!FixCheck)
            {
                return;
            }
            
        }
        #endregion
    }


}