using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using SpotifyAPI;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Melodify
{
    /// <summary>
    /// Interaction logic for MusicVideos.xaml
    /// </summary>
    public partial class MusicVideos : Window
    {
        int _volume = 0;
        System.Timers.Timer timer;
        private MainWindow _window;
        String _currentSong = "None";

        SpotifyWebAPI _spotify = new SpotifyWebAPI()
        {
            AccessToken = (string)App.Current.Properties["AccessToken"],
            TokenType = (string)App.Current.Properties["TokenType"]
        };

        YouTubeService youtube = new YouTubeService(new BaseClientService.Initializer()
        {
            ApplicationName = "Melodify",
            ApiKey = "AIzaSyDMpkq9kWkITxDpT96m2c5K4a6ey4oQiGM",
        });

        public MusicVideos(MainWindow window)
        {
            InitializeComponent();
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
            webView.Dispatcher.Invoke(() =>
            {
                if (context.Error == null)
                {
                    if (context.Item.Error == null)
                    {
                        String songName = context.Item.Name;
                        String artistName = context.Item.Artists[0].Name;
                        if (_currentSong != songName)
                        {
                            _currentSong = songName;
                            SearchResource.ListRequest listRequest = youtube.Search.List("snippet");
                            listRequest.MaxResults = 1;
                            listRequest.Type = "video";
                            listRequest.Q = songName + " - " + artistName;
                            listRequest.VideoEmbeddable = SearchResource.ListRequest.VideoEmbeddableEnum.True__;
                            listRequest.VideoSyndicated = SearchResource.ListRequest.VideoSyndicatedEnum.True__;
                            SearchListResponse resp = listRequest.Execute();

                            string videoID = resp.Items[0].Id.VideoId;
                            string startTime = (context.ProgressMs / 1000).ToString();
                            if (Spotify.GetSetVolume() != 0)
                            {
                                _volume = Spotify.GetSetVolume();
                            }
                            Spotify.GetSetVolume(0);

                            YoutubePlayer(videoID, startTime);
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
            else if (e.Key == Key.Escape)
            {
                timer.Stop();
                this.webView.Navigate((Uri)null);
                this.Close();
                Spotify.GetSetVolume(_volume);
                _window.FullNow(false);
                _window.Visibility = Visibility.Visible;
            }
            e.Handled = true;
        }

        private void YoutubePlayer(string videoCode, string startTime)
        {
            webView.Height = 0;
            webView.Navigate(new Uri("https://novac.dev/x/Melodify?videoCode=" + videoCode + "&startTime=" + startTime));
        }

        private void webView_NavigationCompleted(object sender, Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.WebViewControlNavigationCompletedEventArgs e)
        {
            webView.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            webView.Focus();
        }

        private void WebView_AcceleratorKeyPressed(object sender, Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.WebViewControlAcceleratorKeyPressedEventArgs e)
        {
            if ((e.VirtualKey == Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.VirtualKey.F4 && Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
               ^ e.VirtualKey == Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.VirtualKey.Escape)
            {
                timer.Stop();
                webView.Navigate((Uri)null);
                this.Close();
                Spotify.GetSetVolume(_volume);
                _window.FullNow(false);
                _window.Visibility = Visibility.Visible;
            }
        }
    }
}
