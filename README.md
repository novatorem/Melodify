# Melodify

Yet another spotify "client" that runs alongisde the program, removing certain features and adding others. This program uses [SpotifyAPI-NET](https://github.com/JohnnyCrazy/SpotifyAPI-NET) in a WPF .NET Core application.

## Features

<table style="text-align:center;">
  <tr>
    <td>Miniplayer with playing track info</td>
    <td>Progress bar and controls</td>
    <td>Full screen view with album art</td>
  </tr>
  <tr>
    <td>Playlist control and playback</td>
    <td>Top songs playback and preview</td>
    <td>Favorite song from miniplayer</td>
  </tr>
  <tr>
    <td>Music Videos in full screen</td>
    <td><sub><sup>🚧</sup></sub> Full user statistics and info</td>
    <td><sub><sup>🚧</sup></sub> Lyrics in full screen</td>
  </tr>
</table>

## Overview
![](images/main.JPG) ![](images/melodify.gif)

The default view of the application. The blue circles are just recorded clicks, to show some functionality.

Hovering over it reveals four buttons that perform the following:

Location | Symbol | Purpose
:---: | :---: | ---
↖|`⛶`|Expands the program to be fullscreen with blur
↗|`∞`|Menu with various views based on your taste
↙|`≡`|Opens the playlist view, public and private
↘|`♡`|Likes a song, adding it to your favorite tracks

![](images/hoverRaw.jpg)
![](images/hoverInfo.jpg)

Mouse clicks and Keyboard controls are also supported.

Color | Keystroke | Purpose
:---: | :---: | ---
Green | `spacebar` | Plays/Pauses current song
Blue | `left`/`right` | Returns to previous song or skips current song
Black | ⠀ | Allows dragging of window to desired location
⠀ | `f` | Resizes to fullscreen

#### Fullscreen View
![](images/fullscreen.jpg)
#### Top Songs View (click for sound)
[![Preview Top Songs](images/topSongs.gif)](https://streamable.com/m08hx)

There are other features in play, with a number more planned! I work on this during the weekends, but you're also welcome to submit feature or pull requests at any point.

## Installation

 #### Windows Users
>
> Go to the [Releases](https://github.com/novatorem/Melodify/releases) page and download the latest release

Currently only supports Windows 10, planning on further expansion if this picks up.

## Development setup

Developed on Visual Studio 2019 - a WPF application using .NET

You'll need to get your own Spotify API and YouTube API access.
Set up a Resources.resx file under the Solution Properties.
You'll then need three variables, with their respective keys:
- SpotID
- SpotSecret
- YoutubeAPI

## Known Issues

- Running as an admin breaks the music video feature

## Various Views

Just to give an idea on what it generally looks like, the miniature view blurs and darkens the album art and uses it as the background as such:

![](images/multi.jpg)