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
            if (Config.Current.sArrayValue[Enums.sArrayValue.Pages] != null)
                if (Config.Current.sArrayValue[Enums.sArrayValue.Pages].Length != 0)
                {
                    try
                    {
                        PageCollection pages = null;
                        for (int i = 0; Config.Current.sArrayValue[Enums.sArrayValue.Pages].Length > i; i++)
                        {
                            if (Config.Current.sArrayValue[Enums.sArrayValue.Pages][i] == "Plugin")
                                pages = PluginManager.GetPages();
                            {
                                LAPP.Page p = Utility.GetPageFromString(Config.Current.sArrayValue[Enums.sArrayValue.Pages][i]);
                                if (p != null)
                                {
                                    pages = new PageCollection(false);
                                    pages.Add(p);
                                }
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