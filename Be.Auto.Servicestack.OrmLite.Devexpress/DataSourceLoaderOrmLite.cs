using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using ServiceStack;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Legacy;
using ServiceStack.Script;
using static ServiceStack.OrmLite.Dapper.SqlMapper;

namespace Be.Auto.Servicestack.OrmLite.Devexpress
{
    public static class DataSourceLoaderOrmLite
    {
        public static readonly Assembly DevExtremeAsssembly = typeof(LoadResult).Assembly;

        public static LoadResult Load<TEntity>(DataSourceLoadOptionsBase loadOptionsBase, IDbConnection con,
            SqlExpression<TEntity> sqlExpression) where TEntity : class, new()
        {
            return AppyFilters(loadOptionsBase, con, sqlExpression, false);
        }
        public static LoadResult LoadInclude<TEntity>(DataSourceLoadOptionsBase loadOptionsBase, IDbConnection con,
            SqlExpression<TEntity> sqlExpression) where TEntity : class, new()
        {
            return AppyFilters(loadOptionsBase, con, sqlExpression, true);
        }

        private static LoadResult AppyFilters<TEntity>(DataSourceLoadOptionsBase loadOptionsBase, IDbConnection con,
            SqlExpression<TEntity> sqlExpression, bool loadSelect = false) where TEntity : class, new()
        {
            if (loadOptionsBase.IsCountQuery)
            {
                return new LoadResult()
                {
                    totalCount = Convert.ToInt32(con.Count(sqlExpression))
                };
            }

            var groupFields = new List<string>();
            var selectFields = new List<string>();
            foreach (var groupingInfo in (loadOptionsBase.Group ?? []))
            {
                if (string.IsNullOrEmpty(groupingInfo.GroupInterval))
                {
                    groupFields.Add(groupingInfo.Selector);
                    selectFields.Add(groupingInfo.Selector);
                }
                else
                {
                    var property = typeof(TEntity).GetProperty(groupingInfo.Selector)?.PropertyType?.GetUnderlyingTypeCode();

                    if (property == TypeCode.DateTime)
                    {
                        var providerName = con.GetDialectProvider().GetType().Name;

                        if (providerName.Contains("SqlServer", StringComparison.OrdinalIgnoreCase))
                        {
                            groupFields.Add($"FORMAT({groupingInfo.Selector},'yyyy-MM-dd')");
                            selectFields.Add($"FORMAT({groupingInfo.Selector},'yyyy-MM-dd') AS {groupingInfo.Selector}");
                        }

                        else if (providerName.Contains("MySql", StringComparison.OrdinalIgnoreCase))
                        {
                            groupFields.Add($"DATE_FORMAT({groupingInfo.Selector}, '%Y-%m-%d')");
                            selectFields.Add($"DATE_FORMAT({groupingInfo.Selector}, '%Y-%m-%d') AS {groupingInfo.Selector}");
                        }

                        else if (providerName.Contains("PostgreSQL", StringComparison.OrdinalIgnoreCase))
                        {
                            groupFields.Add($"TO_CHAR({groupingInfo.Selector}, 'YYYY-MM-DD')");
                            selectFields.Add($"TO_CHAR({groupingInfo.Selector}, 'YYYY-MM-DD') AS {groupingInfo.Selector}");
                        }
                        else
                        {
                            groupFields.Add($"{groupingInfo.GroupInterval}({groupingInfo.Selector})");
                            selectFields.Add($"{groupingInfo.GroupInterval}({groupingInfo.Selector}) AS {groupingInfo.Selector}");
                        }

                    }
                    else
                    {
                        groupFields.Add($"{groupingInfo.GroupInterval}({groupingInfo.Selector})");
                        selectFields.Add($"{groupingInfo.GroupInterval}({groupingInfo.Selector}) AS {groupingInfo.Selector}");
                    }


                }

            }

            selectFields = selectFields.Distinct().ToList();
            groupFields = groupFields.Distinct().ToList();

            if (selectFields.Any())
            {

                sqlExpression = sqlExpression.Select(string.Join(",", selectFields));
            }
            else
            {
                if (loadOptionsBase.Select?.Any() == true)
                {
                    sqlExpression = sqlExpression.Select(string.Join(",", loadOptionsBase.Select));
                }
            }

            var whereExpression = FilterExpression.Compile<TEntity>(loadOptionsBase);

            sqlExpression = sqlExpression.Where(whereExpression);


            if (groupFields.Count > 0)
            {
                sqlExpression = sqlExpression.GroupBy(string.Join(",", groupFields));

                sqlExpression = loadOptionsBase.Group!.Any(t => t.Desc) ? sqlExpression.OrderByDescending(string.Join(",", groupFields.Distinct())) : sqlExpression.OrderBy(string.Join(",", groupFields.Distinct()));
            }
            else
            {
                sqlExpression = SortExpression.Compile(sqlExpression, loadOptionsBase);
            }


            var count = con.RowCount(sqlExpression);

            if (loadOptionsBase.Skip > 0)
                sqlExpression = sqlExpression.Skip(loadOptionsBase.Skip);
            if (loadOptionsBase.Take > 0)
                sqlExpression = sqlExpression.Take(loadOptionsBase.Take);



            if (groupFields.Any())
            {
                var result = con.Select(sqlExpression);
                loadOptionsBase.Filter?.Clear();
                var loadResult = DataSourceLoader.Load(result, loadOptionsBase);
                return new LoadResult()
                {
                    data = loadResult.data,
                    totalCount = Convert.ToInt32(count)
                };
            }
            else
            {
                var result = loadSelect ? con.LoadSelect(sqlExpression) : con.Select(sqlExpression);

                return new LoadResult()
                {
                    data = result,
                    totalCount = Convert.ToInt32(count),

                };
            }
        }
    }
}