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
        }

        private void Populate_Title()
        {
            PrivateProfile user = _spotify.GetPrivateProfile();
            try
            {
                title.Text = user.DisplayName;
                BitmapImage bimage = new BitmapImage();
                bimage.BeginInit();
                bimage.UriSource = new Uri(user.Images[0].Url, UriKind.Absolute);
                bimage.EndInit();
                pPicture.ImageSource = bimage;
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
