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
using System.Windows.Shapes;

namespace Melodify
{
    /// <summary>
    /// Interaction logic for MusicVideos.xaml
    /// </summary>
    public partial class MusicVideos : Window
    {
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
            webBrowser1.Dispatcher.Invoke(() =>
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
                            listRequest.Q = songName + "-" + artistName;
                            listRequest.MaxResults = 1;
                            listRequest.Type = "video";
                            listRequest.Order = Google.Apis.YouTube.v3.SearchResource.ListRequest.OrderEnum.ViewCount;
                            SearchListResponse resp = listRequest.Execute();

                            string videoID = resp.Items[0].Id.VideoId;
                            string startTime = (context.ProgressMs / 1000).ToString();

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

        private void YoutubePlayer(string videoCode, string startTime)
        {
            var sb = new StringBuilder();

            sb.Append("<html>");
            sb.Append("    <head>");
            sb.Append("        <meta http-equiv=\"X-UA-Compatible\" content=\"IE=Edge\"/>");
            sb.Append("        <style type=\"text/css\">body, html{margin: 0; padding: 0; height: 100%; overflow: hidden;}#content{position: absolute; left: 0; right: 0; bottom: 0; top: 0px;}</style> ");
            sb.Append("    </head>");
            sb.Append("    <body>");
            sb.Append("        <div id=\"content\">");
            sb.Append("            <iframe width=\"100%\" height=\"100%\" src=\"https://www.youtube.com/embed/" + videoCode + "?autoplay=1&start=" + startTime + "\"");
            sb.Append("                </iframe>");
            sb.Append("        </div>");
            sb.Append("    </body>");
            sb.Append("</html>");

            this.webBrowser1.NavigateToString(sb.ToString());
            this.webBrowser1.Focus();
        }
    }
}
