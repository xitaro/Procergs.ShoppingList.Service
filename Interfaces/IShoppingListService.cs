using Procergs.ShoppingList.Service.Dtos;

namespace Procergs.ShoppingList.Service.Interfaces
{
    public interface IShoppingListService
    {
        // List
        public Task<IEnumerable<ShoppingListDto>> GetAllByUserAsync(string userCpf);
        public Task<ShoppingListDto> GetByIDAsync(Guid userID);
        public Task<ShoppingListDto> CreateAsync(CreateShoppingListDto createShoppingListDto);
        public Task UpdateAsync(UpdateShoppingListDto updateShoppingListDto);

        // List / Products
        public Task AddProductAsync(Guid listID, ProductDto productDto);
        public Task RemoveProductAsync(Guid listID, string gtinToRemove);

        // List / Elasticsearch
        public Task<BestPlaceDto> FindBestBuyPlace(SearchDto pesquisaDto);

    }
}
