using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //SpotifyAPI spotAPI = new SpotifyAPI("b875781a51d540039acb8fd0aab33e11", "ddc0ef0527744d0d8024448f803de52d");
            var _spotify = new SpotifyWebAPI()
            {
                AccessToken = (string)App.Current.Properties["AccessToken"],
                TokenType = (string)App.Current.Properties["TokenType"]
            };
            PlaybackContext context = _spotify.GetPlayingTrack();
            System.Diagnostics.Debug.WriteLine(context.Item.Name);
            song.Content = context.Item.Name;
            artist.Text = context.Item.Artists[0].Name;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var _spotify = new SpotifyWebAPI()
            {
                AccessToken = (string)App.Current.Properties["AccessToken"],
                TokenType = (string)App.Current.Properties["TokenType"]
            };
            FullTrack track = _spotify.GetTrack("3Hvu1pq89D4R0lyPBoujSv");
            System.Diagnostics.Debug.WriteLine(track.Name);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
