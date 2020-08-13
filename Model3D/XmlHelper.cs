using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CS3D
{
   

    /// <summary>
    /// XML配置文件操作类
    /// </summary>
    public class XmlHelper
    {
        #region 信息提示事件
        public delegate void HintEventHandler(string hint);
        /// <summary>
        /// 信息提示事件
        /// </summary>
        /// <param name="state"></param>
        public event HintEventHandler HintEvent;
        #endregion

        #region object
        public static XmlDocument xmlDoc; //读取xml配置文件
        #endregion

        #region variables
        public string xmlPath = "Config.xml";
        #endregion

        #region 单例模式
        private static readonly XmlHelper instance = null;
        static XmlHelper()
        {
            instance = new XmlHelper();
        }
        private XmlHelper()
        {
        }
        public static XmlHelper Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        /// <summary>
        /// 获取XML节点信息
        /// </summary>
        /// <param name="arg">xpath</param>
        /// <returns></returns>
        public string GetXMLInformation(string arg)
        {
            string value = "";
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlPath);
                XmlNode xnl = xmlDoc.SelectSingleNode(arg);
                value = xnl.InnerText;
            }
            catch (Exception ex)
            {
                if (HintEvent != null) HintEvent(string.Format("{0}-{1}", arg, ex.ToString()));
            }
            return value;
        }

        /// <summary>
        /// 从指定xml文件中获取参数
        /// </summary>
        /// <param name="file"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public string GetXmlFromFile(string file, string arg)
        {
            string value = "";
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(file);
                XmlNode xnl = xmlDoc.SelectSingleNode(arg);
                value = xnl.InnerText;
            }
            catch (Exception ex)
            {
                if (HintEvent != null) HintEvent(ex.ToString());
            }
            return value;
        }

        /// <summary>
        /// 修改XML节点信息
        /// </summary>
        /// <param name="node">xpath</param>
        /// <param name="arg">值</param>
        public void UpdateXMLInformation(string node, string arg)
        {
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlPath);
                XmlNode xnl = xmlDoc.SelectSingleNode(node);
                xnl.InnerText = arg;
                xmlDoc.Save(xmlPath);
            }
            catch (Exception ex)
            {
                if (HintEvent != null) HintEvent(ex.ToString());
            }
        }
    }
}
