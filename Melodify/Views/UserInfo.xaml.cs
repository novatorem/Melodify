using SpotifyAPI;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

                // Genre text

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
