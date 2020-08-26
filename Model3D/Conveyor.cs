using System;
using System.Collections;
using System.Windows.Media.Media3D;

namespace CS3D
{
    public enum Direction//运动方向
    {
        Horizontal = 1,//横向移动  左右
        Vertical,//纵向移动  前后
        Oblique,//斜（斜 左 右 上 下）
        PickUpPort,//取货 放货
        Ex_Import//放货
    }
    public class Conveyor
    {
        private bool isReady_;

        private string name_;
        private double speed_x;
        private double speed_y;
        private double speed_z;


        private Point3D startPos_;
        private Point3D endPos_;
        private Direction direction_;

        public Conveyor(string name,string missionType,string shelfNo,Direction direction,ModelPosition modelPosition)
        {
            this.name_ = name;
            this.direction_ = direction;
            this.speed_x = GetSpeed_X(name_);
            this.speed_y = GetSpeed_Y(name_);
            this.speed_z = GetSpeed_Z(name_);
            if (missionType.Equals("StockIn"))
            {

                switch (direction)
                {
                    case Direction.Horizontal://输送带 横向
                        this.startPos_ = GetLeftPos(name);
                        this.endPos_ = GetRightPos(name);
                        break;
                    case Direction.Vertical://输送带 纵向
                        this.startPos_ = GetBackPos(name);
                        this.endPos_ = GetFrontPos(name);
                        break;
                    case Direction.Oblique://堆垛机 斜向
                        this.startPos_ = GetLeftBottomPos(name);
                        this.endPos_ = modelPosition.GetShelfLinePos(shelfNo);
                        break;
                    case Direction.PickUpPort://取放货口 
                        this.startPos_ = GetBackPos(name);
                        this.endPos_ = GetFrontPos(name);
                        break;
                    case Direction.Ex_Import://进出货位
                        this.startPos_ = modelPosition.GetShelfLinePos(shelfNo);
                        this.endPos_ = modelPosition.GetShelfPos(shelfNo);
                        break;
                }

            }else if (missionType.Equals("StockOut"))
            {
                switch (direction)
                {
                    case Direction.Horizontal:
                        this.startPos_ = GetRightPos(Name);
                        this.endPos_ = GetLeftPos(Name);
                        break;
                    case Direction.Vertical:
                        this.startPos_ = GetFrontPos(Name);
                        this.endPos_ = GetBackPos(Name);
                        break;
                    case Direction.Oblique:
                        this.endPos_ = GetLeftBottomPos(this.Name);//终点 
                        this.startPos_ = modelPosition.GetShelfLinePos(shelfNo);//巷道中的位置
                        break;
                    case Direction.PickUpPort:
                        this.startPos_ = GetBackPos(name);
                        this.endPos_ = GetFrontPos(name);
                        break;
                    case Direction.Ex_Import:
                        this.startPos_ = modelPosition.GetShelfPos(shelfNo);
                        this.endPos_ = modelPosition.GetShelfLinePos(shelfNo);
                        break;
                }
               
            }

        }
        public bool IsReady { get => isReady_; set => isReady_ = value; }

        public double Speed_X { get => speed_x; }
        public double Speed_Y { get => speed_y; }
        public double Speed_Z { get => speed_z; }

        public string Name { get => name_; }

        public Point3D StartPos { get => startPos_; set => startPos_ = value; }

        public Point3D EndPos { get => endPos_; set => endPos_ = value; }
        public Direction Direction { get => direction_; set => direction_ = value; }

        protected ArrayList observers = new ArrayList();

        public void Attach(IMission mission)
        {
            observers.Add(mission);
        }

        public void Detach(IMission mission)
        {
            observers.Remove(mission);
        }

        public virtual double GetSpeed_X(string conveyorName)
        {
            return Convert.ToDouble(XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/speed_x"));
        }
        public virtual double GetSpeed_Y(string conveyorName)
        {
            return Convert.ToDouble(XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/speed_y"));
        }
        public virtual double GetSpeed_Z(string conveyorName)
        {
            return Convert.ToDouble(XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + conveyorName + "/speed_z"));
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

}
