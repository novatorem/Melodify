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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Melodify
{
    /// <summary>
    /// Interaction logic for FullScreen.xaml
    /// </summary>
    public partial class FullScreen : Window
    {
        int progress = 0;
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

            // Checks user settings regarding progress bar
            if (Properties.Settings.Default.ProgressBar != "true")
            {
                Progressbar.IsChecked = false;
                progressGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                Progressbar.IsChecked = true;
                progressGrid.Visibility = Visibility.Visible;
            }
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
                        if ((context.Item.Album.Images[0].Url != cover.Source.ToString()) && (context.Item.Album.Images[0].Url != null))
                        {
                            BitmapImage albumArt = new BitmapImage();
                            albumArt.BeginInit();
                            albumArt.UriSource = new Uri(context.Item.Album.Images[0].Url);
                            albumArt.EndInit();
                            cover.Source = albumArt;
                            userCover.Source = albumArt;
                            userCover.SetValue(WidthProperty, DependencyProperty.UnsetValue);
                            userCover.SetValue(HeightProperty, DependencyProperty.UnsetValue);
                            // Gets the total length of the song
                            progress = context.Item.DurationMs;
                        }
                        DoubleAnimation animation = new DoubleAnimation();
                        animation.From = progressBar.ActualWidth;
                        animation.To = ((double)(context.ProgressMs) / (double)(progress)) * this.Width;
                        animation.Duration = new Duration(TimeSpan.FromSeconds(1));
                        animation.FillBehavior = FillBehavior.Stop;
                        progressBar.BeginAnimation(Rectangle.WidthProperty, animation);
                        progressBar.Width = ((double)(context.ProgressMs) / (double)(progress)) * this.Width;
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

        private void YoutubeGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            youtubeClick.Visibility = Visibility.Collapsed;
        }

        private void YoutubeGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            youtubeClick.Visibility = Visibility.Visible;
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
        private void Youtube_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            this.Close();
            MusicVideos musicVideos = new MusicVideos(_window);
            musicVideos.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            musicVideos.WindowState = WindowState.Maximized;
            musicVideos.Show();
        }

        private void Seek_Playback(object sender, RoutedEventArgs e)
        {
            int position = (int)((Mouse.GetPosition(this).X / this.Width) * 100);
            Spotify.SeekPlayback(position);
            e.Handled = true;
        }

        private void Progressbar_Click(object sender, RoutedEventArgs e)
        {
            if (progressGrid.Visibility == Visibility.Visible)
            {
                Progressbar.IsChecked = false;
                progressGrid.Visibility = Visibility.Collapsed;
                Properties.Settings.Default.ProgressBar = "false";
            }
            else if (progressGrid.Visibility == Visibility.Collapsed)
            {
                Progressbar.IsChecked = true;
                progressGrid.Visibility = Visibility.Visible;
                Properties.Settings.Default.ProgressBar = "true";
            }
            Properties.Settings.Default.Save();
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
