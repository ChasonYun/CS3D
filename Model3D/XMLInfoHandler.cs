using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Text.RegularExpressions;

namespace CS3D
{
    public class XMLInfoHandler
    {
        private static volatile XMLInfoHandler instance = null;
        private static readonly object obj_Lock = new object();

        #region 信息提示事件
        public delegate void HintEventHandler(string hint);
        /// <summary>
        /// 信息提示事件
        /// </summary>
        /// <param name="state"></param>
        public event HintEventHandler HintEvent;
        #endregion

        private string nodeName = "/Config/GoodsPath/";

        public string NodeName { get => nodeName; set => nodeName = value; }

        private XMLInfoHandler()
        {

        }

        public static XMLInfoHandler GetCalcPathHandler()
        {
            if (instance == null)
            {
                lock (obj_Lock)
                {
                    if (instance == null)
                    {
                        instance = new XMLInfoHandler();
                    }
                }
            }
            return instance;
        }

        public Queue<Point3D> GetGoodsPath(string goodsName)
        {
            Queue<Point3D> point3Ds = new Queue<Point3D>();
            List<Point3D> listP = new List<Point3D>();
            try
            {
                //string temp = XmlHelper.Instance.GetXmlFromFile(XmlHelper.Instance.XmlPath, NodeName + goodsName).Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("{", "").Replace("}", "").Replace(" ", "");
                //string[] tempStrs = temp.Split(';');
                //foreach (string str in tempStrs)
                //{
                //    temp = str.Replace("(", "").Replace(")", "");
                //    string[] _tempStrs = temp.Split(',');

                //    Point3D point3D = new Point3D(Convert.ToDouble(_tempStrs[0]), Convert.ToDouble(_tempStrs[1]), Convert.ToDouble(_tempStrs[2]));
                //    listP.Add(point3D);
                //}
                //Point3D formerP = new Point3D();
                //Point3D latterP = new Point3D();
                //for (int i = 0; i < listP.Count - 1; i++)
                //{
                //    formerP = listP[i];
                //    latterP = listP[i + 1];
                //    for (int j = 0; j < 5; j++)
                //    {
                //        Point3D tempP = new Point3D((latterP.X - formerP.X) / 5 * (j + 1), (latterP.Y - formerP.Y) / 5 * (j + 1), (latterP.Z - formerP.Z) / 5 * (j + 1));
                //        point3Ds.Enqueue(tempP);
                //    }
                //}

            }
            catch (Exception ex)
            {
                if (HintEvent != null)
                {
                    HintEvent(ex.ToString());
                }
            }
            return point3Ds;
        }
    }
}
