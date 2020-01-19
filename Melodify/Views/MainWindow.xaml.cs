using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Melodify
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpotifyAPI spotAPI;
        bool _pauseAPI = false;
        SpotifyWebAPI _spotify;

        public MainWindow()
        {
            InitializeComponent();
            MouseDown += Window_MouseDown;
            Left = SystemParameters.WorkArea.Width - Width;
            Top = SystemParameters.WorkArea.Height - Height;

            // Initalize some instances to ensure continuing playback
            App.Current.Properties["playlistID"] = "";
            App.Current.Properties["suggestionMode"] = false;
            App.Current.Properties["userPause"] = false;

            spotAPI = new SpotifyAPI("b875781a51d540039acb8fd0aab33e11", "ddc0ef0527744d0d8024448f803de52d");
            // Sleep for two seconds while waiting for login to process
            // Needs to be fixed later as it will take more time for user to log in - implement null checker
            Thread.Sleep(2000);
            _spotify = new SpotifyWebAPI()
            {
                AccessToken = (string)Application.Current.Properties["AccessToken"],
                TokenType = (string)Application.Current.Properties["TokenType"]
            };

            // Timer to get the information
            System.Timers.Timer timer = new System.Timers.Timer(350);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            // Timer to refresh the access token
            System.Timers.Timer accesser = new System.Timers.Timer(3500000);
            accesser.Elapsed += Access_Elapsed;
            accesser.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_pauseAPI)
            {
                return;
            }
            try
            {
                PlaybackContext context = _spotify.GetPlayingTrack();

                Title.Dispatcher.Invoke(() =>
                {
                    if (context.Error == null)
                    {
                        if (context.Item.Error == null)
                        {
                            Title.Content = context.Item.Name;
                            Author.Content = context.Item.Artists[0].Name;

                            BitmapImage albumArt = new BitmapImage();
                            albumArt.BeginInit();
                            albumArt.UriSource = new Uri(context.Item.Album.Images[0].Url);
                            albumArt.EndInit();
                            if ((albumArt.UriSource.AbsoluteUri != cover.Source.ToString()) && (albumArt.UriSource.AbsoluteUri != null)) {
                                cover.Source = albumArt;
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Inside- " + context.Error.Message);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Outside- " + context.Error.Message);
                    }
                });

                if ((bool)App.Current.Properties["suggestionMode"] == true && (bool)App.Current.Properties["userPause"] == false && !context.IsPlaying)
                {
                    Spotify.ResumePlayback();
                }
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine("Error main timer- " + err.Message);
            }
        }

        private void Access_Elapsed(object sender, ElapsedEventArgs e)
        {
            spotAPI.authenticate();
            _spotify = new SpotifyWebAPI()
            {
                AccessToken = (string)Application.Current.Properties["AccessToken"],
                TokenType = (string)Application.Current.Properties["TokenType"]
            };
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Other_Click(object sender, RoutedEventArgs e)
        {
            Spotify.CurrentTrackSuggestion();
        }

        private void Full_Click(object sender, RoutedEventArgs e)
        {
            FullNow(true);
            FullScreen fullScreen = new FullScreen(this);
            fullScreen.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            fullScreen.Show();
            fullScreen.WindowState = WindowState.Maximized;
        }

        public void FullNow(bool pauseAPI)
        {
            _pauseAPI = pauseAPI;
        }

        private void Self_Click(object sender, RoutedEventArgs e)
        {
            Spotify.UserTrackSuggestion();
        }

        private void Love_Click(object sender, RoutedEventArgs e)
        {
            Spotify.LoveSong();
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            mainMenu.Show();
        }

        private void PausePlay_Click(object sender, RoutedEventArgs e)
        {
            Spotify.PausePlaySong();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Spotify.NextSong();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            Spotify.PreviousSong();
        }

        private void Playlist_Click(object sender, RoutedEventArgs e)
        {
            Playlists playlists = new Playlists();
            playlists.Show();
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            fullClick.Visibility = Visibility.Collapsed;
            selfClick.Visibility = Visibility.Collapsed;
            loveClick.Visibility = Visibility.Collapsed;
            infoClick.Visibility = Visibility.Collapsed;
        }

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            fullClick.Visibility = Visibility.Visible;
            selfClick.Visibility = Visibility.Visible;
            loveClick.Visibility = Visibility.Visible;
            infoClick.Visibility = Visibility.Visible;
        }

        private void CMExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CMGithub_Click(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd",
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = $"/c start https://github.com/novatorem/Melodify"
            };
            Process.Start(psi);
        }

        private void CMInfo_Click(object sender, RoutedEventArgs e)
        {
            AppInfo appInfo = new AppInfo();
            appInfo.Show();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                Spotify.PreviousSong();
            } else if (e.Key == Key.Right)
            {
                Spotify.NextSong();
            } else if (e.Key == Key.Space ^ e.Key == Key.Enter)
            {
                Spotify.PausePlaySong();
            } else if (e.Key == Key.F)
            {
                FullNow(true);
                FullScreen fullScreen = new FullScreen(this);
                fullScreen.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                fullScreen.Show();
                fullScreen.WindowState = WindowState.Maximized;
            }
            e.Handled = true;
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FullNow(true);
            FullScreen fullScreen = new FullScreen(this);
            fullScreen.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            fullScreen.Show();
            fullScreen.WindowState = WindowState.Maximized;
            e.Handled = true;
        }
    }
}
