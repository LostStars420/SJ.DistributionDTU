using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportProtocol.DistributionControl
{
    /// <summary>
    /// 控制功能代码枚举
    /// </summary>
    public enum ControlCode
    {
        UPDATE_NEIGHBOUR = 0x01, //更新邻居配置信息
        GET_MESSAGE = 0x02, //获取信息
        REPLY_MESSAGE = 0x03, //应答信息
        LOOP_STATUS = 0x04, //周期性状态信息

        LOCAL_MESSAGE = 0x05, //本地信息
        AIM_MESSAGE = 0x06, //目的信息
        STATUS_MESSAGE = 0x07, //故障信息
        REMOVAL_MESSAGE = 0x08, //切除信息
        NANOPB_TYPE = 0x09, //NANOPB 信息
        GET_LOG = 0x0A, //获取日志

        CAL_POWER_AREA = 0x10, //计算配电区域
        CAL_PATH = 0x11, //计算路径
        CAL_PATH_ALL = 0x12, //计算路径
        DELETE_ONE = 0x20, //删除指定拓扑节点   
  
        SWITCH_OPERATE = 0x30, //开关操作

        ADD_STATION = 0x40, //增加站点

        GET_ALL = 0x50, //获取所有开关信息
        JUDGE_CONNECT = 0x51, //判断联络开关

    };
    public enum LogCode
    {
        LOG_EXCEPTION = 1,//异常，错误等记录
    }

    public enum NanopbType
    {
        LOG_EXCEPTION = 1,//异常，错误等记录
        STATION_MESSAGE = 2,//站点信息
        NANOPB_GET_STATION = 3, //获取站点信息
    }
    
}
