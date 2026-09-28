using System.Reflection;

namespace ObjectViews;

internal sealed class ClrNode(object value) : IViewNode
{
    public TResult Get<TResult>(MemberPath path)
    {
        var result = Traverse(path);
        if (result is null)
        {
            return default!;
        }

        if (result is TResult typed)
        {
            return typed;
        }

        throw new ObjectViewException(
            $"Value at '{path.DisplayName}' is '{result.GetType()}', not '{typeof(TResult)}'.");
    }

    public IViewNode GetObject(MemberPath path)
    {
        var result = Traverse(path);
        return result is null
            ? throw new ObjectViewException($"Object at '{path.DisplayName}' is null.")
            : new ClrNode(result);
    }

    public IEnumerable<IViewNode> GetArray(MemberPath path)
    {
        var result = Traverse(path);
        if (result is null)
        {
            throw new ObjectViewException($"Array at '{path.DisplayName}' is null.");
        }

        if (result is not System.Collections.IEnumerable enumerable || result is string)
        {
            throw new ObjectViewException($"Value at '{path.DisplayName}' is not an array or collection.");
        }

        return Enumerate(enumerable, path.DisplayName);
    }

    private object? Traverse(MemberPath path)
    {
        object? current = value;

        foreach (var member in path.Members)
        {
            if (current is null)
            {
                throw new ObjectViewException(
                    $"Cannot read '{path.DisplayName}' because '{member.Name}' has a null parent.");
            }

            try
            {
                current = member switch
                {
                    PropertyInfo property => property.GetValue(current),
                    FieldInfo field => field.GetValue(current),
                    _ => throw new ObjectViewException(
                        $"Member '{member.Name}' is not a property or field.")
                };
            }
            catch (TargetInvocationException exception)
            {
                throw new ObjectViewException(
                    $"Getter for '{member.Name}' threw an exception.",
                    exception.InnerException ?? exception);
            }
        }

        return current;
    }

    private static IEnumerable<IViewNode> Enumerate(
        System.Collections.IEnumerable values,
        string path)
    {
        foreach (var item in values)
        {
            if (item is null)
            {
                throw new ObjectViewException($"Array at '{path}' contains a null item.");
            }

            yield return new ClrNode(item);
        }
    }
}
