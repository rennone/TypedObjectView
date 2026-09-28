using System.Linq.Expressions;

namespace ObjectViews;

internal sealed class TypedObjectView<T>(IViewNode node) : IObjectView<T>
{
    public TResult Get<TResult>(Expression<Func<T, TResult>> selector)
    {
        var path = MemberPath.From(selector);
        return node.Get<TResult>(path);
    }

    public IObjectView<TObject> Object<TObject>(Expression<Func<T, TObject>> selector)
    {
        var path = MemberPath.From(selector);
        return new TypedObjectView<TObject>(node.GetObject(path));
    }

    public IEnumerable<IObjectView<TElement>> Array<TElement>(
        Expression<Func<T, IEnumerable<TElement>>> selector)
    {
        var path = MemberPath.From(selector);
        return Enumerate(node.GetArray(path));

        static IEnumerable<IObjectView<TElement>> Enumerate(IEnumerable<IViewNode> nodes)
        {
            foreach (var item in nodes)
            {
                yield return new TypedObjectView<TElement>(item);
            }
        }
    }
}
