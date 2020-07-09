using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using ChoicePhase.PlatformModel.DataItemSet;
using ChoicePhase.PlatformModel;
using TransportProtocol.Comself;
using ChoicePhase.PlatformModel.GetViewData;
using ChoicePhase.PlatformModel.Helper;
using TransportProtocol.DistributionControl;
using System.Net;
//using System.Windows;
using Distribution.station;
using ChoicePhase.ControlPlatform.ViewModel;
using System.Threading;
using System.Text;
using GooseGenerator;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;

namespace ChoicePhase.ControlPlatform.ViewModel
{

    public class SwitchListViewModel : ViewModelBase
    {
        private readonly ushort _downAddress;
        private readonly ushort _localAddress;
        private PlatformModelServer modelServer;
        private SwitchPropertyIndex _attributeIndex = SwitchPropertyIndex.SwitchList;


        private ushort _remoteID = 0;
        private ushort _remotePort = 0;

        //修改xml文件
        private XmlOperation xmlOperation;

        /// <summary>
        /// Initializes a new instance of the DataGridPageViewModel class.
        /// </summary>
        public SwitchListViewModel()
        {

            LoadDataCommand = new RelayCommand(ExecuteLoadDataCommand);

            SetpointOperate = new RelayCommand<string>(ExecuteSetpointOperate);
            DataGridMenumSelected = new RelayCommand<string>(ExecuteDataGridMenumSelected);
            SelectedItemCommand = new RelayCommand<ObservableCollection<object>>(ExecuteSelectedItemCommand);

            modelServer = PlatformModelServer.GetServer();
           // modelServer.RTUConnect(modelServer.LocalAddr);
            _downAddress = modelServer.CommServer.DownAddress;
            _localAddress = modelServer.LocalAddr;
            _remoteID = _downAddress;
            _remotePort = (ushort)modelServer.UdpRemotePort;
            _stationNameList = modelServer.MonitorData.StationNameList;

            UnloadedCommand = new RelayCommand<string>(ExecuteUnloadedCommand);

            _operateCmd = new ObservableCollection<string>();
            xmlOperation = new XmlOperation(modelServer);

            InNum = 4;      //设置InNum的默认值
            VANID = "000";  // 设置VANID的默认值
        }


        /************** 属性 **************/
        private ObservableCollection<SwitchProperty> _userData;
        /// <summary>
        /// 用户信息数据
        /// </summary>
        public ObservableCollection<SwitchProperty> UserData
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
            UserData = modelServer.MonitorData.ReadAttribute(_attributeIndex, false);
            //_macAddress = modelServer.MonitorData.GetMacAddr(_attributeIndex);
            RaisePropertyChanged("MacAddress");

            SelectedIndex = 0;
            _selectedItems = new List<SwitchProperty>(1);
            _selectedItems.Add(UserData[SelectedIndex]);

            OperateCmd = modelServer.MonitorData.OperateCmd;
        }
        #endregion

        #region 值选择，下载,读取

        List<SwitchProperty> _selectedItems;
        public RelayCommand<ObservableCollection<object>> SelectedItemCommand { get; private set; }

        //加载用户数据
        void ExecuteSelectedItemCommand(ObservableCollection<object> list)
        {

            _selectedItems = new List<SwitchProperty>(list.Count);
            foreach (var m in list)
            {
                _selectedItems.Add(m as SwitchProperty);
            }
        }

        public RelayCommand<string> UnloadedCommand { get; private set; }

        void ExecuteUnloadedCommand(string obj)
        {
            FixCheck = false;
        }

        private byte _macAddress = 0x10;

        public string MacAddress
        {
            get
            {
                return _macAddress.ToString("X2");
            }
            set
            {
                byte.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out _macAddress);
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
                ////更新suoyin
                //var newIndex = (SwitchProperty)(_selectMacIndex * 2); //此处是偶数设置
                //if (_attributeIndex != newIndex)
                //{
                //    //选择不相等，则重新载入数据
                //    _attributeIndex = newIndex;
                //    _macAddress = modelServer.MonitorData.GetMacAddr(_attributeIndex);
                //    RaisePropertyChanged("MacAddress");
                //    UserData = modelServer.MonitorData.ReadAttribute(_attributeIndex, false);

                //}
                RaisePropertyChanged("SelectMacIndex");
            }
        }
        private byte _fuctionCode = 0x01;

        /// <summary>
        /// 功能码
        /// </summary>
        public string FunctionCode
        {
            get
            {
                return _fuctionCode.ToString("X2");
            }
            set
            {
                byte.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out _fuctionCode);
                RaisePropertyChanged("FunctionCode");
            }
        }
        private ushort _destAddress = 0xAA;
        /// <summary>
        /// 目的地址
        /// </summary>
        public string DestAddress
        {
            get
            {
                return _destAddress.ToString("X2");
            }
            set
            {
                ushort.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out _destAddress);
                RaisePropertyChanged("DestAddress");
            }
        }

        private uint _workAddress = 0xC0A80A01;
        /// <summary>
        /// 工作地址
        /// </summary>
        public string WorkAddress
        {
            get
            {
                return _workAddress.ToString("X2");
            }
            set
            {
                uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out _workAddress);
                RaisePropertyChanged("WorkAddress");
            }
        }

        public string WorkAddressIP
        {
            get
            {
                return IntToIp(_workAddress);
            }
            set
            {
                IpToInt(value,out _workAddress);
                RaisePropertyChanged("WorkAddressIP");
            }
        }

        /// <summary>
        /// 将ip转换为int
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="ipInt"></param>
        /// <returns></returns>
        public uint IpToInt(string ip,out uint ipInt)
        {
            char[] separator = new char[] { '.' };
            string[] items = ip.Split(separator);
            ipInt = uint.Parse(items[0]) << 24
                    | uint.Parse(items[1]) << 16
                    | uint.Parse(items[2]) << 8
                    | uint.Parse(items[3]);
            return ipInt;
        }

        /// <summary>
        /// 将int型的IP转换为字符串的IP
        /// </summary>
        /// <param name="ipInt"></param>
        /// <returns></returns>
        private string IntToIp(uint ipInt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((ipInt >> 24) & 0xFF).Append(".");
            sb.Append((ipInt >> 16) & 0xFF).Append(".");
            sb.Append((ipInt >> 8) & 0xFF).Append(".");
            sb.Append(ipInt & 0xFF);
            return sb.ToString();
        }


        private ObservableCollection<string> _operateCmd;

        public ObservableCollection<string> OperateCmd
        {
            get
            {
                return _operateCmd;
            }
            set
            {
                _operateCmd = value;
                RaisePropertyChanged("OperateCmd");
            }
        }
        private int _selectOperateCmdIndex;

        /// <summary>
        /// 选择的MACName
        /// </summary>
        public int SelectOperateCmdIndex
        {
            get
            {
                return _selectOperateCmdIndex;
            }
            set
            {
                _selectOperateCmdIndex = value;
                RaisePropertyChanged("SelectOperateCmdIndex");
            }
        }

       

        /// <summary>
        /// 定值功能
        /// </summary>
        public RelayCommand<string> SetpointOperate { get; private set; }


        void ExecuteSetpointOperate(string str)
        {
            try
            {
                switch (str)
                {
                    case "Read":
                        {
                            break;
                        }
                    case "Update":
                        {
                            break;
                        }
                    case "UpdateNeighbor":
                        {
                            RaisePropertyChanged("UserData");
                            UpdateComment(UserData);
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


        #region 生成Goose订阅文件
        private string path = "Goose\\cfg";

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
                RaisePropertyChanged("Path");
            }
        }


        //private string selectedFile;
        ///// <summary>
        ///// 生成配置文件的路径
        ///// </summary>
        //public string SelectedFile
        //{
        //    get
        //    {
        //        return selectedFile;
        //    }
        //    set
        //    {
        //        selectedFile = value;
        //        RaisePropertyChanged("SelectedFile");
        //    }
        //}

        private int inNum;

        public int InNum
        {
            get
            {
                return inNum;
            }
            set
            {
                inNum = value;
                RaisePropertyChanged("InNum");
            }
        }

        private int almNum;

        public int AlmNum
        {
            get
            {
                return almNum;
            }
            set
            {
                almNum = value;
                RaisePropertyChanged("AlmNum");
            }
        }


        #endregion


        #region 修改Icd文件的MAC地址

        private string vanId;

        public string VANID
        {
            get
            {
                return vanId;
            }
            set
            {
                vanId = value;
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
        private bool mantainceCheck = false;
        /// <summary>
        /// 检测使能
        /// </summary>
        public bool MantainceCheck
        {
            get
            {
                return mantainceCheck;
            }
            set
            {
                if (modelServer.RtuServer == null)
                {
                    MessageBox.Show("请先打开连接");
                    return;
                }
                mantainceCheck = value;
                RaisePropertyChanged("MantainceCheck");
                if (mantainceCheck)
                {
                    _remotePort = (ushort)modelServer.UdpMaintancePort;
                    modelServer.RtuServer.RemotePort = modelServer.UdpMaintancePort;
                }
                else
                {
                    _remotePort = (ushort)modelServer.UdpRemotePort;
                    if (modelServer.RtuServer == null)
                    {
                        MessageBox.Show("请先打开连接");
                        return;
                    }
                    modelServer.RtuServer.RemotePort = modelServer.UdpRemotePort;
                }

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


        public RelayCommand<string> DataGridMenumSelected { get; private set; }

        private void ExecuteDataGridMenumSelected(string name)
        {
            try
            {
                switch (name)
                {
                    case "Reload":
                        {
                            UserData = modelServer.MonitorData.ReadAttribute(_attributeIndex, true);
                            break;
                        }
                    case "Save":
                        {
                            modelServer.MonitorData.InsertAttribute(_attributeIndex);
                            break;
                        }
                    case "AddUp":
                        {
                            var item = new SwitchProperty(0, "1234", 1, 1, 1, "1,2,3", 123,"1,2,3",0, "线路","1",0, "母线", "CPU1","1,2,3", "01-0C-CD-01-00-41", 1041, 1041, 5000);
                            if (SelectedIndex > -1)
                            {
                                UserData.Insert(SelectedIndex, item);
                            }
                            else
                            {
                                UserData.Add(item);
                            }
                            break;
                        }
                    case "AddDown":
                        {
                            var item = new SwitchProperty(0, "1234", 1, 1, 1, "1,2,3", 123, "1,2,3", 0, "线路", "1", 0, "母线", "CPU1", "1,2,3", "01-0C-CD-01-00-41", 1041, 1041, 5000);
                            if (SelectedIndex > -1)
                            {
                                if (SelectedIndex < UserData.Count - 1)
                                {
                                    UserData.Insert(SelectedIndex + 1, item);
                                }
                                else
                                {
                                    UserData.Add(item);
                                }
                            }
                            else
                            {
                                UserData.Add(item);
                            }
                            break;
                        }
                    case "DeleteSelect":
                        {
                            foreach (var m in _selectedItems)
                            {
                                //var result = MessageBox.Show("是否删除选中行:" + gridTelesignalisation.SelectedItem.ToString(),
                                //    "确认删除", MessageBoxButton.OKCancel);
                                var result = true;
                                if (result)
                                {
                                    UserData.RemoveAt(SelectedIndex);
                                }
                            }
                              
                            RaisePropertyChanged("UserData");
                            break;
                        }
                    case "SetNewValue"://设定新值
                        {
                            foreach (var m in _selectedItems)
                            {
                                int index = UserData.IndexOf(m);
                                var list = ReserialTopologyData(UserData, index);
                                var frame = new RTUFrame(_localAddress, _destAddress, (byte)ControlCode.UPDATE_NEIGHBOUR, list.ToArray(), (byte)list.Count);
                                modelServer.RtuServer.SendFrame(frame, _workAddress, _remotePort);

                                System.Threading.Thread.Sleep(600);
                            }
                            break;
                        }
                    case "SetID"://设定新值
                        {
                            var list = new List<UInt32>();
                            foreach (var m in _selectedItems)
                            {
                                int index = UserData.IndexOf(m);
                                UInt32 ip = (UInt32)TypeConvert.IpToInt(UserData[index].IP);
                                list.Add(ip);
                            }
                            if (list.Count <= 0)
                            {
                                throw new Exception("数据过小");
                            }
                            // byte[] data = new byte[10];
                            //// ProtocolAnylast.EncodeStationMessage(list.ToArray(), ref data);
                            var data = SerailStation(list);

                            var frame = new RTUFrame(_localAddress, _destAddress, (byte)ControlCode.NANOPB_TYPE, data, (ushort)data.Length);
                            modelServer.RtuServer.SendFrame(frame, _workAddress, _remotePort);
                            break;
                        }
                    case "SetAllID"://设定新值
                        {
                            SetAllID();
                            break;
                        }
                    case "GetStation":
                        {
                            foreach (var m in _selectedItems)
                            {
                                var data = new byte[] { (byte)NanopbType.NANOPB_GET_STATION };
                                var frame = new RTUFrame(_localAddress, _destAddress, (byte)ControlCode.NANOPB_TYPE, data, (ushort)data.Length);
                                uint address = (uint)(TypeConvert.IpToInt(m.IP));
                                modelServer.RtuServer.SendFrame(frame, address, modelServer.UdpRemotePort);//固定为获取串口

                                System.Threading.Thread.Sleep(100);
                            }

                            //var data = new byte[] { (byte)NanopbType.NANOPB_GET_STATION };
                            //var frame = new RTUFrame(_localAddress, _destAddress, (byte)ControlCode.NANOPB_TYPE, data, (ushort)data.Length);
                            //modelServer.RtuServer.SendFrame(frame, _workAddress, _remotePort);
                            break;
                        }
                    case "SetLocal":
                        {
                            if (selectedIndex >= 0)
                            {
                                //大小端翻转
                                byte[] ip = (byte[])UserData[selectedIndex].IpArray.Clone();
                                Array.Reverse(ip);
                                var frame = new RTUFrame(_localAddress, _destAddress,
                                (byte)ControlCode.LOCAL_MESSAGE, ip, (byte)ip.Length);
                                modelServer.RtuServer.SendFrame(frame, _workAddress, _remotePort);
                            }

                            break;
                        }
                    case "SetAim":
                        {
                            if (selectedIndex >= 0)
                            {
                                //大小端翻转
                                byte[] ip = (byte[])UserData[selectedIndex].IpArray.Clone();
                                Array.Reverse(ip);
                                var frame = new RTUFrame(_localAddress, _destAddress,
                                (byte)ControlCode.AIM_MESSAGE, ip, (byte)ip.Length);
                                modelServer.RtuServer.SendFrame(frame, _workAddress, _remotePort);
                            }
                            break;
                        }
                    case "SetStation":
                        {
                            foreach (var m in _selectedItems)
                            {
                                int index = UserData.IndexOf(m);
                                var list = ReserialTopologyData(UserData, index);
                                var frame = new RTUFrame(_localAddress, _destAddress, (byte)ControlCode.ADD_STATION, list.ToArray(), (byte)list.Count);
                                modelServer.RtuServer.SendFrame(frame, _workAddress, _remotePort);

                                System.Threading.Thread.Sleep(600);
                            }
                            break;
                        }

                    case "SetStationProtobuf":
                        {
                            foreach (var m in _selectedItems)
                            {
                                int index = UserData.IndexOf(m);
                                var dataArray = SerialStationMessage(UserData, index);
                                var frame = new RTUFrame(_localAddress, _destAddress, (byte)ControlCode.NANOPB_TYPE, dataArray, (ushort)dataArray.Length);                                
                                modelServer.RtuServer.SendFrame(frame, _workAddress, _remotePort);
                                System.Threading.Thread.Sleep(600);
                            }
                            break;
                        }
                    case "ControlOperateCommand":
                        {
                            DealOperateCmd();
                            break;
                        }
                    case "GenerateGoose":
                        {
                            //生成goose文件
                            char[] separator = new char[] { ',', '，' };
                            for (int j = 0; j < UserData.Count; j++)
                            {
                                string[] neighboor = UserData[j].NeighboorCollect.Split(separator);
                                int gooseNum = neighboor.Length;
                                var ied = UserData[j].Addr.ToString();  //STU
                                string[] reffcda = new string[24];
                                string[] inName = new string[24];


                                string[] refTwo = { ied + "MEAS/MMXU1$ST$TotVA", ied + "MEAS/MMXU1$ST$Hz", ied + "MEAS/MMXU1$ST$PhV" };
                                string[] invarTwo = { "MEAS/MMXU1$TotVA", "MEAS/MMXU1$Hz", "MEAS/MMXU1$PhV" };

                                for (int i = 0; i < InNum; i++)
                                {
                                    reffcda[i] = string.Format("{0}PIGO/PTRC{1}$ST$Tr$general", ied, i + 1);
                                    inName[i] = string.Format("LD0/GGIO17$Ind{0}$stVal", i + 1);
                                }
                                for (int i = 0; i < AlmNum; i++)
                                {
                                    reffcda[InNum + i] = string.Format("{0}LD0/GGIO17$ST$Alm{1}$stVal", ied, i + 1);
                                    inName[InNum + i] = string.Format("LD0/GGIO17$Alm{0}$stVal", i + 1);
                                }
                                var goCB = new GoCBTx(ied + @"PIGO/LLN0$GO$gocb0",
                                   ied + @"PIGO/LLN0$GO$gocb0", ied + @"PIGO/LLN0$dsGOOSE0", 1);
                                var goCBTwo = new GoCBTx(ied + @"MEAS/LLN0$GO$gocb1", @"MEAS/LLN0$GO$gocb1", ied + @"MEAS/LLN0$GO$dsGoose1", 1);

                                uint ipInt;
                                IpToInt(UserData[j].IP, out ipInt);
                                var secondIndex = (byte)((ipInt & 0xffff) >> 8);
                                var txb = new GooseTxBlock(goCB, UserData[j].MACAddress, (InNum + AlmNum), reffcda, inName,UserData[j].APPID, UserData[j].MaxTime);
                                
                                //var txbTwo = new GooseTxBlock(goCBTwo, (byte)(0x80 + j+1), 3, refTwo, invarTwo);

                                //下面打印输入
                                string reffcdaInputOne;
                                string outNameOne;
                                List<GoCBRx> gocbList = new List<GoCBRx>();
                                var goRxInput = new List<GoRxInput>();
                                byte idCn = 1;
                                //gocb的数量
                                string neibourCollect = null;
                                
                                if (UserData[j].MCount != 0 && UserData[j].NCount != 0)
                                {
                                    neibourCollect = UserData[j].N + "," + UserData[j].M;
                                }
                                if (UserData[j].MCount == 0)
                                {
                                    neibourCollect = UserData[j].N;
                                }

                                if (UserData[j].NCount == 0)
                                {
                                    neibourCollect = UserData[j].M;
                                }
                                string[] gocbNeibour = neibourCollect.Split(separator);
                                int gocbNum = gocbNeibour.Length;
                                for (int i = 0; i < gocbNum; i++, idCn++)
                                {
                                    //获取邻居的Addr
                                    int neibourIndex = int.Parse(gocbNeibour[i]);
                                    var iedInput = UserData[neibourIndex - 1].Addr;
                                    int currgocb = int.Parse(iedInput.Substring(3));
                                    var gocbOne = new GoCBRx(iedInput + "PIGO/LLN0$GO$gocb0", iedInput + "PIGO/LLN0$GO$gocb0",
                                        UserData[neibourIndex - 1].APPID, iedInput + "PIGO/LLN0$dsGOOSE0", 1, UserData[neibourIndex - 1].MACAddress);
                                    gocbList.Add(gocbOne);

                                    //goRxInput赋值
                                    for (int kk = 0; kk < InNum; kk++)
                                    {
                                        reffcdaInputOne = string.Format(UserData[neibourIndex - 1].Addr + "LD0/GGIO17.ST.Ind{0}.stVal", kk + 1);
                                        outNameOne = string.Format("PIGO/GOINGGIO1.SPCS{0}.stVal", Convert.ToDouble((kk + 1) + (InNum  * i)).ToString("00"));
                                        goRxInput.Add(new GoRxInput(idCn, kk + 1, reffcdaInputOne, "Bool", outNameOne));
                                    }
                                    for (int kk = 0; kk < AlmNum; kk++)
                                    {
                                        reffcdaInputOne = string.Format(UserData[neibourIndex - 1].Addr + "LD0/GGIO17.ST.Alm{0}.stVal",  kk + 1);
                                        outNameOne = string.Format("LD0/GOINGGIO{0}.Alm{1}.stVal", idCn, kk + 1);
                                        goRxInput.Add(new GoRxInput(idCn, InNum + kk + 1, reffcdaInputOne, "Bool", outNameOne));
                                    }
                                    // index++;
                                    //}
                                }

                                //此参数用来跳过自身，如果等于此值，就跳过，否则就加入列表中
                                // current++;

                                //打印输入
                                var txbInput = new GooseRxBlock(gocbList.ToArray(), goRxInput);

                                //写入到txt文件中
                                StringBuilder str = new StringBuilder();
                                str.AppendFormat(Path + "\\stu{0}-goose.txt", j + 1);
                                string filePath = str.ToString();

                                FileStream fs = File.Create(filePath);
                                using (StreamWriter sw = new StreamWriter(fs))
                                {
                                    sw.WriteLine(txb.ToString());
                                    sw.WriteLine(txbInput.ToString());
                                }
                            }
                            break;
                        }
                    case "SelectPath":
                        {
                            FolderBrowserDialog fbd = new FolderBrowserDialog();
                            if (fbd.ShowDialog() == DialogResult.OK)
                            {
                                path = fbd.SelectedPath;
                                RaisePropertyChanged("Path");
                            }
                            break;
                        }
                    //case "SelectFilePath":
                    //    {
                    //        OpenFileDialog dialog = new OpenFileDialog();
                    //        dialog.Multiselect = false;
                    //        dialog.Title = "请选择文件";
                    //        dialog.Filter = "所有文件(*.*)|*.*";
                    //        if (dialog.ShowDialog() == DialogResult.OK)
                    //        {
                    //            selectedFile = dialog.FileName;
                    //            RaisePropertyChanged("SelectedFile");
                    //            xmlOperation = new XmlOperation(modelServer, selectedFile);
                    //        }
                    //        break;
                    //    }
                    case "GenerateCinfigFile":
                        {
                            //生成icd直接存储在Goose中
                            xmlOperation.UpdateXml(".\\Goose", InNum, VANID);
                            //生成.cfg文件
                            Process proc = null;
                            try
                            {
                                string targetDir = string.Format(@".\Goose");//this is where testChange.bat lies
                                proc = new Process();
                                proc.StartInfo.WorkingDirectory = targetDir;
                                proc.StartInfo.FileName = "make.bat";
                                proc.StartInfo.Arguments = string.Format("10");//this is argument
                                                                               //proc.StartInfo.CreateNoWindow = true;
                                                                               //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;//这里设置DOS窗口不显示，经实践可行
                                proc.Start();
                                proc.WaitForExit();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
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


        private byte[] SerailStation(List<uint> list)
        {
            StationMessage model = new StationMessage();
            model.idCollect.AddRange(list);

            var sData = ProtocolAnylast.Serialize(model);

            byte[] data = new byte[sData.Length + 3];
            Array.Copy(sData, 0, data, 3, sData.Length);
            return data;
        }

        private void SetAllID()
        {
            foreach (var node in UserData)
            {
                //测试
                var list = new List<UInt32>();
                foreach (var m in UserData)
                {
                    UInt32 ip = (UInt32)TypeConvert.IpToInt(m.IP);
                    list.Add(ip);

                }
                if (list.Count <= 0)
                {
                    throw new Exception("数据过小");
                }

                // ProtocolAnylast.EncodeStationMessage(list.ToArray(), ref data);

                //测试
                var data = SerailStation(list);
                //测试
                UInt32 adrress = (UInt32)TypeConvert.IpToInt(node.IP);
                var frame = new RTUFrame(_localAddress,  (ushort)adrress, (byte)ControlCode.NANOPB_TYPE, data, (ushort)data.Length);
                modelServer.RtuServer.SendFrame(frame, _workAddress, _remotePort);
                System.Threading.Thread.Sleep(600);
            }
        }


        private void DealOperateCmd()
        {
            byte value;
            bool result = modelServer.MonitorData.OperateCollect.TryGetValue(OperateCmd[SelectOperateCmdIndex], out value);
            if ((!result) || (_selectedItems.Count == 0))
            {
                return;
            }
            int offset = 0;
            byte[] cmdData = new byte[_selectedItems.Count * 5 + 1];
            cmdData[offset++] = (byte)_selectedItems.Count;

            byte[] ip = new byte[] { 192, 168, 10, 1 };
            foreach (var m in _selectedItems)
            {
                int index = UserData.IndexOf(m);
                ip = (byte[])UserData[index].IpArray.Clone();
                Array.Reverse(ip);
                Array.Copy(ip, 0, cmdData, offset, 4);
                offset += 4;
                cmdData[offset++] = value;
            }

            var frame = new RTUFrame(_localAddress, _destAddress,
              (byte)ControlCode.SWITCH_OPERATE, cmdData, (ushort)cmdData.Length);
            ip[0] = 0xFF;//广播
            var adress = new IPAddress(ip);
            modelServer.RtuServer.SendFrame(frame, (uint)adress.Address, _remotePort);
        }
        #region 列表数据转换成通讯数据

        private void UpdateComment(ObservableCollection<SwitchProperty> collect)
        {
            for (int i = 0; i < collect.Count; i++)
            {
                if (collect[i] == null)
                {
                    collect[i].Comment = "集合为初始化。";
                    continue;
                }
                collect[i].Comment = "IP:";
                if (collect[i].NeighboorArray == null)
                {
                    collect[i].Comment = "IP邻居合集为空。";
                    continue;
                }
                RaisePropertyChanged("NeighboorCollect");
                foreach (var m in collect[i].NeighboorArray)
                {

                    if (m > collect.Count)
                    {
                        collect[i].Comment += "?索引超出范围";
                    }
                    collect[i].Comment += collect[m - 1].IP + "; ";
                }
            }

        }

        private byte[] SerialStationMessage(ObservableCollection<SwitchProperty> userData, int row)
        {
            var userElement = userData[row];
            if (userElement == null)
            {
                throw new Exception("userData[row] 为空");
            }
            if (userElement.NeighboorArray == null)
            {
                throw new Exception("IP邻居合集为空。");
            }
            StationMessage message = new StationMessage();

            message.id_own = (uint)TypeConvert.IpToInt(userData[row].IP);
            for (int i = 0; i < userData.Count; i++)
            {
                var element = new node_property();
                element.id = (uint)TypeConvert.IpToInt(userData[i].IP);
                element.state = (uint)userData[i].State;
                element.type = (uint)userData[i].Type;                                            
                if (userData[i].NeighboorNum != userData[i].NeighboorArray.Length)
                {
                    throw new Exception("开关数量不统一。");
                }
                foreach (var mm in _userData[i].NeighboorArray)
                {
                    if (mm > userData.Count)
                    {
                        throw new Exception("?索引超出范围。");
                    }
                    byte[] ip = (byte[])userData[mm - 1].IpArray.Clone();
                    Array.Reverse(ip);
                   element.neighbourCollect.Add(TypeConvert.Combine(ip, 0));
                }
             
                message.node.Add(element);

            }

            //ID列表
            var list = new List<UInt32>();
            foreach (var m in UserData)
            {
                UInt32 ip = (UInt32)TypeConvert.IpToInt(m.IP);
                list.Add(ip);

            }
            if (list.Count <= 0)
            {
                throw new Exception("数据过小");
            }

            message.idCollect.AddRange(list);
            

            var sData = ProtocolAnylast.Serialize(message);


            byte[] data = new byte[sData.Length + 3];
            Array.Copy(sData, 0, data, 3, sData.Length);
            data[0] = (byte)NanopbType.STATION_MESSAGE;
            data[1] = (byte)sData.Length;
            data[2] = (byte)(sData.Length >> 8);

            return data;            
        }


        private List<byte> ReserialTopologyData(ObservableCollection<SwitchProperty> userData, int row)
        {
            var userElement = userData[row];
            if (userElement == null)
            {
                throw new Exception("userData[row] 为空");
            }
            if (userElement.NeighboorArray == null)
            {
                throw new Exception("IP邻居合集为空。");
            }
            List<byte> list = new List<byte>();

            //设置开关拓扑结构IP为拓扑结构IP
            list.Add(userElement.IpArray[3]);
            list.Add(userElement.IpArray[2]);
            list.Add(userElement.IpArray[1]);
            list.Add(userElement.IpArray[0]);
            //拓扑类型 1-单开关类型
            list.Add(1);
            //开关数量1
            list.Add(1);
            //开关属性开始设置


            //自身IP
            list.Add(userElement.IpArray[3]);
            list.Add(userElement.IpArray[2]);
            list.Add(userElement.IpArray[1]);
            list.Add(userElement.IpArray[0]);
            //开关类型
            list.Add((byte)userElement.Type);
            list.Add((byte)userElement.State);
            //开关邻居数量
            if (userElement.NeighboorNum != userElement.NeighboorArray.Length)
            {
                throw new Exception("开关数量不统一。");
            }
            list.Add((byte)userElement.NeighboorNum);
            foreach (var m in userElement.NeighboorArray)
            {

                if (m > userData.Count)
                {
                    throw new Exception("?索引超出范围。");
                }
                list.Add(userData[m - 1].IpArray[3]);
                list.Add(userData[m - 1].IpArray[2]);
                list.Add(userData[m - 1].IpArray[1]);
                list.Add(userData[m - 1].IpArray[0]);
            }
            ushort len = (ushort)(list.Count + 2 + 2);
            list.Insert(0, (byte)len);
            list.Insert(1, (byte)(len >> 8));
            var sumCheck = GenCRC.CumulativeSumCalculate(list.ToArray(), 0, (ushort)list.Count);
            list.Add((byte)sumCheck);
            list.Add((byte)(sumCheck >> 8));
            return list;
        }
        #endregion
    }
}