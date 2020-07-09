using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace UdpDebugMonitor.Model
{
    public class ReciveIp : ObservableObject
    {
        private IPEndPoint _endpoint;

        public IPEndPoint CurrentEndpoint
        {
            get
            {
                return _endpoint;
            }
        }

        /// <summary>
        /// IP
        /// </summary>
        public string IP
        {
            get
            {
                return _endpoint.Address.ToString();
            }
        }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port
        {
            get
            {
                return _endpoint.Port;
            }

        }

        private List<byte> _reicveRawData;
        /// <summary>
        /// 接收数据，原始数据
        /// </summary>
        public List<byte> ReicveRawData
        {
            get
            {
                return _reicveRawData;
            }
        }

        /// <summary>
        /// 更新字符串数组
        /// </summary>
        /// <param name="data"></param>
        public void UpdateReicveRawData(byte[] data)
        {
            _reicveRawData.AddRange(data);
            //字节数组转换为字符
            string str = System.Text.Encoding.Default.GetString(data);
            _reicveCharacter.Append(str);
            _showStr = _reicveCharacter.ToString();
            RaisePropertyChanged("ReicveCharacter");
        }

        private StringBuilder _reicveCharacter;

        public StringBuilder ReceiveCharacter
        {
            get
            {
                return _reicveCharacter;
            }
            set
            {
                _reicveCharacter = value;
            }
        }
        private string _showStr;

        /// <summary>
        /// 接收数据，转换成字符形式
        /// </summary>
        public string ReicveCharacter
        {
            get
            {
                return _showStr;//_showStr
            }
            set
            {
                _showStr = value;
            }

        }
        public ReciveIp(IPEndPoint endpoint)
        {
            _reicveRawData = new List<byte>();
            _reicveCharacter = new StringBuilder(1024 * 1024);
            _showStr = "";
            _endpoint = new IPEndPoint(endpoint.Address, endpoint.Port);
        }
    }
}
