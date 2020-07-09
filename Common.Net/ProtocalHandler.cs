using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Net
{
    public class ProtocolHandler
    {
        private string partialProtocal; // 保存不完整的协议
        public ProtocolHandler()
        {
            partialProtocal = "";
        }
        public string[] GetProtocol(string input)
        {
            return GetProtocol(input, null);
        }
        // 获得协议
        private string[] GetProtocol(string input, List<string> outputList)
        {
            if (outputList == null)
                outputList = new List<string>();
            if (String.IsNullOrEmpty(input))
                return outputList.ToArray();
            if (!String.IsNullOrEmpty(partialProtocal))
                input = partialProtocal + input;
            string pattern = "(^<protocol>.*?</protocol>)";
            // 如果有匹配，说明已经找到了，是完整的协议
            if (Regex.IsMatch(input, pattern))
            {
                // 获取匹配的值
                string match = Regex.Match(input, pattern).Groups[0].Value;
                outputList.Add(match);
                partialProtocal = "";
                // 缩短 input 的长度
                input = input.Substring(match.Length);
                // 递归调用
                GetProtocol(input, outputList);
            }
            else
            {
                // 如果不匹配，说明协议的长度不够，
                // 那么先缓存，然后等待下一次请求
                partialProtocal = input;
            }
            return outputList.ToArray();
        }
    }
}
