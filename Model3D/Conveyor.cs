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
    public abstract class Conveyor
    {
        public abstract string name { get; }
        public abstract bool isReady { get; set; }//是否被占用

        public abstract Direction direction { get; set; }//运动方向（前、后、左、右）

        /// <summary>
        /// 内部偏移量
        /// </summary>
        public abstract Point3D innerOffSet { get; set; }

        /// <summary>
        /// 传送带速度
        /// </summary>
        public abstract double speed { get; }

        /// <summary>
        /// 后绝对位置
        /// </summary>
        public abstract Point3D backPos { get; }

        /// <summary>
        /// 前绝对位置
        /// </summary>
        public abstract Point3D frontPos { get; }

        /// <summary>
        /// 左绝对位置
        /// </summary>
        public abstract Point3D leftPos { get; }
        /// <summary>
        /// 右绝对位置
        /// </summary>
        public abstract Point3D rightPos { get; }

        /// <summary>
        /// 堆垛机 左下 坐标
        /// </summary>
        public abstract Point3D leftBottomPos { get; }
        /// <summary>
        /// 堆垛机  右上 坐标
        /// </summary>
        public abstract Point3D rightTopPos { get; }
        /// <summary>
        /// 堆垛机 左上 坐标
        /// </summary>
        public abstract Point3D leftTopPos { get; }

        /// <summary>
        /// 堆垛机 右下坐标 
        /// </summary>
        public abstract Point3D rightBottomPos { get; }

        protected ArrayList observers = new ArrayList();//

        public abstract void Attach(IMission mission);//

        public abstract void Detach(IMission observer);//

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

        public virtual Point3D GetBottomLeftPos(string conveyorName)
        {
            string tempStr = XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/leftbottomPos");
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
                ((IMission)obs).Update(name, isReady);
            }
        }
    }

    public class Conveyor_1 : Conveyor
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
        public Conveyor_1()
        {

        }
        public Conveyor_1(Direction direction)
        {
            this.name_ = "conveyor_1";
            this.direction_ = direction;
            this.speed_ = base.GetSpeed(name_);
            this.backPos_ = base.GetBackPos(name_);
            this.frontPos_ = base.GetFrontPos(name_);
        }
        public override bool isReady { get => isReady_; set => isReady_ = value; }


        public override double speed { get => speed_; }
        public override Point3D innerOffSet { get => innerOffSet_; set => innerOffSet_ = value; }

        public override Point3D backPos { get => backPos_; }

        public override Point3D frontPos { get => frontPos_; }

        public override Point3D leftPos { get => leftPos_; }
        public override Point3D rightPos { get => rightPos_; }

        public override string name { get => name_; }

        public override Direction direction { get => direction_; set => direction_ = value; }

        public override Point3D leftBottomPos => throw new NotImplementedException();

        public override Point3D rightTopPos => throw new NotImplementedException();

        public override Point3D leftTopPos => throw new NotImplementedException();

        public override Point3D rightBottomPos => throw new NotImplementedException();

        public override void Attach(IMission mission)
        {
            observers.Add(mission);
        }

        public override void Detach(IMission mission)
        {
            observers.Remove(mission);
        }
    }

    public class Conveyor_4 : Conveyor
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
        public Conveyor_4()
        {

        }
        public Conveyor_4(Direction direction)
        {
            this.name_ = "conveyor_4";
            this.direction_ = direction;
            this.speed_ = base.GetSpeed(name_);
            this.leftPos_ = base.GetLeftPos(name_);
            this.rightPos_ = base.GetRightPos(name_);
        }
        public override bool isReady { get => isReady_; set => isReady_ = value; }


        public override double speed { get => speed_; }
        public override Point3D innerOffSet { get => innerOffSet_; set => innerOffSet_ = value; }

        public override Point3D backPos { get => backPos_; }

        public override Point3D frontPos { get => frontPos_; }

        public override Point3D leftPos { get => leftPos_; }
        public override Point3D rightPos { get => rightPos_; }

        public override string name { get => name_; }

        public override Direction direction { get => direction_; set => direction_ = value; }

        public override Point3D leftBottomPos => throw new NotImplementedException();

        public override Point3D rightTopPos => throw new NotImplementedException();

        public override Point3D leftTopPos => throw new NotImplementedException();

        public override Point3D rightBottomPos => throw new NotImplementedException();

        public override void Attach(IMission mission)
        {
            observers.Add(mission);
        }

        public override void Detach(IMission mission)
        {
            observers.Remove(mission);
        }
    }
    public class Conveyor_3 : Conveyor
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
        public Conveyor_3()
        {

        }
        public Conveyor_3(Direction direction)
        {
            this.name_ = "conveyor_3";
            this.direction_ = direction;
            this.speed_ = base.GetSpeed(name_);
            this.leftPos_ = base.GetLeftPos(name_);
            this.rightPos_ = base.GetRightPos(name_);
        }
        public override bool isReady { get => isReady_; set => isReady_ = value; }


        public override double speed { get => speed_; }
        public override Point3D innerOffSet { get => innerOffSet_; set => innerOffSet_ = value; }

        public override Point3D backPos { get => backPos_; }

        public override Point3D frontPos { get => frontPos_; }

        public override Point3D leftPos { get => leftPos_; }
        public override Point3D rightPos { get => rightPos_; }

        public override string name { get => name_; }

        public override Direction direction { get => direction_; set => direction_ = value; }

        public override Point3D leftBottomPos => throw new NotImplementedException();

        public override Point3D rightTopPos => throw new NotImplementedException();

        public override Point3D leftTopPos => throw new NotImplementedException();

        public override Point3D rightBottomPos => throw new NotImplementedException();

        public override void Attach(IMission mission)
        {
            observers.Add(mission);
        }

        public override void Detach(IMission mission)
        {
            observers.Remove(mission);
        }
    }
    public class DS_1 : Conveyor
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
        public DS_1()
        {

        }
        public DS_1(Direction direction)
        {
            this.name_ = "ds_1";
            this.direction_ = direction;
            this.speed_ = base.GetSpeed(name_);
            this.backPos_ = base.GetBackPos(name_);
            this.frontPos_ = base.GetFrontPos(name_);
            this.rightPos_ = base.GetRightPos(name_);
            this.leftPos_ = base.GetLeftPos(name_);
        }
        public override bool isReady { get => isReady_; set => isReady_ = value; }


        public override double speed { get => speed_; }
        public override Point3D innerOffSet { get => innerOffSet_; set => innerOffSet_ = value; }

        public override Point3D backPos { get => backPos_; }

        public override Point3D frontPos { get => frontPos_; }

        public override Point3D leftPos { get => leftPos_; }
        public override Point3D rightPos { get => rightPos_; }

        public override string name { get => name_; }

        public override Direction direction { get => direction_; set => direction_ = value; }

        public override Point3D leftBottomPos => throw new NotImplementedException();

        public override Point3D rightTopPos => throw new NotImplementedException();

        public override Point3D leftTopPos => throw new NotImplementedException();

        public override Point3D rightBottomPos => throw new NotImplementedException();

        public override void Attach(IMission mission)
        {
            observers.Add(mission);
        }

        public override void Detach(IMission mission)
        {
            observers.Remove(mission);
        }
    }

    public class Conveyor_8 : Conveyor
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
        public Conveyor_8()
        {

        }
        public Conveyor_8(Direction direction)
        {
            this.name_ = "conveyor_8";
            this.direction_ = direction;
            this.speed_ = base.GetSpeed(name_);

        }
        public override bool isReady { get => isReady_; set => isReady_ = value; }


        public override double speed { get => speed_; }
        public override Point3D innerOffSet { get => innerOffSet_; set => innerOffSet_ = value; }

        public override Point3D backPos { get => backPos_; }

        public override Point3D frontPos { get => frontPos_; }

        public override Point3D leftPos { get => leftPos_; }
        public override Point3D rightPos { get => rightPos_; }
        public override string name { get => name_; }

        public override Direction direction { get => direction_; set => direction_ = value; }

        public override Point3D leftBottomPos => throw new NotImplementedException();

        public override Point3D rightTopPos => throw new NotImplementedException();

        public override Point3D leftTopPos => throw new NotImplementedException();

        public override Point3D rightBottomPos => throw new NotImplementedException();

        public override void Attach(IMission mission)
        {
            observers.Add(mission);
        }

        public override void Detach(IMission mission)
        {
            observers.Remove(mission);
        }
    }

    public class Conveyor_15 : Conveyor
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
        public Conveyor_15()
        {

        }
        public Conveyor_15(Direction direction)
        {
            this.name_ = "conveyor_15";
            this.direction_ = direction;
            this.speed_ = base.GetSpeed(name_);

        }
        public override bool isReady { get => isReady_; set => isReady_ = value; }


        public override double speed { get => speed_; }
        public override Point3D innerOffSet { get => innerOffSet_; set => innerOffSet_ = value; }

        public override Point3D backPos { get => backPos_; }

        public override Point3D frontPos { get => frontPos_; }

        public override Point3D leftPos { get => leftPos_; }
        public override Point3D rightPos { get => rightPos_; }

        public override string name { get => name_; }

        public override Direction direction { get => direction_; set => direction_ = value; }

        public override Point3D leftBottomPos => throw new NotImplementedException();

        public override Point3D rightTopPos => throw new NotImplementedException();

        public override Point3D leftTopPos => throw new NotImplementedException();

        public override Point3D rightBottomPos => throw new NotImplementedException();

        public override void Attach(IMission mission)
        {
            observers.Add(mission);
        }

        public override void Detach(IMission mission)
        {
            observers.Remove(mission);
        }
    }

    public class Conveyor_18 : Conveyor
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
        public Conveyor_18()
        {

        }
        public Conveyor_18(Direction direction)
        {
            this.name_ = "conveyor_18";
            this.direction_ = direction;
            this.speed_ = base.GetSpeed(name_);
            //this.width_ = base.GetWidth(name_);
            //this.length_ = base.GetLength(name_);
        }
        public override bool isReady { get => isReady_; set => isReady_ = value; }


        public override double speed { get => speed_; }
        public override Point3D innerOffSet { get => innerOffSet_; set => innerOffSet_ = value; }

        public override Point3D backPos { get => backPos_; }

        public override Point3D frontPos { get => frontPos_; }

        public override Point3D leftPos { get => leftPos_; }
        public override Point3D rightPos { get => rightPos_; }

        public override string name { get => name_; }

        public override Direction direction { get => direction_; set => direction_ = value; }

        public override Point3D leftBottomPos => throw new NotImplementedException();

        public override Point3D rightTopPos => throw new NotImplementedException();

        public override Point3D leftTopPos => throw new NotImplementedException();

        public override Point3D rightBottomPos => throw new NotImplementedException();

        public override void Attach(IMission mission)
        {
            observers.Add(mission);
        }

        public override void Detach(IMission mission)
        {
            observers.Remove(mission);
        }
    }



    public class DS_3 : Conveyor
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
        public DS_3()
        {

        }
        public DS_3(Direction direction)
        {
            this.name_ = "ds_3";
            this.direction_ = direction;
            this.speed_ = base.GetSpeed(name_);
            //this.width_ = base.GetWidth(name_);
            //this.length_ = base.GetLength(name_);
        }
        public override bool isReady { get => isReady_; set => isReady_ = value; }


        public override double speed { get => speed_; }
        public override Point3D innerOffSet { get => innerOffSet_; set => innerOffSet_ = value; }

        public override Point3D backPos { get => backPos_; }

        public override Point3D frontPos { get => frontPos_; }

        public override Point3D leftPos { get => leftPos_; }
        public override Point3D rightPos { get => rightPos_; }

        public override string name { get => name_; }

        public override Direction direction { get => direction_; set => direction_ = value; }

        public override Point3D leftBottomPos => throw new NotImplementedException();

        public override Point3D rightTopPos => throw new NotImplementedException();

        public override Point3D leftTopPos => throw new NotImplementedException();

        public override Point3D rightBottomPos => throw new NotImplementedException();

        public override void Attach(IMission mission)
        {
            observers.Add(mission);
        }

        public override void Detach(IMission mission)
        {
            observers.Remove(mission);
        }
    }
    public class DS_5 : Conveyor
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
        public DS_5()
        {

        }
        public DS_5(Direction direction)
        {
            this.name_ = "ds_5";
            this.direction_ = direction;
            this.speed_ = base.GetSpeed(name_);
            //this.width_ = base.GetWidth(name_);
            //this.length_ = base.GetLength(name_);
        }
        public override bool isReady { get => isReady_; set => isReady_ = value; }


        public override double speed { get => speed_; }
        public override Point3D innerOffSet { get => innerOffSet_; set => innerOffSet_ = value; }

        public override Point3D backPos { get => backPos_; }

        public override Point3D frontPos { get => frontPos_; }

        public override Point3D leftPos { get => leftPos_; }
        public override Point3D rightPos { get => rightPos_; }

        public override string name { get => name_; }

        public override Direction direction { get => direction_; set => direction_ = value; }

        public override Point3D leftBottomPos => throw new NotImplementedException();

        public override Point3D rightTopPos => throw new NotImplementedException();

        public override Point3D leftTopPos => throw new NotImplementedException();

        public override Point3D rightBottomPos => throw new NotImplementedException();

        public override void Attach(IMission mission)
        {
            observers.Add(mission);
        }

        public override void Detach(IMission mission)
        {
            observers.Remove(mission);
        }
    }

    public class Conveyor_17 : Conveyor
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
        public Conveyor_17()
        {

        }
        public Conveyor_17(Direction direction)
        {
            this.name_ = "conveyor_17";
            this.direction_ = direction;
            this.speed_ = base.GetSpeed(name_);
            //this.width_ = base.GetWidth(name_);
            //this.length_ = base.GetLength(name_);
        }
        public override bool isReady { get => isReady_; set => isReady_ = value; }


        public override double speed { get => speed_; }
        public override Point3D innerOffSet { get => innerOffSet_; set => innerOffSet_ = value; }

        public override Point3D backPos { get => backPos_; }

        public override Point3D frontPos { get => frontPos_; }

        public override Point3D leftPos { get => leftPos_; }
        public override Point3D rightPos { get => rightPos_; }

        public override string name { get => name_; }

        public override Direction direction { get => direction_; set => direction_ = value; }

        public override Point3D leftBottomPos => throw new NotImplementedException();

        public override Point3D rightTopPos => throw new NotImplementedException();

        public override Point3D leftTopPos => throw new NotImplementedException();

        public override Point3D rightBottomPos => throw new NotImplementedException();

        public override void Attach(IMission mission)
        {
            observers.Add(mission);
        }

        public override void Detach(IMission mission)
        {
            observers.Remove(mission);
        }
    }


    public class Stacker1_1 : Conveyor
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
        public Stacker1_1()
        {

        }
        public Stacker1_1(Direction direction)
        {
            this.name_ = "stacker1_1";
            this.direction_ = direction;
            this.speed_ = base.GetSpeed(name_);
            this.backPos_ = base.GetBackPos(name_);
            this.frontPos_ = base.GetFrontPos(name_);

        }
        public override bool isReady { get => isReady_; set => isReady_ = value; }


        public override double speed { get => speed_; }
        public override Point3D innerOffSet { get => innerOffSet_; set => innerOffSet_ = value; }

        public override Point3D backPos { get => backPos_; }

        public override Point3D frontPos { get => frontPos_; }

        public override Point3D leftPos { get => leftPos_; }
        public override Point3D rightPos { get => rightPos_; }

        public override string name { get => name_; }

        public override Direction direction { get => direction_; set => direction_ = value; }

        public override Point3D leftBottomPos => throw new NotImplementedException();

        public override Point3D rightTopPos => throw new NotImplementedException();

        public override Point3D leftTopPos => throw new NotImplementedException();

        public override Point3D rightBottomPos => throw new NotImplementedException();

        public override void Attach(IMission mission)
        {
            observers.Add(mission);
        }

        public override void Detach(IMission mission)
        {
            observers.Remove(mission);
        }
    }

    public class Stacker1_2 : Conveyor
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

        private Point3D leftBottomPos_;
        private Point3D rightTopPos_;
        public Stacker1_2()
        {

        }
        public Stacker1_2(Direction direction, string shelfNo,ModelPosition modelPosition)
        {
            this.name_ = "stacker1_2";
            this.direction_ = direction;
            this.speed_ = base.GetSpeed(name_);
            this.leftBottomPos_ = base.GetBottomLeftPos(name_);
            this.rightTopPos_ = modelPosition.GetShelfPos(shelfNo);
            this.rightTopPos_ = new Point3D(rightTopPos_.X, -3331.24, rightTopPos_.Z);
        }
        public override bool isReady { get => isReady_; set => isReady_ = value; }


        public override double speed { get => speed_; }
        public override Point3D innerOffSet { get => innerOffSet_; set => innerOffSet_ = value; }

        public override Point3D backPos { get => backPos_; }

        public override Point3D frontPos { get => frontPos_; }

        public override Point3D leftPos { get => leftPos_; }
        public override Point3D rightPos { get => rightPos_; }

        public override string name { get => name_; }

        public override Direction direction { get => direction_; set => direction_ = value; }

        public override Point3D leftBottomPos { get => leftBottomPos_; }

        public override Point3D rightTopPos { get => rightTopPos_; }

        public override Point3D leftTopPos => throw new NotImplementedException();

        public override Point3D rightBottomPos => throw new NotImplementedException();

        public override void Attach(IMission mission)
        {
            observers.Add(mission);
        }

        public override void Detach(IMission mission)
        {
            observers.Remove(mission);
        }
    }

    public class Stacker1_3 : Conveyor
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

      
        public Stacker1_3()
        {

        }
        public Stacker1_3(Direction direction, string shelfNo,ModelPosition modelPosition)
        {
            this.name_ = "stacker1_3";
            this.direction_ = direction;
            this.speed_ = base.GetSpeed(name_);
            this.backPos_ = modelPosition.GetShelfPos(shelfNo);
            if (direction == Direction.Back)
            {
                this.frontPos_ = new Point3D(backPos_.X, backPos_.Y + 1331.248, backPos_.Z);
            }
            else if (direction == Direction.Front)
            {
                this.frontPos_ = new Point3D(backPos_.X, backPos_.Y - 1331.248, backPos_.Z);
            }

            //this.width_ = base.GetWidth(name_);
            //this.length_ = base.GetLength(name_);
        }
        public override bool isReady { get => isReady_; set => isReady_ = value; }


        public override double speed { get => speed_; }
        public override Point3D innerOffSet { get => innerOffSet_; set => innerOffSet_ = value; }

        public override Point3D backPos { get => backPos_; }

        public override Point3D frontPos { get => frontPos_; }

        public override Point3D leftPos { get => leftPos_; }
        public override Point3D rightPos { get => rightPos_; }

      

        public override string name { get => name_; }

        public override Direction direction { get => direction_; set => direction_ = value; }

        public override Point3D leftBottomPos => throw new NotImplementedException();

        public override Point3D rightTopPos => throw new NotImplementedException();

        public override Point3D leftTopPos => throw new NotImplementedException();

        public override Point3D rightBottomPos => throw new NotImplementedException();

        public override void Attach(IMission mission)
        {
            observers.Add(mission);
        }

        public override void Detach(IMission mission)
        {
            observers.Remove(mission);
        }
    }
}
