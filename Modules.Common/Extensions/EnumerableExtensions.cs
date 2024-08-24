using System.Collections.ObjectModel;

namespace Modules.Common.Extensions;

public static class EnumerableExtensions
{
    public static ObservableCollection<TSource> ToObservableCollection<TSource>(this IEnumerable<TSource> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = new ObservableCollection<TSource>();

        foreach (var item in source)
        {
            result.Add(item);
        }

        return result;
    }

    public static void RemoveAll<T>(this ObservableCollection<T> source, IEnumerable<T> itemsToRemove)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(itemsToRemove);

        foreach (var item in itemsToRemove)
        {
            source.Remove(item);
        }
    }
}
