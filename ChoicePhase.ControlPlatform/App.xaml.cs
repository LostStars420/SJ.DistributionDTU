using System.Windows;
using GalaSoft.MvvmLight.Threading;
using System;

namespace ChoicePhase.ControlPlatform
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        //protected override void OnStartup(StartupEventArgs e)
        //{
        //     SplashScreen s = new SplashScreen("pictures/sojo-c.jpg");
        //     s.Show(false);
        //     s.Close(new TimeSpan(0, 0, 5));           


        //    base.OnStartup(e);
        //}

    }
}
