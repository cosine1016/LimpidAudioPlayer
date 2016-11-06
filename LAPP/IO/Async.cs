using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAPP.IO
{
    /// <summary>
    /// 非同期でタグを読み取った後、指定のインスタンスに対するデリゲートを実行します
    /// このクラスはWPFを前提として作成されています
    /// </summary>
    public static class AsyncTagReader<T>
    {
        public delegate T TagReadingCompletedDelegate(MediaFile MediaFile, T CurrentInstance);

        public static T GetInstance(TagReadingCompletedDelegate Delegate, string FilePath, T Instance)
        {
            MediaFile mf;

            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                mf = new MediaFile(FilePath);
                Instance = Delegate(mf, Instance);
            }), null);

            return Instance;
        }
    }
}
