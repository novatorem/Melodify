using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;

namespace Melodify
{
    public class SpotifyAPI
    {
        private string _clientId;
        private string _secretId;
        private Token token;
        private AuthorizationCodeAuth auth;
        private SpotifyWebAPI api;

        public SpotifyAPI() { }

        public SpotifyAPI(string clientId, string secretId, string redirectUrl = "http://localhost:4002", Boolean authed = false)
        {
            _clientId = clientId;
            _secretId = secretId;

            if (!authed)
            {

                System.Diagnostics.Debug.WriteLine("Authorizing for the first time");
                auth = new AuthorizationCodeAuth(
                    _clientId,
                    _secretId,
                    redirectUrl,
                    redirectUrl,
                    Scope.UserReadPrivate | Scope.UserReadCurrentlyPlaying | Scope.UserTopRead | Scope.Streaming | Scope.UserModifyPlaybackState | Scope.UserLibraryModify | Scope.UserReadPlaybackState | Scope.PlaylistReadPrivate
                );

                auth.AuthReceived += async (sender, payload) =>
                {
                    auth.Stop();
                    token = await auth.ExchangeCode(payload.Code);
                    api = new SpotifyWebAPI()
                    {
                        TokenType = token.TokenType,
                        AccessToken = token.AccessToken
                    };
                    App.Current.Properties["TokenType"] = api.TokenType;
                    App.Current.Properties["AccessToken"] = api.AccessToken;
                };
                auth.Start();
                auth.OpenBrowser();
                authed = true;
            }
        }
        public void Authenticate()
        {
            Token newToken = auth.RefreshToken(token.RefreshToken).Result;
            api.TokenType = newToken.TokenType;
            api.AccessToken = newToken.AccessToken;
            App.Current.Properties["TokenType"] = api.TokenType;
            App.Current.Properties["AccessToken"] = api.AccessToken;
        }
    }
}
