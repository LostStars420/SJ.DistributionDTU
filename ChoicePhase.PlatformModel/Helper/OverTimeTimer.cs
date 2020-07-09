using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ChoicePhase.PlatformModel.Helper
{
    /// <summary>
    /// 超时定时器
    /// </summary>
    /// 
    public class OverTimeTimer
    {

        private int id = 0;
        private Timer timer;
        private Action delegateAction;

        static int ID= 0;

        private int overTime;
        /// <summary>
        /// 超时处理
        /// </summary>
        /// <param name="overTime">超时时间ms</param>
        /// <param name="action">超时后的动作</param>
        public OverTimeTimer(int overT, Action action)
        {
            this.overTime = overT;
            delegateAction = action;
            timer = new Timer(overTime);
            ID++;
            id = ID;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            timer.Stop();
            timer.Close();
            delegateAction();
          // Console.WriteLine("Stop:" + id.ToString() + ":" + DateTime.Now.ToString("HH:MM:fff"));

        }
        /// <summary>
        /// 复位定时器重新开始计时
        /// </summary>
        public void ReStartTimer()
        {
            //先停止以前定时器
            if (timer != null)
            {
                timer.Enabled = false;
                timer.Elapsed -= timer_Elapsed;
                timer.Stop();
                timer.Close();
                timer.Dispose();
                
            }

            timer = new Timer(overTime);
            timer.AutoReset = false;
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = true;
            timer.Start();
           //Console.WriteLine("Start:" + id.ToString() + ":" + DateTime.Now.ToString("HH:MM:fff"));
        }

        /// <summary>
        /// 停止定时器
        /// </summary>
        public void StopTimer()
        {
            if (timer.Enabled)
            {
                timer.Enabled = false;
                timer.Elapsed -= timer_Elapsed;
                timer.Stop();
                timer.Close();
                timer.Dispose();
            }
        }

    }
}
