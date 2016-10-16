using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace LAP.Utils
{
    class PluginManager
    {
        public const string PluginDirectory = @"Plugin\";

        public static List<Plugin> InitializedPlugin { get; set; }

        public static event EventHandler PluginEnableChanged;

        static PluginManager()
        {
            LoadPlugin();
        }

        private static void LoadPlugin()
        {
            Directory.CreateDirectory(PluginDirectory);

            InitializedPlugin = new List<Plugin>();
            string[] files = Directory.GetFiles(PluginDirectory, "*.dll", SearchOption.TopDirectoryOnly);

            for (int i = 0; files.Length > i; i++)
            {
                try
                {
                    Plugin plg = new Plugin(files[i]);
                    plg.EnableChanged += Plg_EnableChanged;
                    InitializedPlugin.Add(plg);
                }
                catch(ReflectionTypeLoadException ex)
                {
                    LAP.Dialogs.LogWindow.Append("Failed to load plugin : " + files[i]);
                    LAP.Dialogs.LogWindow.Append(ex.LoaderExceptions.ToString());
                }
                catch (Exception)
                {
                    LAP.Dialogs.LogWindow.Append("Failed to load plugin : " + files[i]);
                }
            }
        }

        public static void ReLoadPlugin()
        {
            for(int i = 0;InitializedPlugin.Count > i; i++)
            {
                string title = InitializedPlugin[i].Instance.Title;
                InitializedPlugin[i].Instance.Dispose();
                InitializedPlugin[i].EnableChanged -= Plg_EnableChanged;
                InitializedPlugin[i].Enabled = false;
                LAP.Dialogs.LogWindow.Append(title + " : Disposed");
            }

            InitializedPlugin.Clear();
            LoadPlugin();
        }

        private static void Plg_EnableChanged(object sender, EventArgs e)
        {
            PluginEnableChanged?.Invoke(sender, e);
        }

        internal class Plugin
        {
            public Plugin(string Path)
            {
                Path = System.IO.Path.GetFullPath(Path);
                Asm = Assembly.LoadFile(Path);
                this.Path = Path;
                
                foreach(Type t in Asm.GetTypes())
                {
                    try
                    {
                        Instance = Activator.CreateInstance(t) as LAPP.LimpidAudioPlayerPlugin;
                    }
                    catch (Exception) { }

                    if (Instance != null)
                    {
                        LAP.Dialogs.LogWindow.Append("Plugin loaded : " + Path);
                        return;
                    }
                }

                throw new Exception();
            }

            public string Path { get; set; }

            public Assembly Asm { get; set; }

            public LAPP.LimpidAudioPlayerPlugin Instance { get; set; }

            public event EventHandler EnableChanged;

            private bool ena = true;
            public bool Enabled
            {
                get { return ena; }
                set
                {
                    if(ena != value)
                    {
                        ena = value;
                        EnableChanged?.Invoke(this, new EventArgs());
                    }
                }
            }
        }
    }
}
