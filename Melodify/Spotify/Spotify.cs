using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;

namespace Melodify
{
    class Spotify
    {
        public static void UserTrackSuggestion()
        {
            try
            {
                using var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = (string)App.Current.Properties["AccessToken"],
                    TokenType = (string)App.Current.Properties["TokenType"]
                };


                PlaybackContext context = _spotify.GetPlayingTrack();
                PlaybackContext playbackContext = _spotify.GetPlayback();

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
                    recTracks.Add(_spotify.GetRecommendations(trackSeed: new List<string> { track }, limit: 5).Tracks[random.Next(2)].Uri);
                }
                // Play the random songs as a standalone playlist
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
                using var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = (string)App.Current.Properties["AccessToken"],
                    TokenType = (string)App.Current.Properties["TokenType"]
                };

                ErrorResponse err = _spotify.ResumePlayback(contextUri: (string)App.Current.Properties["playlistID"], offset: "");
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed at Spotify/resumePlayback");
            }
        }

        public static void LoveSong(string ID)
        {
            try
            {
                using var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = (string)App.Current.Properties["AccessToken"],
                    TokenType = (string)App.Current.Properties["TokenType"]
                };

                PlaybackContext context = _spotify.GetPlayingTrack();
                ErrorResponse response = _spotify.SaveTrack(ID);

            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed at Spotify/LoveSong");
            }
        }

        public static void UnLoveSong(string ID)
        {
            try
            {
                using var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = (string)App.Current.Properties["AccessToken"],
                    TokenType = (string)App.Current.Properties["TokenType"]
                };

                PlaybackContext context = _spotify.GetPlayingTrack();
                ErrorResponse response = _spotify.RemoveSavedTracks(new List<string> { ID });

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
                using var _spotify = new SpotifyWebAPI()
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
                using var _spotify = new SpotifyWebAPI()
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

        public static void SeekPlayback(int position)
        {
            try
            {
                using var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = (string)App.Current.Properties["AccessToken"],
                    TokenType = (string)App.Current.Properties["TokenType"]
                };
                PlaybackContext context = _spotify.GetPlayingTrack();
                ErrorResponse error = _spotify.SeekPlayback((int)((position/100.0) * context.Item.DurationMs));
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
                using var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = (string)App.Current.Properties["AccessToken"],
                    TokenType = (string)App.Current.Properties["TokenType"]
                };
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

        public static int GetSetVolume(int volume = 255)
        {
            try
            {
                using var _spotify = new SpotifyWebAPI()
                {
                    AccessToken = (string)App.Current.Properties["AccessToken"],
                    TokenType = (string)App.Current.Properties["TokenType"]
                };
                // Below should in theory work, but doesn't seem to
                PlaybackContext context = _spotify.GetPlayback();
                if (volume == 255)
                {
                    return context.Device.VolumePercent;
                } else
                {
                    ErrorResponse _ = _spotify.SetVolume(volume);
                    return 1;
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed at Spotify/GetSetVolume");
                return -1;
            }
        }
    }
}
