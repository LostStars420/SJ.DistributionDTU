using System;
using System.Windows;
using System.Windows.Controls;

namespace ChoicePhase.ControlPlatform.View
{
    /// <summary>
    /// Description for OptionConfigUI.
    /// </summary>
    public partial class OptionConfigUI : Window
    {
        /// <summary>
        /// Initializes a new instance of the OptionConfigUI class.
        /// </summary>
        public OptionConfigUI()
        {
            InitializeComponent();
           
        }

        private void passWordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
          if (sender is PasswordBox)
            {
                var pass = ((PasswordBox)sender);
                var name = pass.Name;
                var word = pass.Password;
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<Tuple<string,string>>(new Tuple<string, string>(name, word), "OptionViewPassword");
            }
        }
    }
}