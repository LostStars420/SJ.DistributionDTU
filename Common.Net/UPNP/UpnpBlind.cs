using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NATUPNPLib;
using System.Net;
using System.Net.Sockets;

namespace Net.UPNP
{
    public class UpnpBlind
    {
        public string hostName;
        public string HostName
        {
            get { return hostName; }
        }
        public IPAddress innerIpv4;
        public IPAddress InerIPV4
        {
            get { return innerIpv4; }
        }

        private int eport; //外部端口
        private int iport; //内部端口

        private UPnPNAT upnpnat;//创建COM类型

        public event EventHandler<NetDataArrivedEventArgs> ExceptionArrived; //异常消息
        public UpnpBlind(int veport,int viport)
        {
            try
            {           
                innerIpv4 = GetHostInerIP();//获取内网IP
                //UPnP绑定信息
                eport = viport;
                iport = viport; 
            }
            catch (Exception ex)
            {
                this.ExceptionArrived(this, new NetDataArrivedEventArgs(ex.Message));
            }
        }
        public bool StartBlind(string des)
        {
            var description = des;
            //创建COM类型
            upnpnat = new UPnPNAT();
            var mappings = upnpnat.StaticPortMappingCollection;

            //错误判断
            if (mappings == null)
            {
                Console.WriteLine("没有检测到路由器，或者路由器不支持UPnP功能。");
                return false;
            }

            //添加之前的ipv4变量（内网IP），内部端口，和外部端口
            mappings.Add(eport, "TCP", iport, innerIpv4.ToString(), true, description);
            return true;
        }
        private IPAddress GetHostInerIP()
        {
            hostName = Dns.GetHostName(); //获取主机名称
            //从当前Host Name解析IP地址，筛选IPv4地址是本机的内网IP地址。
            var addrlist = Dns.GetHostEntry(hostName).AddressList;
            IPAddress ipv4 = null;
            foreach (var m in addrlist)
            {
                if (m.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipv4 = m;
                    break;
                }
               
            }
            return ipv4;
        }
    }
}
