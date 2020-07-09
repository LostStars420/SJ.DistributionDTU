using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Net
{
    public class RequestHandler
    {
        private string temp = string.Empty;
        public string[] GetActualString(string input)
        {
            return GetActualString(input, null);
        }
        private string[] GetActualString(string input, List<string> outputList)
        {
            if (outputList == null)
                outputList = new List<string>();
            if (!String.IsNullOrEmpty(temp))
                input = temp + input;
            string output = "";
            string pattern = @"(?<=^\[length=)(\d+)(?=\])";
            int length;
            if (Regex.IsMatch(input, pattern))
            {
                Match m = Regex.Match(input, pattern);
                // 获取消息字符串实际应有的长度
                length = Convert.ToInt32(m.Groups[0].Value);
                // 获取需要进行截取的位置
                int startIndex = input.IndexOf(']') + 1;
                // 获取从此位置开始后所有字符的长度
                output = input.Substring(startIndex);
                if (output.Length == length)
                {
                    // 如果 output 的长度与消息字符串的应有长度相等
                    // 说明刚好是完整的一条信息
                    outputList.Add(output);
                    temp = "";
                }
                else if (output.Length < length)
                {
                    // 如果之后的长度小于应有的长度，
                    // 说明没有发完整，则应将整条信息，包括元数据，全部缓存
                    // 与下一条数据合并起来再进行处理
                    temp = input;
                    // 此时程序应该退出，因为需要等待下一条数据到来才能继续处理
                }
                else if (output.Length > length)
                {
                    // 如果之后的长度大于应有的长度，
                    // 说明消息发完整了，但是有多余的数据
                    // 多余的数据可能是截断消息，也可能是多条完整消息
                    // 截取字符串
                    output = output.Substring(0, length);
                    outputList.Add(output);
                    temp = "";
                    // 缩短 input 的长度
                    input = input.Substring(startIndex + length);
                    // 递归调用
                    GetActualString(input, outputList); 
                }
            }
            else
            { // 说明“[”， “]”就不完整
                temp = input;
            }
            return outputList.ToArray();
        }
        private void PrintOutput(string input)
        {
            Console.WriteLine(input);
            string[] outputArray = GetActualString(input);
            foreach (string output in outputArray)
            {
                Console.WriteLine(output);
            }
            Console.WriteLine();
        }
    }
}
