using System.Linq.Expressions;
using System.Reflection;
using DevExtreme.AspNet.Data;
using ServiceStack.OrmLite;

namespace Be.Auto.Servicestack.OrmLite.Devexpress;

internal static class SortExpression
{
    private static readonly Type? Utils = DataSourceLoaderOrmLite.DevExtremeAsssembly.GetType("DevExtreme.AspNet.Data.Utils");
    private static readonly MethodInfo? GetPrimaryKey = Utils?.GetMethod("GetPrimaryKey");
    private static readonly MethodInfo? AddRequiredSort = Utils?.GetMethod("AddRequiredSort");
    internal static SqlExpression<TEntity> Compile<TEntity>(SqlExpression<TEntity> sqlExpression, DataSourceLoadOptionsBase loadOptionsBase) where TEntity : class, new()
    {
        var sort = GetFullSort(loadOptionsBase);
        var isFirst = true;
        foreach (var sortingInfo in sort)
        {

            var parameterExpression = Expression.Parameter(typeof(TEntity));
            var memberExpression = Expression.PropertyOrField(parameterExpression, sortingInfo.Selector);
            var convertExpression = Expression.Convert(memberExpression, typeof(object));
            var keySelector = Expression.Lambda<Func<TEntity, object>>(convertExpression, parameterExpression);

            if (isFirst)
            {
                sqlExpression = sortingInfo.Desc ? sqlExpression.OrderByDescending(keySelector) : sqlExpression.OrderBy(keySelector);
                isFirst = false;
            }
            else
            {
                sqlExpression = sortingInfo.Desc ? sqlExpression.ThenByDescending(keySelector) : sqlExpression.ThenBy(keySelector);
            }

        }

        return sqlExpression;
    }

    private static List<SortingInfo?> GetFullSort(DataSourceLoadOptionsBase loadOptionsBase)
    {
        var sort = new List<SortingInfo>();
        if (loadOptionsBase is { IsSummaryQuery: false, Group.Length: >= 1 })
        {
            foreach (var groupingInfo in (IEnumerable<GroupingInfo>)loadOptionsBase.Group)
            {
                if (sort.Any(t => t.Selector.Contains(groupingInfo.Selector))) continue;

                sort.Add((SortingInfo)groupingInfo);
            }
        }
        if (loadOptionsBase is { Sort.Length: >= 1 })
        {
            foreach (var sortingInfo in loadOptionsBase.Sort)
            {
                if (sort.Any(t => t.Selector.Contains(sortingInfo.Selector))) continue;

                sort.Add(sortingInfo);
            }
        }
        var strings = (IEnumerable<string>)Array.Empty<string>();


        if (!string.IsNullOrEmpty(loadOptionsBase.DefaultSort))
            strings = strings.Concat<string>((IEnumerable<string>)new string[1]
            {
                loadOptionsBase.DefaultSort
            });

        if (loadOptionsBase is { SortByPrimaryKey: true, PrimaryKey.Length: >= 1 })
        {
            var primaryKey = GetPrimaryKey?.Invoke(null, new object?[] { loadOptionsBase.PrimaryKey });
            if (primaryKey != null)
            {
                strings = strings.Concat<string>((IEnumerable<string>)primaryKey);
            }

        }
        foreach (var sortingInfo in loadOptionsBase.Sort ?? new SortingInfo[] { })
        {
            strings = strings.Concat<string>((IEnumerable<string>)new string[1]
            {
                sortingInfo.Selector
            });
        }


        var result = AddRequiredSort?.Invoke(null, new object?[] { sort, strings }) as IEnumerable<SortingInfo>;

        return (result ?? new List<SortingInfo>()).GroupBy(t => t.Selector).Select(t => t.FirstOrDefault()).ToList();
    }
}