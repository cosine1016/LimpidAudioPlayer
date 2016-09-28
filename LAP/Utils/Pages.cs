using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAP.Utils
{
    public class Pages
    {
        public string[] PageCollection { get; set; } = new string[] { "Album", "Playlist", "Plugin" };

        public static Page.ListViewPage[] GetPages()
        {
            if (Config.Setting.Pages.PageCollection != null)
                if (Config.Setting.Pages.PageCollection.Length != 0)
                {
                    try
                    {
                        List<Page.ListViewPage> pages = new List<Page.ListViewPage>();
                        for (int i = 0; Config.Setting.Pages.PageCollection.Length > i; i++)
                        {
                            if(Config.Setting.Pages.PageCollection[i] == "Plugin")
                            {
                                for (int pc = 0; PluginManager.InitializedPlugin.Count > pc; pc++)
                                {
                                    foreach(LAPP.Page.Plugin p in PluginManager.InitializedPlugin[pc].Instance.Pages)
                                    {
                                        Page.ListViewPage lvp = new Page.Plugin.Page(p);
                                        if (lvp != null) pages.Add(lvp);
                                    }
                                }
                            }
                            else
                            {
                                Page.ListViewPage lvp = Utility.GetPageFromString(Config.Setting.Pages.PageCollection[i]);
                                if (lvp != null) pages.Add(lvp);
                            }
                        }
                        return pages.ToArray();
                    }
                    catch (Exception) { }
                }

            return new Page.ListViewPage[]
            {
                new Page.Album.Page()
            };
        }
    }
}