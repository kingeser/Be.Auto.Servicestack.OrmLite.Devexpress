# Be.Auto.Servicestack.OrmLite.Devexpress

This project provides a `DataSourceLoader` class that enables data loading functionality compatible with **ServiceStack ORM Lite** and **DevExtreme** components. This package is designed to simplify the process of retrieving data from ORM Lite database connections and performing operations such as pagination, sorting, and filtering in a way that is compatible with DevExtreme components.

## Features

- **Data Loading (Read)**: Responds to data loading requests from DevExtreme components.
- **Filtering**: Allows filtering in database queries.
- **Sorting**: Supports sorting data.
- **Paging**: Supports data pagination.
- **Total Count**: Returns the total number of records along with the data page.

## Installation

### NuGet Package Installation

You can add this package to your project via NuGet:

```bash
Install-Package Be.Auto.Servicestack.OrmLite.Devexpress


Usage
Controller Example
The following example demonstrates how to load data in a way compatible with ServiceStack ORM Lite and DevExtreme. In this example, the DataSourceLoaderOrmLite.Load function is used to query PurchaseOrder data.

[Route("api/[controller]")]
public class PurchaseOrderController : Controller
{
    private readonly IDbConnection _dbConnection;

    public PurchaseOrderController(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    [HttpGet]
    public object Get(DataSourceLoadOptions loadOptions)
    {
        // Create ORM Lite query
        var query = _dbConnection.From<PurchaseOrder>();

        // Load data using DataSourceLoaderOrmLite
        var result = DataSourceLoaderOrmLite.Load(loadOptions, _dbConnection, query);
        
        return result;
    }
}
```
Explanation:
DataSourceLoadOptions: This object contains the paging, sorting, and filtering options sent by DevExtreme components.

_dbConnection: Represents the database connection. It is used to connect to the database via ServiceStack ORM Lite through IDbConnection.

DataSourceLoaderOrmLite.Load: This function takes the database query and DataSourceLoadOptions, loads the data, and returns it in a format compatible with DevExtreme.


License
This project is licensed under the MIT License.
