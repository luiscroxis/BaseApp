namespace BaseApp.Infra.Repository.Orm.Extensions;

using System.Linq.Expressions;

public static class ExpressionIncludeSelectEntiyExtension
{
    public static string AsPath(this LambdaExpression? expression)
    {
        if (expression == null) return "";

        var exp = expression.Body;
        TryParsePath(exp, out var path);
        return path;
    }

    private static bool TryParsePath(Expression expression, out string path)
    {
        path = "";
        var withoutConvert = RemoveConvert(expression);

        if (withoutConvert is MemberExpression memberExpression)
        {
            var thisPart = memberExpression.Member.Name;
            if (!TryParsePath(memberExpression.Expression!, out var parentPart))
                return false;


            path = string.IsNullOrWhiteSpace(parentPart) ? thisPart : (parentPart + "." + thisPart);
        }
        else if (withoutConvert is MethodCallExpression callExpression)
        {
            if (callExpression.Method.Name is "Select" && callExpression.Arguments.Count == 2)
            {
                if (!TryParsePath(callExpression.Arguments[0], out var parentPart))
                    return false;

                if (!string.IsNullOrWhiteSpace(parentPart))
                {
                    if (callExpression.Arguments[1] is LambdaExpression subExpression)
                    {
                        if (!TryParsePath(subExpression.Body, out var thisPart))
                        {
                            return false;
                        }

                        if (!string.IsNullOrWhiteSpace(thisPart))
                        {
                            path = parentPart + "." + thisPart;
                            return true;
                        }
                    }
                }
            }
            else if (callExpression.Method.Name is "Where")
                throw new NotSupportedException("A filtragem de uma expressão de inclusão não é suportada");

            else if (callExpression.Method.Name is "OrderBy" or "OrderByDescending")
                throw new NotSupportedException("A ordenação de uma expressão de inclusão não é suportada");

            return false;
        }

        return true;
    }


    private static Expression RemoveConvert(Expression expression)
    {
        while (expression.NodeType is ExpressionType.Convert or ExpressionType.ConvertChecked)
            expression = ((UnaryExpression)expression).Operand;
        return expression;
    }
}
