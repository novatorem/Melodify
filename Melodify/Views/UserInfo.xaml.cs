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

                introText.Text = user.DisplayName + " from " + user.Country +
                    ", you have " + user.Followers.Total + " followers, making you a " + followerStatement + ".";


                // ADD POPULARITY INFO

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Issue getting info at UserInfo/Populate_Title- " + e.Message);
            }
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
