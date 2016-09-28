using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace LAPP.Utils
{
    public static class NotifyPropertyChangedEx
    {
        public static void AddPropertyChanged<TObj, TProp>(this TObj _this,
            Expression<Func<TObj, TProp>> propertyName, Action<TObj> handler)
            where TObj : INotifyPropertyChanged
        {
            // プロパティ名を取得して
            var name = ((MemberExpression)propertyName.Body).Member.Name;
            // 引数で指定されたプロパティ名と同じだったら、handlerを実行するように
            // PropertyChangedイベントに登録する
            _this.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == name)
                {
                    handler(_this);
                }
            };
        }
    }
}
