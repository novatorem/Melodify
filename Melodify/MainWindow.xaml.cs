using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Melodify
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MouseDown += Window_MouseDown;
            SpotifyAPI spotAPI = new SpotifyAPI("b875781a51d540039acb8fd0aab33e11", "ddc0ef0527744d0d8024448f803de52d");
            // Sleep for two seconds while waiting for login to process
            // Needs to be fixed later as it will take more time for user to log in - implement null checker
            Thread.Sleep(2000);
            System.Timers.Timer timer = new System.Timers.Timer(100);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = (string)App.Current.Properties["AccessToken"],
                    TokenType = (string)App.Current.Properties["TokenType"]
                };
                PlaybackContext context = _spotify.GetPlayingTrack();

                Title.Dispatcher.Invoke(() =>
                {
                    //Title.Content = context.Item.Name;
                    Title.Content = context.Item.Name;
                    Author.Content = context.Item.Artists[0].Name;

                    BitmapImage albumArt = new BitmapImage();
                    albumArt.BeginInit();
                    albumArt.UriSource = new Uri(context.Item.Album.Images[0].Url);
                    albumArt.EndInit();
                    cover.Source = albumArt;
                
                });
            }
            catch
            {
                Title.Content = "song name...";
                Author.Content = "artist name...";
            }
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

        private void Self_Click(object sender, RoutedEventArgs e)
        {
            Spotify.UserTrackSuggestion();
        }
    }
}
