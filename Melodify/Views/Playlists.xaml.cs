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
    /// Interaction logic for Playlists.xaml
    /// </summary>
    public partial class Playlists : Window
    {
        SpotifyWebAPI _spotify = new SpotifyWebAPI()

        {
            AccessToken = (string)App.Current.Properties["AccessToken"],
            TokenType = (string)App.Current.Properties["TokenType"]
        };

        public Playlists(bool adding = false)
        {
            InitializeComponent();
            WindowBlur.SetIsEnabled(this, true);
            MouseDown += Window_MouseDown;
            Populate_Playlists(adding);
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

        private void Populate_Playlists(bool adding)
        {
            int offSet = 50;
            Paging<SimplePlaylist> userPlaylistsInc;
            Paging<SimplePlaylist> userPlaylists = _spotify.GetUserPlaylists(_spotify.GetPrivateProfile().Id, limit: 50);
            while (userPlaylists.Total > offSet)
            {
                userPlaylistsInc = _spotify.GetUserPlaylists(_spotify.GetPrivateProfile().Id, limit: 50, offset: offSet);
                userPlaylists.Items.AddRange(userPlaylistsInc.Items);
                offSet += 50;
            }
            string userID = _spotify.GetPrivateProfile().Id;

            Add_Main(adding);

            foreach (SimplePlaylist playlist in userPlaylists.Items)
            {
                if (userID != playlist.Owner.Id)
                {
                    continue;
                }
                Grid grid = new Grid();


                // Get the image URL
                BitmapImage bimage = new BitmapImage();
                bimage.BeginInit();
                bimage.UriSource = new Uri(playlist.Images[0].Url, UriKind.Absolute);
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

                // Create the playlist text
                TextBlock tBlock = new TextBlock();

                string pName = playlist.Name;
                tBlock.Text = pName;

                tBlock.FontSize = 16;
                tBlock.FontWeight = FontWeights.Bold;
                tBlock.Margin = new Thickness(0, 0, 0, 35);
                tBlock.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                tBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                tBlock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC8C8C8");

                grid.Cursor = Cursors.Hand;
                if (adding)
                {
                    grid.MouseLeave += ((s, e) => Remove_Hover(s, e, grid));
                    grid.MouseEnter += ((s, e) => Add_Hover(s, e, grid, adding));
                    grid.MouseDown += ((s, e) => AddTo_Playlist(s, e, playlist.Id));
                } else
                {
                    grid.MouseLeave += ((s, e) => Remove_Hover(s, e, grid));
                    grid.MouseEnter += ((s, e) => Add_Hover(s, e, grid, adding));
                    grid.MouseDown += ((s, e) => Play_Playlist(s, e, playlist.Uri));
                }

                grid.Children.Add(ellipse);
                grid.Children.Add(tBlock);

                playlists.Children.Add(grid);
            }
        }

        private void Remove_Hover(object s, MouseEventArgs e, Grid grid)
        {
            grid.Children.RemoveAt(2);
        }

        private void Add_Hover(object s, MouseEventArgs e, Grid grid, bool adding)
        {
            Ellipse ellipseHover = new Ellipse();
            // Get the image URL
            BitmapImage bimageHover = new BitmapImage();
            bimageHover.BeginInit();
            if (adding)
            {
                bimageHover.UriSource = new Uri("https://imgur.com/bvwhxjU.png", UriKind.Absolute);
            } else
            {
                bimageHover.UriSource = new Uri("https://imgur.com/OvOsHyN.png", UriKind.Absolute);
            }
            bimageHover.EndInit();

            // Bind the image to a brush
            ImageBrush imageBrushHover = new ImageBrush();
            imageBrushHover.ImageSource = bimageHover;
            imageBrushHover.Stretch = System.Windows.Media.Stretch.UniformToFill;

            // Create the hover
            ellipseHover.Height = 100;
            ellipseHover.Width = 100;
            ellipseHover.Fill = imageBrushHover;
            ellipseHover.Opacity = 0.8;
            ellipseHover.Margin = new Thickness(0, 10, 0, 0);
            ellipseHover.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            grid.Children.Add(ellipseHover);
        }

        private void Play_Playlist(object sender, MouseEventArgs e, string playlistURI)
        {
            // Save their current playback so we can return to it after
            try
            {
                PlaybackContext context = _spotify.GetPlayingTrack();
                PlaybackContext playbackContext = _spotify.GetPlayback();
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Issue saving playback at Playlists/Play_Playlist");
            }

            ErrorResponse err = _spotify.ResumePlayback(contextUri: playlistURI, offset: "");
        }

        private void AddTo_Playlist(object sender, MouseEventArgs e, string playlistID)
        {
            // Save their current playback so we can return to it after
            try
            {
                PlaybackContext context = _spotify.GetPlayingTrack();
                ErrorResponse response = _spotify.AddPlaylistTrack(playlistID, context.Item.Uri);
                this.Close();
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Issue saving playback at Playlists/AddTo_Playlist");
            }
        }

        private void Add_Main(bool adding)
        {
            if (adding)
            {
                likedSongs.MouseLeave += ((s, e) => likedImage.Opacity = 0.25);
                likedSongs.MouseEnter += ((s, e) => likedImage.Opacity = 0.5);
                likedSongs.MouseDown += ((s, e) => {
                    Spotify.LoveSong(_spotify.GetPlayingTrack().Item.Id);
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.GetType() == typeof(MainWindow))
                        {
                            (window as MainWindow).loveClick.Content = "♥";
                        }
                    }
                });
            }
            else
            {
                List<string> songs = new List<string>();
                _spotify.GetSavedTracks().Items.ForEach((song) => songs.Add(song.Track.Uri));

                likedSongs.MouseLeave += ((s, e) => likedImage.Opacity = 0.25);
                likedSongs.MouseEnter += ((s, e) => likedImage.Opacity = 0.5);
                likedSongs.MouseDown += (s, e) => _spotify.ResumePlayback(uris: songs, offset: "");
            }
        }

    }
}
