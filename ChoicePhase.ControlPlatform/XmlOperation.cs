using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using ChoicePhase.PlatformModel;
using ChoicePhase.PlatformModel.DataItemSet;
using ChoicePhase.PlatformModel.GetViewData;

namespace ChoicePhase.ControlPlatform
{
    public class XmlOperation
    {
        private PlatformModelServer modelServer;
        private SwitchPropertyIndex _attributeIndex = SwitchPropertyIndex.SwitchList;

        XmlDocument document;
        String filePath = ".\\STUTEMPLATE.icd";

        public XmlOperation(PlatformModelServer modelServer)
        {
            this.modelServer = modelServer;
            UserData = modelServer.MonitorData.ReadAttribute(_attributeIndex, false);

            document = new XmlDocument();
            document.Load(filePath);
            
            path = new Dictionary<string, string>();
        }

        private ObservableCollection<SwitchProperty> _userData;
        /// <summary>
        /// 用户信息数据
        /// </summary>
        public ObservableCollection<SwitchProperty> UserData
        {
            get
            {
                return _userData;
            }
            set
            {
                _userData = value;
            }
        }

        private Dictionary<string,string> path;
        /// <summary>
        /// key:路径 value:修改的值
        /// </summary>
        public Dictionary<string,string> Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }

        /// <summary>
        /// 更新STU
        /// </summary>
        /// <param name="index"></param>
        private void UpdateSTU(string newSTUName)
        {
            //更新STU
            XmlNodeList connectedNodeList = document.GetElementsByTagName("ConnectedAP");
            for (int i = 0; i < connectedNodeList.Count; i++)
            {
                XmlNode currNode = connectedNodeList.Item(i);
                XmlAttributeCollection attr = currNode.Attributes;
                XmlAttribute attribute = attr["iedName"];
                attribute.Value = newSTUName;
            }

            XmlNodeList iedNodeList = document.GetElementsByTagName("IED");
            for (int i = 0; i < iedNodeList.Count; i++)
            {
                XmlNode currNode = iedNodeList.Item(i);
                XmlAttributeCollection attr = currNode.Attributes;
                XmlAttribute attribute = attr["name"];
                attribute.Value = newSTUName;
            }

            XmlNodeList gseControlNodeList = document.GetElementsByTagName("GSEControl");
            for (int i = 0; i < gseControlNodeList.Count; i++)
            {
                XmlNode currNode = gseControlNodeList.Item(i);
                XmlAttributeCollection attr = currNode.Attributes;
                XmlAttribute attribute = attr["appID"];
                attribute.Value = newSTUName + "PIGO/LLN0$GO$gocb0";
            }
        }


        /// <summary>
        /// 更新Appid
        /// </summary>
        /// <param name="index"></param>
        private void UpdateAppid(int index)
        {
            string newAPPIDValue = index.ToString();

            XmlNodeList appidNodeList = document.GetElementsByTagName("P");
            for (int i = 0; i < appidNodeList.Count; i++)
            {
                XmlNode currNode = appidNodeList.Item(i);
                XmlAttributeCollection attr = currNode.Attributes;
                for (int j = 0; j < attr.Count; j++)
                {
                    XmlNode currAttr = attr[j];
                    if (currAttr.Value.Equals("APPID"))
                    {
                        if (currNode.ChildNodes.Count > 0)
                        {
                            XmlNodeList childNodeList = currNode.ChildNodes;
                            for (int k = 0; k < childNodeList.Count; k++)
                            {
                                XmlNode childNode = childNodeList.Item(k);
                                if (childNode.NodeType == XmlNodeType.Text)
                                {
                                    childNode.InnerText = newAPPIDValue;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新Path值
        /// </summary>
        /// <param name="prefix">前缀STU/CPU</param>
        /// <param name="userData">当前的userData</param>
        private void SetPath(string prefix, SwitchProperty userData)
        {
            Path.Clear();
            Path.Add(prefix + "LD0/LPHD1$SwitchType$stVal", ((int)userData.Type).ToString());
            Path.Add(prefix + "LD0/LPHD1$ID$stVal", userData.APPID.ToString());    //开关ID
            Path.Add(prefix + "LD0/LPHD1$CountN$stVal", userData.NCount.ToString());  //N侧邻居个数
            Path.Add(prefix + "LD0/LPHD1$CountM$stVal", userData.MCount.ToString());  //M侧邻居个数

            string neibourCollect = SetNeiMAndNeiN(userData);

            string[] splitNeibourColl = neibourCollect.Split(new char[] { '，', ',' });
            for (int i = 0; i < splitNeibourColl.Length; i++)
            {
                int configId = int.Parse(splitNeibourColl[i]);
                //根据邻居N+M列表获取APPID
                string newValue = UserData[configId - 1].APPID.ToString();
                string post = "LD0/LPHD1$ID" + (i + 1).ToString() + "$stVal";
                Path.Add(prefix + post, newValue);
            }
        }


        /// <summary>
        /// 获取邻居列表邻居 = M邻居 + N邻居
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        private string SetNeiMAndNeiN(SwitchProperty userData)
        {
            //获取邻居列表
            string neibourCollect = null;
            if (userData.MCount != 0 && userData.NCount != 0)
            {
                neibourCollect = userData.N + "," + userData.M;
            }
            if (userData.MCount == 0)
            {
                neibourCollect = userData.N;
            }

            if (userData.NCount == 0)
            {
                neibourCollect = userData.M;
            }
            return neibourCollect;
        }


        /// <summary>
        /// 更新val值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="userData"></param>
        /// <param name="ipInt"></param>
        private void UpdateVal(string prefix, SwitchProperty userData)
        {
            SetPath(prefix, userData);
            
            foreach (var pathList in Path.Keys)
            {
                bool isFoundSTU = false;

                String IntechDevice = pathList.Substring(0, pathList.IndexOf("/"));
                String STU = IntechDevice.Substring(0, IntechDevice.Length - 3);
                String LD0 = IntechDevice.Substring(IntechDevice.Length - 3);
                String postStr = pathList.Substring(pathList.IndexOf("/") + 1);
                String[] splitPath = postStr.Split('$');

                XmlNodeList rootList = (XmlNodeList)document.GetElementsByTagName("IED");
                for (int i = 0; i < rootList.Count; i++)
                {
                    XmlNode currNode = rootList.Item(i);
                    if (currNode.NodeType == XmlNodeType.Element)
                    {
                        XmlAttributeCollection attr = currNode.Attributes;

                        for (int j = 0; j < attr.Count; j++)
                        {
                            if (attr[j].Value.Equals(STU))
                            {
                                isFoundSTU = true;
                                break;
                            }
                        }

                        if (isFoundSTU == true)
                        {
                            XmlNode node = SearchNodeByAttr(currNode, LD0);
                            if (node != null)
                            {
                                XmlNode childNode = SearchLNByAttrCollect(node, splitPath[0]);
                                if (childNode != null)
                                {
                                    int m = 1;
                                    while (m < splitPath.Length)
                                    {
                                        childNode = SearchNodeByAttr(childNode, splitPath[m]);
                                        m++;
                                    }
                                    if (childNode != null)
                                    {
                                        XmlNodeList grandNodeList = childNode.ChildNodes;
                                        for (int l = 0; l < grandNodeList.Count; l++)
                                        {
                                            XmlNode currgrandNode = grandNodeList.Item(l);

                                            if (currgrandNode.ChildNodes.Count > 0)
                                            {
                                                XmlNodeList lastNodeList = currgrandNode.ChildNodes;
                                                for (int k = 0; k < lastNodeList.Count; k++)
                                                {
                                                    if (lastNodeList.Item(k).NodeType == XmlNodeType.Text)
                                                    {
                                                        string newValue = null;
                                                        var result = Path.TryGetValue(pathList, out newValue);
                                                        lastNodeList.Item(k).Value = newValue;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        public void UpdateMac(string newValue)
        {
            try
            {
                    XmlNodeList appidNodeList = document.GetElementsByTagName("P");
                    for (int i = 0; i < appidNodeList.Count; i++)
                    {
                        XmlNode currNode = appidNodeList.Item(i);
                        XmlAttributeCollection attr = currNode.Attributes;
                        for (int j = 0; j < attr.Count; j++)
                        {
                            XmlNode currAttr = attr[j];
                            if (currAttr.Value.Equals("MAC-Address"))
                            {
                                if (currNode.ChildNodes.Count > 0)
                                {
                                    XmlNodeList childNodeList = currNode.ChildNodes;
                                    for (int k = 0; k < childNodeList.Count; k++)
                                    {
                                        XmlNode childNode = childNodeList.Item(k);
                                        if (childNode.NodeType == XmlNodeType.Text)
                                        {
                                            childNode.InnerText = newValue;
                                        }
                                    }
                                }
                            }
                        }
                    }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void UpdateVANID(string newVanId)
        {
            try
            {
                XmlNodeList appidNodeList = document.GetElementsByTagName("P");
                for (int i = 0; i < appidNodeList.Count; i++)
                {
                    XmlNode currNode = appidNodeList.Item(i);
                    XmlAttributeCollection attr = currNode.Attributes;
                    for (int j = 0; j < attr.Count; j++)
                    {
                        XmlNode currAttr = attr[j];
                        if (currAttr.Value.Equals("VLAN-ID"))
                        {
                            if (currNode.ChildNodes.Count > 0)
                            {
                                XmlNodeList childNodeList = currNode.ChildNodes;
                                for (int k = 0; k < childNodeList.Count; k++)
                                {
                                    XmlNode childNode = childNodeList.Item(k);
                                    if (childNode.NodeType == XmlNodeType.Text)
                                    {
                                        childNode.InnerText = newVanId;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private void UpdateFCDA(int count)
        {
            try
            {
                XmlNodeList fcdaNodeList = document.GetElementsByTagName("FCDA");
                //寻找父节点
                XmlNode parentNode = null;
                for (int i = 0; i < fcdaNodeList.Count; i++)
                {
                    XmlNode currNode = fcdaNodeList.Item(i);
                    XmlAttributeCollection attr = currNode.Attributes;
                    parentNode = currNode.ParentNode;
                    break;
                }
                //多于规定数删除
                if (fcdaNodeList.Count > count)
                {
                    int oldFCDACount = fcdaNodeList.Count;
                    while(oldFCDACount > count)
                    {
                        parentNode.RemoveChild(parentNode.LastChild);
                        oldFCDACount--;
                    }
                }
                //少于规定数增加
                else if(fcdaNodeList.Count < count)
                {
                    int oldCount = fcdaNodeList.Count;
                    while (oldCount < count)
                    {
                        oldCount++;
                        XmlElement newNode = document.CreateElement("FCDA", document.DocumentElement.NamespaceURI);
                        newNode.SetAttribute("daName", "general");
                        newNode.SetAttribute("doName", "Tr");
                        newNode.SetAttribute("fc", "ST");
                        newNode.SetAttribute("ldInst", "PIGO");
                        newNode.SetAttribute("lnClass", "PTRC");
                        newNode.SetAttribute("lnInst", oldCount.ToString());
                        parentNode.AppendChild(newNode);
                    }
                    
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// 修改最大时间
        /// 修改最大时间
        /// </summary>
        /// <param name="newMaxTimeName"></param>
        private void UpdateMaxTime(string newMaxTimeName)
        {
            try
            {
                XmlNodeList gseControlNodeList = document.GetElementsByTagName("GSEControl");
                for (int i = 0; i < gseControlNodeList.Count; i++)
                {
                    XmlNode currNode = gseControlNodeList.Item(i);
                    XmlAttributeCollection attr = currNode.Attributes;
                    XmlAttribute attribute = attr["maxTime"];
                    attribute.Value = newMaxTimeName;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        /// <summary>
        ///  更新xml语句
        /// </summary>
        /// <param name="path">生成文件存储位置</param>
        /// <param name="lnNum">FCDA扩展个数</param>
        public void UpdateXml(string path, int lnNum, string vanId)
        {
            foreach (var userData in UserData)
            {
                try
                {
                    uint ipInt;
                    IpToInt(userData.IP, out ipInt);
                    int FileNameIndex = (int)ipInt & 0xff;  //文件名
                    UpdateSTU(userData.Addr);       //修改STU
                    UpdateMac(userData.MACAddress);      //修改MAC地址
                    UpdateVANID(vanId);                 //修改VANID
                    UpdateAppid(userData.APPID);     //修改APPID
                    UpdateVal(userData.Addr, userData);
                    UpdateFCDA(lnNum);    //扩展FCDA
                    UpdateMaxTime(userData.MaxTime.ToString());     //修改最大时间
                    string destfileName = "\\stu" + FileNameIndex.ToString() + ".icd";
                    string destfilePath = path + destfileName;
                    document.Save(destfilePath);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        /// <summary>
        /// 根据属性值查找节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        private XmlNode SearchNodeByAttr(XmlNode node, String attrName)
        {
            XmlNodeList nodeList = node.ChildNodes;
            XmlNode returnNode = null;
            bool isFound = false;
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlNode currNode = nodeList.Item(i);
                if (currNode.NodeType == XmlNodeType.Element)
                {
                    XmlAttributeCollection attr = currNode.Attributes;

                    for (int j = 0; j < attr.Count; j++)
                    {
                        XmlNode attrNode = attr.Item(j);
                        if (attrNode.Value.Equals(attrName))
                        {
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound == true)
                    {
                        returnNode = currNode;
                        break;
                    }
                    else
                    {
                        if (currNode.ChildNodes.Count > 0)
                        {
                            returnNode = SearchNodeByAttr(currNode, attrName);
                        }
                    }
                }
            }
            return returnNode;
        }


        /// <summary>
        /// 根据属性值查找LN
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        private XmlNode SearchLNByAttrCollect(XmlNode node, String attrName)
        {
            XmlNodeList nodeList = node.ChildNodes;
            XmlNode returnNode = null;
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlNode currNode = nodeList.Item(i);
                if (currNode.NodeType == XmlNodeType.Element)
                {
                    XmlAttributeCollection attr = currNode.Attributes;

                    String LN = "";
                    String lnClass = "";
                    String inst = "";
                    String prefix = "";

                    if (attr["lnClass"] != null)
                    {
                        lnClass = attr["lnClass"].Value;
                    }

                    if (attr["inst"] != null)
                    {
                        inst = attr["inst"].Value.ToString();
                    }

                    if (attr["prefix"] != null)
                    {
                        prefix = attr["prefix"].Value;
                    }

                    LN = prefix + lnClass + inst;
                    if (LN.Equals(attrName))
                    {
                        returnNode = currNode;
                        break;
                    }
                    else
                    {
                        if (currNode.ChildNodes.Count > 0)
                        {
                            returnNode = SearchLNByAttrCollect(currNode, attrName);
                        }
                    }
                }
            }
            return returnNode;
        }


        /// <summary>
        /// 判断十六进制字符串hex是否正确
        /// </summary>
        /// <param name="hex">十六进制字符串</param>
        /// <returns>true：不正确，false：正确</returns>
        public bool IsIllegalHexadecimal(string hex, string pattern)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(hex, pattern);
        }


        /// <summary>
        /// 将ip转换为int
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="ipInt"></param>
        /// <returns></returns>
        public void IpToInt(string ip, out uint ipInt)
        {
            char[] separator = new char[] { '.' };
            string[] items = ip.Split(separator);
            ipInt = uint.Parse(items[0]) << 24
                    | uint.Parse(items[1]) << 16
                    | uint.Parse(items[2]) << 8
                    | uint.Parse(items[3]);
        }
    }
}
