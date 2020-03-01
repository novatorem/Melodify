using System.Threading.Tasks;
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

        private async void Gen_Click(object sender, RoutedEventArgs e)
        {
            followedText.Text = "♨\n";
            // Await to make UI update before moving on to window
            await Task.Delay(25).ConfigureAwait(true);
            GPlaylists gPlaylists = new GPlaylists();
            gPlaylists.Show();
            this.Close();
        }

        private async void User_Click(object sender, RoutedEventArgs e)
        {
            personalText.Text = "♨\n";
            // Await to make UI update before moving on to window
            await Task.Delay(25).ConfigureAwait(true);
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
