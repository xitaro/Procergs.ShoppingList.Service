                               using MongoDB.Driver;
using Procergs.ShoppingList.Service.Interfaces;
using Procergs.ShoppingList.Service.Repositories;
using Procergs.ShoppingList.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(serviceProvider =>
{
    var mongoClient = new MongoClient(builder.Configuration.GetValue<string>("ShoppingListDataBaseSettings:ConnectionString"));
    return mongoClient.GetDatabase(builder.Configuration.GetValue<string>("ServiceSettings:ServiceName")); ;
});

builder.Services.AddSingleton<IShoppingListRepository>(serviceProvider =>
{
    var database = serviceProvider.GetService<IMongoDatabase>();
    return new ShoppingListRepository(database, builder.Configuration.GetValue<string>("ShoppingListDataBaseSettings:CollectionName"));
});

builder.Services.AddSingleton<IShoppingListService, ShoppingListService>();
builder.Services.AddSingleton<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
