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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MediaPortal.GUI.Library;
using MediaPortal.Util;
using MyPlexMedia.Plugin.Config;
using MyPlexMedia.Plugin.Window.Dialogs;
using MyPlexMedia.Plugin.Window.Items;
using PlexMediaCenter.Plex;
using PlexMediaCenter.Plex.Data.Types;

namespace MyPlexMedia.Plugin.Window {
    public partial class Main {
        private static void PlexInterface_OnPlexError(PlexException plexError) {
            Log.Error(plexError);
            CommonDialogs.HideProgressDialog();
            CommonDialogs.ShowNotifyDialog(30, Settings.PLUGIN_NAME + " Error!", plexError.ErrorSource.ToString() + ": " + plexError.Message, Settings.PLEX_ICON_DEFAULT_OFFLINE, CommonDialogs.PLUGIN_NOTIFY_WINDOWS.WINDOW_DIALOG_AUTO);
        }

        private static void PlexInterface_OnResponseProgress(object userToken, int progress) {
            if (progress < 5) {
                return;
            }
            CommonDialogs.ShowProgressDialog(progress, Settings.PLUGIN_NAME, "Fetching Plex Items...",
                                             ((IMenuItem)userToken).Parent.Name + " > " + ((IMenuItem)userToken).Name,
                                             String.Format("Current Progress: {0,3}%", progress.ToString()));

        }

        private void SetBackgroundImage(string imagePath) {
            if (ctrlBackgroundImage == null || ctrlBackgroundImage.ImagePath.Equals(imagePath)) {
                return;
            }
            if (!String.IsNullOrEmpty(imagePath) && File.Exists(imagePath) ) {
                ctrlBackgroundImage.SetFileName(imagePath);
            } else {
                ctrlBackgroundImage.SetFileName(Settings.PLEX_BACKGROUND_DEFAULT);
            }
            //ctrlBackgroundImage.RemoveMemoryImageTexture();
            //ctrlBackgroundImage.DoUpdate();
            //ctrlBackgroundImage.Refresh();
            //GUIWindowManager.Process();
        }

        private static void Navigation_OnMenuItemsFetchStarted(IMenuItem itemToFetch) {
            PlexInterface.ArtworkRetriever.ResetQueue();
            CommonDialogs.ShowWaitCursor();
        }

        private void Navigation_OnMenuItemsFetchCompleted(List<IMenuItem> fetchedMenuItems, int selectedFacadeIndex,
                                                          Settings.PlexSectionLayout preferredLayout) {
            GUIPropertyManager.SetProperty("#currentmodule", String.Join(">", ((IEnumerable<string>)Navigation.History).Reverse().Take(3).Reverse().ToArray()));
            CurrentLayout = preferredLayout.Layout;
            SwitchLayout();
            facadeLayout.Clear();
            facadeLayout.ListLayout.Clear();
            facadeLayout.CoverFlowLayout.Clear();
            facadeLayout.ThumbnailLayout.Clear();
            facadeLayout.FilmstripLayout.Clear();
            facadeLayout.ListLayout.Clear();
            facadeLayout.PlayListLayout.Clear();
            fetchedMenuItems.ForEach(item => facadeLayout.Add(item as MenuItem));
            facadeLayout.RefreshCoverArt();
            facadeLayout.SelectedListItemIndex = selectedFacadeIndex;
            facadeLayout.CoverFlowLayout.SelectCard(selectedFacadeIndex);
            CommonDialogs.HideWaitCursor();
            CommonDialogs.HideProgressDialog();
        }

        private void MenuItem_OnMenuItemSelected(IMenuItem selectedItem) {
            SetBackgroundImage(selectedItem.BackgroundImage);
        }

    }
}