using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.PlatformModel.DataItemSet
{
    /// <summary>
    /// 动作相角设定
    /// </summary>
    public class ActionPhase
    {
        /// <summary>
        /// 动作索引，代表第第几次动作
        /// </summary>
        public byte Index
        {
            get;
            set;
        }

        /// <summary>
        /// 动作相，1-A，2-B，3-C
        /// </summary>
        public byte Phase
        {
            get;
            private set;
        }

        /// <summary>
        /// 合闸时间
        /// </summary>
        public float CloseTime
        {
            get;
            set;
        }

        private double _angle;
        /// <summary>
        /// 动作角度 [0, 360)
        /// </summary>
        public double Angle
        {
            get
            {
                return _angle;
            }
            set
            {
               _angle = value % 360;
               ActionTime = _angle / 360 * Period;
               StartTime = ActionTime - CloseTime;
            }
        }


        /// <summary>
        /// 是否使能
        /// </summary>
        public bool Enable
        {
            set;
            get;
        }

        public float Period
        {
            private set;
            get;
        }

        /// <summary>
        /// 动作时间
        /// </summary>
        public double ActionTime
        {
            get;
            private set;
        }


        /// <summary>
        /// 启动时间
        /// </summary>
        public double StartTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 初始化动作相角属性
        /// </summary>       
        /// <param name="phase">相</param>
        /// <param name="closeTime">合闸时间</param>
        /// <param name="angle">动作角度[0, 360)</param>
        public ActionPhase(byte phase, float closeTime,  bool enable, float period)
        {
            Index = 0;
            Phase = phase;
            CloseTime = closeTime;
            Angle = 0;
            Enable = enable;
            Period = period;
        }

    }
}
