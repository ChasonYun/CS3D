using System;
using System.Windows.Media.Media3D;

namespace CS3D
{
    public class ProductInfo
    {
        public delegate void HintEventHandler(string msg);

        public static event HintEventHandler HintEvent;
        public ProductInfo()
        {

        }

        /// <summary>
        /// 货位  01.01.01 （补齐两位 split by  dot）
        /// </summary>
        /// <param name="shelfNo">库位号  排 层 列 </param>
        public ProductInfo(string shelfNo)
        {
            try
            {
                string[] shelfNo_ = shelfNo.Trim().Split('.');
                line = Convert.ToInt32(shelfNo_[0]);

                level = Convert.ToInt32(shelfNo_[1]);

                column = Convert.ToInt32(shelfNo_[2]);
            }
            catch (Exception ex)
            {
                throw new Exception("prouctId is illegal" + ex.ToString());
            }
        }

        /// <summary>
        /// 库位信息
        /// </summary>
        /// <param name="shelfNo">库位号</param>
        /// <param name="shelfState">库位状态</param>
        /// <param name="productName">货物名称</param>
        /// <param name="productId">货物Id</param>
        /// <param name="lastUpTime">时间</param>
        public ProductInfo(string shelfNo, string shelfState, string productName, string productId, string lastUpTime)
        {
            try
            {
                this.shelfNo = shelfNo;
                this.shelfState = Convert.ToInt32(shelfState);
                this.productName = productName;
                this.productId = Convert.ToString(productId);
                this.lastUpTime = Convert.ToDateTime(lastUpTime);
            }
            catch (Exception ex)
            {

                throw new Exception("productInfo is illegal" + ex.ToString());
            }
        }

        /// <summary>
        /// 库位号
        /// </summary>
        /// <param name="line">排</param>
        /// <param name="column">层</param>
        /// <param name="level">列</param>
        public ProductInfo(int line, int level, int column)
        {
            this.line = line;
            this.column = column;
            this.level = level;
            this.shelfNo = line.ToString().PadLeft(2, '0') + "." + level.ToString().PadLeft(2, '0') + "." + column.ToString().PadLeft(2, '0');
        }

        private string shelfNo;//货位信息

        /// <summary>
        /// 货位号
        /// </summary>
        public string ShelfNo { get => shelfNo; set => shelfNo = value; }
        /// <summary>
        /// 偏移量
        /// </summary>
        public Point3D ProductOffSet { get => productOffSet; set => productOffSet = value; }
        /// <summary>
        /// 货物id
        /// </summary>
        public string ProductId { get => productId; set => productId = value; }
        /// <summary>
        /// 货物名称
        /// </summary>
        public string ProductName { get => productName; set => productName = value; }
        /// <summary>
        /// 货位状态
        /// </summary>
        public int ShelfState { get => shelfState; set => shelfState = value; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime LastUpTime { get => lastUpTime; set => lastUpTime = value; }
        public ModelVisual3D Model { get => model; set => model = value; }


        private int line;


        private int level;


        private int column;


        private string productName;

        private string productId;
        /// <summary>
        /// 货物在世界坐标系中的偏移量
        /// </summary>
        private Point3D productOffSet = new Point3D();


        private int shelfState;

        private DateTime lastUpTime;

        private ModelVisual3D model;

    }

    /// <summary>
    /// 维护 堆垛机部件 位置
    /// </summary>
    public class StackerPartsInfo
    {
        string modelName;
        private Point3D stackerOffSet;
        public StackerPartsInfo(string modelName)
        {
            this.ModelName = modelName;
        }

        public string ModelName { get => modelName; set => modelName = value; }
        public Point3D StackerOffSet { get => stackerOffSet; set => stackerOffSet = value; }
    }


    public class ShelfInfo
    {
        string modelName;
        private Point3D shelfOffSet;
        private ModelVisual3D model;
        public ShelfInfo(string modelName)
        {
            this.ModelName = modelName;
        }

        public Point3D ShelfOffSet { get => shelfOffSet; set => shelfOffSet = value; }
        public ModelVisual3D Model { get => model; set => model = value; }
        public string ModelName { get => modelName; set => modelName = value; }
    }

    public class ModelPosition
    {


        private Point3D productOriPos;


        private Point3D stockInEntranceOriPos;

        private Point3D shelfOriPos;

        private double shelfDistance;


        private Point3D stackerOriPos_1;


        private Point3D platformOriPos_1;


        private Point3D topForkOriPos_1;


        private Point3D middleForkOriPos_1;


        private Point3D bottomForkOriPos_1;


        private Point3D vIFS_1;



        private Point3D stackerOriPos_2;


        private Point3D platformOriPos_2;


        private Point3D topForkOriPos_2;


        private Point3D middleForkOriPos_2;


        private Point3D bottomForkOriPos_2;


        private Point3D vIFS_2;
        public ModelPosition()
        {
            try
            {
                string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "ProductOriPos");
                string[] tempStrs = tempStr.Trim().Split(',');
                productOriPos = new Point3D(Convert.ToDouble(tempStrs[0]), Convert.ToDouble(tempStrs[1]), Convert.ToDouble(tempStrs[2]));

                tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "StockInEntranceOriPos");
                tempStrs = tempStr.Trim().Split(',');
                stockInEntranceOriPos = new Point3D(Convert.ToDouble(tempStrs[0]), Convert.ToDouble(tempStrs[1]), Convert.ToDouble(tempStrs[2]));

                tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "ShelfOriPos");
                tempStrs = tempStr.Trim().Split(',');
                shelfOriPos = new Point3D(Convert.ToDouble(tempStrs[0]), Convert.ToDouble(tempStrs[1]), Convert.ToDouble(tempStrs[2]));

                tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "ShelfDistance");
                shelfDistance = Convert.ToDouble(tempStr.Trim());

                tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "StackerOriPos_1");
                tempStrs = tempStr.Trim().Split(',');
                stackerOriPos_1 = new Point3D(Convert.ToDouble(tempStrs[0]), Convert.ToDouble(tempStrs[1]), Convert.ToDouble(tempStrs[2]));

                tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "PlatformOriPos_1");
                tempStrs = tempStr.Trim().Split(',');
                platformOriPos_1 = new Point3D(Convert.ToDouble(tempStrs[0]), Convert.ToDouble(tempStrs[1]), Convert.ToDouble(tempStrs[2]));

                tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "TopForkOriPos_1");
                tempStrs = tempStr.Trim().Split(',');
                topForkOriPos_1 = new Point3D(Convert.ToDouble(tempStrs[0]), Convert.ToDouble(tempStrs[1]), Convert.ToDouble(tempStrs[2]));

                tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "MiddleForkOriPos_1");
                tempStrs = tempStr.Trim().Split(',');
                middleForkOriPos_1 = new Point3D(Convert.ToDouble(tempStrs[0]), Convert.ToDouble(tempStrs[1]), Convert.ToDouble(tempStrs[2]));

                tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "BottomForkOriPos_1");
                tempStrs = tempStr.Trim().Split(',');
                bottomForkOriPos_1 = new Point3D(Convert.ToDouble(tempStrs[0]), Convert.ToDouble(tempStrs[1]), Convert.ToDouble(tempStrs[2]));

                tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "VIFS_1");
                tempStrs = tempStr.Trim().Split(',');
                vIFS_1 = new Point3D(Convert.ToDouble(tempStrs[0]), Convert.ToDouble(tempStrs[1]), Convert.ToDouble(tempStrs[2]));


                tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "StackerOriPos_2");
                tempStrs = tempStr.Trim().Split(',');
                stackerOriPos_2 = new Point3D(Convert.ToDouble(tempStrs[0]), Convert.ToDouble(tempStrs[1]), Convert.ToDouble(tempStrs[2]));

                tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "PlatformOriPos_2");
                tempStrs = tempStr.Trim().Split(',');
                platformOriPos_2 = new Point3D(Convert.ToDouble(tempStrs[0]), Convert.ToDouble(tempStrs[1]), Convert.ToDouble(tempStrs[2]));

                tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "TopForkOriPos_2");
                tempStrs = tempStr.Trim().Split(',');
                topForkOriPos_2 = new Point3D(Convert.ToDouble(tempStrs[0]), Convert.ToDouble(tempStrs[1]), Convert.ToDouble(tempStrs[2]));

                tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "MiddleForkOriPos_2");
                tempStrs = tempStr.Trim().Split(',');
                middleForkOriPos_2 = new Point3D(Convert.ToDouble(tempStrs[0]), Convert.ToDouble(tempStrs[1]), Convert.ToDouble(tempStrs[2]));

                tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "BottomForkOriPos_2");
                tempStrs = tempStr.Trim().Split(',');
                bottomForkOriPos_2 = new Point3D(Convert.ToDouble(tempStrs[0]), Convert.ToDouble(tempStrs[1]), Convert.ToDouble(tempStrs[2]));

                tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + "VIFS_2");
                tempStrs = tempStr.Trim().Split(',');
                vIFS_2 = new Point3D(Convert.ToDouble(tempStrs[0]), Convert.ToDouble(tempStrs[1]), Convert.ToDouble(tempStrs[2]));

            }
            catch (Exception ex)
            {
                throw new Exception("ModelPosition initialize has Exception" + ex.ToString());
            }
        }

        /// <summary>
        /// 货物模型起点绝对坐标
        /// </summary>
        public Point3D ProductOriPos { get => productOriPos; set => productOriPos = value; }
        /// <summary>
        /// 入库口模型起点绝对坐标
        /// </summary>
        public Point3D StockInEntrance { get => stockInEntranceOriPos; set => stockInEntranceOriPos = value; }
        /// <summary>
        /// 堆垛机立柱 1 绝对坐标
        /// </summary>
        public Point3D StackerOriPos_1 { get => stackerOriPos_1; set => stackerOriPos_1 = value; }
        /// <summary>
        /// 载货台 1 绝对坐标
        /// </summary>
        public Point3D PlatformOriPos_1 { get => platformOriPos_1; set => platformOriPos_1 = value; }
        /// <summary>
        /// 上叉 1  绝对坐标
        /// </summary>
        public Point3D TopForkOriPos_1 { get => topForkOriPos_1; set => topForkOriPos_1 = value; }
        /// <summary>
        /// 中叉 1 绝对坐标
        /// </summary>
        public Point3D MiddleForkOriPos_1 { get => middleForkOriPos_1; set => middleForkOriPos_1 = value; }
        /// <summary>
        /// 下叉 1  绝对坐标
        /// </summary>
        public Point3D BottomForkOriPos_1 { get => bottomForkOriPos_1; set => bottomForkOriPos_1 = value; }
        /// <summary>
        /// 堆垛机立柱 2 绝对坐标
        /// </summary>
        public Point3D StackerOriPos_2 { get => stackerOriPos_2; set => stackerOriPos_2 = value; }
        /// <summary>
        /// 载货台 2 绝对坐标
        /// </summary>
        public Point3D PlatformOriPos_2 { get => platformOriPos_2; set => platformOriPos_2 = value; }
        /// <summary>
        /// 上叉 2  绝对坐标
        /// </summary>
        public Point3D TopForkOriPos_2 { get => topForkOriPos_2; set => topForkOriPos_2 = value; }
        /// <summary>
        /// 中叉 2 绝对坐标
        /// </summary>
        public Point3D MiddleForkOriPos_2 { get => middleForkOriPos_2; set => middleForkOriPos_2 = value; }
        /// <summary>
        /// 下叉 2  绝对坐标
        /// </summary>
        public Point3D BottomForkOriPos_2 { get => bottomForkOriPos_2; set => bottomForkOriPos_2 = value; }

        /// <summary>
        /// 货架 01.01.01 绝对坐标
        /// </summary>
        public Point3D ShelfOriPos { get => shelfOriPos; set => shelfOriPos = value; }
        /// <summary>
        /// VIFS_1
        /// </summary>
        public Point3D VIFS_1 { get => vIFS_1; set => vIFS_1 = value; }
        /// <summary>
        /// VIFS_2
        /// </summary>
        public Point3D VIFS_2 { get => vIFS_2; set => vIFS_2 = value; }

        /// <summary>
        /// 货架X间距
        /// </summary>
        public double ShelfDistance { get => shelfDistance; set => shelfDistance = value; }

        /// <summary>
        /// 获取指定库位号 相对于 原始货物模型的 相对位移
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <returns></returns>
        public Point3D Get_ShelfState_1_OffSet(string shelfNo)
        {

            double x = shelfOriPos.X;
            double y = shelfOriPos.Y;
            double z = shelfOriPos.Z;
            string[] tempStrs = shelfNo.Trim().Split('.');
            int line = Convert.ToInt32(tempStrs[0]);
            int level = Convert.ToInt32(tempStrs[1]);
            int column = Convert.ToInt32(tempStrs[2]);
            if (level == 1)
            {
                z = 1347.0;
            }
            else if (level == 2)
            {
                z = 2411.056;
                if(line==3 || line == 6)
                {
                    z = 2571.928;
                }
            }
            else if (level == 3)
            {
                z = 3911.056;
                if (line == 3 || line == 6)
                {
                    z = 4071.928;
                }
            }
            else if (level == 4)
            {
                z = 5411.056;
                if (line == 3 || line == 6)
                {
                    z = 5571.928;
                }
            }
            else if (level == 5)
            {
                z = 6911.056;
                if (line == 3 || line == 6)
                {
                    z = 7071.928;
                }
            }

            if (line == 1)
            {
                y = -1999.992;
            }
            else if (line == 2)
            {
                y = -4662.488;
            }
            else if (line == 3)
            {
                y = -6242.315;
            }
            else if (line == 4)
            {
                y = -7578.094;
            }
            else if (line == 5)
            {
                y = -10240.591;
            }
            else if (line == 6)
            {
                y = -11549.55;
            }
            if ((column & 1) == 1)//奇
            {
                x = x - ((column - 1) / 2) * 2390.43;
            }
            else //偶
            {
                x = x - (column / 2 - 1) * 2390.43 - 1117.715;
            }
            //if (column == 1)
            //{
            //    x = 12694.44;
            //}
            //else if (column == 2)
            //{
            //    x = 11576.725;
            //}
            //else if (column == 3)
            //{
            //    x = 10304.44;
            //}
            //else if (column == 4)
            //{
            //    x = 9186.725;
            //}
            //else if (column == 5)
            //{
            //    x = 7914.44;
            //}
            Point3D point3D = new Point3D(x - productOriPos.X, y - productOriPos.Y, z - productOriPos.Z);
            return point3D;
        }

        /// <summary>
        /// 获取货物模型的绝对坐标
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <returns></returns>
        public Point3D GetShelfPos(string shelfNo)
        {
            Point3D offset = Get_ShelfState_1_OffSet(shelfNo);
            return new Point3D(offset.X + productOriPos.X, offset.Y + productOriPos.Y, offset.Z + productOriPos.Z);
        }

        /// <summary>
        /// 解析 排层列 序号
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <param name="line"></param>
        /// <param name="level"></param>
        /// <param name="column"></param>
        public void Get_Line_Level_Column(string shelfNo, out int line, out int level, out int column)
        {
            string[] shelfNo_ = shelfNo.Trim().Split('.');
            line = Convert.ToInt32(shelfNo_[0]);
            level = Convert.ToInt32(shelfNo_[1]);
            column = Convert.ToInt32(shelfNo_[2]);
        }

        /// <summary>
        /// 获取 对应的货架口的绝对位置信息
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <returns></returns>
        public Point3D GetShelfLinePos(string shelfNo)
        {
            int line, level, column;
            Get_Line_Level_Column(shelfNo, out line, out level, out column);
            if (line.Equals(1) || line.Equals(2))
            {
                return new Point3D(GetShelfPos(shelfNo).X, TopForkOriPos_1.Y, GetShelfPos(shelfNo).Z);
            }
            else if (line.Equals(3) || line.Equals(4) || line.Equals(5) || line.Equals(6))
            {
                return new Point3D(GetShelfPos(shelfNo).X, TopForkOriPos_2.Y, GetShelfPos(shelfNo).Z);
            }
            else
            {
                throw new Exception("无法识别的货位号" + "GetShelfLinePos()");
            }
        }
    }
}
