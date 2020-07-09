using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ChoicePhase.ControlPlatform.ViewModel;
using ChoicePhase.PlatformModel;

namespace ChoicePhase.ControlPlatform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
 
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();

            //注册MVVMLight消息
            //  Messenger.Default.Register<string>(this, "ShowUserView", ShowUserView);
            
            Messenger.Default.Register<Exception>(this, "ExceptionMessage", ExceptionMessage);
            Messenger.Default.Register<string>(this, "MessengerSrcollToEnd", SrcollToEnd);
            Messenger.Default.Register<Dispatcher>(this, "GetDispatcher", MakeDispatcher);
            
        }

        private void MakeDispatcher(Dispatcher obj)
        {
            Messenger.Default.Send<Dispatcher>(this.Dispatcher, "Dispatcher");
        }

        public new void Close()
        {
            PlatformModelServer.GetServer().Close();
            base.Close();
        }
        /// <summary>
        /// 异常信息
        /// </summary>
        /// <param name="obj"></param>
        private void ExceptionMessage(Exception obj)
        {
            if (obj is NullReferenceException)
            {
                MessageBox.Show("请打开网口");
                return;
            }

            if (obj is ObjectDisposedException)
            {
                MessageBox.Show("连接已关闭，请重新打开连接");
                return;
            }

            if (obj is ArgumentOutOfRangeException)
            {
                MessageBox.Show("数据越界", "warn");
                return;
            }

            MessageBox.Show(obj.Message, obj.Source);
        }

        /// <summary>
        /// 将文本框下拉到最后
        /// </summary>
        /// <param name="obj"></param>
        private void SrcollToEnd(string obj)
        {

            Action<string> toend = ar =>
            {
                switch (ar)
                {
                    case "txtLinkMessage":
                        {
                            txtLinkMessage.ScrollToEnd();
                            break;
                        }
                    case "txtRawSendMessage":
                        {
                            txtRawSendMessage.ScrollToEnd();
                            break;
                        }
                    case "txtRawReciveMessage":
                        {
                            txtRawReciveMessage.ScrollToEnd();
                            break;
                        }
                    case "txtExceptionMessage":
                        {
                            txtExceptionMessage.ScrollToEnd();
                            break;
                        }
                    case "txtStatusMessage":
                        {
                            txtStatusMessage.ScrollToEnd();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            };
           // Dispatcher.Invoke(toend, obj);
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if( sender is TextBox)
            {
                ((TextBox)sender).ScrollToEnd();
            }
        }
      
    }
}