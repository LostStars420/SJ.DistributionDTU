/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:ChoicePhase.ControlPlatform.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;


namespace ChoicePhase.ControlPlatform.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<CommunicationViewModel>();
            SimpleIoc.Default.Register<MonitorViewModel >();
            SimpleIoc.Default.Register<OptionConfigUIViewModel>();
            SimpleIoc.Default.Register<SwitchListViewModel>();
            SimpleIoc.Default.Register<FaultTestViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public static MainViewModel main;
        public static MainViewModel Main
        {
            get
            {
                if (main == null)
                {
                    main = ServiceLocator.Current.GetInstance<MainViewModel>();
                }
                return main;

            }
        }
        public static CommunicationViewModel communication;
        public static CommunicationViewModel Communication
        {
            get
            {
                if (communication == null)
                {
                    communication = ServiceLocator.Current.GetInstance<CommunicationViewModel>();
                }
                return communication;
            }
        }
       
        public static MonitorViewModel monitor;
        public static MonitorViewModel Monitor
        {
            get
            {
                if (monitor == null)
                {
                    monitor = ServiceLocator.Current.GetInstance<MonitorViewModel>();
                }
                return monitor;
            }
        }

        public static OptionConfigUIViewModel optionConfig;

        public static OptionConfigUIViewModel OptionConfig
        {
            get
            {
                if (optionConfig == null)
                {
                    optionConfig = ServiceLocator.Current.GetInstance<OptionConfigUIViewModel>();
                }
                return optionConfig;
            }
        }

        public static SwitchListViewModel switchList;

        public static SwitchListViewModel SwitchList
        {
            get
            {
                if (switchList == null)
                {
                    switchList = ServiceLocator.Current.GetInstance<SwitchListViewModel>();
                }
                return switchList;
            }
        }

        public static FaultTestViewModel faultTest;

        public static FaultTestViewModel FaultTest
        {
            get
            {
                if (faultTest == null)
                {
                    faultTest = ServiceLocator.Current.GetInstance<FaultTestViewModel>();
                }
                return faultTest;
            }
        }


        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}