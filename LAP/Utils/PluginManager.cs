using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
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
        static PluginManager()
        {
            if (!Directory.Exists(Config.Current.Path[Enums.Path.PluginDirectory]))
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(Assembly.GetExecutingAssembly().Location);
                psi.Verb = "RunAs";
                psi.Arguments = "-InitPluginDir";
                System.Diagnostics.Process.Start(psi).WaitForExit();
            }

            LoadInfo();
            LoadPlugin();
        }

        internal static int PluginCount { get { return InitializedPlugin.Count; } }
        private static Plugins InfoCollection { get; set; }
        private static List<Plugin> InitializedPlugin { get; set; }

        public static void LoadInfo()
        {
            string PluginInfoFile = Config.Current.Path[Enums.Path.PluginManagementFile];
            if (File.Exists(PluginInfoFile))
            {
                try
                {
                    XmlSerializer ser = new XmlSerializer(typeof(Plugins));
                    using (StreamReader sr = new StreamReader(PluginInfoFile))
                    {
                        InfoCollection = (Plugins)ser.Deserialize(sr);
                    }
                }
                catch (Exception) { InfoCollection = new Plugins(); }
            }
            else
            {
                InfoCollection = new Plugins();
            }
        }

        public static void CleanUp()
        {
            Dictionary<string, bool> existc = new Dictionary<string, bool>();

            for(int i = 0;InfoCollection.Functions.Count > i; i++)
            {
                bool rem = false;
                if (!existc.ContainsKey(InfoCollection.Functions[i].Path))
                {
                    existc[InfoCollection.Functions[i].Path] =
                        !File.Exists(InfoCollection.Functions[i].Path);
                }

                rem = existc[InfoCollection.Functions[i].Path];

                if (!rem)
                {
                    Type type = Type.GetType(InfoCollection.Functions[i].TypeName, false);
                    if (type == null)
                        rem = true;
                }

                if (rem)
                {
                    InfoCollection.Functions.RemoveAt(i);
                    i--;
                }
                else
                {
                    InfoCollection.Functions[i].LastWriteDate = File.GetLastWriteTime(InfoCollection.Functions[i].Path);
                }
            }
        }

        public static void ReLoadPlugin()
        {
            for (int i = 0; InitializedPlugin.Count > i; i++)
            {
                string title = InitializedPlugin[i].Instance.Title;
                InitializedPlugin[i].Instance.Dispose();
                InitializedPlugin[i].Enabled = false;
                Dialogs.LogWindow.Append(title + " : Disposed");
            }

            InitializedPlugin.Clear();
            LoadPlugin();
        }

        public static void SaveInfo()
        {
            string PluginInfoFile = Config.Current.Path[Enums.Path.PluginManagementFile];
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(Plugins));
                using (StreamWriter sw = new StreamWriter(PluginInfoFile))
                {
                    ser.Serialize(sw, InfoCollection);
                }
            }
            catch (Exception) { InfoCollection = new Plugins(); }
        }

        internal static void SetFile(LAPP.IO.MediaFile File)
        {
            Plugin[] pls = GetPlugins(true);
            for(int i = 0;pls.Length > i; i++)
            {
                pls[i].Instance.SetFile(File);
            }
        }

        internal static T[] GetEnabledItem<T>(T[] BaseItems)
        {
            List<T> items = new List<T>();

            for (int i = 0; BaseItems.Length > i; i++)
            {
                if (IsEnabledFunction(BaseItems[i]))
                    items.Add(BaseItems[i]);
            }

            return items.ToArray();
        }

        internal static bool IsEnabledFunction<T>(T Function)
        {
            for (int i = 0; InfoCollection.Functions.Count > i; i++)
            {
                PluginFunction pf = InfoCollection.Functions[i];
                string type_str = Function.GetType().ToString();

                if(pf.TypeName == type_str)
                {
                    if(pf.LastWriteDate != File.GetLastWriteTime(pf.Path))
                    {
                        pf.LastWriteDate = File.GetLastWriteTime(pf.Path);
                        pf.TypeName = Function.GetType().ToString();
                        pf.AssemblyQualifiedName = Function.GetType().AssemblyQualifiedName;
                        pf.Title = Function.ToString();
                    }

                    return pf.Enabled;
                }
            }

            Assembly asm = Function.GetType().Assembly;
            PluginFunction npf = new PluginFunction();
            npf.LastWriteDate = File.GetLastWriteTime(asm.Location);
            npf.Enabled = Config.Current.bValue[Enums.bValue.UseNewPlugin];
            npf.TypeName = Function.GetType().ToString();
            npf.AssemblyQualifiedName = Function.GetType().AssemblyQualifiedName;
            npf.Path = asm.Location;
            npf.Title = Function.ToString();
            InfoCollection.Functions.Add(npf);

            return npf.Enabled;
        }

        internal static Collection<System.Windows.FrameworkElement> GetMediaPanelItems()
        {
            Collection<System.Windows.FrameworkElement> items = new Collection<System.Windows.FrameworkElement>();

            for (int i = 0; InitializedPlugin.Count > i; i++)
            {
                if (InitializedPlugin[i].Enabled)
                {
                    for (int j = 0; InitializedPlugin[i].Instance.MediaPanelItems.Count > j; j++)
                    {
                        items.Add(InitializedPlugin[i].Instance.MediaPanelItems[j]);
                    }
                }
            }

            return new Collection<System.Windows.FrameworkElement>(GetEnabledItem(items.ToArray()));
        }

        internal static LAPP.PageCollection GetPages()
        {
            LAPP.PageCollection pages = new LAPP.PageCollection(false);

            for (int i = 0; InitializedPlugin.Count > i; i++)
            {
                if (InitializedPlugin[i].Enabled)
                    pages.AddRange(InitializedPlugin[i].Instance.Pages);
            }

            return new LAPP.PageCollection(false, GetEnabledItem(pages.ToArray()));
        }

        internal static Plugin[] GetPlugins(bool RemoveDisabledItem)
        {
            if (RemoveDisabledItem)
            {
                List<Plugin> pls = new List<Plugin>();
                for (int i = 0; InitializedPlugin.Count > i; i++)
                    if (InitializedPlugin[i].Enabled) pls.Add(InitializedPlugin[i]);
                return pls.ToArray();
            }
            else
                return InitializedPlugin.ToArray();
        }

        internal static Collection<PluginFunction> GetFunctions()
        {
            return InfoCollection.Functions;
        }

        internal static Collection<LAPP.Wave.IWaveOutPlugin> GetWaveOutputs()
        {
            Collection<LAPP.Wave.IWaveOutPlugin> outs
                = new Collection<LAPP.Wave.IWaveOutPlugin>();

            for (int i = 0; InitializedPlugin.Count > i; i++)
            {
                if (InitializedPlugin[i].Enabled)
                {
                    for (int j = 0; InitializedPlugin[i].Instance.Providers.Count > j; j++)
                    {
                        outs.Add(InitializedPlugin[i].Instance.WaveOutputs[i]);
                    }
                }
            }
            
            return new Collection<LAPP.Wave.IWaveOutPlugin>(GetEnabledItem(outs.ToArray()));
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

            return new Collection<NWrapper.IManagableProvider>(GetEnabledItem(pros.ToArray()));
        }

        internal static LAPP.Setting.ISettingItem[] GetSettings()
        {
            List<LAPP.Setting.ISettingItem> sets = new List<LAPP.Setting.ISettingItem>();

            for (int i = 0; InitializedPlugin.Count > i; i++)
            {
                if (InitializedPlugin[i].Enabled)
                    sets.AddRange(InitializedPlugin[i].Instance.SettingItems);
            }

            return GetEnabledItem(sets.ToArray());
        }

        internal static LAPP.DisposableItemCollection<LAPP.Wave.WaveStreamPlugin> GetWaveStreams()
        {
            LAPP.DisposableItemCollection<LAPP.Wave.WaveStreamPlugin> streams
                = new LAPP.DisposableItemCollection<LAPP.Wave.WaveStreamPlugin>();

            for (int i = 0; InitializedPlugin.Count > i; i++)
            {
                if (InitializedPlugin[i].Enabled)
                    streams.AddRange(InitializedPlugin[i].Instance.WaveStreams.ToArray());
            }

            return new LAPP.DisposableItemCollection<LAPP.Wave.WaveStreamPlugin>(GetEnabledItem(streams.ToArray()));
        }

        private static void LoadPlugin()
        {
            InitializedPlugin = new List<Plugin>();
            string[] files = Directory.GetFiles(Config.Current.Path[Enums.Path.PluginDirectory], "*.dll", SearchOption.TopDirectoryOnly);

            for (int i = 0; files.Length > i; i++)
            {
                Plugin plg = new Plugin(files[i]);
                if (plg.Instance != null)
                {
                    InitializedPlugin.Add(plg);
                    PluginInfo pi = UpdatePluginEnabled(plg);

                    plg.EnableChanged += (sender, e) =>
                    {
                        pi.Enabled = plg.Enabled;
                    };
                }
            }
        }

        private static PluginInfo UpdatePluginEnabled(Plugin Plugin)
        {
            PluginInfo pi = new PluginInfo(Plugin.Asm, Config.Current.bValue[Enums.bValue.UseNewPlugin]);
            for(int i = 0;InfoCollection.Informations.Count > i; i++)
            {
                if (pi.AssemblyGuid == InfoCollection.Informations[i].AssemblyGuid)
                {
                    Plugin.Enabled = InfoCollection.Informations[i].Enabled;
                    return InfoCollection.Informations[i];
                }
            }
            
            InfoCollection.Informations.Add(pi);
            return pi;
        }

        public class Plugin
        {
            public event EventHandler EnableChanged;

            private bool ena = true;

            public Plugin(string Path)
            {
                Path = System.IO.Path.GetFullPath(Path);
                Asm = Assembly.LoadFile(Path);
                this.Path = Path;

                try
                {
                    foreach (Type t in Asm.GetTypes())
                    {
                        try
                        {
                            Instance = Activator.CreateInstance(t) as LAPP.LimpidAudioPlayerPlugin;
                        }
                        catch (Exception) { }

                        if (Instance != null)
                        {
                            Dialogs.LogWindow.Append("Plugin loaded : " + Path);
                            return;
                        }
                    }
                }
                catch (Exception) { }
            }

            public Assembly Asm { get; set; }

            public bool Enabled
            {
                get { return ena; }
                set
                {
                    if (ena != value)
                    {
                        ena = value;
                        EnableChanged?.Invoke(this, new EventArgs());
                    }
                }
            }

            public LAPP.LimpidAudioPlayerPlugin Instance { get; set; }
            public string Path { get; set; }

            public PluginInfo GetInfo()
            {
                return new PluginInfo(Asm, Enabled);
            }
        }

        public class PluginInfo
        {
            public PluginInfo()
            {
            }

            public PluginInfo(Assembly PluginAsm, bool Enabled)
            {
                AssemblyGuid
                    = new Guid(((GuidAttribute)Attribute.GetCustomAttribute(PluginAsm, typeof(GuidAttribute))).Value);
                this.Enabled = Enabled;
            }

            public Guid AssemblyGuid { get; set; }
            public bool Enabled { get; set; } = true;
        }

        public class Plugins
        {
            public Collection<PluginFunction> Functions { get; set; } = new Collection<PluginFunction>();
            public Collection<PluginInfo> Informations { get; set; } = new Collection<PluginInfo>();
        }

        public class PluginFunction
        {
            public string TypeName { get; set; }
            public string AssemblyQualifiedName { get; set; }
            public string Title { get; set; }
            public DateTime LastWriteDate { get; set; }
            public string Path { get; set; }
            public bool Enabled { get; set; }
        }
    }
}