using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Melodify
{
    class Spotify
    {

        public static void CurrentTrackSuggestion()
        {

            try
            {
                var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = (string)App.Current.Properties["AccessToken"],
                    TokenType = (string)App.Current.Properties["TokenType"]
                };

                PlaybackContext context = _spotify.GetPlayingTrack();
                PlaybackContext playbackContext = _spotify.GetPlayback();

                // Save their current playback so we can return to it after
                try
                {
                    if (context.Context.Type == "playlist")
                    {
                        string playlistID = playbackContext.Context.Uri;
                        App.Current.Properties["playlistID"] = playlistID;
                        App.Current.Properties["suggestionMode"] = true;
                    }
                } catch
                {
                    System.Diagnostics.Debug.WriteLine("Not playing a playlist, can't get suggestion at Spotify/CurrentTrackSuggestion");
                }

                // Get a suggestion based on currently playing song
                string songID = context.Item.Id;
                string recommendedURI = _spotify.GetRecommendations(trackSeed: new List<string> { songID }).Tracks[0].Uri;
                ErrorResponse err = _spotify.ResumePlayback(uris: new List<string> { recommendedURI }, offset: "");

            }
            catch
            {

                App.Current.Properties["suggestionMode"] = false;
                System.Diagnostics.Debug.WriteLine("Failed to get suggestion at Spotify/CurrentTrackSuggestion");
            }
        }

        public static void UserTrackSuggestion()
        {
            try
            {
                var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = (string)App.Current.Properties["AccessToken"],
                    TokenType = (string)App.Current.Properties["TokenType"]
                };


                PlaybackContext context = _spotify.GetPlayingTrack();
                PlaybackContext playbackContext = _spotify.GetPlayback();

                // Save their current playback so we can return to it after
                try
                {
                    if (context.Context.Type == "playlist")
                    {
                        string playlistID = playbackContext.Context.Uri;
                        App.Current.Properties["playlistID"] = playlistID;
                        App.Current.Properties["suggestionMode"] = true;
                    }
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("Not playing a playlist, can't get suggestion at Spotify/UserTrackSuggestion");
                }

                Random random = new Random();
                // Get users top playing tracks
                Paging<FullTrack> tracks = _spotify.GetUsersTopTracks();
                List<string> favTracks = new List<string>();
                // Get the ID of each track
                tracks.Items.ForEach(item => favTracks.Add(item.Id));
                // Iterate through those IDs and find a random suggested song based on them
                List<string> recTracks = new List<string>();
                foreach (string track in favTracks)
                {
                    recTracks.Add(_spotify.GetRecommendations(trackSeed: new List<string> { track }, limit: 5).Tracks[random.Next(4)].Uri);
                }
                // Play the random songs as a standalone playlist
                App.Current.Properties["suggestionMode"] = true;
                ErrorResponse err = _spotify.ResumePlayback(uris: recTracks, offset: "");

            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed at Spotify/UserTrackSuggestion");
            }
        }

        public static void ResumePlayback()
        {
            try
            {
                var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = (string)App.Current.Properties["AccessToken"],
                    TokenType = (string)App.Current.Properties["TokenType"]
                };

                ErrorResponse err = _spotify.ResumePlayback(contextUri: (string)App.Current.Properties["playlistID"], offset: "");
                App.Current.Properties["suggestionMode"] = false;
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed at Spotify/resumePlayback");
            }
        }

        public static void LoveSong()
        {
            try
            {
                var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = (string)App.Current.Properties["AccessToken"],
                    TokenType = (string)App.Current.Properties["TokenType"]
                };

                PlaybackContext context = _spotify.GetPlayingTrack();
                ErrorResponse response = _spotify.SaveTrack(context.Item.Id);

            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed at Spotify/LoveSong");
            }
        }

        public static void NextSong()
        {
            try
            {
                var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = (string)App.Current.Properties["AccessToken"],
                    TokenType = (string)App.Current.Properties["TokenType"]
                };

                ErrorResponse error = _spotify.SkipPlaybackToNext();

            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed at Spotify/NextSong");
            }
        }

        public static void PreviousSong()
        {
            try
            {
                var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = (string)App.Current.Properties["AccessToken"],
                    TokenType = (string)App.Current.Properties["TokenType"]
                };

                ErrorResponse error = _spotify.SkipPlaybackToPrevious();

            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed at Spotify/PreviousSong");
            }
        }

        public static void PausePlaySong()
        {
            try
            {
                var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = (string)App.Current.Properties["AccessToken"],
                    TokenType = (string)App.Current.Properties["TokenType"]
                };
                // Below should in theory work, but doesn't seem to
                PlaybackContext context = _spotify.GetPlayback();
                if (context.IsPlaying)
                {
                    ErrorResponse error = _spotify.PausePlayback();
                    App.Current.Properties["userPause"] = true;
                }
                else
                {
                    ErrorResponse error = _spotify.ResumePlayback(offset: "");
                    App.Current.Properties["userPause"] = false;
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed at Spotify/PausePlaySong");
            }
        }


    }
}
