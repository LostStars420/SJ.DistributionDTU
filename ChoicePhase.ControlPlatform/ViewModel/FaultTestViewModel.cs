using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ChoicePhase.PlatformModel;
using ChoicePhase.PlatformModel.DataItemSet;
using ChoicePhase.PlatformModel.GetViewData;

namespace ChoicePhase.ControlPlatform.ViewModel
{
    public class FaultTestViewModel : ViewModelBase
    {
        private SwitchPropertyIndex _faultStatusIndex = SwitchPropertyIndex.FaultStatus;
        private PlatformModelServer modelServer;
        private FaultTest faultTest;

        public FaultTestViewModel()
        {
            modelServer = PlatformModelServer.GetServer();

            LoadDataCommand = new RelayCommand(ExecuteLoadDataCommand);

            DataGridMenumSelected = new RelayCommand<string>(ExecuteDataGridMenumSelected);
           
            UnloadedCommand = new RelayCommand<string>(ExecuteUnloadedCommand);

            faultTest = new FaultTest(modelServer);

            CloseOpenCmd = new ObservableCollection<string>();
            for (int i = 0; i < faultTest.OpenCloseCmd.Count; i++)
            {
                CloseOpenCmd.Add(faultTest.OpenCloseCmd[i]);
            }

            StartEnable = true;
            CancleEnable = false;
        }

        private ObservableCollection<FaultStatus> _faultStatus;
        /// <summary>
        /// 合分闸状态信息
        /// </summary>
        public ObservableCollection<FaultStatus> FaultStatus
        {
            get { return _faultStatus; }
            set
            {
                _faultStatus = value;
                RaisePropertyChanged("FaultStatus");
            }
        }

        private ObservableCollection<string> _closeOpenCmd;
        /// <summary>
        /// 合分闸指令
        /// </summary>
        public ObservableCollection<string> CloseOpenCmd
        {
            get
            {
                return _closeOpenCmd;
            }
            set
            {
                _closeOpenCmd = value;
                RaisePropertyChanged("CloseOpenCmd");
            }
        }


        private int _selectCloseOpenCmdIndex;
        /// <summary>
        /// 选择的合分闸选项
        /// </summary>
        public int SelectCloseOpenCmdIndex
        {
            get
            {
                return _selectCloseOpenCmdIndex;
            }
            set
            {
                _selectCloseOpenCmdIndex = value;
                RaisePropertyChanged("SelectCloseOpenCmdIndex");
            }
        }


        #region 加载数据命令：LoadDataCommand
        
        public RelayCommand LoadDataCommand { get; private set; }
        /// <summary>
        /// 加载数据
        /// </summary>
        void ExecuteLoadDataCommand()
        {
            FaultStatus = modelServer.MonitorData.ReadFaultStatus(_faultStatusIndex, false);

            SelectedIndex = 0;
        }
        #endregion

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


        #region 列表操作

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

        public RelayCommand<string> UnloadedCommand { get; private set; }

        void ExecuteUnloadedCommand(string obj)
        {
            FixCheck = false;
        }


        private bool startEnable;
        /// <summary>
        /// 启动测试按钮使能设置
        /// </summary>
        public bool StartEnable
        {
            get
            {
                return startEnable;
            }
            set
            {
                startEnable = value;
                RaisePropertyChanged("StartEnable");
            }
        }


        private bool cancleEnable;
        /// <summary>
        /// 取消测试按钮使能设置
        /// </summary>
        public bool CancleEnable
        {
            get
            {
                return cancleEnable;
            }
            set
            {
                cancleEnable = value;
                RaisePropertyChanged("CancleEnable");
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
                            FaultStatus = modelServer.MonitorData.ReadFaultStatus(_faultStatusIndex, true);
                            break;
                        }
                    case "SetNewValue"://设定新值
                        {
                            //        foreach (var m in _selectedItems)
                            //        {
                            //            int index = FaultStatus.IndexOf(m);
                            //            var list = ReserialTopologyData(UserData, index);
                            //            var frame = new RTUFrame(_localAddress, _destAddress, (byte)ControlCode.UPDATE_NEIGHBOUR, list.ToArray(), (byte)list.Count);
                            //            modelServer.RtuServer.SendFrame(frame, _workAddress, _remotePort);

                            //            System.Threading.Thread.Sleep(600);
                            //        }
                            break;
                       }
                    case "Save":
                        {
                            modelServer.MonitorData.InsertFaultStatus(_faultStatusIndex);
                            break;
                        }
                    case "AddUp":
                        {
                            var item = new FaultStatus(0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0);
                            if (SelectedIndex > -1)
                            {
                                FaultStatus.Insert(SelectedIndex, item);
                            }
                            else
                            {
                                FaultStatus.Add(item);
                            }
                            break;
                        }
                    case "AddDown":
                        {
                            var item = new FaultStatus(0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0);
                            if (SelectedIndex > -1)
                            {
                                if (SelectedIndex < FaultStatus.Count - 1)
                                {
                                    FaultStatus.Insert(SelectedIndex + 1, item);
                                }
                                else
                                {
                                    FaultStatus.Add(item);
                                }
                            }
                            else
                            {
                                FaultStatus.Add(item);
                            }
                            break;
                        }
                    case "AddRight":
                        {
                            break;
                        }
                    case "DeleteSelect":
                        {
                            if (SelectedIndex > -1)
                            {
                                //var result = MessageBox.Show("是否删除选中行:" + gridTelesignalisation.SelectedItem.ToString(),
                                //    "确认删除", MessageBoxButton.OKCancel);
                                var result = true;
                                if (result)
                                {
                                    FaultStatus.RemoveAt(SelectedIndex);
                                }
                            }
                            RaisePropertyChanged("FaultStatus");
                            break;
                        }
                    //case "Revert":
                    //    {
                    //        faultTest.Revert();
                    //        break;
                    //    }
                    //case "UpdateData":
                    //    {
                    //        faultTest.UpdateData();
                    //        break;
                    //    }
                    //case "OpenK1":
                    //    {
                    //        faultTest.OpenOrCloseFault("192.168.10.91", "K1合闸");
                    //        break;
                    //    }
                    //case "CloseK1":
                    //    {
                    //        faultTest.OpenOrCloseFault("192.168.10.91", "K1分闸");
                    //        break;
                    //    }
                    //case "TestFaultBefore":
                    //    {
                    //        var result = CheckStatus(BeforeFaultStatus);
                    //        if (result.Count != 0)
                    //        {
                    //            MessageBox.Show("故障隔离前合分闸状态：" + false);
                    //        }
                    //        else
                    //        {
                    //            MessageBox.Show("故障隔离前合分闸状态：" + true);
                    //        }
                    //        break;
                    //    }
                    //case "TestFaultBehind":
                    //    {
                    //        var result = faultTest.CheckStatus(BehindFaultStatusF1);
                    //        if (result.Count == 0)
                    //        {
                    //            MessageBox.Show("故障隔离后合分闸状态：" + true);
                    //        }
                    //        else
                    //        {
                    //            MessageBox.Show("故障隔离后合分闸状态：" + false);
                    //        }
                    //        break;
                    //    }
                    case "StartTest":
                        {
                            faultTest.StartThread();
                            StartEnable = false;
                            CancleEnable = true;
                            break;
                        }
                    case "CancleTest":
                        {
                            faultTest.CancleThread();
                            CancleEnable = false;
                            StartEnable = true;
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

    }
}
