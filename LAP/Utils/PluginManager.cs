using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace LAP.Utils
{
    public class PluginChangedEventArgs : EventArgs
    {
        public PluginChangedEventArgs(PluginManager.Plugin Plugin)
        {
            this.Plugin = Plugin;
        }

        public PluginChangedEventArgs(PluginManager.Plugin Plugin, bool Unload) : this(Plugin)
        {
            this.Unload = Unload;
        }

        public PluginManager.Plugin Plugin { get; set; }

        public bool Unload { get; set; }
    }

    public class PluginManager
    {
        public const string PluginDirectory = @"Plugin\";

        public static List<Plugin> InitializedPlugin { get; set; }

        private static PluginInfoCollection InfoCollection { get; set; }

        public static event EventHandler<PluginChangedEventArgs> PluginChanged;

        internal static LAPP.PageCollection GetPages()
        {
            LAPP.PageCollection pages = new LAPP.PageCollection(false);

            for (int i = 0; InitializedPlugin.Count > i; i++)
            {
                if(InitializedPlugin[i].Enabled)
                    pages.AddRange(InitializedPlugin[i].Instance.Pages);
            }

            return pages;
        }

        internal static LAPP.DisposableItemCollection<LAPP.Wave.WaveStreamPlugin> GetWaveStreams()
        {
            LAPP.DisposableItemCollection<LAPP.Wave.WaveStreamPlugin> streams
                = new LAPP.DisposableItemCollection<LAPP.Wave.WaveStreamPlugin>();

            for(int i = 0;InitializedPlugin.Count > i; i++)
            {
                if (InitializedPlugin[i].Enabled)
                    streams.AddRange(InitializedPlugin[i].Instance.WaveStreams.ToArray());
            }

            return streams;
        }

        internal static Collection<LAPP.Setting.ISettingItem> GetSettings()
        {
            Collection<LAPP.Setting.ISettingItem> sets
                = new Collection<LAPP.Setting.ISettingItem>();

            for (int i = 0; InitializedPlugin.Count > i; i++)
            {
                if (InitializedPlugin[i].Enabled)
                {
                    for(int j = 0;InitializedPlugin[i].Instance.SettingItems.Count > j; j++)
                    {
                        sets.Add(InitializedPlugin[i].Instance.SettingItems[j]);
                    }
                }
            }

            return sets;
        }

        internal static Collection<NWrapper.IManagableProvider> GetProviders()
        {
            Collection<NWrapper.IManagableProvider> pros
                = new Collection<NWrapper.IManagableProvider>();

            for (int i = 0; InitializedPlugin.Count > i; i++)
            {
                if (InitializedPlugin[i].Enabled)
                {
                    for (int j = 0; InitializedPlugin[i].Instance.Providers.Count > j; j++)
                    {
                        pros.Add(InitializedPlugin[i].Instance.Providers[j]);
                    }
                }
            }

            return pros;
        }

        static PluginManager()
        {
            if (!Directory.Exists(PluginDirectory))
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(Assembly.GetExecutingAssembly().Location);
                psi.Verb = "RunAs";
                psi.Arguments = "-InitPluginDir";
                System.Diagnostics.Process.Start(psi).WaitForExit();
            }

            LoadInfo();
            LoadPlugin();
        }

        private static void LoadPlugin()
        {
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
                    Dialogs.LogWindow.Append("Failed to load plugin : " + files[i]);
                    Dialogs.LogWindow.Append(ex.LoaderExceptions.ToString());
                }
                catch (Exception)
                {
                    Dialogs.LogWindow.Append("Failed to load plugin : " + files[i]);
                }
            }
        }

        private static void LoadInfo()
        {
            string PluginInfoFile = Config.Current.Path[Enums.Path.PluginManagementFile];
            if (File.Exists(PluginInfoFile))
            {
                try
                {
                    XmlSerializer ser = new XmlSerializer(typeof(PluginInfoCollection));
                    using (StreamReader sr = new StreamReader(PluginInfoFile))
                    {
                        InfoCollection = (PluginInfoCollection)ser.Deserialize(sr);
                    }
                }
                catch (Exception) { InfoCollection = new PluginInfoCollection(); }
            }
            else
                InfoCollection = new PluginInfoCollection();
        }

        private static void SaveInfo()
        {
            string PluginInfoFile = Config.Current.Path[Enums.Path.PluginManagementFile];
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(PluginInfoCollection));
                using (StreamWriter sw = new StreamWriter(PluginInfoFile))
                {
                    ser.Serialize(sw, InfoCollection);
                }
            }
            catch (Exception) { InfoCollection = new PluginInfoCollection(); }
        }

        public static void UnloadPlugin(Plugin Plugin)
        {
            if (InitializedPlugin.Remove(Plugin))
            {
                PluginChanged?.Invoke(null, new PluginChangedEventArgs(Plugin, true));
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
            SaveInfo();
            Plugin plg = (Plugin)sender;
            PluginChanged?.Invoke(sender, new PluginChangedEventArgs(plg));
        }

        public class Plugin
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
                        Dialogs.LogWindow.Append("Plugin loaded : " + Path);
                    }
                }
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

            public PluginInfo GetInfo()
            {
                return new PluginInfo(Asm, Enabled);
            }
        }

        public class PluginInfo
        {
            public PluginInfo() { }

            public PluginInfo(Assembly PluginAsm, bool Enabled)
            {
                AssemblyGuid
                    = new Guid(((GuidAttribute)Attribute.GetCustomAttribute(PluginAsm, typeof(GuidAttribute))).Value);
                this.Enabled = Enabled;
            }

            public bool Enabled { get; set; } = true;

            public Guid AssemblyGuid { get; set; }
        }

        public class PluginInfoCollection : List<PluginInfo> { }
    }
}
