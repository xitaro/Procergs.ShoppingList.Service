using Procergs.ShoppingList.Service.Dtos;

namespace Procergs.ShoppingList.Service.Repositories
{
    public interface IShoppingListRepository
    {
        public Task<IReadOnlyCollection<Entities.ShoppingList>> GetAllByUserAsync(Guid userID);
        public Task<Entities.ShoppingList> GetByIDAsync(Guid id);
        public Task CreateAsync(Entities.ShoppingList entity);
        public Task UpdateAsync(Entities.ShoppingList entity);
    }
}