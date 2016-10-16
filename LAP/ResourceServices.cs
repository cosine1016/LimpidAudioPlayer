using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace LAP
{
    /// <summary>
    /// 多言語化されたリソースと、言語の切り替え機能を提供します。
    /// </summary>
    internal class Lang : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region singleton members

        private static readonly Lang _current = new Lang();
        public static Lang Current
        {
            get { return _current; }
        }

        #endregion

        private readonly Properties.Resources _resources = new Properties.Resources();

        /// <summary>
        /// 多言語化されたリソースを取得します。
        /// </summary>
        public Properties.Resources Resources
        {
            get { return _resources; }
        }

        /// <summary>
        /// 指定されたカルチャ名を使用して、リソースのカルチャを変更します。
        /// </summary>
        /// <param name="name">カルチャの名前。</param>
        public void ChangeCulture(string name)
        {
            Properties.Resources.Culture = CultureInfo.GetCultureInfo(name);
            RaisePropertyChanged("Resources");
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
