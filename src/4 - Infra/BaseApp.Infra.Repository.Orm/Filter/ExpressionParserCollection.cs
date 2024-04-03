using System.Linq.Expressions;

namespace BaseApp.Infra.Repository.Orm.Filter;


internal class ExpressionParserCollection : List<ExpressionParser>
{
    public ParameterExpression ParameterExpression { get; set; }

    public List<ExpressionParser> Ordered()
    {
        return this.OrderBy(b => b.Criteria.UseOr).ToList();
    }
}

