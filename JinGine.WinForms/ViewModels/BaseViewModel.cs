using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace JinGine.WinForms.ViewModels;

public abstract class BaseViewModel<T> : INotifyPropertyChanged
    where T : BaseViewModel<T>
{
    public event PropertyChangedEventHandler? PropertyChanged;

    internal IObservable<TProperty> ObserveChanges<TProperty>(Expression<Func<T, TProperty>> propertySelector)
    {
        if (propertySelector.Body is not MemberExpression { Member: PropertyInfo propertyInfo })
            throw new ArgumentException(ExceptionMessages.BaseViewModel_OnPropertyChanged_Should_be_a_property_selector, nameof(propertySelector));

        var propertyName = propertyInfo.Name;
        var propertySelectorFunc = propertySelector.Compile();

        return Observable
            .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => handler.Invoke,
                h => PropertyChanged += h,
                h => PropertyChanged -= h)
            .Where(e => e.EventArgs.PropertyName == propertyName)
            .Select(_ => propertySelectorFunc((T)this));
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<TField>(ref TField field, TField value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<TField>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}