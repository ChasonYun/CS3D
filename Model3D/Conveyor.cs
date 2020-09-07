using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace CS3D
{
    public enum Direction_//位置
    {
        Horizontal = 1,//横向移动  左右
        Vertical,//纵向移动  前后
        Roadway,//斜（斜 左 右 上 下）货叉跟随即可
        PortIn,//入货方向 根据任务类型 判断 货物是否跟随运动
        PortOut,//出货方向 根据任务类型 判断 货物是否跟随运动
        ShelfIn,//伸叉 
        ShelfOut//取货
    }

    public class ConveyorFactory
    {
        public static Conveyor GetConveyor(string conveyorName, MissionType missionType, string shelfNo, Direction_ Direction, ModelPosition modelPosition)
        {
            Conveyor conveyor = null;
            switch (Direction)
            {
                case Direction_.Vertical:
                    conveyor = new Conveyor_V(missionType, conveyorName, Direction);
                    break;
                case Direction_.Horizontal:
                    conveyor = new Conveyor_H(missionType, conveyorName, Direction);
                    break;
                case Direction_.Roadway:
                    conveyor = new Stacker_RoadWay(missionType, conveyorName, shelfNo, modelPosition, Direction);
                    break;
                case Direction_.PortIn:
                    conveyor = new Stacker_Port(missionType, conveyorName, Direction);
                    break;
                case Direction_.PortOut:
                    conveyor = new Stacker_Port(missionType, conveyorName, Direction);
                    break;
                case Direction_.ShelfOut:
                    conveyor = new Stacker_ShelfOut(missionType, conveyorName, shelfNo, modelPosition, Direction);
                    break;
                case Direction_.ShelfIn:
                    conveyor = new Stacker_ShelfIn(missionType, conveyorName, shelfNo, modelPosition, Direction);
                    break;
            }
            return conveyor;
        }

        public static Conveyor GetConveyor(string conveyorName, MissionType missionType, string startShelfNo, string endShelfNo, Direction_ Direction_, ModelPosition modelPosition)
        {
            Conveyor conveyor = null;
            switch (Direction_)
            {
                case Direction_.Roadway:
                    conveyor = new Stacker_RoadWay(missionType, conveyorName, startShelfNo, endShelfNo, modelPosition, Direction_);
                    break;
            }
            return conveyor;
        }
    }

    /// <summary>
    /// 抽象 传送带 类 
    /// 
    /// </summary>
    /// 
    public abstract class Conveyor
    {
        public Point3D StartPos { get; set; }
        public Point3D EndPos { get; set; }

        public double Speed_Y { get; set; }
        public double Speed_Z { get; set; }
        public double Speed_X { get; set; }

        public bool IsReady { get; set; }

        public Direction_ Direction { get; set; }

        public string Name { get; set; }
        protected List<IMission> missions = new List<IMission>();//观察者列表  任务观察 输送带

        public Conveyor(string name, Direction_ Direction)
        {
            this.Name = name;
            this.Direction = Direction;
        }

        /// <summary>
        /// 设置起点与终点 
        /// </summary>
        /// <param name="startPos"> 起点</param>
        /// <param name="endPos">终点</param>
        protected void SetPos(Point3D startPos, Point3D endPos)
        {
            this.StartPos = startPos;
            this.EndPos = endPos;
        }

        public double GetSpeed_X()
        {
            return Convert.ToDouble(XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + Name + "/speed_x"));
        }
        public double GetSpeed_Y()
        {
            return Convert.ToDouble(XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + Name + "/speed_y"));
        }
        public double GetSpeed_Z()
        {
            return Convert.ToDouble(XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + Name + "/speed_z"));
        }

        protected void SetSpeed()
        {
            this.Speed_X = GetSpeed_X();
            this.Speed_Y = GetSpeed_Y();
            this.Speed_Z = GetSpeed_Z();
        }

        public void Attach(IMission mission)
        {
            missions.Add(mission);
        }

        public void Detach(IMission mission)
        {
            missions.Remove(mission);
        }

        public void Notify(string missionId, string name, bool isReady)
        {
            foreach (IMission obs in missions)
            {
                ((IMission)obs).Update(missionId, name, isReady);
            }
        }
    }

    /// <summary>
    /// 水平运动的输送带  根据 任务类型  自动判断 起点与终点的值
    /// </summary>
    public class Conveyor_H : Conveyor
    {
        public Conveyor_H(MissionType missionType, string conveyorName, Direction_ Direction_) : base(conveyorName, Direction_)
        {
            try
            {
                SetSpeed();
                if (missionType.Equals(MissionType.ProdIn) || missionType.Equals(MissionType.PalletIn) || missionType.Equals(MissionType.BackIn))
                {
                    SetPos(GetLeftPos(conveyorName), GetRightPos(conveyorName));
                }
                else if (missionType.Equals(MissionType.ProdOut) || missionType.Equals(MissionType.PalletOut) || missionType.Equals(MissionType.GetPallet) || missionType.Equals(MissionType.AddPallet))
                {
                    SetPos(GetRightPos(conveyorName), GetLeftPos(conveyorName));
                }
                else
                {
                    throw new Exception("Conveyor_H has exception: Undefined MissionType");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private Point3D GetLeftPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/leftPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);
            return new Point3D(x, y, z);
        }

        private Point3D GetRightPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/rightPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);
            return new Point3D(x, y, z);
        }
    }

    /// <summary>
    /// 竖直方向的输送带
    /// </summary>
    public class Conveyor_V : Conveyor
    {
        public Conveyor_V(MissionType missionType, string conveyorName, Direction_ Direction_) : base(conveyorName, Direction_)
        {
            try
            {
                SetSpeed();
                if (missionType.Equals(MissionType.ProdIn) || missionType.Equals(MissionType.BackIn) || missionType.Equals(MissionType.PalletIn) || missionType.Equals(MissionType.AddPallet))
                {
                    SetPos(GetBackPos(conveyorName), GetFrontPos(conveyorName));
                }
                else if (missionType.Equals(MissionType.ProdOut) || missionType.Equals(MissionType.PalletOut) || missionType.Equals(MissionType.GetPallet))
                {
                    SetPos(GetFrontPos(conveyorName), GetBackPos(conveyorName));
                }
                else
                {
                    throw new Exception("Conveyor_V has exception: Undefined MissionType");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private Point3D GetBackPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/backPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);
            return new Point3D(x, y, z);
        }

        private Point3D GetFrontPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/frontPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);
            return new Point3D(x, y, z);
        }
    }

    public class Stacker_Port : Conveyor
    {
        public Stacker_Port(MissionType missionType, string conveyorName, Direction_ Direction_) : base(conveyorName, Direction_)
        {
            SetPos(GetBackPos(conveyorName), GetFrontPos(conveyorName));
            SetSpeed();
        }

        private Point3D GetBackPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/backPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);
            return new Point3D(x, y, z);
        }

        private Point3D GetFrontPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/frontPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);
            return new Point3D(x, y, z);
        }
    }

    /// <summary>
    /// 出叉
    /// </summary>
    public class Stacker_Port_ForkIn : Conveyor
    {
        public Stacker_Port_ForkIn(MissionType missionType, string conveyorName, Direction_ Direction_) : base(conveyorName, Direction_)
        {
            if (missionType.Equals(MissionType.ProdIn) || missionType.Equals(MissionType.BackIn) || missionType.Equals(MissionType.PalletIn) || missionType.Equals(MissionType.AddPallet))
            {
                SetPos(GetBackPos(conveyorName), GetFrontPos(conveyorName));
            }
            else if (missionType.Equals(MissionType.ProdOut) || missionType.Equals(MissionType.PalletOut) || missionType.Equals(MissionType.GetPallet))
            {
                SetPos(GetBackPos(conveyorName), GetFrontPos(conveyorName));
            }
            SetSpeed();
        }

        private Point3D GetBackPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/backPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);
            return new Point3D(x, y, z);
        }

        private Point3D GetFrontPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/frontPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);
            return new Point3D(x, y, z);
        }
    }

    public class Stacker_RoadWay : Conveyor
    {
        public Stacker_RoadWay(MissionType missionType, string conveyorName, string shelfNo, ModelPosition modelPosition, Direction_ Direction_) : base(conveyorName, Direction_)
        {
            if (missionType.Equals(MissionType.ProdIn) || missionType.Equals(MissionType.BackIn) || missionType.Equals(MissionType.PalletIn))
            {
                SetPos(GetLeftBottomPos(conveyorName), modelPosition.GetShelfLinePos(shelfNo));
            }
            else if (missionType.Equals(MissionType.ProdOut) || missionType.Equals(MissionType.PalletOut) || missionType.Equals(MissionType.GetPallet))
            {
                SetPos(modelPosition.GetShelfLinePos(shelfNo), GetLeftBottomPos(conveyorName));
            }
            SetSpeed();
        }

        public Stacker_RoadWay(MissionType missionType, string conveyorName, string startShelfNo, string endShelfNo, ModelPosition modelPosition, Direction_ Direction_) : base(conveyorName, Direction_)
        {

            SetPos(modelPosition.GetShelfLinePos(startShelfNo), modelPosition.GetShelfLinePos(endShelfNo));
            SetSpeed();
        }

        private Point3D GetLeftBottomPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/leftBottomPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);
            return new Point3D(x, y, z);
        }
    }

  
    public class Stacker_ShelfIn : Conveyor
    {
        public Stacker_ShelfIn(MissionType missionType, string conveyorName, string shelfNo, ModelPosition modelPosition, Direction_ direction) : base(conveyorName, direction)
        {
            SetPos(modelPosition.GetShelfLinePos(shelfNo), modelPosition.GetShelfPos(shelfNo));
            SetSpeed();
        }
    }

 
    public class Stacker_ShelfOut : Conveyor
    {
        public Stacker_ShelfOut(MissionType missionType,string conveyorName, string shelfNo, ModelPosition modelPosition, Direction_ direction) : base(conveyorName, direction)
        {
            SetPos(modelPosition.GetShelfPos(shelfNo), modelPosition.GetShelfLinePos(shelfNo));
            SetSpeed();
        }
    }
    //public class Conveyor
    //{
    //    private bool isReady_;

    //    private string name_;
    //    private double speed_x;
    //    private double speed_y;
    //    private double speed_z;


    //    private Point3D startPos_;
    //    private Point3D endPos_;
    //    private Direction_ Direction__;

    //    public Conveyor(string name, string missionType, string shelfNo, Direction_ Direction_, ModelPosition modelPosition)
    //    {
    //        this.name_ = name;
    //        this.Direction__ = Direction_;
    //        this.speed_x = GetSpeed_X(name_);
    //        this.speed_y = GetSpeed_Y(name_);
    //        this.speed_z = GetSpeed_Z(name_);
    //        if (missionType.Equals("StockIn"))
    //        {

    //            switch (Direction_)
    //            {
    //                case Direction_.Horizontal://输送带 横向
    //                    this.startPos_ = GetLeftPos(name);
    //                    this.endPos_ = GetRightPos(name);
    //                    break;
    //                case Direction_.Vertical://输送带 纵向
    //                    this.startPos_ = GetBackPos(name);
    //                    this.endPos_ = GetFrontPos(name);
    //                    break;
    //                case Direction_.Roadway://堆垛机 斜向
    //                    this.startPos_ = GetLeftBottomPos(name);
    //                    this.endPos_ = modelPosition.GetShelfLinePos(shelfNo);
    //                    break;
    //                case Direction_.Port://取放货口 
    //                    this.startPos_ = GetBackPos(name);
    //                    this.endPos_ = GetFrontPos(name);
    //                    break;
    //                case Direction_.Shelf://进出货位
    //                    this.startPos_ = modelPosition.GetShelfLinePos(shelfNo);
    //                    this.endPos_ = modelPosition.GetShelfPos(shelfNo);
    //                    break;
    //            }

    //        }
    //        else if (missionType.Equals("StockOut"))
    //        {
    //            switch (Direction_)
    //            {
    //                case Direction_.Horizontal:
    //                    this.startPos_ = GetRightPos(Name);
    //                    this.endPos_ = GetLeftPos(Name);
    //                    break;
    //                case Direction_.Vertical:
    //                    this.startPos_ = GetFrontPos(Name);
    //                    this.endPos_ = GetBackPos(Name);
    //                    break;
    //                case Direction_.Roadway:
    //                    this.endPos_ = GetLeftBottomPos(this.Name);//终点 
    //                    this.startPos_ = modelPosition.GetShelfLinePos(shelfNo);//巷道中的位置
    //                    break;
    //                case Direction_.Port:
    //                    this.startPos_ = GetBackPos(name);
    //                    this.endPos_ = GetFrontPos(name);
    //                    break;
    //                case Direction_.Shelf:
    //                    this.startPos_ = modelPosition.GetShelfPos(shelfNo);
    //                    this.endPos_ = modelPosition.GetShelfLinePos(shelfNo);
    //                    break;
    //            }

    //        }

    //    }
    //    public bool IsReady { get => isReady_; set => isReady_ = value; }

    //    public double Speed_X { get => speed_x; }
    //    public double Speed_Y { get => speed_y; }
    //    public double Speed_Z { get => speed_z; }

    //    public string Name { get => name_; }

    //    public Point3D StartPos { get => startPos_; set => startPos_ = value; }

    //    public Point3D EndPos { get => endPos_; set => endPos_ = value; }
    //    public Direction_ Direction_ { get => Direction__; set => Direction__ = value; }

    //    protected List<IMission> observers = new List<IMission>();

    //    public void Attach(IMission mission)
    //    {
    //        observers.Add(mission);
    //    }

    //    public void Detach(IMission mission)
    //    {
    //        observers.Remove(mission);
    //    }

    //    public virtual double GetSpeed_X(string conveyorName)
    //    {
    //        return Convert.ToDouble(XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/speed_x"));
    //    }
    //    public virtual double GetSpeed_Y(string conveyorName)
    //    {
    //        return Convert.ToDouble(XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/speed_y"));
    //    }
    //    public virtual double GetSpeed_Z(string conveyorName)
    //    {
    //        return Convert.ToDouble(XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/speed_z"));
    //    }
    //    public virtual Point3D GetBackPos(string conveyorName)
    //    {
    //        string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/backPos");
    //        string[] tempStrs = tempStr.Trim().Split(',');
    //        double x = Convert.ToDouble(tempStrs[0]);
    //        double y = Convert.ToDouble(tempStrs[1]);
    //        double z = Convert.ToDouble(tempStrs[2]);

    //        return new Point3D(x, y, z);
    //    }

    //    public virtual Point3D GetFrontPos(string conveyorName)
    //    {
    //        string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/frontPos");
    //        string[] tempStrs = tempStr.Trim().Split(',');
    //        double x = Convert.ToDouble(tempStrs[0]);
    //        double y = Convert.ToDouble(tempStrs[1]);
    //        double z = Convert.ToDouble(tempStrs[2]);

    //        return new Point3D(x, y, z);
    //    }

    //    public virtual Point3D GetLeftPos(string conveyorName)
    //    {
    //        string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/leftPos");
    //        string[] tempStrs = tempStr.Trim().Split(',');
    //        double x = Convert.ToDouble(tempStrs[0]);
    //        double y = Convert.ToDouble(tempStrs[1]);
    //        double z = Convert.ToDouble(tempStrs[2]);

    //        return new Point3D(x, y, z);
    //    }
    //    public virtual Point3D GetRightPos(string conveyorName)
    //    {
    //        string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/rightPos");
    //        string[] tempStrs = tempStr.Trim().Split(',');
    //        double x = Convert.ToDouble(tempStrs[0]);
    //        double y = Convert.ToDouble(tempStrs[1]);
    //        double z = Convert.ToDouble(tempStrs[2]);

    //        return new Point3D(x, y, z);
    //    }

    //    public virtual Point3D GetLeftBottomPos(string conveyorName)
    //    {
    //        string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/leftBottomPos");
    //        string[] tempStrs = tempStr.Trim().Split(',');
    //        double x = Convert.ToDouble(tempStrs[0]);
    //        double y = Convert.ToDouble(tempStrs[1]);
    //        double z = Convert.ToDouble(tempStrs[2]);
    //        return new Point3D(x, y, z);
    //    }

    //    public virtual Point3D GetLeftTopPos(string conveyorName)
    //    {
    //        string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/leftTopPos");
    //        string[] tempStrs = tempStr.Trim().Split(',');
    //        double x = Convert.ToDouble(tempStrs[0]);
    //        double y = Convert.ToDouble(tempStrs[1]);
    //        double z = Convert.ToDouble(tempStrs[2]);
    //        return new Point3D(x, y, z);
    //    }

    //    public virtual Point3D GetRightTopPos(string conveyorName)
    //    {
    //        string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/rightTopPos");
    //        string[] tempStrs = tempStr.Trim().Split(',');
    //        double x = Convert.ToDouble(tempStrs[0]);
    //        double y = Convert.ToDouble(tempStrs[1]);
    //        double z = Convert.ToDouble(tempStrs[2]);
    //        return new Point3D(x, y, z);
    //    }

    //    public virtual Point3D GetRightBottomPos(string conveyorName)
    //    {
    //        string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/rightBottomPos");
    //        string[] tempStrs = tempStr.Trim().Split(',');
    //        double x = Convert.ToDouble(tempStrs[0]);
    //        double y = Convert.ToDouble(tempStrs[1]);
    //        double z = Convert.ToDouble(tempStrs[2]);
    //        return new Point3D(x, y, z);
    //    }


    //    public void Notify()//通知方法
    //    {
    //        foreach (var obs in observers)
    //        {
    //            ((IMission)obs).Update(Name, IsReady);
    //        }
    //    }
    //}

}
