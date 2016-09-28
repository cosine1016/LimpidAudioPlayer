using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAP.Utils
{
    public class Strings
    {
        public string ApplicationTitle { get; set; } = "Limpid Audio Player";

        public string CreatePlaylist { get; set; } = "Create Playlist";

        public string EditPlaylist { get; set; } = "Edit Playlist";

        public string CreatePlaylistButton { get; set; } = "Create";

        public string CreateAlbum { get; set; } = "Create Album";

        public string Unknown { get; set; } = "Unknown";

        public string Stream { get; set; } = "Stream";

        public string BackContent { get; set; } = "Back";

        public string Discs { get; set; } = "Discs";

        public string NoDisc { get; set; } = "NoDisc";

        public string[] SameNamePlaylistExist { get; set; } = new string[] { "Playlist", "Same name playlist has already exist.\nDo you want to overwrite?"};

        public string GettingFileInformations { get; set; } = "Getting File Infotmations. Please Wait...";

        public StatusString Status { get; set; } = new StatusString();

        public ContextMenuString ContextMenu { get; set; } = new ContextMenuString();

        public WindowString Window { get; set; } = new WindowString();

        public CategoryNameString CategoryName { get; set; } = new CategoryNameString();

        public DriveNameString DriveName { get; set; } = new DriveNameString();

        public SettingString Setting { get; set; } = new SettingString();

        public PathString Path { get; set; } = new PathString();

        public SubLabelFormatString SubLabelFormat { get; set; } = new SubLabelFormatString();

        public OptionalViewString OptionalView { get; set; } = new OptionalViewString();

        public ExceptionMessageString ExceptionMessage { get; set; } = new ExceptionMessageString();

        public class StatusString
        {
            public string Playing { get; set; } = "Play";

            public string Pause { get; set; } = "Pause";

            public string Rendering { get; set; } = "Rendering";
        }

        public class ContextMenuString
        {
            public string Edit { get; set; } = "Edit";

            public string Delete { get; set; } = "Delete";

            public string Remove { get; set; } = "Remove";

            public string Property { get; set; } = "Property";

            public string ShowInExplorer { get; set; } = "Show in Explorer";
        }

        public class WindowString
        {
            public PlaylistString Playlist { get; set; } = new PlaylistString();

            public class PlaylistString
            {
                public string NameLabel { get; set; } = "Name";

                public string StickyLabel { get; set; } = "Sticky";

                public string OpenButton { get; set; } = "Open";

                public string AddButton { get; set; } = "Add";

                public string CreateButton { get; set; } = "Create";

                public string EditButton { get; set; } = "Edit";

                public string FileLabel { get; set; } = "File";

                public string DirectoryLabel { get; set; } = "Directory";

                public string FilterLabel { get; set; } = "Filter";

                public string TopDirectoryOnlyRadio { get; set; } = "TopDirectoryOnly";

                public string AllDirectoriesRadio { get; set; } = "AllDirectories";
            }
        }

        public class CategoryNameString
        {
            public string Album { get; set; } = "Album";

            public string Playlist { get; set; } = "Playlist";

            public string Explorer { get; set; } = "Explorer";

            public string Favorite { get; set; } = "Favorite";

            public string Setting { get; set; } = "Setting";
        }

        public class DriveNameString
        {
            public DriveNameString()
            {
                ReplacedText = new ReplacedTextString();
                ReplaceText = new ReplaceTextString();

                MainLabelText = ReplaceText.VolumeLabel + "(" + ReplaceText.Name + ")";
                SubLabelText = ReplaceText.DriveType + "Drive - " + ReplaceText.TotalFreeSpace + "/" +
                ReplaceText.TotalSize + "(" + ReplaceText.DriveFormat + ")";
            }

            public string MainLabelText { get; set; }

            public string SubLabelText { get; set; }

            public ReplacedTextString ReplacedText { get; set; } = new ReplacedTextString();

            public ReplaceTextString ReplaceText { get; set; } = new ReplaceTextString();

            public class ReplacedTextString
            {
                public string NullVolumeLabel { get; set; } = "LocalDisc";

                public string CDRom { get; set; } = "Disc";

                public string Fixed { get; set; } = "Fixed";

                public string Network { get; set; } = "Network";

                public string Ram { get; set; } = "Ram";

                public string Removable { get; set; } = "Removable";

                public string Unknown { get; set; } = "Unknown";
            }

            public class ReplaceTextString
            {
                public string Name { get; set; } = "*Name*";

                public string DriveType { get; set; } = "*DriveType*";

                public string VolumeLabel { get; set; } = "*VolumeLabel*";

                public string DriveFormat { get; set; } = "*FileSystem*";

                public string TotalSize { get; set; } = "*TotalSize*";

                public string TotalFreeSpace { get; set; } = "*TotalFreeSpace*";

                public string TotalUsedSpace { get; set; } = "*TotalUsedSpace*";
            }
        }

        public class SettingString
        {
            public string UISetting = "UI";

            public string RenderingSetting = "Rendering";

            public string LanguageSetting = "Language";

            public class UI
            {
                public string UseAnimation = "Animation";

                public string UseSearchBox = "SearchBox";
            }

            public class Rendering
            {
                public string[] WASAPI { get; set; } = new string[] { "Enable WASAPI",
                    "If Wasapi Enabled, This Program Disable 8-bit, 24-bit & 32-bit Floating-point Output Formats" };
            }
        }

        public class PathString
        {
            public string Directory { get; set; } = "Directory";

            public string File { get; set; } = "File";

            public string NotFound { get; set; } = "NotFound";

            public string SearchPattern { get; set; } = "*.*";
        }

        public class SubLabelFormatString
        {
            public SubLabelFormatString()
            {
                Format = ReplacementStrings.Artist + " - " + ReplacementStrings.Album + "(" + ReplacementStrings.Extension + ")";
            }

            public ReplacementStringsString ReplacementStrings { get; set; } = new ReplacementStringsString();

            public class ReplacementStringsString
            {
                public string Title { get; set; } = "%Tit%";

                public string Album{ get; set; } = "%Alb%";

                public string Artist { get; set; } = "%Art%";

                public string Extension { get; set; } = "%Ext%";
            }

            public string Format { get; set; }

            public string CreateSubLabelString(string Title, string Album, string Artist, string Extension, string NullString)
            {
                if (string.IsNullOrEmpty(Title)) Title = NullString;
                if (string.IsNullOrEmpty(Album)) Album = NullString;
                if (string.IsNullOrEmpty(Artist)) Artist = NullString;
                if (string.IsNullOrEmpty(Extension)) Extension = NullString;
                return Format.Replace(ReplacementStrings.Title, Title).Replace(ReplacementStrings.Album, Album)
                    .Replace(ReplacementStrings.Artist, Artist).Replace(ReplacementStrings.Extension, Extension);
            }
        }

        public class OptionalViewString
        {
            public string Open { get; set; } = "Open";

            public string Exit { get; set; } = "Exit";
        }

        public class ExceptionMessageString
        {
            public string[] RenderingError { get; set; } = new string[] { "Failed to render file",
                "Not enough codec or the file is not include audio stream." , "Click to Show Description" };

            public string DirectoryNotFound { get; set; } = "Directory Not Found";

            public string[] UnauthorizedAccess { get; set; } = new string[] { "Unauthorized Access",
                "Failed to open directory.\nPermission denied.", "Click to Show Description" };

            public string NotImplemented { get; set; } = "Function Not Implemented";

            public PathExceptionString PathException { get; set; } = new PathExceptionString();

            public class PathExceptionString
            {
                public string IncorrectPath { get; set; } = "Please Enter the Correct Path";

                public string UsingInvalidChars { get; set; } = "Can Not Use Invalid Chars";

                public string SetToDefaultFilter { get; set; } = "Set to Default Filter";
            }
        }
    }
}
