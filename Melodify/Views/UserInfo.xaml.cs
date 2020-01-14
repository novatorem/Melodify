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
        public UserInfo()
        {
            InitializeComponent();
            WindowBlur.SetIsEnabled(this, true);
            MouseDown += Window_MouseDown;


            var _spotify = new SpotifyWebAPI()
            {
                AccessToken = (string)App.Current.Properties["AccessToken"],
                TokenType = (string)App.Current.Properties["TokenType"]
            };

            PrivateProfile user = _spotify.GetPrivateProfile();

            title.Text = user.DisplayName;
            BitmapImage bimage = new BitmapImage();
            bimage.BeginInit();
            bimage.UriSource = new Uri(user.Images[0].Url, UriKind.Absolute);
            bimage.EndInit();
            pPicture.ImageSource = bimage;
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
