using System.Linq.Expressions;

namespace ObjectViews;

/// <summary>
/// Provides typed, read-only access to an object without requiring the complete
/// object graph to be materialized as CLR instances.
/// </summary>
/// <typeparam name="T">The type whose shape is exposed by this view.</typeparam>
public interface IObjectView<T>
{
    /// <summary>Gets a value selected by a property or field path.</summary>
    TResult Get<TResult>(Expression<Func<T, TResult>> selector);

    /// <summary>Gets a nested object as another view.</summary>
    IObjectView<TObject> Object<TObject>(Expression<Func<T, TObject>> selector);

    /// <summary>Gets an array or collection as a sequence of element views.</summary>
    IEnumerable<IObjectView<TElement>> Array<TElement>(
        Expression<Func<T, IEnumerable<TElement>>> selector);
}
