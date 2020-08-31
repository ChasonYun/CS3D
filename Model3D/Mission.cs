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
    public enum MissionType
    {
        [Description("入库")]
        StockIn = 1,
        [Description("出库")]
        StockOut,
        [Description("移库")]
        Transfer,
        [Description("分拣")]
        Sorting
    }

    public delegate void MissionSuccess(IMission mission, MissionType missionType, string shelfNo);
    public interface IMission//观察者模式也可以使用 delegate 简化  接口 
    {
        event MissionSuccess MisSuccess;
        void Update(string conveyorName, bool IsReady);
    }

    /// <summary>
    /// 负责维护 任务模型  内部数据  offset
    /// </summary>
    public class Mission : IMission
    {
        public event MissionSuccess MisSuccess;

        MissionType missionType;//任务类型  //  入库  出库  
        string shelfNo;//终点或起点 货架号

        int line, level, column;
        ModelPosition modelPosition;

        public Point3D totalOffSet;
        public Point3D worldPosition;

        private Point3D productStackerOriPos;



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
        public Mission(MissionType missionType, string shelfNo, ModelPosition modelPosition, Dictionary<string, ProductInfo> product_Info, Dictionary<string, StackerPartsInfo> stackerParts_Info)//入库 出库 叠盘 拆盘
        {
            this.modelPosition = modelPosition;
            this.missionType = missionType;
            this.shelfNo = shelfNo;
            this.product_Info = product_Info;
            this.stackerParts_Info = stackerParts_Info;
            modelPosition.Get_Line_Level_Column(shelfNo, out line, out level, out column);
            if (line.Equals(1) || line.Equals(2))
            {
                productStackerOriPos = modelPosition.ProductStackerOriPos_1;
            }
            else if (line.Equals(3) || line.Equals(4) || line.Equals(5) || line.Equals(6))
            {
                productStackerOriPos = modelPosition.ProductStackerOriPos_2;
            }

            worldPosition = modelPosition.StockInEntranceOriPos;

            GetPath(shelfNo);//获取该任务的 路径list

            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 50;//一秒 10 次
        }

        private void GetPath(string shelfNo)
        {
            try
            {
               
                if (missionType.Equals(MissionType.StockIn))//入库
                {
                    CalcPath_StockIn(pathNodes, shelfNo);
                }
                else if (missionType.Equals(MissionType.StockOut))//出库
                {
                    CalcPath_StockOut(pathNodes, shelfNo);
                }
                else if (missionType.Equals(MissionType.Transfer))//移库
                {

                }
                else if (missionType.Equals(MissionType.Sorting))//分拣
                {
                    CalcPath_Sorting(pathNodes, shelfNo);
                  
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetPath has exception" + ex.ToString());
            }

        }

        private void CalcPath_StockIn(List<Conveyor> pathNodes,string shelfNo)
        {
            try
            {
                string[] pathStr = shelfNo.Split('.');
                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_1", missionType, shelfNo, Direction.Vertical, modelPosition));

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_1", missionType, shelfNo, Direction.Vertical, modelPosition));

                if (pathStr[0].Equals("01") || pathStr[0].Equals("02"))
                {

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_4", missionType, shelfNo, Direction.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_3", missionType, shelfNo, Direction.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_1", missionType, shelfNo, Direction.Port, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_2", missionType, shelfNo, Direction.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_3", missionType, shelfNo, Direction.Shelf, modelPosition));

                }
                else if (pathStr[0].Equals("03") || pathStr[0].Equals("04") || pathStr[0].Equals("05") || pathStr[0].Equals("06"))
                {
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_8", missionType, shelfNo, Direction.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_3", missionType, shelfNo, Direction.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_15", missionType, shelfNo, Direction.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_5", missionType, shelfNo, Direction.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_18", missionType, shelfNo, Direction.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_17", missionType, shelfNo, Direction.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_1", missionType, shelfNo, Direction.Port, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_2", missionType, shelfNo, Direction.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_3", missionType, shelfNo, Direction.Shelf, modelPosition));

                }
            }
            catch (Exception ex)
            {
                throw new Exception("CalcPathNode_StockIn has exception:" + ex.ToString());
            }
        }

        private void CalcPath_StockOut(List<Conveyor> pathNodes,string shelfNo)
        {
            try
            {
                string[] pathStr = shelfNo.Split('.');
                if (!product_Info.ContainsKey(shelfNo))
                {
                    throw new Exception("未检索到货物信息" + shelfNo);
                }
                if (pathStr[0].Equals("01") || pathStr[0].Equals("02"))
                {
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_3", missionType, shelfNo, Direction.Shelf, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_2", missionType, shelfNo, Direction.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_4", missionType, shelfNo, Direction.Port, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_10", missionType, shelfNo, Direction.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_11", missionType, shelfNo, Direction.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_12", missionType, shelfNo, Direction.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_9", missionType, shelfNo, Direction.Vertical, modelPosition));

                }
                else if (pathStr[0].Equals("03") || pathStr[0].Equals("04") || pathStr[0].Equals("05") || pathStr[0].Equals("06"))//2号 堆垛机
                {
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_3", missionType, shelfNo, Direction.Shelf, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_2", missionType, shelfNo, Direction.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_4", missionType, shelfNo, Direction.Port, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_21", missionType, shelfNo, Direction.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_22", missionType, shelfNo, Direction.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_7", missionType, shelfNo, Direction.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_20", missionType, shelfNo, Direction.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_6", missionType, shelfNo, Direction.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_16", missionType, shelfNo, Direction.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_4", missionType, shelfNo, Direction.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_9", missionType, shelfNo, Direction.Vertical, modelPosition));
                }

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_2", missionType, shelfNo, Direction.Vertical, modelPosition));

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_2", missionType, shelfNo, Direction.Vertical, modelPosition));
            }
            catch (Exception ex)
            {
                throw new Exception("CalcPathNode_StockOut has exception:" + ex.ToString());
            }
        }

        private void CalcPath_Sorting(List<Conveyor> pathNodes,string shelfNo)
        {
            try
            {
                string[] pathStr = shelfNo.Split('.');
                if (!product_Info.ContainsKey(shelfNo))
                {
                    throw new Exception(string.Format("shelfNo_{0} does not exit:", shelfNo));
                }
                if (pathStr[0].Equals("01") || pathStr[0].Equals("02"))
                {
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_3", missionType, shelfNo, Direction.Shelf, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_2", missionType, shelfNo, Direction.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker1_4", missionType, shelfNo, Direction.Port, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_10", missionType, shelfNo, Direction.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_11", missionType, shelfNo, Direction.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_12", missionType, shelfNo, Direction.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_9", missionType, shelfNo, Direction.Vertical, modelPosition));

                }
                else if (pathStr[0].Equals("03") || pathStr[0].Equals("04") || pathStr[0].Equals("05") || pathStr[0].Equals("06"))//2号 堆垛机
                {
                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_3", missionType, shelfNo, Direction.Shelf, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_2", missionType, shelfNo, Direction.Roadway, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("stacker2_4", missionType, shelfNo, Direction.Port, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_21", missionType, shelfNo, Direction.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_22", missionType, shelfNo, Direction.Horizontal, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_7", missionType, shelfNo, Direction.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_20", missionType, shelfNo, Direction.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_6", missionType, shelfNo, Direction.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_16", missionType, shelfNo, Direction.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_4", missionType, shelfNo, Direction.Vertical, modelPosition));

                    AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_9", missionType, shelfNo, Direction.Vertical, modelPosition));
                }

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("ds_2", missionType, shelfNo, Direction.Vertical, modelPosition));

                AddPathNode(pathNodes, ConveyorFactory.GetConveyor("conveyor_2", missionType, shelfNo, Direction.Vertical, modelPosition));


            }
            catch (Exception ex)
            {
                throw new Exception("CalcPathNode_Sorting has exception:" + ex.ToString());
            }
        }

        private void AddPathNode(List<Conveyor> pathNodes, Conveyor conveyor)
        {
            conveyor.Attach(this);
            pathNodes.Add(conveyor);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
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
                        case Direction.Vertical://前后左右不需要有堆垛机参与
                            leftVector = GetLeftVector(currentConv.EndPos);//终点坐标   剩余位移量
                            Move_Vertical(leftVector, currentConv);
                            break;
                        case Direction.Horizontal:
                            leftVector = GetLeftVector(currentConv.EndPos);
                            Move_Horizontal(leftVector, currentConv);
                            break;
                        case Direction.Port://堆垛机需要移动
                            leftVector = GetLeftVector(currentConv.EndPos);
                            Move_Vertical(leftVector, currentConv);
                            SetStackerPos();
                            break;
                        case Direction.Shelf://堆垛机需要移动
                            leftVector = GetLeftVector(currentConv.EndPos);
                            Move_Vertical(leftVector, currentConv);
                            SetStackerPos();
                            break;
                        case Direction.Roadway://斜 左上  左下  右上 右下  需要有堆垛机参与  
                            leftVector = GetLeftVector(currentConv.EndPos);
                            Move_Oblique(leftVector, currentConv);
                            leftVector = GetLeftVector(currentConv.EndPos);
                            SetStackerPos();
                            if (leftVector.X == 0 && leftVector.Z == 0)
                            {
                                currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                                break;
                            }
                            break;
                    }
                }
                totalOffSet = product_Info[shelfNo].ProductOffSet;
                worldPosition = new Point3D(totalOffSet.X + modelPosition.ProductOriPos.X, totalOffSet.Y + modelPosition.ProductOriPos.Y, totalOffSet.Z + modelPosition.ProductOriPos.Z);
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
        private void GetAbsPos(out double x, out double y, out double z)
        {
            Point3D currentOffSet = product_Info[shelfNo].ProductOffSet;
            x = currentOffSet.X + modelPosition.ProductOriPos.X;
            y = currentOffSet.Y + modelPosition.ProductOriPos.Y;
            z = currentOffSet.Z + modelPosition.ProductOriPos.Z;
        }

        private Vector3D GetLeftVector(Point3D endPos)
        {
            double x, y, z;
            GetAbsPos(out x, out y, out z);
            return Point3D.Subtract(endPos, new Point3D(x, y, z));
        }

        private void Move_Vertical(Vector3D leftVector, Conveyor currentConv)
        {
            if (leftVector.Y > 0)
            {
                product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y + currentConv.Speed_Y, product_Info[shelfNo].ProductOffSet.Z);
                leftVector = GetLeftVector(currentConv.EndPos);
                if (leftVector.Y <= 0)
                {
                    currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                }
            }
            else if (leftVector.Y < 0)
            {
                product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y - currentConv.Speed_Y, product_Info[shelfNo].ProductOffSet.Z);
                leftVector = GetLeftVector(currentConv.EndPos);
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

        private void Move_Horizontal(Vector3D leftVector, Conveyor currentConv)
        {
            if (leftVector.X > 0)
            {
                product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X + currentConv.Speed_X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z);
                leftVector = GetLeftVector(currentConv.EndPos);
                if (leftVector.X <= 0)
                {
                    currentConv = PathNodeDone(pathNodes, currentConv, shelfNo, product_Info);
                }
            }
            else if (leftVector.X < 0)
            {
                product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X - currentConv.Speed_X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z);
                leftVector = GetLeftVector(currentConv.EndPos);
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

        private void Move_Oblique(Vector3D leftVector, Conveyor currentConv)
        {

            if (leftVector.X > 0)
            {
                if (leftVector.Z > 0)
                {
                    product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X + currentConv.Speed_X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z + currentConv.Speed_Z);//动一下
                    leftVector = GetLeftVector(currentConv.EndPos);
                    if (leftVector.X <= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(currentConv.EndPos.X - modelPosition.ProductOriPos.X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z);//动一下
                    }
                    if (leftVector.Z <= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y, currentConv.EndPos.Z - modelPosition.ProductOriPos.Z);
                    }
                    leftVector = GetLeftVector(currentConv.EndPos);
                }
                else
                {
                    product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X + currentConv.Speed_X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z - currentConv.Speed_Z);
                    leftVector = GetLeftVector(currentConv.EndPos);
                    if (leftVector.X <= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(currentConv.EndPos.X - modelPosition.ProductOriPos.X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z);//动一下
                    }
                    if (leftVector.Z >= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y, currentConv.EndPos.Z - modelPosition.ProductOriPos.Z);
                    }
                    leftVector = GetLeftVector(currentConv.EndPos);
                }
            }
            else if (leftVector.X <= 0)
            {
                if (leftVector.Z > 0)
                {
                    product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X - currentConv.Speed_X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z + currentConv.Speed_Z);//动一下
                    leftVector = GetLeftVector(currentConv.EndPos);
                    if (leftVector.X >= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(currentConv.EndPos.X - modelPosition.ProductOriPos.X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z);//动一下
                    }
                    if (leftVector.Z <= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y, currentConv.EndPos.Z - modelPosition.ProductOriPos.Z);
                    }
                    leftVector = GetLeftVector(currentConv.EndPos);
                }
                else
                {
                    product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X - currentConv.Speed_X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z - currentConv.Speed_Z);
                    leftVector = GetLeftVector(currentConv.EndPos);
                    if (leftVector.X >= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(currentConv.EndPos.X - modelPosition.ProductOriPos.X, product_Info[shelfNo].ProductOffSet.Y, product_Info[shelfNo].ProductOffSet.Z);//动一下
                    }
                    if (leftVector.Z >= 0)
                    {
                        product_Info[shelfNo].ProductOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X, product_Info[shelfNo].ProductOffSet.Y, currentConv.EndPos.Z - modelPosition.ProductOriPos.Z);
                    }
                    leftVector = GetLeftVector(currentConv.EndPos);
                }
            }
        }


        /// <summary>
        /// 设置堆垛机的位置
        /// </summary>
        private void SetStackerPos()
        {
            double x, y, z;//货物模型的绝对坐标
            GetAbsPos(out x, out y, out z);
            Point3D stackerOffset = new Point3D(x - productStackerOriPos.X, y - productStackerOriPos.Y, z - productStackerOriPos.Z);
            if (line.Equals(1) || line.Equals(2))
            {
                stackerParts_Info["duiduojilizhu001"].StackerOffSet = new Point3D(stackerOffset.X, 0, 0);
                stackerParts_Info["zaihuotai001"].StackerOffSet = new Point3D(stackerOffset.X, 0, stackerOffset.Z);
                stackerParts_Info["shangcha001"].StackerOffSet = new Point3D(stackerOffset.X, 0, stackerOffset.Z);
                stackerParts_Info["zhongcha001"].StackerOffSet = new Point3D(stackerOffset.X, 0, stackerOffset.Z);
                stackerParts_Info["xiacha001"].StackerOffSet = new Point3D(stackerOffset.X, 0, stackerOffset.Z);
                stackerParts_Info["VIFS001"].StackerOffSet = new Point3D(stackerOffset.X, 0, stackerOffset.Z);
            }
            else
            {
                stackerParts_Info["duiduojilizhu002"].StackerOffSet = new Point3D(stackerOffset.X, 0, 0);
                stackerParts_Info["zaihuotai002"].StackerOffSet = new Point3D(stackerOffset.X, 0, stackerOffset.Z);
                stackerParts_Info["shangcha002"].StackerOffSet = new Point3D(stackerOffset.X, 0, stackerOffset.Z);
                stackerParts_Info["zhongcha002"].StackerOffSet = new Point3D(stackerOffset.X, 0, stackerOffset.Z);
                stackerParts_Info["xiacha002"].StackerOffSet = new Point3D(stackerOffset.X, 0, stackerOffset.Z);
                stackerParts_Info["VIFS002"].StackerOffSet = new Point3D(stackerOffset.X, 0, stackerOffset.Z);
            }
        }

        public void Update(string conveyorName, bool IsReady)
        {
            for (int i = 0; i < pathNodes.Count; i++)
            {
                if (pathNodes[i].Name.Equals(conveyorName))
                {
                    pathNodes[i].IsReady = IsReady;
                }
            }
            timer.Start();
        }
    }
}
