using MongoDB.Driver;
using Nest;

namespace Procergs.ShoppingList.Service.Repositories
{
    public class ShoppingListRepository : IShoppingListRepository
    {
        private readonly IMongoCollection<Entities.ShoppingList> dbCollection;

        private readonly FilterDefinitionBuilder<Entities.ShoppingList> filterDefinitionBuilder = Builders<Entities.ShoppingList>.Filter;
        
        public ShoppingListRepository(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<Entities.ShoppingList>(collectionName);
        }

        public async Task<IReadOnlyCollection<Entities.ShoppingList>> GetAllByUserAsync(string userCpf)
        {
            FilterDefinition<Entities.ShoppingList> filter = filterDefinitionBuilder.Eq(entity => entity.UserCpf, userCpf);
            return await dbCollection.Find(filter).ToListAsync();
        }

        public async Task<Entities.ShoppingList> GetByIDAsync(Guid id)
        {
            FilterDefinition<Entities.ShoppingList> filter = filterDefinitionBuilder.Eq(entity => entity.Id, id);
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

            FilterDefinition<Entities.ShoppingList> filter = filterDefinitionBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);

            await dbCollection.ReplaceOneAsync(filter, entity);
        }    
    }
}
