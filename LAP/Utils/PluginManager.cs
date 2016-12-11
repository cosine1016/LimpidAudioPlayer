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
        public static event EventHandler<PluginChangedEventArgs> PluginChanged;

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

        public static void ReLoadPlugin()
        {
            for (int i = 0; InitializedPlugin.Count > i; i++)
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

        public static void UnloadPlugin(Plugin Plugin)
        {
            if (InitializedPlugin.Remove(Plugin))
            {
                PluginChanged?.Invoke(null, new PluginChangedEventArgs(Plugin, true));
            }
        }

        internal static void SetFile(LAPP.IO.MediaFile File)
        {
            Plugin[] pls = GetPlugins();
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
                string t_str = BaseItems[i].GetType().ToString();
                if (InfoCollection.Types.ContainsKey(t_str))
                {
                    if (InfoCollection.Types[t_str].Enabled)
                        items.Add(BaseItems[i]);
                }
                else
                {
                    InfoCollection.Types[t_str] = new StateOfType()
                    {
                        Enabled = true,
                        Type = BaseItems[i].GetType()
                    };

                    items.Add(BaseItems[i]);
                }
            }

            return items.ToArray();
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

        internal static Plugin[] GetPlugins()
        {
            List<Plugin> pls = new List<Plugin>();
            for (int i = 0; InitializedPlugin.Count > i; i++)
                if (InitializedPlugin[i].Enabled) pls.Add(InitializedPlugin[i]);
            return pls.ToArray();
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
                    plg.EnableChanged += Plg_EnableChanged;
                    InitializedPlugin.Add(plg);
                }
            }
        }

        private static void Plg_EnableChanged(object sender, EventArgs e)
        {
            Plugin plg = (Plugin)sender;
            PluginChanged?.Invoke(sender, new PluginChangedEventArgs(plg));
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
            public SerializableDictionary<string, StateOfType> Types = new SerializableDictionary<string, StateOfType>();
            public Collection<PluginInfo> Info { get; set; } = new Collection<PluginInfo>();
        }

        public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
        {
            XmlSchema IXmlSerializable.GetSchema()
            {
                return null;
            }

            void IXmlSerializable.ReadXml(XmlReader reader)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(KeyValueItem));
                reader.ReadStartElement();
                try
                {
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        KeyValueItem item = (KeyValueItem)serializer.Deserialize(reader);
                        Add(item.Key, item.Value);
                    }
                }
                finally
                {
                    reader.ReadEndElement();
                }
            }

            void IXmlSerializable.WriteXml(XmlWriter writer)
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add(String.Empty, String.Empty);
                XmlSerializer serializer = new XmlSerializer(typeof(KeyValueItem));
                foreach (var key in Keys)
                {
                    KeyValueItem item = new KeyValueItem();
                    item.Key = key;
                    item.Value = this[key];
                    serializer.Serialize(writer, item, ns);
                }
            }

            [XmlRoot("KeyValue")]
            public class KeyValueItem
            {
                public TKey Key;
                public TValue Value;
            }
        }

        public class StateOfType : IXmlSerializable
        {
            public bool Enabled { get; set; } = true;
            public Type Type { get; set; }

            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TypeSerializable));
                if (reader.IsEmptyElement)
                    return;

                TypeSerializable ser = serializer.Deserialize(reader) as TypeSerializable;
                if (ser != null)
                {
                    Type = Type.GetType(ser.TypeName);
                    Enabled = ser.Enabled;
                }
            }

            public void WriteXml(XmlWriter writer)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TypeSerializable));
                serializer.Serialize(writer, new TypeSerializable() { TypeName = Type.ToString(), Enabled = Enabled });
            }

            public class TypeSerializable
            {
                public bool Enabled { get; set; }
                public string TypeName { get; set; }
            }
        }
    }
}