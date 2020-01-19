using System.Windows;
using System.Windows.Input;

namespace Melodify
{
    /// <summary>
    /// Interaction logic for PlaylistMenu.xaml
    /// </summary>
    public partial class PlaylistMenu : Window
    {
        public PlaylistMenu()
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

        private void Gen_Click(object sender, RoutedEventArgs e)
        {
            GPlaylists gPlaylists = new GPlaylists();
            gPlaylists.Show();
            this.Close();
        }

        private void User_Click(object sender, RoutedEventArgs e)
        {
            Playlists playlists = new Playlists();
            playlists.Show();
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
