using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.PlatformModel.DataItemSet
{
    
    public class CommandIdentifyItem : ObservableObject
    {
        private int _ID;

        /// <summary>
        /// 命令
        /// </summary>
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
                RaisePropertyChanged("ID");
            }
        }


        private string _comment;

        /// <summary>
        /// 说明内容
        /// </summary>
        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
                RaisePropertyChanged("Comment");
            }
        }
    }
}
