using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAPP;

namespace LAP.Utils
{
    public class Pages
    {
        public string[] PageCollection { get; set; } = new string[] { "Album", "Playlist", "Plugin" };

        public static PageCollection GetPages()
        {
            if (Config.Setting.Pages.PageCollection != null)
                if (Config.Setting.Pages.PageCollection.Length != 0)
                {
                    try
                    {
                        PageCollection pages = new PageCollection(false);
                        for (int i = 0; Config.Setting.Pages.PageCollection.Length > i; i++)
                        {
                            if(Config.Setting.Pages.PageCollection[i] == "Plugin")
                            {
                                for (int pc = 0; PluginManager.InitializedPlugin.Count > pc; pc++)
                                {
                                    if (PluginManager.InitializedPlugin[pc].Enabled)
                                    {
                                        foreach (LAPP.Page p in PluginManager.InitializedPlugin[pc].Instance.Pages)
                                        {
                                            if (p != null) pages.Add(p);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                LAPP.Page p = Utility.GetPageFromString(Config.Setting.Pages.PageCollection[i]);
                                if (p != null) pages.Add(p);
                            }
                        }
                        return pages;
                    }
                    catch (Exception) { }
                }

            return new PageCollection(false);
        }
    }
}