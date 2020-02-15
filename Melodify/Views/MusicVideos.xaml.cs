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
using YoutubeExplode;

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

        YoutubeClient client = new YoutubeClient();
        YouTubeService youtube = new YouTubeService(new BaseClientService.Initializer()
        {
            ApplicationName = "Melodify",
            ApiKey = "AIzaSyDMpkq9kWkITxDpT96m2c5K4a6ey4oQiGM",
        });

        public MusicVideos(MainWindow window)
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
                            SearchListResponse resp = listRequest.Execute();
                            string videoID = resp.Items[0].Id.VideoId;
                            ShowYouTubeVideo(videoID);
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
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ;
            //if (e.ChangedButton == MouseButton.Left)
            //    this.DragMove();
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

        private string GetYouTubeVideoPlayerHTML(string videoCode)
        {
            var sb = new StringBuilder();

            const string youtubeURL = @"http://www.youtube.com/v/";
            sb.Append("<html>");
            sb.Append("    <head>");
            sb.Append("        <meta http-equiv=\"X-UA-Compatible\" content=\"IE=Edge\"/>");
            sb.Append("        <style type=\"text/css\">body, html{margin: 0; padding: 0; height: 100%; overflow: hidden;}#content{position: absolute; left: 0; right: 0; bottom: 0; top: 0px;}</style> ");
            sb.Append("    </head>");
            sb.Append("    <body>");
            sb.Append("        <div id=\"content\">");
            sb.Append("            <iframe width=\"100%\" height=\"100%\" src=\"https://www.youtube.com/embed/" + videoCode + "?autoplay=1\"");
            sb.Append("                </iframe>");
            sb.Append("        </div>");
            sb.Append("    </body>");
            sb.Append("</html>");
            System.Diagnostics.Debug.WriteLine(sb.ToString());
            //sb.Append("<html>");
            //sb.Append("    <head>");
            //sb.Append("        <meta name=\"viewport\" content=\"width=device-width; height=device-height;\">");
            //sb.Append("    </head>");
            //sb.Append("    <body marginheight=\"0\" marginwidth=\"0\" leftmargin=\"0\" topmargin=\"0\" style=\"overflow-y: hidden\">");
            //sb.Append("        <object width=\"100%\" height=\"100%\">");
            //sb.Append("            <param name=\"movie\" value=\"" + youtubeURL + videoCode + "?version=3&amp;rel=0\" />");
            //sb.Append("            <param name=\"allowFullScreen\" value=\"true\" />");
            //sb.Append("            <param name=\"allowscriptaccess\" value=\"always\" />");
            //sb.Append("            <embed src=\"" + youtubeURL + videoCode + "?version=3&amp;rel=0\"");
            //sb.Append("                   width=\"100%\" height=\"100%\" allowscriptaccess=\"always\" allowfullscreen=\"true\" />");
            //sb.Append("        </object>");
            //sb.Append("    </body>");
            //sb.Append("</html>");

            return sb.ToString();
        }

        public void ShowYouTubeVideo(string videoCode)
        {
            this.webBrowser1.NavigateToString(GetYouTubeVideoPlayerHTML(videoCode));
        }
    }
}
