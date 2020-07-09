using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChoicePhase.DeviceNet.Element;
using ChoicePhase.DeviceNet.LogicApplyer;
using ChoicePhase.Modbus;
using ChoicePhase.PlatformModel.DataItemSet;
using ChoicePhase.PlatformModel.GetViewData;
using ChoicePhase.PlatformModel.Helper;

namespace ChoicePhase.PlatformModel
{
    /// <summary>
    /// PlatformModel服务 超时部分
    /// </summary>
    public  partial class PlatformModelServer
    {


        /// <summary>
        /// 设备超时处理
        /// </summary>
        private void DeviceOverTimeDeal()
        {
            LogicalUI.StatusBar.SetDevice(false);
            LogicalUI.StatusBar.SetDevice(false);
            ControlNetServer.StopLinkServer(); //停止所有连接
        }

        /// <summary>
        /// 更新全局配置参数
        /// </summary>
        private void UpadteConfigParameter()
        {
            //读取配置文件
            XMLOperate.ReadLastConfigRecod();
            NodeAttribute.CurrentUser = XMLOperate.ReadUserRecod();
        }





        #region 多帧接收处理
        /// <summary>
        /// 多帧帧缓冲
        /// </summary>
        private List<byte> _multiFrameBuffer;
        /// <summary>
        /// 最小接收索引
        /// </summary>
        private byte _lastIndex;


        /// <summary>
        /// 从站上传，多帧接收处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PollingService_MultiFrameArrived(object sender, MultiFrameEventArgs e)
        {
            //判断是否为首帧
            if (e.Index == 0)
            {
                _multiFrameBuffer.Clear();//清空历史数据
                _lastIndex = 0;
                _multiFrameBuffer.AddRange(e.ByteData);
                return;
            }

            int currentIndex = e.Index & 0x7F; //当前索引
            //是否为递增索引,正常接收
            if ((_lastIndex + 1) == currentIndex)
            {
                _multiFrameBuffer.AddRange(e.ByteData);
                _lastIndex++;
            }
            else
            {

            }
            //判断是否为最后一帧
            if ((e.Index & 0x80) == 0x80)
            {

                //TODO:显示
                MonitorData.StatusMessage += "\n\n" + DateTime.Now.ToLongTimeString() + "  多帧接收:\n";
                MonitorData.StatusMessage += "接收完成" + "\n";
                //适当处理
                StringBuilder stb = new StringBuilder(_multiFrameBuffer.Count * 4);
                if (_multiFrameBuffer.Count % 2 == 0)
                {
                    for (int i = 0; i < _multiFrameBuffer.Count; i += 2)
                    {
                        stb.AppendFormat("{0},", _multiFrameBuffer[i] + 256 * _multiFrameBuffer[i + 1]);
                    }
                    MonitorData.StatusMessage += "\n\n" + "  有效数据:\n";
                    MonitorData.StatusMessage += stb.ToString() + "\n";
                }
            }


        }

        #endregion


        /// <summary>
        /// 判断RTU帧是否含有CAN帧并提取
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        private CanMessage GetCanMessage(RTUFrame frame)
        {
            if (frame.Function != _upCode)//是否为0xAA 是否为上送
            {
                return null;
            }
            if (frame.DataLen <= 2)
            {
                return null;
            }
            var id = frame.FrameData[0] + ((ushort)(frame.FrameData[1]) << 8);
            var can = new CanMessage((ushort)id, frame.FrameData, 2, frame.DataLen - 2);
            return can;

        }


        /// <summary>
        /// 数组转化为字符串数组
        /// </summary>
        /// <param name="data">数据数组</param>
        /// <param name="start">开始索引</param>
        /// <param name="len">转换长度</param>
        /// <returns>转换后的字符串</returns>
        private string ByteToString(byte[] data, int start, int len)
        {
            StringBuilder strBuild = new StringBuilder(len * 3 + 10);
            for (int i = start; i < start + len; i++)
            {
                strBuild.AppendFormat("{0:X2} ", data[i]);
            }

            return strBuild.ToString();
        }

        /// <summary>
        /// 获取错误描述
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="serverData"></param>
        public string GetErrorComment(byte mac, byte[] serverData)
        {

            var des = GetIDDescription((CommandIdentify)serverData[1]);
            var node = LogicalUI.GetNdoe(mac);
            if (node != null)
            {
                string error1 = "";

                if (node.Mac == NodeAttribute.MacSynController)
                {
                    error1 = ErrorCode.GetTongbuErrorComment(serverData[2]);
                }
                else if ((node.Mac == NodeAttribute.MacPhaseA) ||
                    (node.Mac == NodeAttribute.MacPhaseB) || (node.Mac == NodeAttribute.MacPhaseC))
                {
                    error1 = ErrorCode.GetYongciErrorComment(serverData[2]);
                }
                string str = string.Format("主动应答错误:{0}(0x{1:X2}),{2},{3}", node.Name, mac, des, error1);
                return str;
            }
            else
            {
                string error1 = "错误代码:" + serverData[2].ToString("X2");
                string error2 = "附加错误代码:" + serverData[3].ToString("X2");
                var str = "MAC:" + mac.ToString("X2") + des + " " + error1 + " " + error2;
                return str;
            }
        }
        /// <summary>
        /// 获取CAN错误状态
        /// </summary>
        /// <param name="can"></param>
        /// <returns></returns>
        private string GetCANError(CanMessage can)
        {
            if (can.DataLen >= 6)
            {
                StringBuilder strBuildA = new StringBuilder(128);
                StringBuilder strBuild = new StringBuilder(128);
                if (can.Data[4] != 0)
                {
                    strBuild.AppendLine("接收错误计数:" + can.Data[4].ToString());
                }
                if (can.Data[5] != 0)
                {
                    strBuild.AppendLine("发送错误计数:" + can.Data[5].ToString());
                }
                int state = 0;
                if (can.Data[3] != 0)
                {
                    for (int i = 0; i < 8; i++)
                    {

                        state = (can.Data[3] >> i) & 0x01;
                        //为0--跳过
                        if (state == 0)
                        {
                            continue;
                        }
                        switch (i)
                        {
                            case 0:
                                {
                                    strBuild.AppendLine("EWARN:发送器或接收器处于警告错误状态位");
                                    break;
                                }
                            case 1:
                                {
                                    strBuild.AppendLine("RXWAR： 接收器处于警告错误状态位");
                                    break;
                                }
                            case 2:
                                {
                                    strBuild.AppendLine("TXWAR： 发送器处于警告错误状态位");
                                    break;
                                }
                            case 3:
                                {
                                    strBuild.AppendLine("RXEP： 接收器处于总线被动错误状态位");
                                    break;
                                }

                            case 4:
                                {
                                    strBuild.AppendLine("TXEP： 发送器处于总线被动错误状态位");
                                    break;
                                }
                            case 5:
                                {
                                    strBuild.AppendLine("TXBO： 发送器处于总线关闭错误状态位");
                                    break;
                                }

                            case 6:
                                {
                                    strBuild.AppendLine("RX1OVR： 接收缓冲区 1 溢出位");
                                    break;
                                }
                            case 7:
                                {
                                    strBuild.AppendLine("RX0OVR： 接收缓冲区 0 溢出位");
                                    break;
                                }
                        }
                    }
                    //当有信息时进行显示
                    if (strBuild.Length != 0)
                    {

                        strBuildA.AppendLine(ByteToString(can.Data, 0, can.DataLen));
                        strBuildA.Append(strBuild);

                    }
                }
                return strBuildA.ToString();

            }
            return "不完整的Device信息：" + ByteToString(can.Data, 0, can.DataLen);


        }
        /// <summary>
        /// 获取ID描述
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>描述词</returns>
        public string GetIDDescription(CommandIdentify id)
        {
            string des = "";
            switch (id)
            {
                case CommandIdentify.CloseAction:
                    {
                        des = "合闸执行";
                        break;
                    }
                case CommandIdentify.MasterParameterRead:
                    {
                        des = "主站参数读取";
                        break;
                    }
                case CommandIdentify.MasterParameterSetOne:
                    {
                        des = "主站参数设置";
                        break;
                    }
                case CommandIdentify.MutltiFrame:
                    {
                        des = "多帧";
                        break;
                    }
                case CommandIdentify.OpenAction:
                    {
                        des = "分闸执行";
                        break;
                    }
                case CommandIdentify.ReadyClose:
                    {
                        des = "合闸预制";
                        break;
                    }
                case CommandIdentify.ReadyOpen:
                    {
                        des = "分闸预制";
                        break;
                    }
                case CommandIdentify.SubstationStatuesChange:
                    {
                        des = "子站状态上传";
                        break;
                    }
                case CommandIdentify.SyncOrchestratorCloseAction:
                    {
                        des = "同步控制器合闸执行";
                        break;
                    }
                case CommandIdentify.SyncOrchestratorReadyClose:
                    {
                        des = "同步控制器合闸预制";
                        break;
                    }
                case CommandIdentify.SyncReadyClose:
                    {
                        des = "同步合闸预制";
                        break;
                    }
                case CommandIdentify.ConfigMode:
                    {
                        des = "配置模式";
                        break;
                    }
                case CommandIdentify.SynTimeSequence:
                    {
                        des = "时序同步脉冲信号";
                        break;
                    }
                default:
                    {
                        des = "未识别的ID";
                        break;
                    }
            }
            return des + "(0x" + ((byte)id).ToString("X2") + ")";
        }
    }
}
