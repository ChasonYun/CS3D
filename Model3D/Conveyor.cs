using System;
using System.Collections;
using System.Windows.Media.Media3D;

namespace CS3D
{
    public enum Direction//运动方向
    {
        Right = 1,
        Left,
        Front,
        Back,
        Bottom,
        Top,
        Top_Rigth,
        Bottom_Right,
        Top_Left,
        Bottom_Left
    }
    public class Conveyor
    {
        private bool isReady_;
        private Direction direction_;
        private Point3D innerOffSet_;

        private string name_;
        private double speed_;

        private Point3D backPos_;
        private Point3D frontPos_;
        private Point3D leftPos_;
        private Point3D rightPos_;

        private Point3D rightTopPos_;
        private Point3D rightBottomPos_;
        private Point3D leftTopPos_;
        private Point3D leftBottomPos_;
        public Conveyor(string name, Direction direction)
        {
            this.name_ = name;
            this.direction_ = direction;
            this.speed_ = GetSpeed(name_);
            this.backPos_ = GetBackPos(name_);
            this.frontPos_ = GetFrontPos(name_);
            this.rightPos_ = GetRightPos(name_);
            this.leftPos_ = GetLeftPos(name_);
            this.rightTopPos_ = GetRightTopPos(name_);
            this.rightBottomPos_ = GetRightBottomPos(name_);
            this.leftTopPos_ = GetLeftTopPos(name_);
            this.leftBottomPos_ = GetLeftBottomPos(name_);
        }
        public bool IsReady { get => isReady_; set => isReady_ = value; }


        public double Speed { get => speed_; }
        public Point3D InnerOffSet { get => innerOffSet_; set => innerOffSet_ = value; }

        public Point3D BackPos { get => backPos_; }

        public Point3D FrontPos { get => frontPos_; }

        public Point3D LeftPos { get => leftPos_; }
        public Point3D RightPos { get => rightPos_; }

        public string Name { get => name_; }

        public Direction Direction { get => direction_; set => direction_ = value; }

        public Point3D LeftBottomPos { get => leftBottomPos_; set => leftBottomPos_ = value; }

        public Point3D RightTopPos { get => rightTopPos_; set => rightTopPos_ = value; }

        public Point3D LeftTopPos { get => leftTopPos_; set => leftTopPos_ = value; }

        public Point3D RightBottomPos { get => rightBottomPos_; set => rightBottomPos_ = value; }

        protected ArrayList observers = new ArrayList();//

        public void Attach(IMission mission)
        {
            observers.Add(mission);
        }

        public void Detach(IMission mission)
        {
            observers.Remove(mission);
        }

        public virtual double GetSpeed(string conveyorName)
        {
            return Convert.ToDouble(XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/speed"));
        }

        public virtual Point3D GetBackPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/backPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);

            return new Point3D(x, y, z);
        }

        public virtual Point3D GetFrontPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/frontPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);

            return new Point3D(x, y, z);
        }

        public virtual Point3D GetLeftPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/leftPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);

            return new Point3D(x, y, z);
        }
        public virtual Point3D GetRightPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/rightPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);

            return new Point3D(x, y, z);
        }

        public virtual Point3D GetLeftBottomPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/leftBottomPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);
            return new Point3D(x, y, z);
        }

        public virtual Point3D GetLeftTopPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/leftTopPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);
            return new Point3D(x, y, z);
        }

        public virtual Point3D GetRightTopPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/rightTopPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);
            return new Point3D(x, y, z);
        }

        public virtual Point3D GetRightBottomPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/rightBottomPos");
            string[] tempStrs = tempStr.Trim().Split(',');
            double x = Convert.ToDouble(tempStrs[0]);
            double y = Convert.ToDouble(tempStrs[1]);
            double z = Convert.ToDouble(tempStrs[2]);
            return new Point3D(x, y, z);
        }

        public void Notify()//通知方法
        {
            foreach (var obs in observers)
            {
                ((IMission)obs).Update(Name, IsReady);
            }
        }
    }

    public class Conveyor_1 : Conveyor
    {
        public Conveyor_1(Direction direction) : base("conveyor_1", direction)
        {

        }
    }

    public class Conveyor_2 : Conveyor
    {
        public Conveyor_2(Direction direction) : base("conveyor_2", direction)
        {

        }
    }
    public class Conveyor_3 : Conveyor
    {

        public Conveyor_3(Direction direction) : base("conveyor_3", direction)
        {

        }
    }
    public class Conveyor_4 : Conveyor
    {

        public Conveyor_4(Direction direction) : base("conveyor_4", direction)
        {

        }
    }
    public class Conveyor_5 : Conveyor
    {

        public Conveyor_5(Direction direction) : base("conveyor_5", direction)
        {

        }
    }

    public class Conveyor_6 : Conveyor
    {

        public Conveyor_6(Direction direction) : base("conveyor_6", direction)
        {

        }
    }
    public class Conveyor_7 : Conveyor
    {
        public Conveyor_7(Direction direction) : base("conveyor_7", direction)
        {

        }
    }
    public class Conveyor_8 : Conveyor
    {
        public Conveyor_8(Direction direction) : base("conveyor_8", direction)
        {

        }
    }
    public class Conveyor_9 : Conveyor
    {

        public Conveyor_9(Direction direction) : base("conveyor_9", direction)
        {

        }
    }
    public class Conveyor_10 : Conveyor
    {

        public Conveyor_10(Direction direction) : base("conveyor_10", direction)
        {

        }
    }
    public class Conveyor_11 : Conveyor
    {

        public Conveyor_11(Direction direction) : base("conveyor_11", direction)
        {

        }
    }
    public class Conveyor_12 : Conveyor
    {

        public Conveyor_12(Direction direction) : base("conveyor_12", direction)
        {

        }
    }
    public class Conveyor_13 : Conveyor
    {

        public Conveyor_13(Direction direction) : base("conveyor_13", direction)
        {

        }
    }

    public class Conveyor_14 : Conveyor
    {

        public Conveyor_14(Direction direction) : base("conveyor_14", direction)
        {

        }
    }

    public class Conveyor_15 : Conveyor
    {
        private bool isReady_;
        public Conveyor_15(Direction direction) : base("conveyor_15", direction)
        {

        }
    }
    public class Conveyor_16 : Conveyor
    {

        public Conveyor_16(Direction direction) : base("conveyor_16", direction)
        {

        }
    }
    public class Conveyor_17 : Conveyor
    {
        public Conveyor_17(Direction direction) : base("conveyor_17", direction)
        {

        }
    }
    public class Conveyor_18 : Conveyor
    {
        public Conveyor_18(Direction direction) : base("conveyor_18", direction)
        {

        }
    }
    public class Conveyor_19 : Conveyor
    {

        public Conveyor_19(Direction direction) : base("conveyor_19", direction)
        {

        }
    }
    public class Conveyor_20 : Conveyor
    {

        public Conveyor_20(Direction direction) : base("conveyor_20", direction)
        {

        }
    }
    public class Conveyor_21 : Conveyor
    {

        public Conveyor_21(Direction direction) : base("conveyor_21", direction)
        {

        }
    }

    public class DS_1 : Conveyor
    {
        public DS_1(Direction direction) : base("ds_1", direction)
        {

        }
    }
    public class DS_2 : Conveyor
    {
        public DS_2(Direction direction) : base("ds_2", direction)
        {

        }
    }
    public class DS_3 : Conveyor
    {
        public DS_3(Direction direction) : base("ds_3", direction)
        {

        }
    }
    public class DS_4 : Conveyor
    {
        public DS_4(Direction direction) : base("ds_4", direction)
        {

        }
    }
    public class DS_5 : Conveyor
    {
        public DS_5(Direction direction) : base("ds_5", direction)
        {

        }
    }




    public class Stacker1_1 : Conveyor
    {
        public Stacker1_1(Direction direction) : base("stacker1_1", direction)
        {

        }
    }

    public class Stacker1_2 : Conveyor
    {
        public Stacker1_2(Direction direction, string shelfNo, ModelPosition modelPosition) : base("stacker1_2", direction)
        {

        }
    }

    public class Stacker1_3 : Conveyor
    {
        public Stacker1_3(Direction direction, string shelfNo, ModelPosition modelPosition) : base("stacker1_3", direction)
        {

        }
    }
    public class Stacker1_4 : Conveyor
    {
        public Stacker1_4(Direction direction, string shelfNo, ModelPosition modelPosition) : base("stacker1_4", direction)
        {

        }
    }
    public class Stacker1_5 : Conveyor
    {
        public Stacker1_5(Direction direction) : base("stacker1_5", direction)
        {

        }
    }

    public class Stacker2_1 : Conveyor
    {
        public Stacker2_1(Direction direction) : base("stacker2_1", direction)
        {

        }
    }

    public class Stacker2_2 : Conveyor
    {
        public Stacker2_2(Direction direction, string shelfNo, ModelPosition modelPosition) : base("stacker2_2", direction)
        {

        }
    }

    public class Stacker2_3 : Conveyor
    {
        public Stacker2_3(Direction direction, string shelfNo, ModelPosition modelPosition) : base("stacker2_3", direction)
        {

        }
    }
    public class Stacker2_4 : Conveyor
    {
        public Stacker2_4(Direction direction, string shelfNo, ModelPosition modelPosition) : base("stacker2_4", direction)
        {

        }
    }
    public class Stacker2_5 : Conveyor
    {
        public Stacker2_5(Direction direction) : base("stacker2_5", direction)
        {

        }
    }
}
