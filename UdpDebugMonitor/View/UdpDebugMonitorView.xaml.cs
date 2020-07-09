
﻿using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using UdpDebugMonitor.Model;
using UdpDebugMonitor.ViewModel;

namespace UdpDebugMonitor.View
{
    /// <summary>
    /// UdpDebugMonitorView.xaml 的交互逻辑
    /// </summary>
    public partial class UdpDebugMonitorView : Window
    {
        //private UdpDebugMonitorViewModel udp;
        public UdpDebugMonitorView()
        {
            //udp = new UdpDebugMonitorViewModel();
            InitializeComponent();
            Messenger.Default.Register<Dispatcher>(this, "GetDispatcher", MakeDispatcher);

            this.DataContext = new UdpDebugMonitorViewModel();

            //卸载当前(this)对象注册的所有MVVMLight消息
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
            this.Closing += Close ;
            
        }


        public  void Close(object o, System.ComponentModel.CancelEventArgs e)
        {
            PlatformModelServer.GetServer().Close();
            Environment.Exit(0);
        }


        private void MakeDispatcher(Dispatcher obj)
        {
            Messenger.Default.Send<Dispatcher>(this.Dispatcher, "Dispatcher");
        }


        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (sender is TextBox)
                {
                    TextBox textBox = (TextBox)sender;
                    ((TextBox)sender).ScrollToEnd();
                    textBox.SelectionStart = textBox.Text.Length;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } 
        }

        private void gridProtectSetPoint_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (gridProtectSetPoint.SelectedItem!=null)
            {
                var selectedIP = (gridProtectSetPoint.SelectedItem as ReciveIp).IP.ToString();
                var selectedPort = (gridProtectSetPoint.SelectedItem as ReciveIp).Port;
                Messenger.Default.Send<Tuple<string, int>>(new Tuple<string, int>(selectedIP, selectedPort), "UpdateIPPort");
            }
        }
    }
}


