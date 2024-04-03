using System.Linq.Expressions;

namespace BaseApp.Infra.Repository.Orm.Filter;

internal class ExpressionParser
{
    public WhereClause Criteria { get; set; }
    public Expression FieldToFilter { get; set; }
    public Expression FilterBy { get; set; }
}
