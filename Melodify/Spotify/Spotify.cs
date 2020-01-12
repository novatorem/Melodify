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

        private static string playlistID = "";
        private static Boolean suggestionMode = false;

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
                System.Diagnostics.Debug.WriteLine(context);
                string songID = context.Item.Id;
                System.Diagnostics.Debug.WriteLine(songID);

                suggestionMode = true;
                var playbackContext = _spotify.GetPlayback();
                var playlistID = playbackContext.Context;
                System.Diagnostics.Debug.WriteLine(playlistID);

                string recommendedURI = _spotify.GetRecommendations(trackSeed: new List<string> { songID }).Tracks[0].Uri;

                System.Diagnostics.Debug.WriteLine(recommendedURI);
                ErrorResponse err = _spotify.ResumePlayback(uris: new List<string> { recommendedURI }, offset: "");


            }
            catch
            {
                // Create the source, if it does not already exist.
                if (!EventLog.SourceExists("Melodify"))
                {
                    EventLog.CreateEventSource("Melodify", "getSuggestion");
                    return;
                }

                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog();
                myLog.Source = "Melodify";

                // Write an informational entry to the event log.    
                myLog.WriteEntry("Failed to get suggestion at Spotify/getSuggestion");
                System.Diagnostics.Debug.WriteLine("Failed to get suggestion at Spotify/getSuggestion");
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
                suggestionMode = true;
                ErrorResponse err = _spotify.ResumePlayback(uris: recTracks, offset: "");

            }
            catch
            {
                // Create the source, if it does not already exist.
                if (!EventLog.SourceExists("Melodify"))
                {
                    EventLog.CreateEventSource("Melodify", "UserTrackSuggestion");
                    return;
                }

                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog();
                myLog.Source = "Melodify";

                // Write an informational entry to the event log.    
                myLog.WriteEntry("Failed at Spotify/UserTrackSuggestion");
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

                ErrorResponse err = _spotify.ResumePlayback(contextUri: playlistID, offset: "");
                suggestionMode = false;
            }
            catch
            {
                // Create the source, if it does not already exist.
                if (!EventLog.SourceExists("Melodify"))
                {
                    EventLog.CreateEventSource("Melodify", "resumePlayback");
                    return;
                }

                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog();
                myLog.Source = "Melodify";

                // Write an informational entry to the event log.    
                myLog.WriteEntry("Failed at Spotify/resumePlayback");
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
                // Create the source, if it does not already exist.
                if (!EventLog.SourceExists("Melodify"))
                {
                    EventLog.CreateEventSource("Melodify", "LoveSong");
                    return;
                }

                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog();
                myLog.Source = "Melodify";

                // Write an informational entry to the event log.    
                myLog.WriteEntry("Failed at Spotify/LoveSong");
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
                // Create the source, if it does not already exist.
                if (!EventLog.SourceExists("Melodify"))
                {
                    EventLog.CreateEventSource("Melodify", "NextSong");
                    return;
                }

                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog();
                myLog.Source = "Melodify";

                // Write an informational entry to the event log.    
                myLog.WriteEntry("Failed at Spotify/NextSong");
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
                // Create the source, if it does not already exist.
                if (!EventLog.SourceExists("Melodify"))
                {
                    EventLog.CreateEventSource("Melodify", "PreviousSong");
                    return;
                }

                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog();
                myLog.Source = "Melodify";

                // Write an informational entry to the event log.    
                myLog.WriteEntry("Failed at Spotify/PreviousSong");
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
                }
                else
                {
                    ErrorResponse error = _spotify.ResumePlayback(offset: "");
                }
            }
            catch
            {
                // Create the source, if it does not already exist.
                if (!EventLog.SourceExists("Melodify"))
                {
                    EventLog.CreateEventSource("Melodify", "PausePlaySong");
                    return;
                }

                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog();
                myLog.Source = "Melodify";

                // Write an informational entry to the event log.    
                myLog.WriteEntry("Failed at Spotify/PausePlaySong");
                System.Diagnostics.Debug.WriteLine("Failed at Spotify/PausePlaySong");
            }
        }

    }
}
