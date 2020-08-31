using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace CS3D
{
    public enum Direction//位置
    {
        Horizontal = 1,//横向移动  左右
        Vertical,//纵向移动  前后
        Roadway,//斜（斜 左 右 上 下）
        Port,//取货 放货
        Shelf//放货
    }

    public class ConveyorFactory
    {
        public static Conveyor GetConveyor(string conveyorName, MissionType missionType, string shelfdNo, Direction direction, ModelPosition modelPosition)
        {
            Conveyor conveyor = null;
            switch (direction)
            {
                case Direction.Vertical:
                    conveyor = new Conveyor_V(missionType, conveyorName, direction);
                    break;
                case Direction.Horizontal:
                    conveyor = new Conveyor_H(missionType, conveyorName, direction);
                    break;
                case Direction.Roadway:
                    conveyor = new Stacker_RoadWay(missionType, conveyorName, shelfdNo, modelPosition, direction);
                    break;
                case Direction.Port:
                    conveyor = new Stacker_Port(missionType, conveyorName, direction);
                    break;
                case Direction.Shelf:
                    conveyor = new Stacker_Shelf(missionType, conveyorName, shelfdNo, modelPosition, direction);
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

        public Direction Direction { get; set; }

        public string Name { get; set; }
        protected List<IMission> missions = new List<IMission>();//观察者列表  任务观察 输送带

        public Conveyor(string name, Direction direction)
        {
            this.Name = name;
            this.Direction = direction;
        }

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

        public void Notify(string name, bool isReady)
        {
            foreach (IMission obs in missions)
            {
                ((IMission)obs).Update(name, isReady);
            }
        }
    }

    /// <summary>
    /// 水平运动的输送带  根据 任务类型  自动判断 起点与终点的值
    /// </summary>
    public class Conveyor_H : Conveyor
    {
        public Conveyor_H(MissionType missionType, string conveyorName, Direction direction) : base(conveyorName, direction)
        {
            if (missionType.Equals(MissionType.StockIn))
            {
                SetPos(GetLeftPos(conveyorName), GetRightPos(conveyorName));
            }
            else if (missionType.Equals(MissionType.StockOut))
            {
                SetPos(GetRightPos(conveyorName), GetLeftPos(conveyorName));
            }
            SetSpeed();

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
        public Conveyor_V(MissionType missionType, string conveyorName, Direction direction) : base(conveyorName, direction)
        {
            if (missionType.Equals(MissionType.StockIn))
            {
                SetPos(GetBackPos(conveyorName), GetFrontPos(conveyorName));
            }
            else if (missionType.Equals(MissionType.StockOut))
            {
                SetPos(GetFrontPos(conveyorName), GetBackPos(conveyorName));
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

    /// <summary>
    /// 堆垛机进出货口
    /// </summary>
    public class Stacker_Port : Conveyor
    {
        public Stacker_Port(MissionType missionType, string conveyorName, Direction direction) : base(conveyorName, direction)
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

    public class Stacker_RoadWay : Conveyor
    {
        public Stacker_RoadWay(MissionType missionType, string conveyorName, string shelfNo, ModelPosition modelPosition, Direction direction) : base(conveyorName, direction)
        {
            if (missionType.Equals(MissionType.StockIn))
            {
                SetPos(GetLeftBottomPos(conveyorName), modelPosition.GetShelfLinePos(shelfNo));
            }
            else if (missionType.Equals(MissionType.StockOut))
            {
                SetPos(modelPosition.GetShelfLinePos(shelfNo), GetLeftBottomPos(conveyorName));
            }
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


    public class Stacker_Shelf : Conveyor
    {
        public Stacker_Shelf(MissionType missionType, string conveyorName, string shelfNo, ModelPosition modelPosition, Direction direction) : base(conveyorName, direction)
        {
            if (missionType.Equals(MissionType.StockIn))
            {
                SetPos(modelPosition.GetShelfLinePos(shelfNo), modelPosition.GetShelfPos(shelfNo));
            }
            else if (missionType.Equals(MissionType.StockOut))
            {
                SetPos(modelPosition.GetShelfPos(shelfNo), modelPosition.GetShelfLinePos(shelfNo));
            }
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
    //    private Direction direction_;

    //    public Conveyor(string name, string missionType, string shelfNo, Direction direction, ModelPosition modelPosition)
    //    {
    //        this.name_ = name;
    //        this.direction_ = direction;
    //        this.speed_x = GetSpeed_X(name_);
    //        this.speed_y = GetSpeed_Y(name_);
    //        this.speed_z = GetSpeed_Z(name_);
    //        if (missionType.Equals("StockIn"))
    //        {

    //            switch (direction)
    //            {
    //                case Direction.Horizontal://输送带 横向
    //                    this.startPos_ = GetLeftPos(name);
    //                    this.endPos_ = GetRightPos(name);
    //                    break;
    //                case Direction.Vertical://输送带 纵向
    //                    this.startPos_ = GetBackPos(name);
    //                    this.endPos_ = GetFrontPos(name);
    //                    break;
    //                case Direction.Roadway://堆垛机 斜向
    //                    this.startPos_ = GetLeftBottomPos(name);
    //                    this.endPos_ = modelPosition.GetShelfLinePos(shelfNo);
    //                    break;
    //                case Direction.Port://取放货口 
    //                    this.startPos_ = GetBackPos(name);
    //                    this.endPos_ = GetFrontPos(name);
    //                    break;
    //                case Direction.Shelf://进出货位
    //                    this.startPos_ = modelPosition.GetShelfLinePos(shelfNo);
    //                    this.endPos_ = modelPosition.GetShelfPos(shelfNo);
    //                    break;
    //            }

    //        }
    //        else if (missionType.Equals("StockOut"))
    //        {
    //            switch (direction)
    //            {
    //                case Direction.Horizontal:
    //                    this.startPos_ = GetRightPos(Name);
    //                    this.endPos_ = GetLeftPos(Name);
    //                    break;
    //                case Direction.Vertical:
    //                    this.startPos_ = GetFrontPos(Name);
    //                    this.endPos_ = GetBackPos(Name);
    //                    break;
    //                case Direction.Roadway:
    //                    this.endPos_ = GetLeftBottomPos(this.Name);//终点 
    //                    this.startPos_ = modelPosition.GetShelfLinePos(shelfNo);//巷道中的位置
    //                    break;
    //                case Direction.Port:
    //                    this.startPos_ = GetBackPos(name);
    //                    this.endPos_ = GetFrontPos(name);
    //                    break;
    //                case Direction.Shelf:
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
    //    public Direction Direction { get => direction_; set => direction_ = value; }

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
