using SpotifyAPI;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Melodify
{
    /// <summary>
    /// Interaction logic for FullScreen.xaml
    /// </summary>
    public partial class FullScreen : Window
    {
        System.Timers.Timer timer;
        private MainWindow _window;
        SpotifyWebAPI _spotify = new SpotifyWebAPI()

        {
            AccessToken = (string)App.Current.Properties["AccessToken"],
            TokenType = (string)App.Current.Properties["TokenType"]
        };
        

        public FullScreen(MainWindow window)
        {
            InitializeComponent();
            MouseDown += Window_MouseDown;
            timer = new System.Timers.Timer(350);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            _window = window;
            _window.FullNow(true);
            _window.Visibility = Visibility.Collapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
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
                        if ((albumArt.UriSource.AbsoluteUri != cover.Source.ToString()) && (albumArt.UriSource.AbsoluteUri != null))
                        {
                            cover.Source = albumArt;
                            userCover.Source = albumArt;
                            userCover.SetValue(HeightProperty, DependencyProperty.UnsetValue);
                            userCover.SetValue(WidthProperty, DependencyProperty.UnsetValue);
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
        }

        private void ExitGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            exitClick.Visibility = Visibility.Collapsed;
        }

        private void ExitGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            exitClick.Visibility = Visibility.Visible;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Cover_Click(object sender, MouseButtonEventArgs e)
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

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            this.Close();
            _window.FullNow(false);
            _window.Visibility = Visibility.Visible;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                Spotify.PreviousSong();
            }
            else if (e.Key == Key.Right)
            {
                Spotify.NextSong();
            }
            else if (e.Key == Key.Space ^ e.Key == Key.Enter)
            {
                Spotify.PausePlaySong();
            } else if (e.Key == Key.Escape)
            {
                timer.Stop();
                this.Close();
                _window.FullNow(false);
                _window.Visibility = Visibility.Visible;
            }
            e.Handled = true;
        }
    }
}
