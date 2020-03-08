using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Melodify
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public MainMenu()
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

        private async void Songs_Click(object sender, RoutedEventArgs e)
        {
            songsText.Text = "\n♬\n";
            // Await to make UI update before moving on to window
            await Task.Delay(25).ConfigureAwait(true);
            TopSongs topSongs = new TopSongs();
            topSongs.Show();
            this.Close();
        }

        private async void User_Click(object sender, RoutedEventArgs e)
        {
            userText.Text = "\n♨\n";
            // Await to make UI update before moving on to window
            await Task.Delay(25).ConfigureAwait(true);
            UserInfo userInfo = new UserInfo();
            userInfo.Show();
            this.Close();
        }

        private async void Artists_ClickAsync(object sender, RoutedEventArgs e)
        {
            artistText.Text = "\n♬\n";
            // Await to make UI update before moving on to window
            await Task.Delay(25).ConfigureAwait(true);
            TopArtists topArtists = new TopArtists();
            topArtists.Show();
            Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
