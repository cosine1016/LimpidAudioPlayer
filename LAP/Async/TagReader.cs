using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAP.Async
{
    /// <summary>
    /// 指定した型のインスタンスをすぐに返す非同期のタグリーダー
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class TagReader<T>
    {
        public delegate T TagReadingCompletedDelegate(LAPP.MediaFile MediaFile, T CurrentInstance);

        public static T GetInstance(TagReadingCompletedDelegate Delegate, string FilePath, T Instance)
        {
            LAPP.MediaFile mf;

            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                mf = new LAPP.MediaFile(FilePath);
                Instance = Delegate(mf, Instance);
            }), null);

            return Instance;
        }
    }
}
