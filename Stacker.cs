using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace CS3D
{
    public abstract class Stacker
    {
        public abstract string name { get; }
        public abstract bool isReady { get; set; }//是否被占用

        public abstract Direction direction_UpperFork { get; set; }//上叉运动方向（前、后、左、右）
        public abstract Direction direction_Platform { get; set; }//载货台运动方向
        public abstract Direction direction_Stacker { get; set; }//堆垛机运动方向

        /// <summary>
        /// 内部偏移量
        /// </summary>
        public abstract double innerOffSet_UpperFork { get; set; }
        public abstract double innerOffSet_Platform { get; set; }
        public abstract double innerOffSet_Stacker { get; set; }

        /// <summary>
        /// 速度
        /// </summary>
        public abstract double speed_UpperFork { get; }
        public abstract double speed_Platform { get; }
        public abstract double speed_Stacker { get; }


        /// <summary>
        /// 起点的 某个方向的绝对坐标
        /// </summary>
        public abstract double startPos_UpperFork { get; }
        public abstract double startPos_Platform { get; }
        public abstract double startPos_Stacker { get; }
        /// <summary>
        /// 终点绝对位置
        /// </summary>
        public abstract double endPos_UpperFork { get; }
        public abstract double endPos_Platform { get; }
        public abstract double endPos_Stacker { get; }

        protected ArrayList observers = new ArrayList();//

        public abstract void Attach(IMission mission);//向集合中添加

        public abstract void Detach(IMission observer);//删除一个对象

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stackerName">stacker_1 stacker_2</param>
        /// <param name="partName">speed_upperfork speed_platform speed_stacker</param>
        /// <returns></returns>
        public virtual double GetSpeed(string stackerName, string partName)
        {
            return Convert.ToDouble(XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + stackerName + "/" + partName).Trim());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stackerName"></param>
        /// <param name="partName">startPos_Platform  startPos_Stacker endPos_UpperFork startPos_UpperFork </param>
        /// <returns></returns>
        public virtual double GetPos(string stackerName,string partName)
        {
            return Convert.ToDouble(XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + stackerName + "/" + partName).Trim());
        }


        public  double GetEndPos(string stackerName,string partName)
        {
            double endPos = 0;

            return endPos;
        }

        public void Notify()//通知方法
        {
            foreach (var obs in observers)
            {
                ((IMission)obs).Update(name, isReady);
            }
        }
    }

    //public class UpperFork_1 : Stacker
    //{
    //    string name_;
    //    bool isReady_;
    //    Direction direction_;
    //    Point3D innerOffSet_;
    //    double speed_;
    //    double startPos_;
    //    double endPos_;

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="partName">speed_upperfork/speed_platform speed_stacker </param>
    //    public UpperFork_1()
    //    {
    //        this.name_ = "stacker_1";
    //        this.speed_ = base.GetSpeed(name_, "speed_upperfork");

    //    }

    //    public override string name => name_;

    //    public override bool isReady { get => isReady_; set => isReady_ = value; }
    //    public override Direction direction { get => direction_; set => direction_ = value; }
    //    public override Point3D innerOffSet { get => innerOffSet_; set => innerOffSet_ = value; }

    //    public override double speed { get => speed_; }

    //    public override double startPos => throw new NotImplementedException();

    //    public override double endPos => throw new NotImplementedException();

    //    public override void Attach(IMission mission)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void Detach(IMission observer)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override double GetEndPos()
    //    {
    //        return Convert.ToDouble(XmlHelper.Instance.GetXMLInformation("/Config/Model3D/" + name_ + "/"));
    //    }

    //    public override double GetStartPos()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
