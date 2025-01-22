using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Be.Auto.Servicestack.OrmLite.Devexpress;
using Demo.Models;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.OrmLite;

namespace Demo.Controllers {

    [Route("api/[controller]")]
    public class EmployeeController(OrmLiteConnectionFactory ormLiteConnectionFactory) : Controller {

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions) {
            
            using var db = ormLiteConnectionFactory.OpenDbConnection();

            var query = db.From<Employee>();

            return DataSourceLoaderOrmLite.Load(loadOptions, db, query);
        }

    }
}