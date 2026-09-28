namespace ObjectViews;

internal sealed class DeferredNode : IViewNode
{
    private readonly Lazy<IViewNode> node;

    public DeferredNode(Func<IViewNode> factory)
    {
        node = new Lazy<IViewNode>(factory, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public TResult Get<TResult>(MemberPath path) => node.Value.Get<TResult>(path);

    public IViewNode GetObject(MemberPath path) => node.Value.GetObject(path);

    public IEnumerable<IViewNode> GetArray(MemberPath path) => node.Value.GetArray(path);
}
