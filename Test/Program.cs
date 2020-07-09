using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InteropDemo
{
    class Program
    {
        //[DllImport(@"../../../x64\Release/Protocol.Test.dll", EntryPoint = "test01", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        //extern static int test01(int a, int b, int c);

        //[DllImport(@"../../../x64\Release/Protocol.Test.dll", EntryPoint = "test02", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        //extern static int test02(int a, int b);


        [DllImport(@"../../../x64\Release/TransportProtocol.Embed.dll", EntryPoint = "ProtocolInit", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        extern static byte ProtocolInit(UInt16 local_address);

        [DllImport(@"../../../x64\Release/TransportProtocol.Embed.dll", EntryPoint = "ProtocolAnylastDeal", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        extern static byte ProtocolAnylastDeal(byte[] inData, UInt16 inLen, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ushort[] usedLen,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] Z, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ushort[] outLen);
        //static void Main(string[] args)
        //{
        //    Assembly assembly = Assembly.LoadFrom(@"../../../x64\Release/Protocol.Test.dll");
        //    Type type = assembly.GetType("test01");
        //    Object obj = Activator.CreateInstance(type, null);
        //    MethodInfo mi = type.GetMethod("test01");
        //}
        //static void Main(string[] args)
        //{
        //    NativeMethodEx.LoadDll(@"../../../x64\Release/Protocol.Test.dll");
        //    NativeMethodEx.LoadFun("test01");

        //    object[] Parameters = new object[] { (int)1 , (int)2,(int)3 }; // 实参为 0

        //    Type[] ParameterTypes = new Type[] { typeof(int), typeof(int), typeof(int) }; // 实参类型为 int

        //    ModePass[] themode = new ModePass[] { ModePass.ByValue, ModePass.ByValue, ModePass.ByValue }; // 传送方式为值传

        //    Type Type_Return = typeof(int); // 返回类型为 int 
        //    var str = NativeMethodEx.Invoke(Parameters, ParameterTypes, themode, Type_Return).ToString();
        //    Console.WriteLine(str);
        //     str = NativeMethodEx.Invoke(Parameters, ParameterTypes, themode, Type_Return).ToString();
        //    Console.WriteLine(str);
        //     str = NativeMethodEx.Invoke(Parameters, ParameterTypes, themode, Type_Return).ToString();
        //    Console.WriteLine(str);
        //    Console.Read();
        //}

        //static void Main(string[] args)
        //{
        //    //1. 动态加载C++ Dll
        //    int hModule = NativeMethod.LoadLibrary(@"../../../x64\Release/Protocol.Test.dll");
        //    if (hModule == 0) return;

        //    //2. 读取函数指针
        //    IntPtr intPtr = NativeMethod.GetProcAddress(hModule, "test01");
        //    if (intPtr == null)
        //    {
        //        Console.WriteLine("intPtr = null");
        //    }
        //    else
        //    {

        //        //3. 将函数指针封装成委托
        //        var ProtocolInit = (test01)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(test01));

        //        //4. 测试
        //        //ProtocolInit(0x1234);
        //    }
        //    //Console.WriteLine(addFunction(1, 2));
        //    Console.Read();
        //}
        // delegate byte ProtocolInitDelegate(ushort local_address);
        // delegate int test01(int a, int b, int c);
        void Main(string[] args)
        {
            //int r1 = test01(1, 2, 3);
            //int r2 = test02(5, 2);
            //Console.WriteLine("test01结果：" + r1.ToString());
            //Console.WriteLine("test02结果：" + r2.ToString());
            //r1 = test01(1, 2, 3);
            //r2 = test02(5, 2);
            //Console.WriteLine("test01结果：" + r1.ToString());
            //Console.WriteLine("test02结果：" + r2.ToString());

            byte result = ProtocolInit(0x1234);
            Console.WriteLine("ProtocolInit： {0}", result);
            var used = new ushort[] { 0 };
            var outData = new byte[100];
            var outLen = new ushort[] { 0 };
            ProtocolAnylastDeal(new byte[] { 1, 2, 3, 4, 5, 6 }, 6, used, outData, outLen);



            Console.ReadKey();
        }
    }
}


