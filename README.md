# Melodify

Yet another spotify "client" that runs alongisde the program, removing certain features and adding others. This program uses [SpotifyAPI-NET](https://github.com/JohnnyCrazy/SpotifyAPI-NET) in a WPF .NET Core application. 

![](images/main.JPG)
![](images/hover.jpg)

The default view of the application.
Hovering over it reveals four buttons that perform the following:

Location | Symbol | Purpose
:---: | :---: | ---
↖|`い`|Expands the program to be fullscreen with blur
↗|`り`|Menu with various views based on your taste
↙|`ト`|Opens the playlist view, public and private
↘|`グ`|Likes a song, adding it to your favorite tracks

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
#### Top Songs View (video)
[![Preview Top Songs](images/topSongs.jpg)](https://streamable.com/m08hx)

There are other features in play, with a number more planned! I work on this during the weekends, but you're also welcome to submit feature or pull requests at any point.

## Planned Features
- Play music video in full screen view
- Display lyrics in full screen view
- Implement user statistics view
- Caching system for loading playlists

## Installation

 #### Windows Users
>
> Go to the [Releases](https://github.com/novatorem/Melodify/releases) page and download the latest release
>

 #### Mac Users
>
> Not supported at the moment
>

 #### Linux Users
>
> Not supported at the moment
>

## Development setup

Developed on Visual Studio 2019

A WPF application using .NET

## Various Views

Just to give an idea on what it generally looks like, the miniature view blurs and darkens the album art and uses it as the background as such:

![](images/multi.jpg)