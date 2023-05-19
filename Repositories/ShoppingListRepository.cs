using MongoDB.Driver;
using Procergs.ShoppingList.Service.Dtos;

namespace Procergs.ShoppingList.Service.Repositories
{
    public class ShoppingListRepository : IShoppingListRepository
    {
        private const string collectionName = "shoppingList";
        private readonly IMongoCollection<Entities.ShoppingList> dbCollection;
        private readonly FilterDefinitionBuilder<Entities.ShoppingList> filterDefinitionBuilder = Builders<Entities.ShoppingList>.Filter;
        
        public ShoppingListRepository()
        {
            // Declare how connect to the Mongo DB
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            // The actual datebase
            var database = mongoClient.GetDatabase("ShoppingList");

            dbCollection = database.GetCollection<Entities.ShoppingList>(collectionName);

        }

        public static Guid id = new Guid("a68d8b72-f33b-11ed-a05b-0242ac120003");
        public static string gtin = "7894900010015";
        public static string name = "Coca cola zero";

        public static readonly List<ProductDto> products = new()
        {
          new ProductDto(gtin, name, 5),
        };

        public Entities.ShoppingList ShoppingList = new Entities.ShoppingList { Id = id, Name = "Lista Teste", Products = products };

        public async Task<Entities.ShoppingList> GetByUserIDAsync(Guid userID)
        {
            FilterDefinition<Entities.ShoppingList> filter = filterDefinitionBuilder.Eq(entity => entity.UserID, userID);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Entities.ShoppingList entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(Entities.ShoppingList entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            FilterDefinition<Entities.ShoppingList> filter = filterDefinitionBuilder.Eq(existingEntity => existingEntity.UserID, entity.UserID);

            await dbCollection.ReplaceOneAsync(filter, entity);
        }    
    }
}
