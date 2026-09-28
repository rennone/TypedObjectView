namespace ObjectViews;

internal interface IViewNode
{
    TResult Get<TResult>(MemberPath path);

    IViewNode GetObject(MemberPath path);

    IEnumerable<IViewNode> GetArray(MemberPath path);
}
