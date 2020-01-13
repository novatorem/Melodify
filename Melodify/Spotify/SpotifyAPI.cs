using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Melodify
{
    public class SpotifyAPI
    {
        private string _clientId;
        private string _secretId;

        public SpotifyAPI() { }

        public SpotifyAPI(string clientId, string secretId, string redirectUrl = "http://localhost:4002")
        {
            _clientId = clientId;
            _secretId = secretId;

            if (!string.IsNullOrEmpty(_clientId) && !string.IsNullOrEmpty(_secretId))
            {
                ImplicitGrantAuth auth = new ImplicitGrantAuth(
                    _clientId,
                    redirectUrl,
                    redirectUrl,
                    Scope.UserReadPrivate | Scope.UserReadCurrentlyPlaying | Scope.UserTopRead | Scope.Streaming | Scope.UserModifyPlaybackState | Scope.UserLibraryModify | Scope.UserReadPlaybackState
                );
                
                auth.AuthReceived += (sender, payload) =>
                {
                    auth.Stop();
                    SpotifyWebAPI api = new SpotifyWebAPI()
                    {
                        TokenType = payload.TokenType,
                        AccessToken = payload.AccessToken
                    };
                    App.Current.Properties["TokenType"] = api.TokenType;
                    App.Current.Properties["AccessToken"] = api.AccessToken;
                };
                auth.Start();
                auth.OpenBrowser();
            }
        }
    }
}
