using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media.Media3D;

namespace CS3D
{
    /// <summary>
    /// 任务类型 
    /// </summary>
    public enum MissionType
    {
        [Description("出库")]// 货架 到 出货口
        ProdOut = 1,
        [Description("入库")]// 入货口 到 货架
        ProdIn,
        [Description("移库")]// 货架 到 货架
        ProdTransfer,
        [Description("回流")]//出货口 到 货架
        BackIn,
        [Description("托盘出库")]//拆盘机要托盘（货架 到 拆盘机）
        PalletOut,
        [Description("托盘入库")]//叠盘机 入库（叠盘机 到 货架）
        PalletIn,
        [Description("拆盘")]//拆盘机 到 入库口
        GetPallet,
        [Description("叠盘")]//出库口 到 叠盘机
        AddPallet
    }

    public delegate void MissionSuccess(IMission mission, MissionType missionType, string shelfNo);
    public interface IMission : IDisposable//观察者模式也可以使用 delegate 简化  接口 
    {
        event MissionSuccess MisSuccess;
        void Update(string missionId, string conveyorName, bool IsReady);
    }

    /// <summary>
    ///  offset
    /// </summary>
    public class Mission : IMission, IDisposable
    {
        public event MissionSuccess MisSuccess;
        string missionId;//任务号
        MissionType missionType;//任务类型  //  入库  出库  
        string startShelfNo;//起点 货架号
        string endShelfNo;//终点货架号
        string shelfNo;
        int endLine;
        int startLine;
        ModelPosition modelPosition;

        public Point3D totalOffSet;
        private Point3D worldPosition;

        private Point3D productStackerOriPos;

        public Point3D cameraPosition;
        public Vector3D cameraLookDirection;
        public Vector3D cameraUpDirection;


        Timer timer = new Timer();

        List<Conveyor> pathNodes = new List<Conveyor>();//改任务需要走过的 传送带 序号


        Dictionary<string, ProductInfo> product_Info;
        Dictionary<string, StackerPartsInfo> stackerParts_Info;



        /// <summary>
        /// 任务构造函数
        /// </summary>
        /// <param name="missionType">任务类型 StockIn StockOut</param>
        /// <param name="shelfNo">库位号</param>
        /// <param name="modelPosition.ProductOriPos">模型起点坐标</param>
        /// <param name="product_Info">货物模型字典</param>
        /// <param name="stackerParts_Info">堆垛机模型字典</param>
        public Mission(string missionId, MissionType missionType, string shelfNo, ModelPosition modelPosition, Dictionary<string, ProductInfo> product_Info, Dictionary<string, StackerPartsInfo> stackerParts_Info)//入库 出库 叠盘 拆盘
        {
            this.missionId = missionId;
            this.modelPosition = modelPosition;
            this.missionType = missionType;
            if (missionType.Equals(MissionType.ProdTransfer))//移库 计算 
            {
                string[] shelfs = shelfNo.Split(' ');
                this.startShelfNo = shelfs[0];
                this.endShelfNo = shelfs[1];
                this.shelfNo = startShelfNo;
            }
            else
            {
                this.endShelfNo = shelfNo;
                this.shelfNo = endShelfNo;
            }
            this.product_Info = product_Info;
            this.stackerParts_Info = stackerParts_Info;
            string[] pathStr = endShelfNo.Split('.');
            endLine = Convert.ToInt32(pathStr[0]);
            if (endLine.Equals(1) || endLine.Equals(2))
            {
                productStackerOriPos = modelPosition.ProductStackerOriPos_1;
            }
            else if (endLine.Equals(3) || endLine.Equals(4) || endLine.Equals(5) || endLine.Equals(6))
            {
                productStackerOriPos = modelPosition.ProductStackerOriPos_2;
            }

            cameraPosition = modelPosition.OriCameraPosition;
            cameraLookDirection = modelPosition.LookDirection;

            GetPath(endShelfNo);//获取该任务的 路径list

            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 50;//一秒 10 次
        }

        private void GetPath(string shelfNo)
        {
            try
            {
                switch (missionType)
                {
                    case MissionType.ProdIn://入库
                        CalcPath_ProdIn(pathNodes, shelfNo);
                        break;
                    case MissionType.ProdOut://出库
                        CalcPath_ProdOut(pathNodes, shelfNo);
                        break;
                    case MissionType.ProdTransfer://移库
                        CalcPath_ProdTransfer(pathNodes, startShelfNo, endShelfNo);
                        break;
                    case MissionType.BackIn://回流
                        CalcPath_BackIn(pathNodes, shelfNo);
                        break;
                    case MissionType.PalletOut://要托盘
                        CalcPath_PalletOut(pathNodes, shelfNo);
                        break;
                    case MissionType.PalletIn://托盘入库
                        CalcPath_PalletIn(pathNodes, shelfNo);
                        break;
                    case MissionType.GetPallet://拆盘
                        CalcPath_GetPallet(pathNodes, shelfNo);
                        break;
                    case MissionType.AddPallet://叠盘
                        CalcPath_AddPallet(pathNodes, shelfNo);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetPath has exception" + ex.ToString());
            }

        }

        private void CalcPath_ProdIn(List<Conveyor> pathNodes, string shelfNo)
        {
            try
            {
                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_1", missionType, shelfNo, Direction_.Vertical, modelPosition));

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_1", missionType, shelfNo, Direction_.Vertical, modelPosition));

                if (endLine.Equals(1) || endLine.Equals(2))
                {

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_4", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_3", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_1", missionType, shelfNo, Direction_.PortIn, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_2", missionType, shelfNo, Direction_.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_3", missionType, shelfNo, Direction_.ShelfIn, modelPosition));

                }
                else if (endLine.Equals(3) || endLine.Equals(4) || endLine.Equals(5) || endLine.Equals(6))
                {
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_8", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_3", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_15", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_5", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_18", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_17", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_1", missionType, shelfNo, Direction_.PortIn, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_2", missionType, shelfNo, Direction_.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_3", missionType, shelfNo, Direction_.ShelfOut, modelPosition));

                }
            }
            catch (Exception ex)
            {
                throw new Exception("CalcPathNode_StockIn has exception:" + ex.ToString());
            }
        }

        private void CalcPath_ProdOut(List<Conveyor> pathNodes, string shelfNo)
        {
            try
            {
                if (!product_Info.ContainsKey(shelfNo))
                {
                    throw new Exception("未检索到货物信息" + shelfNo);
                }
                if (endLine.Equals(1) || endLine.Equals(2))
                {
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_3", missionType, shelfNo, Direction_.ShelfOut, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_2", missionType, shelfNo, Direction_.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_4", missionType, shelfNo, Direction_.PortOut, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_10", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_11", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_12", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_9", missionType, shelfNo, Direction_.Vertical, modelPosition));

                }
                else if (endLine.Equals(3) || endLine.Equals(4) || endLine.Equals(5) || endLine.Equals(6))
                {
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_3", missionType, shelfNo, Direction_.ShelfOut, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_2", missionType, shelfNo, Direction_.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_4", missionType, shelfNo, Direction_.PortOut, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_21", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_22", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_7", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_20", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_6", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_16", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_4", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_9", missionType, shelfNo, Direction_.Vertical, modelPosition));
                }

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_2", missionType, shelfNo, Direction_.Vertical, modelPosition));

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_2", missionType, shelfNo, Direction_.Vertical, modelPosition));
            }
            catch (Exception ex)
            {
                throw new Exception("CalcPathNode_StockOut has exception:" + ex.ToString());
            }
        }

        private void CalcPath_ProdTransfer(List<Conveyor> pathNodes, string startShelfNo, string endShelfNo)
        {
            try
            {
                if (!product_Info.ContainsKey(startShelfNo))
                {
                    throw new Exception("ClacPath_Transfer has exception:{0} does not exit" + startShelfNo);
                }
                if (product_Info.ContainsKey(endShelfNo))
                {
                    throw new Exception("ClacPath_Transfer has exception:{0} already exit" + endShelfNo);
                }
                string[] startPathStr = startShelfNo.Split('.');
                string[] endPathStr = endShelfNo.Split('.');
                if (endLine.Equals(1) || endLine.Equals(2))
                {
                    ///出库  （堆垛机）移动  入库
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_3", missionType, startShelfNo, Direction_.ShelfOut, modelPosition));
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_2", missionType, startShelfNo, endShelfNo, Direction_.Roadway, modelPosition));
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_3", missionType, endShelfNo, Direction_.ShelfIn, modelPosition));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// 回流至 货位
        /// </summary>
        /// <param name="pathNodes"></param>
        /// <param name="shelfNo"></param>
        private void CalcPath_BackIn(List<Conveyor> pathNodes, string shelfNo)
        {
            try
            {
                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_2", missionType, shelfNo, Direction_.Vertical, modelPosition));
                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_2", missionType, shelfNo, Direction_.Vertical, modelPosition));
                if (endLine.Equals(1) || endLine.Equals(2))
                {
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_5", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_4", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_3", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_1", missionType, shelfNo, Direction_.PortIn, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_2", missionType, shelfNo, Direction_.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_3", missionType, shelfNo, Direction_.ShelfIn, modelPosition));
                }
                else if (endLine.Equals(3) || endLine.Equals(4) || endLine.Equals(5) || endLine.Equals(6))
                {
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_5", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_1", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_8", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_3", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_15", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_5", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_18", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_17", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_1", missionType, shelfNo, Direction_.PortIn, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_2", missionType, shelfNo, Direction_.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_3", missionType, shelfNo, Direction_.ShelfIn, modelPosition));
                }

            }
            catch (Exception ex)
            {

                throw new Exception("CalcPath_BackFlow has exception:" + ex.ToString());
            }
        }

        /// <summary>
        /// 拆盘
        /// </summary>
        /// <param name="pathNodes"></param>
        /// <param name="shelfNo"></param>
        private void CalcPath_GetPallet(List<Conveyor> pathNodes, string shelfNo)
        {
            try
            {
                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_14", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_13", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_4", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_9", missionType, shelfNo, Direction_.Vertical, modelPosition));

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_2", missionType, shelfNo, Direction_.Vertical, modelPosition));

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_5", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_1", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_1", missionType, shelfNo, Direction_.Vertical, modelPosition));
            }
            catch (Exception ex)
            {
                throw new Exception("CalcPath_SubtractTray has exception:" + ex.ToString());
            }
        }

        /// <summary>
        /// 要托盘 拆盘机为空
        /// </summary>
        /// <param name="pathNodes"></param>
        /// <param name="shelfNo"></param>
        private void CalcPath_PalletOut(List<Conveyor> pathNodes, string shelfNo)
        {
            try
            {
                if (!product_Info.ContainsKey(shelfNo))
                {
                    throw new Exception("shelfNo_{0} does not exit" + shelfNo);
                }
                if (endLine.Equals(1) || endLine.Equals(2))
                {
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_3", missionType, shelfNo, Direction_.ShelfOut, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_2", missionType, shelfNo, Direction_.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_4", missionType, shelfNo, Direction_.PortOut, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_10", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_11", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_12", missionType, shelfNo, Direction_.Horizontal, modelPosition));



                }
                else if (endLine.Equals(3) || endLine.Equals(4) || endLine.Equals(5) || endLine.Equals(6))
                {
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_3", missionType, shelfNo, Direction_.ShelfOut, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_2", missionType, shelfNo, Direction_.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_4", missionType, shelfNo, Direction_.PortOut, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_21", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_22", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_7", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_20", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_6", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_16", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_4", missionType, shelfNo, Direction_.Vertical, modelPosition));

                }

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_13", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_14", missionType, shelfNo, Direction_.Horizontal, modelPosition));
            }
            catch (Exception ex)
            {
                throw new Exception("Calc_CallTray has exception:" + ex.ToString());
            }
        }


        private void CalcPath_AddPallet(List<Conveyor> pathNodes, string shelfNo)
        {
            try
            {
                if (!product_Info.ContainsKey(shelfNo))
                {
                    throw new Exception("shelfNo_{0} does not exit" + shelfNo);
                }
                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_2", missionType, shelfNo, Direction_.Vertical, modelPosition));
                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_2", missionType, shelfNo, Direction_.Vertical, modelPosition));
                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_6", missionType, shelfNo, Direction_.Horizontal, modelPosition));
                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_7", missionType, shelfNo, Direction_.Horizontal, modelPosition));
            }
            catch (Exception ex)
            {

                throw new Exception("CalcPath_AddPallet has exception:" + ex.ToString());
            }
        }

        private void CalcPath_PalletIn(List<Conveyor> pathNodes, string shelfNo)
        {

            try
            {
                if (!product_Info.ContainsKey(shelfNo))
                {
                    throw new Exception("shelfNo_{0} does not exit:" + shelfNo);
                }
                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_7", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_6", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_5", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                if (endLine.Equals(1) || endLine.Equals(2))
                {
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_4", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_3", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_1", missionType, shelfNo, Direction_.PortIn, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_2", missionType, shelfNo, Direction_.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_3", missionType, shelfNo, Direction_.ShelfIn, modelPosition));

                }
                else if (endLine.Equals(3) || endLine.Equals(4) || endLine.Equals(5) || endLine.Equals(6))//2号 堆垛机
                {
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_1", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_8", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_3", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_15", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_5", missionType, shelfNo, Direction_.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_18", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_17", missionType, shelfNo, Direction_.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_1", missionType, shelfNo, Direction_.PortIn, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_2", missionType, shelfNo, Direction_.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_3", missionType, shelfNo, Direction_.ShelfIn, modelPosition));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Calc_CallTray has exception:" + ex.ToString());
            }
        }


        private void AddPathNode(List<Conveyor> pathNodes, Conveyor conveyor)
        {
            conveyor.Attach(this);
            pathNodes.Add(conveyor);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Conveyor currentConv = null;//当前的 线路段
                Vector3D leftVector = new Vector3D(0, 0, 0);
                if (pathNodes.Count > 0)
                {
                    currentConv = pathNodes[0];//获取首元素
                    if (currentConv.IsReady)
                    {
                        switch (currentConv.Direction)
                        {
                            case Direction_.Vertical://前后左右不需要有堆垛机参与
                                leftVector = GetLeftVector(shelfNo, currentConv.EndPos);//终点坐标   剩余位移量
                                Move_Vertical(shelfNo, leftVector, currentConv);
                                break;
                            case Direction_.Horizontal:
                                leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                                Move_Horizontal(shelfNo, leftVector, currentConv);
                                break;
                            case Direction_.PortIn:
                                SetStackerPos(shelfNo);
                                Move_Port_In(shelfNo, currentConv);
                                break;
                            case Direction_.PortOut:
                                SetStackerPos(shelfNo);//堆垛机立柱 载货台位置
                                Move_Port_Out(shelfNo, currentConv);
                                break;
                            case Direction_.ShelfIn:
                                SetStackerPos(shelfNo);//
                                Move_Shelf_In(shelfNo, currentConv);
                                break;
                            case Direction_.ShelfOut:
                                SetStackerPos(shelfNo);//
                                Move_Shelf_Out(shelfNo, currentConv);
                                break;
                            case Direction_.Roadway://斜 左上  左下  右上 右下  需要有堆垛机参与  
                                leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                                Move_Oblique(shelfNo, leftVector, currentConv);
                                leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                                SetStackerPos(shelfNo);//堆垛机跟随
                                if (leftVector.X == 0 && leftVector.Z == 0)
                                {
                                    currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                                    break;
                                }
                                break;
                        }
                        SetCameraParam(currentConv);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Timer_Elapsed has exception" + ex.ToString());
            }

        }

        /// <summary>
        /// 判断下一个路径
        /// </summary>
        private Conveyor PathNodeDone(List<Conveyor> pathNodes, Conveyor currentConv, string shelfNo, Dictionary<string, ProductInfo> product_Info)
        {
            Conveyor previousConv = null;
            pathNodes.RemoveAt(0);
            if (pathNodes.Count > 0)
            {
                previousConv = currentConv;
                currentConv = pathNodes[0];
            }
            else
            {
                MisSuccess(this, missionType, shelfNo);//队列为空 任务执行完毕
            }
            if (currentConv.IsReady == false)
            {
                product_Info[shelfNo].ProductOffSet = new Point3D(previousConv.EndPos.X - modelPosition.ProductOriPos.X, previousConv.EndPos.Y - modelPosition.ProductOriPos.Y, previousConv.EndPos.Z - modelPosition.ProductOriPos.Z);
            }
            return currentConv;
        }

        /// <summary>
        /// 获取货物的绝对坐标
        /// </summary>
        /// <param name="offSet"></param>
        /// <param name="modelPosition"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        private void GetAbsPos(string shelfNo, out double x, out double y, out double z)
        {
            Point3D currentOffSet = product_Info[shelfNo].ProductOffSet;
            x = currentOffSet.X + modelPosition.ProductOriPos.X;
            y = currentOffSet.Y + modelPosition.ProductOriPos.Y;
            z = currentOffSet.Z + modelPosition.ProductOriPos.Z;
        }

        private Vector3D GetLeftVector(string shelfNo, Point3D endPos)
        {
            double x, y, z;
            GetAbsPos(shelfNo, out x, out y, out z);
            return Point3D.Subtract(endPos, new Point3D(x, y, z));
        }

        private void Move_Vertical(string shelfNo, Vector3D leftVector, Conveyor currentConv)
        {
            if (leftVector.Y > 0)
            {
                product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y + currentConv.Speed_Y, product_Info[shelfNo].ProductOffSet.Z);
                leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                if (leftVector.Y <= 0)
                {
                    currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                }
            }
            else if (leftVector.Y < 0)
            {
                product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y - currentConv.Speed_Y, product_Info[shelfNo].ProductOffSet.Z);
                leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                if (leftVector.Y >= 0)
                {
                    currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                }
            }
            else
            {
                currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
            }
        }

        private void Move_Horizontal(string shelfNo, Vector3D leftVector, Conveyor currentConv)
        {
            if (leftVector.X > 0)
            {
                product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X + currentConv.Speed_X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z);
                leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                if (leftVector.X <= 0)
                {
                    currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                }
            }
            else if (leftVector.X < 0)
            {
                product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X - currentConv.Speed_X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z);
                leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                if (leftVector.X >= 0)
                {
                    currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                }
            }
            else
            {
                currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
            }
        }

        private void Move_Oblique(string shelfNo, Vector3D leftVector, Conveyor currentConv)
        {

            if (leftVector.X > 0)
            {
                if (leftVector.Z > 0)
                {
                    product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X + currentConv.Speed_X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z + currentConv.Speed_Z);//动一下
                    leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                    if (leftVector.X <= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(currentConv.EndPos.X - modelPosition.ProductOriPos.X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z);//动一下
                    }
                    if (leftVector.Z <= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y, currentConv.EndPos.Z - modelPosition.ProductOriPos.Z);
                    }
                    leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                }
                else
                {
                    product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X + currentConv.Speed_X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z - currentConv.Speed_Z);
                    leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                    if (leftVector.X <= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(currentConv.EndPos.X - modelPosition.ProductOriPos.X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z);//动一下
                    }
                    if (leftVector.Z >= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y, currentConv.EndPos.Z - modelPosition.ProductOriPos.Z);
                    }
                    leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                }
            }
            else if (leftVector.X <= 0)
            {
                if (leftVector.Z > 0)
                {
                    product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X - currentConv.Speed_X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z + currentConv.Speed_Z);//动一下
                    leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                    if (leftVector.X >= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(currentConv.EndPos.X - modelPosition.ProductOriPos.X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z);//动一下
                    }
                    if (leftVector.Z <= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y, currentConv.EndPos.Z - modelPosition.ProductOriPos.Z);
                    }
                    leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                }
                else
                {
                    product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X - currentConv.Speed_X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z - currentConv.Speed_Z);
                    leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                    if (leftVector.X >= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(currentConv.EndPos.X - modelPosition.ProductOriPos.X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z);//动一下
                    }
                    if (leftVector.Z >= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y, currentConv.EndPos.Z - modelPosition.ProductOriPos.Z);
                    }
                    leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                }
            }
        }

        bool isPortInDone = false;
        /// <summary>
        ///  货叉移动  入库口位置
        /// </summary>
        /// <param name="startPos">货叉起始位置 </param>
        /// <param name="endPos">货叉终点位置 </param>
        /// <param name="currentConv"></param>
        private void Move_Port_In(string shelfNo, Conveyor currentConv)
        {
            try
            {
                string str = null;
                Point3D bottomForkOriPos = new Point3D();
                Point3D stackerOffSet = new Point3D();
                if (!isPortInDone)
                {
                    GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
                    if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) > 0)
                    {
                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y + currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                        GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
                        if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) <= 0)
                        {
                            stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.StartPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                            //currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                            isPortInDone = true;
                        }
                    }
                    else if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) < 0)
                    {
                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y - currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                        GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
                        if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) >= 0)
                        {
                            stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.StartPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                            //currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                            isPortInDone = true;
                        }
                    }
                }
                else
                {
                    Vector3D leftVector = new Vector3D();
                    leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                    GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
                    if (leftVector.Y > 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y + currentConv.Speed_Y, product_Info[shelfNo].ProductOffSet.Z);
                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y + currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                        leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                        if (leftVector.Y <= 0)
                        {
                            stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.EndPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                            currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                            isPortInDone = false;
                        }
                    }
                    else if (leftVector.Y < 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y - currentConv.Speed_Y, product_Info[shelfNo].ProductOffSet.Z);
                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y - currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                        leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                        if (leftVector.Y >= 0)
                        {
                            stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.EndPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                            currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                            isPortInDone = false;
                        }
                    }
                    else
                    {
                        currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.EndPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                        isPortInDone = false;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Move_Port_In has exception:" + ex.ToString());
            }
        }

        bool isPortOutDone = false;
        /// <summary>
        /// 货叉移动 出货口位置 1.货叉货物出 2.货叉收回
        /// </summary>
        private void Move_Port_Out(string shelfNo, Conveyor currentConv)
        {
            try
            {
                string str = null;
                Point3D bottomForkOriPos = new Point3D();
                Point3D stackerOffSet = new Point3D();
                if (isPortOutDone)
                {
                    GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
                    if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) > 0)
                    {
                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y + currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                        GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
                        if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) <= 0)
                        {
                            stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.StartPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                            currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                            isPortOutDone = false;
                        }
                    }
                    else if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) < 0)
                    {
                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y - currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                        GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
                        if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) >= 0)
                        {
                            stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.StartPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                            currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                            isPortOutDone = false;
                        }
                    }
                }
                else//货物 货叉 同时出
                {
                    GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
                    Vector3D leftVector = new Vector3D();
                    leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                    if (leftVector.Y > 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y + currentConv.Speed_Y, product_Info[shelfNo].ProductOffSet.Z);
                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y + currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                        leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                        if (leftVector.Y <= 0)
                        {
                            stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.EndPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                            product_Info[shelfNo].ProductOffSet = new Point3D(currentConv.EndPos.X - modelPosition.ProductOriPos.X, currentConv.EndPos.Y - modelPosition.ProductOriPos.Y, currentConv.EndPos.Z - modelPosition.ProductOriPos.Z);
                            //currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                            isPortOutDone = true;
                        }
                    }
                    else if (leftVector.Y < 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y - currentConv.Speed_Y, product_Info[shelfNo].ProductOffSet.Z);
                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y - currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                        leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                        if (leftVector.Y >= 0)
                        {
                            stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.EndPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                            product_Info[shelfNo].ProductOffSet = new Point3D(currentConv.EndPos.X - modelPosition.ProductOriPos.X, currentConv.EndPos.Y - modelPosition.ProductOriPos.Y, currentConv.EndPos.Z - modelPosition.ProductOriPos.Z);
                            //currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                            isPortOutDone = true;
                        }
                    }
                    else
                    {
                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.EndPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                        product_Info[shelfNo].ProductOffSet = new Point3D(currentConv.EndPos.X - modelPosition.ProductOriPos.X, currentConv.EndPos.Y - modelPosition.ProductOriPos.Y, currentConv.EndPos.Z - modelPosition.ProductOriPos.Z);
                        //currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                        isPortOutDone = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Move_Port_Out has exception:" + ex.ToString());
            }
        }

        bool isShelfInDone = false;
        /// <summary>
        /// 货物进入  1. 货物与货叉共同运动进入库位  2. 货物不动 货叉收回
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <param name="currentConv"></param>
        private void Move_Shelf_In(string shelfNo, Conveyor currentConv)
        {
            string str = null;
            Point3D bottomForkOriPos = new Point3D();
            Point3D stackerOffSet = new Point3D();
            GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
            if (!isShelfInDone)//货物货叉共同进入
            {
                Vector3D leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                if (leftVector.Y > 0)
                {
                    product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y + currentConv.Speed_Y, product_Info[shelfNo].ProductOffSet.Z);
                    stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y + currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                    leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                    if (leftVector.Y <= 0)
                    {

                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.EndPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                        product_Info[shelfNo].ProductOffSet = new Point3D(currentConv.EndPos.X - modelPosition.ProductOriPos.X, currentConv.EndPos.Y - modelPosition.ProductOriPos.Y, currentConv.EndPos.Z - modelPosition.ProductOriPos.Z);
                        //currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                        isShelfInDone = true;
                    }
                }
                else if (leftVector.Y < 0)
                {
                    product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y - currentConv.Speed_Y, product_Info[shelfNo].ProductOffSet.Z);
                    stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y - currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                    leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                    if (leftVector.Y >= 0)
                    {

                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.EndPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                        product_Info[shelfNo].ProductOffSet = new Point3D(currentConv.EndPos.X - modelPosition.ProductOriPos.X, currentConv.EndPos.Y - modelPosition.ProductOriPos.Y, currentConv.EndPos.Z - modelPosition.ProductOriPos.Z);
                        //currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                        isShelfInDone = true;
                    }
                }
                else
                {

                    stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.EndPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                    product_Info[shelfNo].ProductOffSet = new Point3D(currentConv.EndPos.X - modelPosition.ProductOriPos.X, currentConv.EndPos.Y - modelPosition.ProductOriPos.Y, currentConv.EndPos.Z - modelPosition.ProductOriPos.Z);
                    //currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                    isShelfInDone = true;
                }
            }
            else//货叉单独出
            {
                GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
                if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) > 0)
                {
                    stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y + currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                    GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
                    if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) <= 0)
                    {
                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.StartPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                        currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                        isShelfInDone = false;
                    }
                }
                else if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) < 0)
                {
                    stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y - currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                    GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
                    if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) >= 0)
                    {
                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.StartPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                        currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                        isShelfInDone = false;
                    }
                }
            }
        }

        bool isShelfOutDone = false;
        /// <summary>
        /// 货物出 1.货叉进入（货物不动） 2.货叉出（货物出）
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <param name="currentConv"></param>
        private void Move_Shelf_Out(string shelfNo, Conveyor currentConv)
        {
            string str = null;
            Point3D bottomForkOriPos = new Point3D();
            Point3D stackerOffSet = new Point3D();
            if (!isShelfOutDone)//货叉进入
            {
                GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
                if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) > 0)
                {
                    stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y + currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                    GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
                    if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) <= 0)
                    {
                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.StartPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                        isShelfOutDone = true;
                    }
                }
                else if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) < 0)
                {
                    stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y - currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                    GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
                    if (currentConv.StartPos.Y - (bottomForkOriPos.Y + stackerOffSet.Y) >= 0)
                    {
                        stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, currentConv.StartPos.Y - bottomForkOriPos.Y, stackerParts_Info[str].StackerOffSet.Z);
                        isShelfOutDone = true;
                    }
                }
            }
            else
            {
                GetShelfForkPos(out str, out bottomForkOriPos, out stackerOffSet);
                Vector3D leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                if (leftVector.Y > 0)
                {
                    product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y + currentConv.Speed_Y, product_Info[shelfNo].ProductOffSet.Z);
                    stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y + currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                    leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                    if (leftVector.Y <= 0)
                    {
                        currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                    }
                }
                else if (leftVector.Y < 0)
                {
                    product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y - currentConv.Speed_Y, product_Info[shelfNo].ProductOffSet.Z);
                    stackerParts_Info[str].StackerOffSet = new Point3D(stackerParts_Info[str].StackerOffSet.X, stackerParts_Info[str].StackerOffSet.Y - currentConv.Speed_Y, stackerParts_Info[str].StackerOffSet.Z);
                    leftVector = GetLeftVector(shelfNo, currentConv.EndPos);
                    if (leftVector.Y >= 0)
                    {
                        currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                    }
                }
                else
                {
                    currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                }
            }
        }

        private void GetShelfForkPos(out string str, out Point3D bottomForkOriPos, out Point3D stackerOffSet_UpFork)
        {
            str = null;
            bottomForkOriPos = new Point3D();
            stackerOffSet_UpFork = new Point3D();
            if (endLine.Equals(1) || endLine.Equals(2))
            {
                str = "shangcha001";
                bottomForkOriPos = modelPosition.BottomForkOriPos_1;
                stackerOffSet_UpFork = stackerParts_Info[str].StackerOffSet;
            }
            else if (endLine.Equals(3) || endLine.Equals(4) || endLine.Equals(5) || endLine.Equals(6))//2  堆垛机
            {
                str = "shangcha002";
                bottomForkOriPos = modelPosition.BottomForkOriPos_2;
                stackerOffSet_UpFork = stackerParts_Info[str].StackerOffSet;
            }
        }


        /// <summary>
        /// 设置堆垛机的位置
        /// </summary>
        private void SetStackerPos(string shelfNo)
        {
            double x, y, z;//货物模型的绝对坐标
            GetAbsPos(shelfNo, out x, out y, out z);
            Point3D stackerOffset = new Point3D(x - productStackerOriPos.X, y - productStackerOriPos.Y, z - productStackerOriPos.Z);
            if (endLine.Equals(1) || endLine.Equals(2))
            {
                stackerParts_Info["duiduojilizhu001"].StackerOffSet = new Point3D(stackerOffset.X, stackerParts_Info["duiduojilizhu001"].StackerOffSet.Y, 0);
                stackerParts_Info["zaihuotai001"].StackerOffSet = new Point3D(stackerOffset.X, stackerParts_Info["zaihuotai001"].StackerOffSet.Y, stackerOffset.Z);
                stackerParts_Info["shangcha001"].StackerOffSet = new Point3D(stackerOffset.X, stackerParts_Info["shangcha001"].StackerOffSet.Y, stackerOffset.Z);
                stackerParts_Info["zhongcha001"].StackerOffSet = new Point3D(stackerOffset.X, stackerParts_Info["zhongcha001"].StackerOffSet.Y, stackerOffset.Z);
                stackerParts_Info["xiacha001"].StackerOffSet = new Point3D(stackerOffset.X, stackerParts_Info["xiacha001"].StackerOffSet.Y, stackerOffset.Z);
                stackerParts_Info["VIFS001"].StackerOffSet = new Point3D(stackerOffset.X, stackerParts_Info["VIFS001"].StackerOffSet.Y, stackerOffset.Z);
            }
            else
            {
                stackerParts_Info["duiduojilizhu002"].StackerOffSet = new Point3D(stackerOffset.X, stackerParts_Info["duiduojilizhu002"].StackerOffSet.Y, 0);
                stackerParts_Info["zaihuotai002"].StackerOffSet = new Point3D(stackerOffset.X, stackerParts_Info["zaihuotai002"].StackerOffSet.Y, stackerOffset.Z);
                stackerParts_Info["shangcha002"].StackerOffSet = new Point3D(stackerOffset.X, stackerParts_Info["shangcha002"].StackerOffSet.Y, stackerOffset.Z);
                stackerParts_Info["zhongcha002"].StackerOffSet = new Point3D(stackerOffset.X, stackerParts_Info["zhongcha002"].StackerOffSet.Y, stackerOffset.Z);
                stackerParts_Info["xiacha002"].StackerOffSet = new Point3D(stackerOffset.X, stackerParts_Info["xiacha002"].StackerOffSet.Y, stackerOffset.Z);
                stackerParts_Info["VIFS002"].StackerOffSet = new Point3D(stackerOffset.X, stackerParts_Info["VIFS002"].StackerOffSet.Y, stackerOffset.Z);
            }
        }

        /// <summary>
        /// 设置堆垛机的叉的 抽插
        /// </summary>
        /// <param name="currentConv"></param>
        /// <returns></returns>
        private bool SetForkPos(Conveyor currentConv)//设置堆垛机的叉位置 出叉  收叉 Y方向 运动
        {
            bool res = false;
            try
            {
                Point3D endPos = currentConv.EndPos;
                double leftY = GetForkLeftY(endPos);
                Point3D tempOffSet = stackerParts_Info["shangcha001"].StackerOffSet;
                if (endLine.Equals(1) || endLine.Equals(2))
                {
                    if (leftY > 0)
                    {
                        stackerParts_Info["shangcha001"].StackerOffSet = new Point3D(tempOffSet.X, tempOffSet.Y + currentConv.Speed_Y, tempOffSet.Z);
                        leftY = GetForkLeftY(endPos);
                        tempOffSet = stackerParts_Info["shangcha001"].StackerOffSet;
                        if (leftY <= 0)
                        {
                            stackerParts_Info["shangcha001"].StackerOffSet = new Point3D(tempOffSet.X, endPos.Y - modelPosition.ProductStackerOriPos_1.Y, tempOffSet.Z);
                            return true;
                        }
                    }
                    else if (leftY < 0)
                    {
                        stackerParts_Info["shangcha001"].StackerOffSet = new Point3D(tempOffSet.X, tempOffSet.Y - currentConv.Speed_Y, tempOffSet.Z);
                        leftY = GetForkLeftY(endPos);
                        tempOffSet = stackerParts_Info["shangcha001"].StackerOffSet;
                        if (leftY >= 0)
                        {
                            stackerParts_Info["shangcha001"].StackerOffSet = new Point3D(tempOffSet.X, endPos.Y - modelPosition.ProductStackerOriPos_1.Y, tempOffSet.Z);
                            return true;
                        }
                    }
                    else
                    {
                        stackerParts_Info["shangcha001"].StackerOffSet = new Point3D(tempOffSet.X, endPos.Y - modelPosition.ProductStackerOriPos_1.Y, tempOffSet.Z);
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return res;
        }

        private double GetForkLeftY(Point3D endPos)
        {
            return endPos.Y - (modelPosition.ProductStackerOriPos_1.Y + stackerParts_Info["shangcha001"].StackerOffSet.Y);
        }

        private void SetCameraParam(Conveyor currentConv)
        {
            if (product_Info.ContainsKey(shelfNo))
            {
                totalOffSet = product_Info[shelfNo].ProductOffSet;
            }
            worldPosition = new Point3D(totalOffSet.X + modelPosition.ProductOriPos.X, totalOffSet.Y + modelPosition.ProductOriPos.Y, totalOffSet.Z + modelPosition.ProductOriPos.Z);//绝对位置
            if (currentConv.Direction.Equals(Direction_.Horizontal) || currentConv.Direction.Equals(Direction_.Vertical))//输送带上的运动
            {
                Point3D temp = Point3D.Add(worldPosition, modelPosition.CameraOffSet_HV);
                cameraPosition = new Point3D(temp.X, temp.Z, -temp.Y);
                cameraLookDirection = new Vector3D(modelPosition.CameraOffSet_HV_LookDirection.X, modelPosition.CameraOffSet_HV_LookDirection.Z, -modelPosition.CameraOffSet_HV_LookDirection.Y);
                cameraUpDirection = modelPosition.CameraOffSet_HV_UpDirection;
            }
            else if (currentConv.Direction.Equals(Direction_.PortIn) || currentConv.Direction.Equals(Direction_.PortOut))
            {
                Point3D temp = Point3D.Add(worldPosition, modelPosition.CameraOffSet_Port);
                cameraPosition = new Point3D(temp.X, temp.Z, -temp.Y);
                cameraLookDirection = new Vector3D(modelPosition.CameraOffSet_Port_LookDirection.X, modelPosition.CameraOffSet_Port_LookDirection.Z, -modelPosition.CameraOffSet_Port_LookDirection.Y);
                cameraUpDirection = modelPosition.CameraOffSet_Port_UpDirection;
            }
            else if (currentConv.Direction.Equals(Direction_.Roadway))
            {
                Point3D temp = Point3D.Add(worldPosition, modelPosition.CameraOffSet_RoadWay);
                temp = new Point3D(worldPosition.X, worldPosition.Y, modelPosition.CameraOffSet_RoadWay.Z);
                cameraPosition = new Point3D(temp.X, temp.Z, -temp.Y);
                cameraLookDirection = new Vector3D(modelPosition.CameraOffSet_RoadWay_LookDirection.X, modelPosition.CameraOffSet_RoadWay_LookDirection.Z, -modelPosition.CameraOffSet_RoadWay_LookDirection.Y);
                cameraUpDirection = modelPosition.CameraOffSet_RoadWay_UpDirection;
            }
            else if (currentConv.Direction.Equals(Direction_.ShelfIn) || currentConv.Direction.Equals(Direction_.ShelfOut))
            {
                Point3D temp = Point3D.Add(worldPosition, modelPosition.CameraOffSet_Shelf);
                cameraPosition = new Point3D(temp.X, temp.Z, -temp.Y);
                cameraLookDirection = new Vector3D(modelPosition.CameraOffSet_Shelf_LookDirection.X, modelPosition.CameraOffSet_Shelf_LookDirection.Z, -modelPosition.CameraOffSet_Shelf_LookDirection.Y);
                cameraUpDirection = modelPosition.CameraOffSet_Shelf_UpDirection;
            }

        }

        public void Update(string missionId, string conveyorName, bool IsReady)
        {
            if (!this.missionId.Equals(missionId))
            {
                return;
            }
            for (int i = 0; i < pathNodes.Count; i++)
            {
                if (pathNodes[i].Name.Equals(conveyorName))
                {
                    pathNodes[i].IsReady = IsReady;
                    timer.Start();
                    return;
                }
            }
            timer.Start();
        }

        public void Dispose()
        {
            timer.Stop();
        }
    }
}
