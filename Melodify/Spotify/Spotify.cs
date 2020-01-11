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
                string songID = context.Item.Id;

                string recommendedURI = _spotify.GetRecommendations(trackSeed: new List<string> { songID }).Tracks[0].Uri;
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

                PlaybackContext context = _spotify.GetPlayingTrack();
                string songID = context.Item.Id;

                string recommendedURI = _spotify.GetRecommendations(trackSeed: new List<string> { songID }).Tracks[0].Uri;
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

    }
}
