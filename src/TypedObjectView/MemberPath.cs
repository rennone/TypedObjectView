using System.Linq.Expressions;
using System.Reflection;

namespace ObjectViews;

internal sealed class MemberPath
{
    private MemberPath(IReadOnlyList<MemberInfo> members)
    {
        Members = members;
    }

    public IReadOnlyList<MemberInfo> Members { get; }

    public string DisplayName => string.Join(".", Members.Select(member => member.Name));

    public static MemberPath From<T, TResult>(Expression<Func<T, TResult>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        Expression current = StripConvert(selector.Body);
        var members = new Stack<MemberInfo>();

        while (current is MemberExpression memberExpression)
        {
            members.Push(memberExpression.Member);
            current = StripConvert(memberExpression.Expression
                ?? throw InvalidSelector(selector));
        }

        if (current != selector.Parameters[0] || members.Count == 0)
        {
            throw InvalidSelector(selector);
        }

        return new MemberPath(members.ToArray());
    }

    private static Expression StripConvert(Expression expression)
    {
        while (expression is UnaryExpression
               {
                   NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked
               } unary)
        {
            expression = unary.Operand;
        }

        return expression;
    }

    private static ObjectViewException InvalidSelector(LambdaExpression selector)
        => new($"Selector '{selector}' must be a property or field path rooted at its parameter.");
}
