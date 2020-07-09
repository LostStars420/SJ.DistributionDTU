using GalaSoft.MvvmLight;
using System;
using System.Runtime.InteropServices;

namespace ChoicePhase.PlatformModel.Helper
{
    /// <summary>
    /// 短浮点数
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    struct ShortFloatType
    {
        /// <summary>
        /// 短浮点数
        /// </summary>
        [FieldOffset(0)]
        public float Float;
        [FieldOffset(0)]
        public byte Byte1;
        [FieldOffset(1)]
        public byte Byte2;
        [FieldOffset(2)]
        public byte Byte3;
        [FieldOffset(3)]
        public byte Byte4;
    }

    /// <summary>
    /// 短浮点数 IEEE STD745 短浮点数
    /// </summary>
    public class ShortFloating
    {
        /// <summary>
        /// 32bit单精度浮点数
        /// </summary>
        public float Value;


        /// <summary>
        /// 字节数组
        /// </summary>
        private byte[] dataArray
        {
            get;
            set;
        }

        /// <summary>
        /// 获取字节数数组
        /// </summary>
        /// <returns></returns>
        public byte[] GetDataArray()
        {
            return dataArray;

        }

        /// <summary>
        /// 短浮点数
        /// </summary>
        /// <param name="value">浮点数</param>
        public ShortFloating(float value)
        {
            dataArray = new byte[4];
            var m = (UInt32)value;
            ShortFloatType tris = new ShortFloatType();
            tris.Float = value;
            dataArray[0] = tris.Byte1;
            dataArray[1] = tris.Byte2;
            dataArray[2] = tris.Byte3;
            dataArray[3] = tris.Byte4;
        }

        /// <summary>
        ///  短浮点数
        /// </summary>
        /// <param name="value">字节数组</param>
        public ShortFloating(byte[] array)
        {
            dataArray = new byte[4];
            Array.Copy(array, 0, dataArray, 0, 4);

            ShortFloatType tris = new ShortFloatType();

            tris.Byte1 = dataArray[0];
            tris.Byte2 = dataArray[1];
            tris.Byte3 = dataArray[2];
            tris.Byte4 = dataArray[3];

            Value = tris.Float;
        }
    }  
   
}