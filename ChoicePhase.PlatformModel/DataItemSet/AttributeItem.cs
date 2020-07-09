using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using System.Net;
using GalaSoft.MvvmLight.Command;
using ChoicePhase.PlatformModel.Helper;

namespace ChoicePhase.PlatformModel.DataItemSet
{
    /// <summary>
    /// 属性参数包含可以应用于设定与参数读取
    /// </summary>
    public class AttributeItem : ObservableObject
    {
        private int _configID;
        /// <summary>
        /// 配置号
        /// </summary>
        public int ConfigID
        {
            get
            {
                return _configID;
            }
            set
            {
                _configID = value;
                RaisePropertyChanged("ConfigID");
            }
        }


        private  string _name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name= value;
                RaisePropertyChanged("Name");
            }
        }

        private int _rawValue;
        /// <summary>
        /// 原始值
        /// </summary>
        public int RawValue
        {
            get
            {
                return _rawValue;
            }
            set
            {
                _rawValue = value;
                UpdateAttribute(value);
                RaisePropertyChanged("RawValue");
            }
        }

        private  int _dataType;
        /// <summary>
        /// 数据类型
        /// </summary>
        public int DataType
        {
            get
            {
                return _dataType;
            }
            set
            {
                _dataType = value;
                RaisePropertyChanged("DataType");
            }
        }

        private double _value;
        /// <summary>
        /// 属性值
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                RaisePropertyChanged("Value"); 
            }
        }
        private double _newValue;
        /// <summary>
        /// 属性值
        /// </summary>
        public double NewValue
        {
            get
            {
                return _newValue;
            }
            set
            {
                _newValue = value;
                UpdateAttribute(value); //更新原始数组
                RaisePropertyChanged("NewValue");

            }
        }

        private  string _comment;
        /// <summary>
        /// 说明内容
        /// </summary>
        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
                RaisePropertyChanged("Comment");
            }
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        private string _timeStamp;
        public string TimeStamp
        {
            get
            {
                return _timeStamp;
            }
            set
            {
                _timeStamp = value;
                RaisePropertyChanged("TimeStamp");
            }

        }

        /// <summary>
        /// 更新属性
        /// </summary>
        /// <param name="data"></param>
        /// <returns>true/false 更新是否成功</returns> 
        public bool UpdateAttribute(byte[] data)
        {
            //浮点数4字节0x4F
            if (_dataType == 0x4F)
            {
                ShortFloating Float = new ShortFloating(data);

                _rawValue = (int)Float.Value;
                RaisePropertyChanged("RawValue");
                
                _value = Float.Value;
                RaisePropertyChanged("Value");
                TimeStamp = DateTime.Now.ToLongTimeString();
                return true;
            }

            int byteNum = _dataType >> 4;
            int signalNum = _dataType & 0x0F;

            UInt32 raw = 0;

            if (data.Length != byteNum)
            {
                return false;
            }

            for (int i = 0; i < data.Length; i++)
            {
                raw = (UInt32)(raw + (double)data[i] * Math.Pow(256, i));
            }
            _rawValue = (int)raw;
            RaisePropertyChanged("RawValue");
            double ration = Math.Pow(0.1, signalNum); //计算系数
            _value = raw * ration;
            RaisePropertyChanged("Value");
            TimeStamp = DateTime.Now.ToLongTimeString();

            return true;
        }

        /// <summary>
        /// 更新属性
        /// </summary>
        /// <param name="setValue">设定的属性值</param>
        /// <returns>true/false 更新是否成功</returns> 
        public bool UpdateAttribute(double setValue)
        {
            if(_dataType == 0x4F)
            {
                ShortFloating Float = new ShortFloating((float)setValue);
                _rawValue = (int)Float.Value;
                RaisePropertyChanged("RawValue");
                TimeStamp = DateTime.Now.ToLongTimeString();
                return true;
            }

            int signalNum = _dataType & 0x0F;

            double ration = Math.Pow(10, signalNum); //计算系数
            var mediumValue = setValue * ration;


            _rawValue = (int)mediumValue;
            RaisePropertyChanged("RawValue");     
            
            TimeStamp = DateTime.Now.ToLongTimeString();
            return true;
        }


        /// <summary>
        /// 更新属性
        /// </summary>
        /// <param name="setValue">设定的属性值</param>
        /// <returns>true/false 更新是否成功</returns> 
        public bool UpdateAttribute(int raw)
        {

            int signalNum = _dataType & 0x0F;

            double ration = Math.Pow(0.1, signalNum); //计算系数
            var mediumValue = (double)raw * ration;

            _newValue = mediumValue;
            RaisePropertyChanged("NewValue");

            TimeStamp = DateTime.Now.ToLongTimeString();
            return true;
        }

        /// <summary>
        /// 获取原始数据字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] GetAttributeByteData()
        {
            UpdateAttribute(NewValue);//更新原始数组

            if (_dataType == 0x4F)
            {
                ShortFloating Float = new ShortFloating((float)_newValue);
                return Float.GetDataArray();
            }
            int byteNum = _dataType >> 4;
            byte[] data = new byte[byteNum];
            var outData = _rawValue;
            for(int i =0; i < byteNum; i++)
            {
                data[i] = (byte)(outData >> (i * 8));
            }
            return data;
        }

        /// <summary>
        /// 属性值
        /// </summary>
        public AttributeItem(int configID, string name, int rawValue, int dataType, double value, string comment)
        {
            _configID = configID;
            _name = name;
            _rawValue = rawValue;
            _dataType = dataType;
            _value = value;           
            _comment = comment;
            _timeStamp = "历史数据载入";
            _newValue = _value;
        }
    }
}
