using Procergs.ShoppingList.Service.Dtos;

namespace Procergs.ShoppingList.Service.Repositories
{
    public interface IShoppingListRepository
    {
        public Task<Entities.ShoppingList> GetByUserIDAsync(Guid userID);
        public Task CreateAsync(Entities.ShoppingList entity);
        public Task UpdateAsync(Entities.ShoppingList entity);
    }
}