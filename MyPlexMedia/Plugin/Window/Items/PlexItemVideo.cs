﻿#region #region Copyright (C) 2005-2011 Team MediaPortal

// 
// Copyright (C) 2005-2011 Team MediaPortal
// http://www.team-mediaportal.com
// 
// MediaPortal is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// MediaPortal is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

using System;
using System.Linq;
using MediaPortal.GUI.Library;
using MediaPortal.GUI.Video;
using MediaPortal.Video.Database;
using MyPlexMedia.Plugin.Config;
using MyPlexMedia.Plugin.Window.Playback;
using PlexMediaCenter.Plex;
using PlexMediaCenter.Plex.Data.Types;

namespace MyPlexMedia.Plugin.Window.Items {
    public class PlexItemVideo : PlexItemBase {

        public MediaContainerVideo Video { get; set; }
        private IMDBMovie MovieDetails { get; set; }

        public PlexItemVideo(IMenuItem parentItem, string title, Uri path, MediaContainerVideo video)
            : base(parentItem, title, path) {
            Video = video;
            RetrieveArt = true;
            OnRetrieveArt += new RetrieveCoverArtHandler(PlexItemVideo_OnRetrieveArt);

            try {
                if (video.type.Equals("episode")) {
                    int season;
                    if (int.TryParse(video.parentIndex, out season)) {
                        Label = String.Format("S{0:00}", season);
                    } else {
                        Label = string.Empty;
                    }
                    Label = Label + String.Format("E{0:00}.{1}", int.Parse(video.index), video.title);
                    video.title = Label;
                }
            } catch { 
            }

            int duration;
            if (int.TryParse(Video.duration, out duration)) {
                Duration = duration;
            }
            if (!string.IsNullOrEmpty(Video.rating)) {
                try {
                    Rating = float.Parse(Video.rating);
                } catch {
                }
            }
            if (!string.IsNullOrEmpty(Video.viewCount)) {
                try {
                    _dvdLabel = Video.viewCount;
                    _isPlayed = Video.viewCount != "0";
                } catch {
                }
            }
            int year;
            if (int.TryParse(Video.year, out year)) {
                Year = year;
            }
            FileInfo = new MediaPortal.Util.FileInformation();
            if (!String.IsNullOrEmpty(Video.originallyAvailableAt)) {
                FileInfo.CreationTime = DateTime.Parse(Video.originallyAvailableAt);
                Label2 = FileInfo.CreationTime.ToShortDateString();
            }
            MovieDetails = new IMDBMovie {
                Plot = Video.summary,
                ThumbURL = IconImage,
                PlotOutline = Video.tagline,
                Title = Video.title,
                RunTime = Duration / 60000,
                Rating = Rating,
                Year = Year,
                MPARating = Video.contentRating,
                TagLine = Video.tagline,
                Genre = string.Join(" | ", Video.Genre.Select(x=>x.tag).ToArray())
            };
            TVTag = MovieDetails;
        }

        void PlexItemVideo_OnRetrieveArt(GUIListItem item) {
            PlexInterface.ArtworkRetriever.QueueArtworkItem(SetIcon, Settings.PLEX_ICON_DEFAULT, UriPath, Video.thumb);
            PlexInterface.ArtworkRetriever.QueueArtworkItem(SetBackground, Settings.PLEX_BACKGROUND_DEFAULT, UriPath, Video.art);
        }

        public override void SetMetaData(MediaContainer infoContainer) {

        }

        public override void OnSelected() {
            if (MovieDetails != null) {
                GUIPropertyManager.SetProperty("#title", MovieDetails.Title);
                GUIPropertyManager.SetProperty("#genre", MovieDetails.Genre);
                GUIPropertyManager.SetProperty("#runtime", MovieDetails.RunTime.ToString());
                GUIPropertyManager.SetProperty("#year", MovieDetails.Year.ToString());
                GUIPropertyManager.SetProperty("#plot", MovieDetails.Plot);
                GUIPropertyManager.SetProperty("#plotoutline", MovieDetails.PlotOutline);
                GUIPropertyManager.SetProperty("#cast", string.Empty);
                GUIPropertyManager.SetProperty("#tagline", MovieDetails.TagLine);
                GUIPropertyManager.SetProperty("#thumb", IconImageBig);
                GUIPropertyManager.SetProperty("#iswatched", _isPlayed.ToString());
            }
            base.OnSelected();
        }

        public override void OnClicked(object sender, EventArgs e) {
            PlexVideoPlayer.PlayBackMedia(UriPath, Video);
        }

        public override void OnInfo() {
            GUIVideoInfo videoInfo = (GUIVideoInfo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIDEO_INFO);
            videoInfo.Movie = (IMDBMovie)TVTag;
            GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_VIDEO_INFO);
        }
    }
}