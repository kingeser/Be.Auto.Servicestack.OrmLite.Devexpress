using Be.Auto.Servicestack.OrmLite.Devexpress;
using Demo.Models;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.OrmLite;

namespace Demo.Controllers
{

    [Route("api/[controller]")]
    public class EmployeeController(OrmLiteConnectionFactory ormLiteConnectionFactory) : Controller
    {

        [HttpGet(nameof(GetEmployees))]
        public object GetEmployees(DataSourceLoadOptions loadOptions)
        {
            using var db = ormLiteConnectionFactory.OpenDbConnection();

            var query = db.From<Employee>();

            return DataSourceLoaderOrmLite.Load(loadOptions, db, query);
        }
        [HttpGet(nameof(GetCities))]
        public object GetCities(DataSourceLoadOptions loadOptions)
        {
            using var db = ormLiteConnectionFactory.OpenDbConnection();

            var query = db.From<Employee>().SelectDistinct(t => t.City).OrderBy(t => t.City);

            return DataSourceLoaderOrmLite.Load(loadOptions, db, query);
        }


    }
}