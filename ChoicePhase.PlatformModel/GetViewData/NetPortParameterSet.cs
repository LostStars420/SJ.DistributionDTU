using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ChoicePhase.PlatformModel.GetViewData
{
    public class NetPortParameterSet : ObservableObject
    {
        public NetPortParameterSet()
        {
            //新加的
            var ipArray = Dns.GetHostAddresses(Dns.GetHostName());
            var localIp = ipArray.First(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            ComBoIPList = new List<IPAddress>();
            for (int i = 0; i < ipArray.Length; i++)
            {
                if (ipArray[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    ComBoIPList.Add(ipArray[i]);
                }
            }
        }

        #region 本地IP地址列表
        private IPAddress comBoIPListItem;
        /// <summary>
        /// 选中IP地址
        /// </summary>
        public IPAddress ComBoIPListItem
        {
            get
            {
                return comBoIPListItem;
            }
            set
            {
                comBoIPListItem = value;
                RaisePropertyChanged(() => ComBoIPListItem);
            }
        }


        private List<IPAddress> comBoIPList;
        /// <summary>
        /// IP地址下拉框
        /// </summary>
        public List<IPAddress> ComBoIPList
        {
            get { return comBoIPList; }
            set
            {
                comBoIPList = value;
                RaisePropertyChanged(() => ComBoIPList);
            }
        }
        #endregion


        private int localPort;

        public int LocalPort
        {
            get
            {
                return localPort;
            }
            set
            {
                localPort = value;
                RaisePropertyChanged(() => LocalPort);
            }
        }
    }
}
