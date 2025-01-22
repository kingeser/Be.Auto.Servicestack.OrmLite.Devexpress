using ServiceStack.OrmLite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorPages()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);


builder.Services.AddScoped<OrmLiteConnectionFactory>(t =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    var dbFactory = new OrmLiteConnectionFactory(connectionString, SqlServer2022Dialect.Provider);

    return dbFactory;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
