using System.Linq.Expressions;
using System.Reflection;
using DevExtreme.AspNet.Data;

namespace Be.Auto.Servicestack.OrmLite.Devexpress;


internal static class FilterExpression
{

    private static readonly Type? Type = DataSourceLoaderOrmLite.DevExtremeAsssembly.GetType("DevExtreme.AspNet.Data.FilterExpressionCompiler");
    private static readonly ConstructorInfo? ConstructorInfo = Type?.GetConstructor(new[] { typeof(Type), typeof(bool), typeof(bool), typeof(bool) });
    private static readonly MethodInfo? CompileMethod = Type?.GetMethod("Compile");
    internal static Expression<Func<TEntity, bool>>? Compile<TEntity>(DataSourceLoadOptionsBase loadOptionsBase) where TEntity : class, new()
    {
      

        var filterExpressionCompiler = ConstructorInfo?.Invoke(new object[] { typeof(TEntity), false, true, true });

        object? lambdaExpression = null;
        if (loadOptionsBase.Filter?.Count >= 1)
        {
            lambdaExpression = CompileMethod?.Invoke(filterExpressionCompiler, new object[] { loadOptionsBase.Filter }) as LambdaExpression;
        }
        else
        {
            lambdaExpression = Expression.Lambda((Expression)Expression.Constant((object)true), Expression.Parameter(typeof(TEntity)));
        }

        return (Expression<Func<TEntity, bool>>)lambdaExpression!;



    }
}