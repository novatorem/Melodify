using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;

namespace Melodify
{
    /// <summary>
    /// Interaction logic for UserInfo.xaml
    /// </summary>
    public partial class UserInfo : Window
    {

        MediaPlayer previewer = new MediaPlayer();
        SpotifyWebAPI _spotify = new SpotifyWebAPI()

        {
            AccessToken = (string)App.Current.Properties["AccessToken"],
            TokenType = (string)App.Current.Properties["TokenType"]
        };

        public UserInfo()
        {
            InitializeComponent();
            WindowBlur.SetIsEnabled(this, true);
            MouseDown += Window_MouseDown;
            Populate_Title();
            Populate_Songs();
        }

        private void Populate_Title()
        {
            PrivateProfile user = _spotify.GetPrivateProfile();

            title.Text = user.DisplayName;
            BitmapImage bimage = new BitmapImage();
            bimage.BeginInit();
            bimage.UriSource = new Uri(user.Images[0].Url, UriKind.Absolute);
            bimage.EndInit();
            pPicture.ImageSource = bimage;
        }

        private void Populate_Songs()
        {
            Paging<FullTrack> tracks = _spotify.GetUsersTopTracks();
            string userArtists = "";
            foreach (FullTrack track in tracks.Items)
            {
                TextBlock tBlock = new TextBlock();

                userArtists = "🎜 " + track.Name + " - " + track.Artists[0].Name;
                tBlock.Text = userArtists;

                tBlock.FontSize = 16;
                tBlock.Margin = new Thickness(0, 5, 0, 0);
                tBlock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC8C8C8");
                tBlock.MouseEnter += new MouseEventHandler((s, e) => Preview_Song(s, e, track.Id));
                tBlock.MouseLeave += new MouseEventHandler(Stop_Preview);
                tBlock.MouseDown += ((s, e) => Open_Song(s, e, track.Uri));

                uSongs.Children.Add(tBlock);
            }
        }

        private void Preview_Song(object sender, EventArgs e, string songID)
        {
            
            // Save their current playback so we can return to it after
            try
            {
                PlaybackContext context = _spotify.GetPlayingTrack();
                PlaybackContext playbackContext = _spotify.GetPlayback();
                if (context.Context.Type == "playlist")
                {
                    string playlistID = playbackContext.Context.Uri;
                    App.Current.Properties["playlistID"] = playlistID;
                    App.Current.Properties["suggestionMode"] = true;
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Issue saving playback at UserInfo/Preview_Song");
            }

            // Play the preview url
            string previewURL = _spotify.GetTrack(songID).PreviewUrl;
            if (previewURL != null)
            {
                previewer.Open(new Uri(previewURL));
                previewer.Play();
            }
        }

        private void Stop_Preview(object sender, EventArgs e)
        {
            previewer.Stop();
        }

        private void Open_Song(object sender, MouseEventArgs e, string songURI)
        {
            ErrorResponse err = _spotify.ResumePlayback(uris: new List<string> { songURI }, offset: "");
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
