﻿using MongoDB.Driver;

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

        public async Task<IReadOnlyCollection<Entities.ShoppingList>> GetAllByUserAsync(Guid userID)
        {
            FilterDefinition<Entities.ShoppingList> filter = filterDefinitionBuilder.Eq(entity => entity.UserID, userID);
            return await dbCollection.Find(filter).ToListAsync();
        }

        public async Task<Entities.ShoppingList> GetByIDAsync(Guid id)
        {
            FilterDefinition<Entities.ShoppingList> filter = filterDefinitionBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filterDefinitionBuilder.Empty).FirstOrDefaultAsync();
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
