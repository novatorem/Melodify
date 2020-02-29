using SpotifyAPI;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            Populate_Intro();
        }

        private void Populate_Intro()
        {
            PrivateProfile user = _spotify.GetPrivateProfile();
            try
            {
                BitmapImage bimage = new BitmapImage();
                bimage.BeginInit();
                bimage.UriSource = new Uri(user.Images[0].Url, UriKind.Absolute);
                bimage.EndInit();
                userCover.Source = bimage;
                userCover.SetValue(WidthProperty, DependencyProperty.UnsetValue);
                userCover.SetValue(HeightProperty, DependencyProperty.UnsetValue);

                string followerStatement;
                int followers = user.Followers.Total;
                switch (followers)
                {
                    case var _ when followers < 3:
                        followerStatement = "hidden gem";
                        break;
                    case var _ when followers < 10:
                        followerStatement = "taste maker";
                        break;
                    case var _ when followers < 20:
                        followerStatement = "likeable curator";
                        break;
                    default:
                        followerStatement = "pillar of the community";
                        break;
                } 

                introText.Text = user.DisplayName + " from " + user.Country +
                    ", you have " + user.Followers.Total + " followers, making you a " + followerStatement + ".";


                // ADD POPULARITY INFO

                Populate_Artists(user.Country, bimage);

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Issue getting info at UserInfo/Populate_Title- " + e.Message);
            }
        }

        private void Populate_Artists(string country, BitmapImage userArt)
        {
            Paging<FullArtist> artists = _spotify.GetUsersTopArtists(limit: 21);

            foreach (FullArtist artist in artists.Items)
            {
                Grid grid = new Grid();

                // Get the image URL
                BitmapImage bimage = new BitmapImage();
                bimage.BeginInit();
                bimage.UriSource = new Uri(artist.Images[1].Url, UriKind.Absolute);
                bimage.EndInit();

                // Bind the image to a brush
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = bimage;
                imageBrush.Stretch = System.Windows.Media.Stretch.UniformToFill;

                // Create the image container
                Ellipse ellipse = new Ellipse();
                ellipse.Height = 100;
                ellipse.Width = 100;
                ellipse.Fill = imageBrush;
                ellipse.Margin = new Thickness(0, 10, 0, 0);
                ellipse.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                // Create the song text
                TextBlock tBlock = new TextBlock();

                tBlock.Text = artist.Name;
                tBlock.FontSize = 16;
                tBlock.Margin = new Thickness(0, 0, 0, 35);
                tBlock.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                tBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                tBlock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC8C8C8");

                // Create the event handlers
                SeveralTracks tracks = _spotify.GetArtistsTopTracks(artist.Id, country);
                grid.MouseEnter += new MouseEventHandler((s, e) => Preview_Song(s, e, tracks.Tracks[0].Id, bimage));
                grid.MouseLeave += new MouseEventHandler((s, e) => Stop_Preview(s, e, userArt));
                grid.MouseDown += ((s, e) => Open_Artist(s, e, tracks));

                grid.Cursor = Cursors.Hand;

                grid.Children.Add(ellipse);
                grid.Children.Add(tBlock);

                uArtists.Children.Add(grid);
            }
        }

        private void Preview_Song(object sender, EventArgs e, string songID, BitmapImage bimage)
        {
            // Play the preview url
            string previewURL = _spotify.GetTrack(songID).PreviewUrl;
            if (previewURL != null)
            {
                previewer.Open(new Uri(previewURL));
                previewer.Play();
                userCover.Source = bimage;
                userCover.SetValue(HeightProperty, DependencyProperty.UnsetValue);
                userCover.SetValue(WidthProperty, DependencyProperty.UnsetValue);
            }
        }

        private void Stop_Preview(object sender, EventArgs e, BitmapImage userArt)
        {
            previewer.Stop();
            userCover.Source = userArt;
        }

        private void Open_Artist(object sender, MouseEventArgs e, SeveralTracks artistTracks)
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
                System.Diagnostics.Debug.WriteLine("Issue saving playback at TopSongs/Preview_Song");
            }
            List<string> songs = new List<string>();
            artistTracks.Tracks.ForEach((track) => songs.Add(track.Uri));

            _ = _spotify.ResumePlayback(uris: songs, offset: "");
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
