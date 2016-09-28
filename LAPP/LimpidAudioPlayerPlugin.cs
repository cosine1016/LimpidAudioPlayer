using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAPP
{
    /// <summary>
    /// Limpid Audio Player用のプラグインを作成するための抽象クラス。
    /// このクラスはIDisposableを継承しています。
    /// </summary>
    public abstract class LimpidAudioPlayerPlugin : IDisposable
    {
        public LimpidAudioPlayerPlugin() { }

        public LimpidAudioPlayerPlugin(Page.Plugin[] Pages)
        {
            this.Pages.AddRange(Pages);
        }

        public virtual DisposableItemCollection<Page.Plugin> Pages { get; set; }
            = new DisposableItemCollection<Page.Plugin>();

        public virtual DisposableItemCollection<Wave.WaveStreamPlugin> WaveStreams { get; set; }
            = new DisposableItemCollection<Wave.WaveStreamPlugin>();

        public abstract void SetFilePath(string FilePath);

        public abstract void SetStream(System.IO.Stream Stream);
        
        public virtual void Dispose()
        {
            if(Pages != null)
            {
                Pages.Clear(true);
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
