using Elasticsearch.Net;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Nest;
using Procergs.ShoppingList.Service.Dtos;
using Procergs.ShoppingList.Service.Extensions;
using Procergs.ShoppingList.Service.Interfaces;
using Procergs.ShoppingList.Service.Repositories;
using Procergs.ShoppingList.Service.Services;
using System;

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

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

// Elasticsearch
builder.Services.AddElasticSearch(builder.Configuration);

// Services
builder.Services.AddSingleton<IShoppingListService, ShoppingListService>();
builder.Services.AddSingleton<IProductService, ProductService>();

// Validators
builder.Services.AddScoped<IValidator<IProductDto>, IProductDtoValidator>();
builder.Services.AddScoped<IValidator<IShoppinListDto>, ShoppingListDtoValidator>();
builder.Services.AddScoped<IValidator<SearchDto>, SearchDtoValidator>();

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
