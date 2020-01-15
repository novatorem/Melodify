using System.Windows;
using System.Windows.Input;

namespace Melodify
{
    /// <summary>
    /// Interaction logic for AppInfo.xaml
    /// </summary>
    public partial class AppInfo : Window
    {
        public AppInfo()
        {
            InitializeComponent();
            WindowBlur.SetIsEnabled(this, true);
            MouseDown += Window_MouseDown;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
