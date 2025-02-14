using System.ComponentModel;
using DevExtreme.AspNet.Data.ResponseModel;
using Newtonsoft.Json;
// ReSharper disable all
#pragma warning disable CS0108, CS0114
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Be.Auto.Servicestack.OrmLite.Devexpress;

public class OrmLiteLoadResult<TEntity> : LoadResult where TEntity : class, new()
{
    public IEnumerable<TEntity> Result => base.data.Cast<TEntity>();
}