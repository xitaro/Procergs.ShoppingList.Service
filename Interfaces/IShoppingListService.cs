using Procergs.ShoppingList.Service.Dtos;

namespace Procergs.ShoppingList.Service.Interfaces
{
    public interface IShoppingListService
    {
        public Task<ShoppingListDto> GetByUserIDAsync(Guid userID);
        public Task<ShoppingListDto> CreateAsync(CreateShoppingListDto createShoppingListDto);
        public Task UpdateAsync(UpdateShoppingListDto updateShoppingListDto);

        public Task AddProductAsync(Guid userID, ProductDto productDto);
        public Task RemoveProductAsync(Guid userID, string gtinToRemove);

    }
}
