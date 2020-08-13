﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media.Media3D;

namespace CS3D
{
    public interface IMission
    {
        void Update(string conveyorName, bool isReady);
    }

    /// <summary>
    /// 负责维护 任务模型  内部数据  offset
    /// </summary>
    public class Mission : IMission
    {
        public delegate void HintEventHandler(string msg);

        public static event HintEventHandler HintEvent;

        string missionType;//任务类型  //  入库  出库  
        string shelfNo;//终点或起点 货架号

        ModelPosition modelPosition;

    

        PerspectiveCamera perspectiveCamera;
       

        Timer timer = new Timer();

        List<Conveyor> pathNodes = new List<Conveyor>();//改任务需要走过的 传送带 序号


        Point3D tempOffSet = new Point3D();
        Dictionary<string, ProductInfo> product_Info;
        Dictionary<string, StackerPartsInfo> stackerParts_Info;

        Conveyor conveyor_1;
        Conveyor conveyor_4;
        Conveyor conveyor_3;
        Conveyor conveyor_8;
        Conveyor conveyor_15;
        Conveyor conveyor_18;
        Conveyor conveyor_17;
        Conveyor ds_1;
        Conveyor ds_3;
        Conveyor ds_5;

        Conveyor stacker1_1;
        Conveyor stacker1_2;
        Conveyor stacker1_3;

      /// <summary>
      /// 任务构造函数
      /// </summary>
      /// <param name="missionType">任务类型 StockIn StockOut</param>
      /// <param name="shelfNo">库位号</param>
      /// <param name="modelPosition.ProductOriPos">模型起点坐标</param>
      /// <param name="startPos">入库口起点坐标</param>
      /// <param name="product_Info">货物模型字典</param>
      /// <param name="stackerParts_Info">堆垛机模型字典</param>
      /// <param name="perspectiveCamera">视角相机</param>
        public Mission(string missionType, string shelfNo,ModelPosition modelPosition, Dictionary<string, ProductInfo> product_Info, Dictionary<string, StackerPartsInfo> stackerParts_Info, PerspectiveCamera perspectiveCamera)//入库 出库 叠盘 拆盘
        {
            this.modelPosition = modelPosition;
            this.missionType = missionType;
            this.shelfNo = shelfNo;
            this.product_Info = product_Info;
            this.stackerParts_Info = stackerParts_Info;
            this.perspectiveCamera = perspectiveCamera;

            GetPath(shelfNo);//获取该任务的 路径list

            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 50;//一秒 10 次
        }

        private void GetPath(string shelfNo)
        {
            try
            {
                string[] pathStr = shelfNo.Split('.');
                if (missionType == "StockIn")//入库
                {
                    if (pathStr[0].Equals("01") || pathStr[0].Equals("02"))
                    {
                        conveyor_1 = new Conveyor_1(Direction.Front);
                        conveyor_1.Attach(this);
                        pathNodes.Add(conveyor_1);

                        ds_1 = new DS_1(Direction.Front);
                        ds_1.Attach(this);
                        pathNodes.Add(ds_1);


                        conveyor_4 = new Conveyor_4(Direction.Right);
                        conveyor_4.Attach(this);
                        pathNodes.Add(conveyor_4);

                        conveyor_3 = new Conveyor_3(Direction.Right);
                        conveyor_3.Attach(this);
                        pathNodes.Add(conveyor_3);

                        stacker1_1 = new Stacker1_1(Direction.Front);
                        stacker1_1.Attach(this);
                        pathNodes.Add(stacker1_1);

                        stacker1_2 = new Stacker1_2(Direction.Top_Rigth, shelfNo, modelPosition);
                        stacker1_2.Attach(this);
                        pathNodes.Add(stacker1_2);

                        if (pathStr[0].Equals("01"))
                        {
                            stacker1_3 = new Stacker1_3(Direction.Back, shelfNo, modelPosition);
                        }
                        else
                        {
                            stacker1_3 = new Stacker1_3(Direction.Back, shelfNo, modelPosition);
                        }
                        stacker1_3.Attach(this);
                        pathNodes.Add(stacker1_3);

                    }
                    else if (pathStr[0].Equals("03") || pathStr[0].Equals("04") || pathStr[0].Equals("05"))
                    {
                        conveyor_1 = new Conveyor_1(Direction.Front);
                        ds_1 = new DS_1(Direction.Front);
                        conveyor_8 = new Conveyor_8(Direction.Front);
                        ds_3 = new DS_3(Direction.Front);
                        conveyor_15 = new Conveyor_15(Direction.Front);

                        ds_5 = new DS_5(Direction.Right);
                        conveyor_18 = new Conveyor_18(Direction.Right);
                        conveyor_17 = new Conveyor_17(Direction.Right);

                        conveyor_1.Attach(this);
                        ds_1.Attach(this);
                        conveyor_8.Attach(this);
                        ds_3.Attach(this);
                        conveyor_15.Attach(this);
                        ds_5.Attach(this);
                        conveyor_18.Attach(this);
                        conveyor_17.Attach(this);

                        pathNodes.Add(conveyor_1);
                        pathNodes.Add(ds_1);
                        pathNodes.Add(conveyor_8);
                        pathNodes.Add(ds_3);
                        pathNodes.Add(conveyor_15);
                        pathNodes.Add(ds_5);
                        pathNodes.Add(conveyor_18);
                        pathNodes.Add(conveyor_17);
                    }
                }
                else if (missionType == "StockOut")//出库
                {
                    if (pathStr[0].Equals("01") || pathStr[0].Equals("02"))
                    {
                        throw new Exception("出库未定义");
                    }
                    else if (pathStr[0].Equals("03") || pathStr[0].Equals("04") || pathStr[0].Equals("05"))
                    {
                        throw new Exception("出库未定义");
                    }
                }
            }
            catch (Exception ex)
            {
                if (HintEvent != null) HintEvent(string.Format("Mission_GetPath() has exception:" + ex.ToString()));
            }

        }

        double x;
        double z;
        double y;
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Conveyor tempConv;//查看路径队首元素
            if (pathNodes.Count > 0)
            {
                tempConv = pathNodes[0];//获取首元素
                if (tempConv.isReady)
                {
                    tempOffSet = product_Info[shelfNo].ProductOffSet;
                    x = tempOffSet.X + modelPosition.ProductOriPos.X;
                    y = tempOffSet.Y + modelPosition.ProductOriPos.Y;
                    z = tempOffSet.Z + modelPosition.ProductOriPos.Z;
                    switch (tempConv.direction)
                    {
                        case Direction.Front://
                            if (y > tempConv.frontPos.Y)
                            {
                                tempConv.innerOffSet = new Point3D(tempConv.innerOffSet.X, tempConv.innerOffSet.Y - tempConv.speed, tempConv.innerOffSet.Z);
                                product_Info[shelfNo].ProductOffSet = new Point3D(tempOffSet.X + tempConv.innerOffSet.X, tempOffSet.Y + tempConv.innerOffSet.Y, tempOffSet.Z + tempConv.innerOffSet.Z);
                            }
                            else
                            {
                                product_Info[shelfNo].ProductOffSet = new Point3D(tempConv.frontPos.X - modelPosition.ProductOriPos.X, tempConv.frontPos.Y - modelPosition.ProductOriPos.Y, tempConv.frontPos.Z - modelPosition.ProductOriPos.Z);
                                pathNodes.RemoveAt(0);
                            }
                            break;
                        case Direction.Back:
                            if (y < tempConv.backPos.Y)
                            {
                                tempConv.innerOffSet = new Point3D(tempConv.innerOffSet.X, tempConv.innerOffSet.Y + tempConv.speed, tempConv.innerOffSet.Z);
                                product_Info[shelfNo].ProductOffSet = new Point3D(tempOffSet.X + tempConv.innerOffSet.X, tempOffSet.Y + tempConv.innerOffSet.Y, tempOffSet.Z + tempConv.innerOffSet.Z);
                            }
                            else
                            {
                                product_Info[shelfNo].ProductOffSet = new Point3D(tempConv.backPos.X - modelPosition.ProductOriPos.X, tempConv.backPos.Y - modelPosition.ProductOriPos.Y, tempConv.backPos.Z - modelPosition.ProductOriPos.Z);
                                pathNodes.RemoveAt(0);
                            }

                            break;
                        case Direction.Right:
                            if (x > tempConv.rightPos.X)
                            {
                                tempConv.innerOffSet = new Point3D(tempConv.innerOffSet.X - tempConv.speed, tempConv.innerOffSet.Y, tempConv.innerOffSet.Z);
                                product_Info[shelfNo].ProductOffSet = new Point3D(tempOffSet.X + tempConv.innerOffSet.X, tempOffSet.Y + tempConv.innerOffSet.Y, tempOffSet.Z + tempConv.innerOffSet.Z);
                            }
                            else
                            {
                                product_Info[shelfNo].ProductOffSet = new Point3D(tempConv.rightPos.X - modelPosition.ProductOriPos.X, tempConv.rightPos.Y - modelPosition.ProductOriPos.Y, tempConv.rightPos.Z - modelPosition.ProductOriPos.Z);
                                pathNodes.RemoveAt(0);
                            }

                            break;
                        case Direction.Left:
                            if (x > tempConv.leftPos.X)
                            {
                                tempConv.innerOffSet = new Point3D(tempConv.innerOffSet.X + tempConv.speed, tempConv.innerOffSet.Y, tempConv.innerOffSet.Z);
                                product_Info[shelfNo].ProductOffSet = new Point3D(tempOffSet.X + tempConv.innerOffSet.X, tempOffSet.Y + tempConv.innerOffSet.Y, tempOffSet.Z + tempConv.innerOffSet.Z);
                            }
                            else
                            {
                                product_Info[shelfNo].ProductOffSet = new Point3D(tempConv.leftPos.X - modelPosition.ProductOriPos.X, tempConv.leftPos.Y - modelPosition.ProductOriPos.Y, tempConv.leftPos.Z - modelPosition.ProductOriPos.Z);
                                pathNodes.RemoveAt(0);
                            }
                            break;

                        case Direction.Top_Rigth:

                            tempConv = (Stacker1_2)tempConv;

                            if (x > tempConv.rightTopPos.X)//X 未就绪
                            {
                                if (z < tempConv.rightTopPos.Z)//Z未就绪
                                {
                                    tempConv.innerOffSet = new Point3D(tempConv.innerOffSet.X - tempConv.speed, tempConv.innerOffSet.Y, tempConv.innerOffSet.Z + tempConv.speed);
                                    product_Info[shelfNo].ProductOffSet = new Point3D(tempOffSet.X + tempConv.innerOffSet.X, tempOffSet.Y + tempConv.innerOffSet.Y, tempOffSet.Z + tempConv.innerOffSet.Z);
                                }
                                else//Z就绪
                                {
                                    tempConv.innerOffSet = new Point3D(tempConv.innerOffSet.X - tempConv.speed, tempConv.innerOffSet.Y, tempConv.rightTopPos.Z - modelPosition.ProductOriPos.Z);
                                    product_Info[shelfNo].ProductOffSet = new Point3D(tempOffSet.X + tempConv.innerOffSet.X, tempOffSet.Y + tempConv.innerOffSet.Y, tempConv.rightTopPos.Z - modelPosition.ProductOriPos.Z);
                                }
                            }
                            else//X就绪
                            {
                                if (z < tempConv.rightTopPos.Z)//Z未就绪
                                {
                                    tempConv.innerOffSet = new Point3D(tempConv.rightTopPos.X - modelPosition.ProductOriPos.X, tempConv.innerOffSet.Y, tempConv.innerOffSet.Z + tempConv.speed);
                                    product_Info[shelfNo].ProductOffSet = new Point3D(tempConv.rightTopPos.X - modelPosition.ProductOriPos.X, tempOffSet.Y + tempConv.innerOffSet.Y, tempOffSet.Z + tempConv.innerOffSet.Z);
                                }
                                else//Z就绪
                                {
                                    tempConv.innerOffSet = new Point3D(tempConv.rightTopPos.X - modelPosition.ProductOriPos.X, tempConv.innerOffSet.Y, tempConv.rightTopPos.Z - modelPosition.ProductOriPos.Z);
                                    product_Info[shelfNo].ProductOffSet = new Point3D(tempConv.rightTopPos.X - modelPosition.ProductOriPos.X, tempConv.rightTopPos.Y - modelPosition.ProductOriPos.Y, tempConv.rightTopPos.Z - modelPosition.ProductOriPos.Z);
                                    pathNodes.RemoveAt(0);
                                }
                            }
                            stackerParts_Info["duiduojilizhu001"].StackerOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X + modelPosition.ProductOriPos.X - modelPosition.StackerOriPos_1.X, 0, 0);
                            stackerParts_Info["zaihuotai001"].StackerOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X + modelPosition.ProductOriPos.X - modelPosition.PlatformOriPos_1.X, 0, product_Info[shelfNo].ProductOffSet.Z + modelPosition.ProductOriPos.Z - modelPosition.PlatformOriPos_1.Z - 650);
                            stackerParts_Info["shangcha001"].StackerOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X + modelPosition.ProductOriPos.X - modelPosition.TopForkOriPos_1.X, 0, product_Info[shelfNo].ProductOffSet.Z + modelPosition.ProductOriPos.Z - modelPosition.TopForkOriPos_1.Z - 650);
                            stackerParts_Info["zhongcha001"].StackerOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X + modelPosition.ProductOriPos.X - modelPosition.MiddleForkOriPos_1.X, 0, product_Info[shelfNo].ProductOffSet.Z + modelPosition.ProductOriPos.Z - modelPosition.MiddleForkOriPos_1.Z - 650);
                            stackerParts_Info["xiacha001"].StackerOffSet = new Point3D(product_Info[shelfNo].ProductOffSet.X + modelPosition.ProductOriPos.X - modelPosition.BottomForkOriPos_1.X, 0, product_Info[shelfNo].ProductOffSet.Z + modelPosition.ProductOriPos.Z - modelPosition.BottomForkOriPos_1.Z - 650);
                            stackerParts_Info["VIFS001"].StackerOffSet = new Point3D(stackerParts_Info["zaihuotai001"].StackerOffSet.X, stackerParts_Info["zaihuotai001"].StackerOffSet.Y, stackerParts_Info["zaihuotai001"].StackerOffSet.Z);
                            break;
                    }
                }
                //perspectiveCamera.Position = new Point3D(perspectiveCamera.Position.X + tempOffSet.X, perspectiveCamera.Position.Y + tempOffSet.Y, perspectiveCamera.Position.Z + tempOffSet.Z);
            }
        }
        public void Update(string conveyorName, bool isReady)
        {
            switch (conveyorName)
            {
                case "conveyor_1":
                    for (int i = 0; i < pathNodes.Count; i++)
                    {
                        if (pathNodes[i].name.Equals("conveyor_1"))
                        {
                            pathNodes[i].isReady = true;
                        }
                    }
                    conveyor_1.isReady = isReady;
                    break;
                case "conveyor_4":
                    for (int i = 0; i < pathNodes.Count; i++)
                    {
                        if (pathNodes[i].name.Equals("conveyor_4"))
                        {
                            pathNodes[i].isReady = true;
                        }
                    }
                    break;
                case "conveyor_3":
                    for (int i = 0; i < pathNodes.Count; i++)
                    {
                        if (pathNodes[i].name.Equals("conveyor_3"))
                        {
                            pathNodes[i].isReady = true;
                        }
                    }
                    break;
                case "conveyor_18":
                    for (int i = 0; i < pathNodes.Count; i++)
                    {
                        if (pathNodes[i].name.Equals("conveyor_18"))
                        {
                            pathNodes[i].isReady = true;
                        }
                    }
                    break;
                case "conveyor_17":
                    for (int i = 0; i < pathNodes.Count; i++)
                    {
                        if (pathNodes[i].name.Equals("conveyor_17"))
                        {
                            pathNodes[i].isReady = true;
                        }
                    }
                    break;
                case "ds_1":
                    for (int i = 0; i < pathNodes.Count; i++)
                    {
                        if (pathNodes[i].name.Equals("ds_1"))
                        {
                            pathNodes[i].isReady = true;
                        }
                    }
                    break;
                case "ds_3":
                    for (int i = 0; i < pathNodes.Count; i++)
                    {
                        if (pathNodes[i].name.Equals("ds_3"))
                        {
                            pathNodes[i].isReady = true;
                        }
                    }
                    break;
                case "ds_5":
                    for (int i = 0; i < pathNodes.Count; i++)
                    {
                        if (pathNodes[i].name.Equals("ds_5"))
                        {
                            pathNodes[i].isReady = true;
                        }
                    }
                    break;
                case "stacker1_1":
                    for (int i = 0; i < pathNodes.Count; i++)
                    {
                        if (pathNodes[i].name.Equals("stacker1_1"))
                        {
                            pathNodes[i].isReady = true;
                        }
                    }
                    break;
                case "stacker1_2":
                    for (int i = 0; i < pathNodes.Count; i++)
                    {
                        if (pathNodes[i].name.Equals("stacker1_2"))
                        {
                            pathNodes[i].isReady = true;
                        }
                    }
                    break;
                case "stacker1_3":
                    for (int i = 0; i < pathNodes.Count; i++)
                    {
                        if (pathNodes[i].name.Equals("stacker1_3"))
                        {
                            pathNodes[i].isReady = true;
                        }
                    }
                    break;
            }
            timer.Start();
        }
    }
}
