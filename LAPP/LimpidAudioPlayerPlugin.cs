using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LAPP
{
    /// <summary>
    /// Limpid Audio Player用のプラグインを作成するための抽象クラス。
    /// このクラスはIDisposableを継承しています。
    /// </summary>
    public abstract class LimpidAudioPlayerPlugin : IDisposable
    {
        public LimpidAudioPlayerPlugin()
        {
            if (string.IsNullOrEmpty(Title)) throw new Exception("タイトルがありません");
        }

        public virtual PageCollection Pages { get; set; }
            = new PageCollection(false);

        public virtual DisposableItemCollection<Wave.WaveStreamPlugin> WaveStreams { get; set; }
            = new DisposableItemCollection<Wave.WaveStreamPlugin>();

        public virtual Collection<Setting.ISettingItem> SettingItems { get; set; }
            = new Collection<Setting.ISettingItem>();

        public virtual Collection<NWrapper.IManagableProvider> Providers { get; set; }
            = new Collection<NWrapper.IManagableProvider>();

        public virtual Collection<System.Windows.FrameworkElement> MediaPanelItems { get; set; }
            = new Collection<System.Windows.FrameworkElement>();

        /// <summary>
        /// プラグインに割り当てられたフォルダパスを取得します。このフォルダを利用するかどうかは自由です
        /// </summary>
        /// <returns>フォルダパス</returns>
        public string GetConfigPath()
        {
            string Path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"/LAP/Config/Plugin/" + Title + "/";
            Directory.CreateDirectory(Path);
            return Path;
        }

        /// <summary>
        /// ファイルがレンダリングされる前に実行されます。
        /// </summary>
        public abstract void SetFile(IO.MediaFile File);
        
        public virtual void Dispose()
        {
            if(Pages != null)
            {
                Pages.Clear();
                Pages = null;
            }

            if(WaveStreams != null)
            {
                WaveStreams.Clear(true);
                WaveStreams = null;
            }
        }

        public abstract string Description { get; }

        public abstract string Title { get; }

        public abstract Version Version { get; }

        public abstract string URL { get; }

        public abstract string Author { get; }
    }

    public class DisposableItemCollection<T> : Collection<T> where T : IDisposable
    {
        public DisposableItemCollection() { }
        public DisposableItemCollection(T[] Items)
        {
            AddRange(Items);
        }

        public void Clear(bool Dispose)
        {
            if (Dispose)
                for (int i = 0; Count > i; i++)
                    base[i].Dispose();
            Clear();
        }

        public void AddRange(T[] items)
        {
            for (int i = 0; items.Length > i; i++)
                Add(items[i]);
        }
    }
}
