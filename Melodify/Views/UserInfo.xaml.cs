using SpotifyAPI;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
            Populate_Genre();
            Populate_Artists();
        }

        private void Populate_Intro()
        {
            PrivateProfile user = _spotify.GetPrivateProfile();
            try
            {
                BitmapImage userArt = new BitmapImage();
                userArt.BeginInit();
                try
                {
                    userArt.UriSource = new Uri(user.Images[0].Url, UriKind.Absolute);
                    userArt.EndInit();
                }
                catch
                {
                    userArt.UriSource = new Uri("https://source.unsplash.com/random/600x600", UriKind.Absolute);
                    userArt.EndInit();
                }
                userCover.Source = userArt;
                userCover.SetValue(WidthProperty, DependencyProperty.UnsetValue);
                userCover.SetValue(HeightProperty, DependencyProperty.UnsetValue);

                // Intro Text
                string followerStatement;
                int followers = user.Followers.Total;
                switch (followers)
                {
                    case var _ when followers < 5:
                        followerStatement = "hidden gem";
                        break;
                    case var _ when followers < 20:
                        followerStatement = "taste maker";
                        break;
                    case var _ when followers < 40:
                        followerStatement = "likeable curator";
                        break;
                    default:
                        followerStatement = "pillar of the community";
                        break;
                }

                RegionInfo location = new RegionInfo(user.Country);
                introText.Text = user.DisplayName.Substring(0, 1).ToUpper() + user.DisplayName.Substring(1) + " from " + location.DisplayName +
                    ", the " + followerStatement + " with " + user.Followers.Total + " followers.";

                // Popularity Text
                int popularity = 0;
                string popularitStatement;

                Paging<FullArtist> artists = _spotify.GetUsersTopArtists();
                artists.Items.ForEach((artist) => popularity += artist.Popularity);

                popularity = popularity / 20;

                switch (popularity)
                {
                    case var _ when popularity < 25:
                        popularitStatement = "obscure";
                        break;
                    case var _ when popularity < 50:
                        popularitStatement = "hidden";
                        break;
                    case var _ when popularity < 75:
                        popularitStatement = "well known";
                        break;
                    default:
                        popularitStatement = "popular";
                        break;
                }
                popMetrics.Text = "Your favorite artsits are in the " + AddOrdinal(popularity) + " percentile of popularity, rather " + popularitStatement + " musicians.";
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Issue getting info at UserInfo/Populate_Title- " + e.Message);
            }
        }

        private void Hyperlink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd",
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = $"/c start https://spotify.me"
            };
            Process.Start(psi);
        }

        private void Populate_Genre()
        {
            Paging<FullTrack> tracks = _spotify.GetUsersTopTracks();
            PrivateProfile user = _spotify.GetPrivateProfile();

            BitmapImage userArt = new BitmapImage();
            userArt.BeginInit();
            try
            {
                userArt.UriSource = new Uri(user.Images[0].Url, UriKind.Absolute);
                userArt.EndInit();
            } catch
            {
                userArt.UriSource = new Uri("https://source.unsplash.com/random/600x600", UriKind.Absolute);
                userArt.EndInit();
            }

            List<string> genres = new List<string>();

            foreach (FullTrack track in tracks.Items)
            {
                _spotify.GetArtist(track.Artists[0].Id).Genres.ForEach((genre) => genres.Add(genre));
            }

            // Gets top 14 genres by recurrence
            genres = genres.GroupBy(v => v)
                  .Select(g => new { Value = g.Key, Count = g.Count() })
                  .OrderByDescending(g => g.Count)
                  .Select(g => g.Value)
                  .Take(14)
                  .Reverse()
                  .ToList();

            List<SearchItem> searched = new List<SearchItem>();

            // Gets a unique playlist for each genre
            genres.ForEach((genre) => {
                int lim = 1;
                while (true)
                {
                    bool noDup = true;
                    var found = _spotify.SearchItems(q: genre, type: SearchType.Playlist, limit: lim, offset: lim - 1);
                    searched.ForEach((single) =>
                    {
                        if (single.Playlists.Items[0].Id == found.Playlists.Items[0].Id)
                        {
                            noDup = false;
                        }
                    });
                    if (noDup)
                    {
                        searched.Add(found);
                        break;
                    } else
                    {
                        lim++;
                    }
                }
            });

            // We reversed because we want to ensure we get the top playlist by worst genre, maximizing average happiness
            genres.Reverse();
            searched.Reverse();
            // Maybe revert back?

            int iteration = 0;
            foreach (SearchItem searchItem in searched)
            {
                SimplePlaylist playlist = searchItem.Playlists.Items[0];
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

                // Create the song text
                TextBlock tBlock = new TextBlock();

                tBlock.Text = genres[iteration];
                tBlock.FontSize = 16;
                tBlock.Margin = new Thickness(0, 0, 0, 35);
                tBlock.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                tBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                tBlock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC8C8C8");

                //// Create the event handlers
                grid.MouseEnter += new MouseEventHandler((s, e) => Preview_Song(s, e, playlist.Id, bimage, grid));
                grid.MouseLeave += new MouseEventHandler((s, e) => Stop_Preview(s, e, userArt, grid));
                grid.MouseDown += ((s, e) => Play_Playlist(s, e, playlist));

                grid.Cursor = Cursors.Hand;

                grid.Children.Add(ellipse);
                grid.Children.Add(tBlock);

                iteration += 1;
                genreInfo.Children.Add(grid);
            }
        }

        private void Preview_Song(object sender, EventArgs e, string playlistID, BitmapImage bimage, Grid grid)
        {
            int track = 0;
            Add_Hover(grid);
            string previewURL = null;
            System.Threading.Tasks.Task.Delay(25);

            // Get the preview url from the first song in the playlist
            FullPlaylist playlist = _spotify.GetPlaylist(playlistID);
            while (previewURL == null)
            {
                previewURL = _spotify.GetTrack(playlist.Tracks.Items[track].Track.Id).PreviewUrl;
                track++;
                if (track > playlist.Tracks.Items.Count - 1)
                {
                    return;
                }
            }
            if (previewURL != null)
            {
                previewer.Open(new Uri(previewURL));
                previewer.Play();
                userCover.Source = bimage;
                userCover.SetValue(HeightProperty, DependencyProperty.UnsetValue);
                userCover.SetValue(WidthProperty, DependencyProperty.UnsetValue);
            }
        }

        private void Stop_Preview(object sender, EventArgs e, BitmapImage userArt, Grid grid)
        {
            previewer.Stop();
            grid.Children.RemoveAt(2);
            userCover.Source = userArt;
        }

        private void Play_Playlist(object sender, MouseEventArgs e, SimplePlaylist playlist)
        {
            ErrorResponse err = _spotify.ResumePlayback(contextUri: playlist.Uri, offset: "");
        }

        private void Add_Hover(Grid grid)
        {
            Ellipse ellipseHover = new Ellipse();
            // Get the image URL
            BitmapImage bimageHover = new BitmapImage();
            bimageHover.BeginInit();
            bimageHover.UriSource = new Uri("https://i.imgur.com/OvOsHyN.png", UriKind.Absolute);
            bimageHover.EndInit();

            // Bind the image to a brush
            ImageBrush imageBrushHover = new ImageBrush();
            imageBrushHover.ImageSource = bimageHover;
            imageBrushHover.Stretch = System.Windows.Media.Stretch.UniformToFill;

            // Create the hover
            ellipseHover.Height = 100;
            ellipseHover.Name = "hoverIcon";
            ellipseHover.Width = 100;
            ellipseHover.Fill = imageBrushHover;
            ellipseHover.Opacity = 0.8;
            ellipseHover.Margin = new Thickness(0, 10, 0, 0);
            ellipseHover.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            grid.Children.Add(ellipseHover);
        }

        private void Populate_Artists()
        {
            PrivateProfile user = _spotify.GetPrivateProfile();

            BitmapImage userArt = new BitmapImage();
            userArt.BeginInit();
            try
            {
                userArt.UriSource = new Uri(user.Images[0].Url, UriKind.Absolute);
                userArt.EndInit();
            }
            catch
            {
                userArt.UriSource = new Uri("https://source.unsplash.com/random/600x600", UriKind.Absolute);
                userArt.EndInit();
            }

            List<FullArtist> artists = new List<FullArtist>();
            artists.Add(_spotify.GetUsersTopArtists(timeRange: TimeRangeType.ShortTerm, limit: 1).Items[0]);
            artists.Add(_spotify.GetUsersTopArtists(timeRange: TimeRangeType.MediumTerm, limit: 1).Items[0]);
            artists.Add(_spotify.GetUsersTopArtists(timeRange: TimeRangeType.LongTerm, limit: 1).Items[0]);

            foreach (FullArtist artist in artists) { 
                Grid grid = new Grid();

                // Get the image URL
                BitmapImage bimage = new BitmapImage();
                bimage.BeginInit();
                bimage.UriSource = new Uri(artist.Images[0].Url, UriKind.Absolute);
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

                // Pass over the top tracks
                SeveralTracks tracks = _spotify.GetArtistsTopTracks(artist.Id, user.Country);

                //// Create the event handlers
                grid.MouseEnter += new MouseEventHandler((s, e) => Preview_Artist(s, e, tracks, user, bimage, grid));
                grid.MouseLeave += new MouseEventHandler((s, e) => Stop_Preview(s, e, userArt, grid));
                grid.MouseDown += ((s, e) => Play_Artist(s, e, tracks));

                grid.Cursor = Cursors.Hand;

                grid.Children.Add(ellipse);
                grid.Children.Add(tBlock);

                artInfo.Children.Add(grid);
            }
        }

        private void Preview_Artist(object sender, EventArgs e, SeveralTracks tracks, PrivateProfile user, BitmapImage bimage, Grid grid)
        {
            int track = 0;
            Add_Hover(grid);
            string previewURL = null;
            System.Threading.Tasks.Task.Delay(25);

            // Get the preview url from the first song in the playlist
            while (previewURL == null)
            {
                previewURL = tracks.Tracks[track].PreviewUrl;
                track++;
                if (track > tracks.Tracks.Count)
                {
                    return;
                }
            }
            if (previewURL != null)
            {
                previewer.Open(new Uri(previewURL));
                previewer.Play();
                userCover.Source = bimage;
                userCover.SetValue(HeightProperty, DependencyProperty.UnsetValue);
                userCover.SetValue(WidthProperty, DependencyProperty.UnsetValue);
            }
        }

        private void Play_Artist(object sender, MouseEventArgs e, SeveralTracks tracks)
        {
            List<string> songs = new List<string>();
            tracks.Tracks.ForEach((track) => songs.Add(track.Uri));

            _ = _spotify.ResumePlayback(uris: songs, offset: "");
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            moreInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3366ff"));
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            moreInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C8C8"));
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

        public static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }
    }
}
